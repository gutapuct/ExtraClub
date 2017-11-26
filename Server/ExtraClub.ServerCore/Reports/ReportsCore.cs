using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.ServiceModel.Reports;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExtraClub.Entities;
using System.Reflection;
using ExtraClub.ServiceModel;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.ServiceModel;

using ExtraClub.ServiceModel.Reports.ClauseParameters;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ExtraClub.ServerCore
{
    public static class ReportsCore
    {
        public static IEnumerable<ReportInfoInt> GetUserReportsList()
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new List<ReportInfoInt>();
                foreach(var r in context.ReportInfos.AsEnumerable()
#if !DEBUG
                    .Where(i => UserManagement.HasPermission(user, i.MethodInfo))
#endif
                    )
                {
                    res.Add(new ReportInfoInt
                    {
                        Key = r.MethodInfo,
                        Name = r.Name,
                        Type = ReportType.Code,
                        Parameters = r.ReportParameters.OrderBy(i => i.Order).Select(i => ReportParamInt.CreateFromContext(i)).ToList(),
                        ReportComments = r.ReportComments
                    });
                }

                var allreps = UserManagement.HasPermission(user, "ViewEditCustomReports");
                foreach(var r in context.CustomReports.ToList().Where(i => i.CompanyId == null || ((allreps || i.CreatedBy == user.UserId || i.CompanyId == null || i.Roles.Any(r => user.Roles.Any(ur => ur.RoleId == r.RoleId))) && (i.CompanyId == user.CompanyId))))
                {
                    var rep = new ReportInfoInt
                    {
                        Key = r.Id.ToString(),
                        Name = r.Name,
                        Type = ReportType.Configured,
                        Parameters = GetParametersForReport(r),
                        ReportComments = r.Comments
                    };
                    rep.RoleIds = r.Roles.Select(i => i.RoleId).ToArray();
                    res.Add(rep);
                }
                var ser = new XmlSerializer(typeof(Pair[]));
                foreach(var sr in context.SavedReports.ToList().Where(i => i.CreatedBy == user.UserId))
                {
                    ReportInfoInt report = null;
                    var r = context.ReportInfos.FirstOrDefault(i => i.MethodInfo == sr.ReportKey);

                    if(r != null)
                    {
                        report = new ReportInfoInt
                        {
                            Key = r.MethodInfo,
                            Name = sr.Name,
                            Type = ReportType.CodeParams,
                            Parameters = r.ReportParameters.OrderBy(i => i.Order).Select(i => ReportParamInt.CreateFromContext(i)).ToList(),
                            ReportComments = r.ReportComments,
                            Id = sr.Id
                        };
                    }
                    else
                    {
                        var gKey = Guid.Parse(sr.ReportKey);
                        var rep1 = context.CustomReports.Single(i => i.Id == gKey);
                        if(rep1 != null)
                        {
                            report = new ReportInfoInt
                            {
                                Key = rep1.Id.ToString(),
                                Name = sr.Name,
                                Type = ReportType.ConfiguredParams,
                                Parameters = GetParametersForReport(rep1),
                                ReportComments = rep1.Comments,
                                Id = sr.Id
                            };
                        }
                    }
                    if(report != null)
                    {
                        var pars = (Pair[])ser.Deserialize(new MemoryStream(sr.SerializedParametersValues));
                        foreach(var p in pars)
                        {
                            var par = report.Parameters.FirstOrDefault(i => i.InternalName == p.Key);
                            if(par != null) par.InstanceValue = p.Value;
                        }
                        res.Add(report);
                    }
                }

                return res.OrderBy(i => i.Name).ToList();

            }
        }

        private static List<ReportParamInt> GetParametersForReport(CustomReport r)
        {
            var res = new List<ReportParamInt>();

            var clause = ReportConstructorProcessor.DeserializeClause(r.XmlClause);

            ProcessClause(res, clause);

            return res;
        }

        private static void ProcessClause(List<ReportParamInt> res, Clause clause)
        {
            if(clause == null) return;
            if(clause is FiniteClause && !String.IsNullOrEmpty(((FiniteClause)clause).ParameterName) && !((FiniteClause)clause).IsFixedValue)
            {
                res.Add(GetReportParameter((FiniteClause)clause));
            }
            else
            {
                ProcessClause(res, clause.LeftPart);
                ProcessClause(res, clause.RightPart);
            }
        }

        private static ReportParamInt GetReportParameter(FiniteClause clause)
        {
            var type = GetParameterType(((FiniteClause)clause).ParameterTypeName);
            return new ReportParamInt
            {
                DisplayName = ((FiniteClause)clause).ParameterName,
                InternalName = ((FiniteClause)clause).ParameterName,
                Type = type.Key,
                GetValuesDelegateType = type.Value
            };
        }

        private static KeyValuePair<ReportParameterType, string> GetParameterType(string typeName)
        {
            var type = Type.GetType(typeName + ", ExtraClub.ServiceModel");
            if(type != null)
            {
                var attrs = type.GetCustomAttributes(typeof(ValueTypeAttribute), true);
                if(attrs.Length > 0)
                {
                    var valType = ((ValueTypeAttribute)attrs[0]).Type;
                    if(valType == typeof(Date))
                        return new KeyValuePair<ReportParameterType, string>(ReportParameterType.Date, null);
                    if(valType == typeof(Guid))
                        return new KeyValuePair<ReportParameterType, string>(ReportParameterType.CustomDropdown, typeName);
                }
            }
            return new KeyValuePair<ReportParameterType, string>(ReportParameterType.String, null);
        }

        public static DataTable GenerateReportUncompressed(string key, Dictionary<string, object> parameters)
        {
            DataTable dt = null;
            using(var context = new ExtraEntities())
            {
                var rep = context.ReportInfos.FirstOrDefault(i => i.MethodInfo == key);


                if(rep == null)
                {
                    var gKey = Guid.Parse(key);
                    var rep1 = context.CustomReports.Single(i => i.Id == gKey);
                    dt = new ReportConstructorProcessor(rep1, parameters).Process();
                }
                else
                {
                    if((ReportType)rep.Type == ReportType.Code)
                    {
                        try
                        {
                            var mi = typeof(CustomReports).GetMethod(rep.MethodInfo);
                            //var resArr = parameters.Select(i => i.Value).ToArray();
                            var pars = mi.GetParameters();
                            var resArr = new object[pars.Length];
                            for(var i = 0; i < pars.Length; i++)
                            {
                                if(parameters.ContainsKey(pars[i].Name))
                                {
                                    var accordingParameterValue = parameters[pars[i].Name];
                                    var type = pars[i].ParameterType;
                                    if(type == typeof(DateTime) || type == typeof(DateTime?))
                                    {
                                        if(!(accordingParameterValue is DateTime) && accordingParameterValue != null)
                                        {
                                            accordingParameterValue = DateTime.Parse(accordingParameterValue.ToString());
                                        }
                                    }
                                    if(type == typeof(bool) || type == typeof(bool?))
                                    {
                                        if(!(accordingParameterValue is bool) && accordingParameterValue != null)
                                        {
                                            accordingParameterValue = Boolean.Parse(accordingParameterValue.ToString());
                                        }
                                    }
                                    if(type == typeof(Guid) || type == typeof(Guid?))
                                    {
                                        if(!(accordingParameterValue is Guid) && accordingParameterValue != null)
                                        {
                                            accordingParameterValue = Guid.Parse(accordingParameterValue.ToString());
                                        }
                                    }
                                    resArr[i] = accordingParameterValue;
                                }
                            }
                            dt = (DataTable)mi.Invoke(new CustomReports(), resArr);
                        }
                        catch(TargetInvocationException ex)
                        {
                            var exc = ex.InnerException;
                            throw new FaultException<string>(exc.Message, exc.Message);
                        }
                    }
                    if((ReportType)rep.Type == ReportType.Stored)
                    {
                        dt = ExecuteStoredProcedure(context, rep, parameters);
                    }
                }
            }
            return dt;
        }


        public static byte[] GenerateReport(string key, Dictionary<string, object> parameters)
        {
            var dt = GenerateReportUncompressed(key, parameters);

            var under = new MemoryStream();
            var stream = new System.IO.Compression.GZipStream(under, System.IO.Compression.CompressionMode.Compress);

            var bf = new BinaryFormatter();
            bf.Serialize(stream, dt ?? new DataTable());

            stream.Close();
            byte[] b = under.ToArray();

            return b;
        }

        private static DataTable ExecuteStoredProcedure(ExtraEntities context, ReportInfo rep, Dictionary<string, object> parameters)
        {
            var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString);
            try
            {
                var user = UserManagement.GetUser(context);
                conn.Open();
                var cmd = new SqlCommand(rep.MethodInfo, conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600000 };
                var compAdded = false;
                foreach(var p in rep.ReportParameters)
                {
                    if(p.IntName.ToLower() == "companyid")
                    {
                        compAdded = true;
                        if(parameters[p.IntName] == null && !UserManagement.HasPermission(user, "AllCompanies"))
                        {
                            parameters[p.IntName] = user.CompanyId;
                        }
                    }
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = p.IntName,
                        SqlDbType = ConvertToSqlType((ReportParameterType)p.Type),
                        Value = parameters[p.IntName] ?? DBNull.Value
                    });
                }
                if(!compAdded)
                {
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "companyId",
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Value = UserManagement.HasPermission(user, "AllCompanies") ? (object)DBNull.Value : (object)user.CompanyId
                    });
                }

                var rdr = cmd.ExecuteReader();
                var res = new DataTable();
                for(int i = 0; i < rdr.FieldCount; i++)
                {
                    res.Columns.Add(rdr.GetName(i), rdr.GetFieldType(i));
                }
                while(rdr.Read())
                {
                    var vals = new object[rdr.FieldCount];
                    rdr.GetValues(vals);
                    res.Rows.Add(vals);
                }
                return res;
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private static SqlDbType ConvertToSqlType(ReportParameterType p)
        {
            if(p == ReportParameterType.Date || p == ReportParameterType.Month) return SqlDbType.Date;
            if(p == ReportParameterType.Division || p == ReportParameterType.Company) return SqlDbType.UniqueIdentifier;
            if(p == ReportParameterType.Good) return SqlDbType.UniqueIdentifier;
            if(p == ReportParameterType.String) return SqlDbType.NVarChar;
            if(p == ReportParameterType.Boolean) return SqlDbType.Bit;
            throw new ArgumentNullException(p.ToString());
        }

        public static void PostParameters(string reportKey, Dictionary<string, object> parameters, string setName)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);

                var ser = new XmlSerializer(typeof(Pair[]));
                using(var ms = new MemoryStream())
                {
                    ser.Serialize(ms, parameters.Select(i => new Pair { Key = i.Key, Value = i.Value }).ToArray());
                    var sp = new SavedReport
                    {
                        CompanyId = user.CompanyId,
                        CreatedBy = user.UserId,
                        Id = Guid.NewGuid(),
                        Name = setName,
                        ReportKey = reportKey,
                        SerializedParametersValues = ms.ToArray()
                    };
                    context.SavedReports.AddObject(sp);
                    context.SaveChanges();
                }
            }
        }

        public static void PostUserReport(ServiceModel.Reports.ReportInfoInt report)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                CustomReport cr;
                if(report.Key == Guid.Empty.ToString())
                {
                    cr = new CustomReport
                    {
                        BaseTypeName = report.BaseTypeName,
                        Comments = report.ReportComments,
                        CompanyId = report.IsCommon ? (Guid?)null : user.CompanyId,
                        CreatedBy = user.UserId,
                        CustomFields = JoinStrings(report.CustomFields),
                        Id = Guid.NewGuid(),
                        Name = report.Name,
                        XmlClause = report.XmlClause//SerializeClause(report.ClauseChain)
                    };
                    context.CustomReports.AddObject(cr);
                }
                else
                {
                    var gId = Guid.Parse(report.Key);
                    cr = context.CustomReports.Single(i => i.Id == gId);
                    cr.BaseTypeName = report.BaseTypeName;
                    cr.Comments = report.ReportComments;
                    cr.CompanyId = report.IsCommon ? (Guid?)null : user.CompanyId;
                    cr.CreatedBy = user.UserId;
                    cr.CustomFields = JoinStrings(report.CustomFields);
                    cr.Name = report.Name;
                    cr.XmlClause = report.XmlClause;
                }
                cr.Roles.Clear();
                foreach(var rId in report.RoleIds)
                {
                    cr.Roles.Add(context.Roles.Single(r => r.RoleId == rId));
                }
                context.SaveChanges();
            }
        }

        private static string JoinStrings(List<string> list)
        {
            var sb = new StringBuilder();
            foreach(var s in list)
            {
                sb.Append(s);
                sb.Append(",");
            }
            return sb.ToString();
        }

        public static CustomReport GetReportForEdit(Guid id)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = context.CustomReports.Single(i => i.Id == id);
                res.Init();
                if(!res.IsFixed && (res.CreatedBy == user.UserId || UserManagement.HasPermission(user, "ViewEditCustomReports")))
                {
                    return res;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void DeleteReport(Guid savedId, string key, ReportType reportType)
        {
            using(var context = new ExtraEntities())
            {
                var id = Guid.Parse(key);
                var user = UserManagement.GetUser(context);
                if(reportType == ReportType.Configured)
                {
                    var rep = context.CustomReports.FirstOrDefault(r => r.Id == id);
                    if(rep != null)
                    {
                        rep.Roles.Clear();
                        context.SavedReports.Where(s => s.ReportKey == key).ToList().ForEach(i => context.DeleteObject(i));
                        context.DeleteObject(rep);
                    }
                }
                else
                {
                    var rep = context.SavedReports.FirstOrDefault(i => i.Id == id);
                    if(rep != null)
                    {
                        context.DeleteObject(rep);
                    }
                }
                context.SaveChanges();
            }
        }


        public static IEnumerable<ReportRecurrency> GetRecurrentReports(Guid userId)
        {
            using(var context = new ExtraEntities())
            {
                var res = context.ReportRecurrencies.Where(i => i.UserId == userId).ToList();
                var reps = GetUserReportsList();
                foreach(var r in res)
                {
                    r.SerializedName = reps.Where(j => j.Key == r.ReportKey).Select(i => i.Name).FirstOrDefault();
                }
                return res;
            }
        }
    }

    public class Pair
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
