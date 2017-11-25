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
    [Description("Текущий оклад")]
    public class NumberParameterEm1 : ClauseNumberParameter<Employee>
    {
        protected override decimal? NumberFunction(Employee i)
        {
            i.Init();
            if (i.SerializedJobPlacement == null) return null;
            return i.SerializedJobPlacement.Salary;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("День рождения")]
    public class NumberParameterEm2 : ClauseNumberParameter<Employee>
    {
        protected override decimal? NumberFunction(Employee i)
        {
            i.Init();
            if (!i.BoundCustomer.Birthday.HasValue) return null;
            return i.BoundCustomer.Birthday.Value.Day;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Месяц рождения")]
    public class NumberParameterEm3 : ClauseNumberParameter<Employee>
    {
        protected override decimal? NumberFunction(Employee i)
        {
            i.Init();
            if (!i.BoundCustomer.Birthday.HasValue) return null;
            return i.BoundCustomer.Birthday.Value.Month;
        }
    }
}
