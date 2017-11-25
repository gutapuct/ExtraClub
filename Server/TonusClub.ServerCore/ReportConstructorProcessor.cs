using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel.Reports;
using System.Xml.Serialization;
using System.IO;
using TonusClub.ServiceModel;
using System.Data;
using TonusClub.Entities;
using System.Data.Objects;
using TonusClub.ServiceModel.Reports.ClauseParameters;
using System.Reflection;
using TonusClub.ServiceModel.Reports.ReportColumns;
using System.Collections;
using System.Diagnostics;


namespace TonusClub.ServerCore
{
    class ReportConstructorProcessor
    {
        TonusEntities context;
        IQueryable<object> query;

        CustomReport reportInfo;

        bool InitOnTable;

        public ReportConstructorProcessor(CustomReport report, Dictionary<string, object> parameters)
        {
            reportInfo = report;
            var clause = DeserializeClause(report.XmlClause);
            bool initOnQuery, initOnTable;
            var includes = GetIncludes(reportInfo, clause, out initOnQuery, out initOnTable);
            //InitOnTable = initOnTable;
            InitOnTable = true;
            context = new TonusEntities();
            var user = UserManagement.GetUser(context);
            var baseType = Type.GetType(report.BaseTypeName + ", TonusClub.ServiceModel");
            if (baseType == typeof(CustomerTarget))
            {
                query = BuildQuery(clause, context.CustomerTargets.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(Customer))
            {
                var src = context.Customers.AddIncludes(includes).Where(i => i.CompanyId == user.CompanyId).ToList();
                if (initOnQuery)
                {
                    src.ForEach(i => i.Init());
                }
                query = BuildQuery(clause, src.AsQueryable(), parameters);
                InitOnTable = initOnTable;
            }
            else if (baseType == typeof(TreatmentEvent))
            {
                if(!context.LocalSettings.Any())
                {
                    throw new NotSupportedException("На ЦС такой отчет построить нельзя!");
                }
                query = BuildQuery(clause, context.TreatmentEvents.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(Treatment))
            {
                query = BuildQuery(clause, context.Treatments.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(Ticket))
            {
                var src = context.Tickets.AddIncludes(includes).Where(i => i.CompanyId == user.CompanyId).ToList();
                if (initOnQuery)
                {
                    src.ForEach(i => i.Init());
                }
                query = BuildQuery(clause, src.AsQueryable(), parameters);
                InitOnTable = initOnTable;
            }
            else if (baseType == typeof(CustomerCard))
            {
                query = BuildQuery(clause, context.CustomerCards.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(GoodSale))
            {
                query = BuildQuery(clause, context.GoodSales.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(Spending))
            {
                query = BuildQuery(clause, context.Spendings.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(Good))
            {
                query = BuildQuery(clause, context.Goods.ToList().AsQueryable(), parameters);
            }
            else if (baseType == typeof(Employee))
            {
                query = BuildQuery(clause, context.Employees.ToList().AsQueryable(), parameters);
            }
            if (!UserManagement.HasPermission(user, "AllCompanies"))
            {
                query = query.Where(i => i.GetValue("CompanyId") == null || (Guid)i.GetValue("CompanyId") == user.CompanyId);
            }
        }

        private string[] GetIncludes(CustomReport reportInfo, Clause clause, out bool initOnQuery, out bool initOnTable)
        {
            var baseType = Type.GetType(reportInfo.BaseTypeName + ", TonusClub.ServiceModel");
            var mapperType = ReportColumnsRegistry.GetColumnMapperType(baseType);
            var res = new List<string>();
            initOnTable = false;
            foreach (var str in reportInfo.CustomFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!initOnTable && mapperType.GetProperty(str).GetCustomAttributes(typeof(InitAttribude), false).Any()) initOnTable = true;
                var attrs = mapperType.GetProperty(str).GetCustomAttributes(typeof(IncludeAttribute), false);
                if (attrs.Any())
                {
                    foreach (IncludeAttribute i in attrs)
                    {
                        foreach (var j in i.Includes)
                        {
                            if (!res.Contains(j)) res.Add(j);
                        }
                    }
                }
            }
            initOnQuery = false;
            CollectClauseInclude(clause, res, ref initOnQuery);

            return res.ToArray();
        }

        private void CollectClauseInclude(Clause clause, List<string> res, ref bool initNeeded)
        {
            if (clause is AndClause || clause is OrClause)
            {
                CollectClauseInclude(clause.LeftPart, res, ref initNeeded);
                CollectClauseInclude(clause.RightPart, res, ref initNeeded);
            }
            else
            {
                foreach (IncludeAttribute attr in ((FiniteClause)clause).Parameter.GetType().GetCustomAttributes(typeof(IncludeAttribute), false))
                {
                    foreach (var j in attr.Includes)
                    {
                        if (!res.Contains(j)) res.Add(j);
                    }
                }
                if (!initNeeded && ((FiniteClause)clause).Parameter.GetType().GetCustomAttributes(typeof(InitAttribude), false).Any()) initNeeded = true;
            }
        }

        static object lockObj = new object();
        static XmlSerializer _serialier;
        static XmlSerializer Serializer
        {
            get
            {
                lock (lockObj)
                {
                    if (_serialier == null)
                    {
                        var knownTypes = new List<Type> { typeof(Clause), typeof(OrClause),
                            typeof(AndClause),
                            typeof(FiniteClause),
                            typeof(Clause)};
                        knownTypes.AddRange(ClauseRegistry.GetRelatedAttributes(null));

                        _serialier = new XmlSerializer(typeof(Clause), knownTypes.ToArray());
                    }
                    return _serialier;
                }
            }
        }

        public static Clause DeserializeClause(byte[] p)
        {
            var ms = new MemoryStream(p);
            return (Clause)Serializer.Deserialize(ms);
        }

        private IQueryable<T> BuildQuery<T>(Clause clause, IQueryable<T> query, Dictionary<string, object> parameters)
        {
            if (clause is FiniteClause)
            {
                var asm = Assembly.GetAssembly(typeof(Clause));
                var type = asm.GetType(((FiniteClause)clause).ParameterTypeName);
                var inst = (ClauseParameter)Activator.CreateInstance(type);
                inst.Operator = ((FiniteClause)clause).Operator;
                if (((FiniteClause)clause).IsFixedValue || inst.Operator == ClauseOperator.False || inst.Operator == ClauseOperator.True)
                {
                    inst.Value = ((FiniteClause)clause).FixedValue;
                }
                else
                {
                    inst.Value = parameters[((FiniteClause)clause).ParameterName];
                }
                query = query.Where(i => inst.QueryFunction(i));
            }
            else
                if (clause is AndClause)
                {
                    query = BuildQuery(clause.LeftPart, query, parameters);
                    query = BuildQuery(clause.RightPart, query, parameters);
                }
                else if (clause is OrClause)
                {
                    query = BuildQuery(clause.LeftPart, query, parameters).Union(BuildQuery(clause.RightPart, query, parameters)).Distinct();
                }

            return query;
        }

        public DataTable Process()
        {
            var res = new DataTable();
            res.ExtendedProperties.Add("EntityType", reportInfo.BaseTypeName);
            res.Columns.Add("_id", typeof(Guid));
            var baseType = Type.GetType(reportInfo.BaseTypeName + ", TonusClub.ServiceModel");
            var cols = ReportColumnsRegistry.GetColumns(baseType);
            var mapperType = ReportColumnsRegistry.GetColumnMapperType(baseType);

            foreach (var str in reportInfo.CustomFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (cols.ContainsKey(str))
                {
                    var t = mapperType.GetProperty(str).PropertyType;
                    if (!(mapperType.GetProperty(str).PropertyType.IsGenericType && mapperType.GetProperty(str).PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        res.Columns.Add(cols[str], t);
                    }
                    else
                    {
                        res.Columns.Add(cols[str], mapperType.GetProperty(str).PropertyType.GetGenericArguments()[0]);
                    }
                }
            }
            foreach (var row in query)
            {
                var mapper = ReportColumnsRegistry.GetColumnMapper(baseType, row);
                if (InitOnTable)
                {
                    var method = row.GetType().GetMethod("Init");
                    if (method != null)
                    {
                        method.Invoke(row, new object[0]);
                    }
                }
                var arr = new ArrayList();
                arr.Add((Guid)row.GetValue("Id"));
                foreach (var str in reportInfo.CustomFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    arr.Add(mapper.GetValue(str) ?? DBNull.Value);
                }
                var resRow = res.Rows.Add(arr.ToArray());
            }
            return res;
        }
    }

    public static class Helpers
    {
        public static ObjectQuery<T> AddIncludes<T>(this ObjectQuery<T> src, string[] includes)
        {
            foreach (var i in includes)
            {
                src = src.Include(i);
            }
            return src;
        }
    }
}
