using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Windows;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Reports.ClauseParameters;
using ExtraClub.Infrastructure;
using ExtraClub.UIControls;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.Reports.Views.ContainedControls
{
    partial class ReportContainerBase
    {
        public static ReportContainerBase CreateInstance(ReportInfoInt report)
        {
            return CreateInstance<object>(report, i => ApplicationDispatcher.UnityContainer.Resolve<ClientContext>().GenerateReport(report.Key, i), null);
        }

        public static ReportContainerBase CreateInstance<T>(ReportInfoInt report, Func<IEnumerable<ReportParamInt>, object> generationFunc, FrameworkElement resultControl)
        {
            var context = ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();
            var res = new ReportContainerBase(Decimal.Parse(AppSettingsManager.GetSetting("Yellow")), Decimal.Parse(AppSettingsManager.GetSetting("Orange")), Decimal.Parse(AppSettingsManager.GetSetting("Red")));
            res._generationFunc = generationFunc;
            res.ReportInfoInt = ViewModelBase.Clone<ReportInfoInt>(report);
            if (res.ReportInfoInt.Parameters != null)
            {
                res.ReportInfoInt.Parameters.ForEach(i => InitParam(context, i));
            }
            var companyParam = res.ReportInfoInt.Parameters.FirstOrDefault(i => i.Type == ReportParameterType.Company);
            if (companyParam != null)
            {
                companyParam.InstanceValueChanged += delegate {
                    res.ReportInfoInt.Parameters.Where(i => i.Type == ReportParameterType.Division).ToList().ForEach(i =>
                        {
                            if (companyParam.InstanceValue is Guid)
                            {
                                i.List = context.GetDivisionsForCompany((Guid)companyParam.InstanceValue)
                                    .ToDictionary(j => j.Id, j => j.Name);
                            }
                            else
                            {
                                i.List = context.GetDivisions().ToDictionary(j => j.Id, j => j.Name);
                            }
                            i.InstanceValue = null;
                        });
                };
            }
            if (resultControl != null)
            {
                res.ResultContent.Content = resultControl;
            }
            return res;
        }

        private static Dictionary<Guid, string> __Divisions;
        private static Dictionary<Guid, string> __Companies;
        private static Dictionary<Guid, string> __Goods;
        private static List<Employee> __Employees;

        internal static void InitParam(ClientContext context, ReportParamInt p)
        {
            if(__Divisions == null)
            {
                __Divisions = context.GetDivisions().ToDictionary(i => i.Id, i => i.Name);
            }
            if(__Companies == null)
            {
                __Companies = context.GetCompanies().ToDictionary(i => i.CompanyId, i => i.CompanyName); ;
            }
            if(__Employees == null)
            {
                __Employees = context.GetEmployees(false, false);
            }
            if(__Goods == null)
            {
                __Goods = context.GetAllGoods().ToDictionary(i => i.Id, i => i.Name);
            }


            if (p.Type == ReportParameterType.Date)
            {
                if (p.InternalName.Contains("start"))
                {
                    p.InstanceValue = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                }
                if (p.InternalName.Contains("end"))
                {
                    p.InstanceValue = DateTime.Today;
                }
            }
            if (p.Type == ReportParameterType.Month)
            {
                if (p.InternalName.Contains("start"))
                {
                    p.InstanceValue = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                }
                if (p.InternalName.Contains("end"))
                {
                    p.InstanceValue = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                }
            }
            if (p.Type == ReportParameterType.Division)
            {
                var src = __Divisions;
                p.List = src;
                if (src.ContainsKey(context.CurrentDivision.Id))
                {
                    p.InstanceValue = context.CurrentDivision.Id;
                }
                else if (src.Count == 1)
                {
                    p.InstanceValue = src.Select(i => i.Key).FirstOrDefault();
                }
            }
            if (p.Type == ReportParameterType.Company)
            {
                var src = __Companies;
                p.List = src;
                if (src.ContainsKey(context.CurrentCompany.CompanyId))
                {
                    p.InstanceValue = context.CurrentCompany.CompanyId;
                }
                else if (src.Count == 1)
                {
                    p.InstanceValue = src.Select(i => i.Key).FirstOrDefault();
                }
            } if (p.Type == ReportParameterType.Good)
            {
                p.List = __Goods;
            }
            if (p.Type == ReportParameterType.Employee)
            {
                p.List = __Employees.ToDictionary(i => i.Id, i => i.SerializedCustomer.FullName);
            }
            if (p.Type == ReportParameterType.Employees)
            {
                p.List = __Employees.Select(i => new SelectListItem { Key = i.Id, Value = i.SerializedCustomer.FullName }).ToList();
            }


            if (p.Type == ReportParameterType.Boolean)
            {
                p.InstanceValue = p.InstanceValue ?? false;
            }
            if (p.Type == ReportParameterType.CustomDropdown)
            {
                var del = Type.GetType(p.GetValuesDelegateType + ", ExtraClub.ServiceModel");
                var inst = (ClauseParameter)Activator.CreateInstance(del);
                p.List = context.ExecuteMethod(channel => inst.GetValuesFunction(channel));
            }
        }
    }
}
