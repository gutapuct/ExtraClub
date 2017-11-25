using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Employees
{
    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("ФИО")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEm1 : ClauseStringParameter<Employee>
    {
        protected override string StringFunction(Employee i)
        {
            return i.BoundCustomer.FullName ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Карта")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEm2 : ClauseStringParameter<Employee>
    {
        protected override string StringFunction(Employee i)
        {
            i.Init();
            return i.SerializedCardNumber ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Должность")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEm3 : ClauseStringParameter<Employee>
    {
        protected override string StringFunction(Employee i)
        {
            i.Init();
            if (i.SerializedJobPlacement == null) return String.Empty;
            return i.SerializedJobPlacement.Job.Name;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Схема премий")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEm4 : ClauseStringParameter<Employee>
    {
        protected override string StringFunction(Employee i)
        {
            i.Init();
            if (i.SerializedJobPlacement == null) return String.Empty;
            if (!i.SerializedJobPlacement.Job.SalarySchemeId.HasValue) return String.Empty;
            return i.SerializedJobPlacement.Job.SalaryScheme.Name;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Подразделение")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEm5 : ClauseStringParameter<Employee>
    {
        protected override string StringFunction(Employee i)
        {
            i.Init();
            if (i.SerializedJobPlacement == null) return String.Empty;
            return i.SerializedJobPlacement.Job.Unit ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Схема работы")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEm6 : ClauseStringParameter<Employee>
    {
        protected override string StringFunction(Employee i)
        {
            i.Init();
            if (i.SerializedJobPlacement == null) return String.Empty;
            return i.SerializedJobPlacement.Job.WorkGraph ?? "";
        }
    }
}
