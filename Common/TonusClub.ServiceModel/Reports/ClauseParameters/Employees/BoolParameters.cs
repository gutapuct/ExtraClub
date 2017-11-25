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
    [Description("В отпуске ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterEm1 : ClauseBoolParameter<Employee>
    {
        protected override bool? BoolFunction(Employee entity)
        {
            return entity.EmployeeVacations.Any(i => i.VacationType == 0 && i.BeginDate <= DateTime.Today && i.EndDate >= DateTime.Today);
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("На больничном ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterEm2 : ClauseBoolParameter<Employee>
    {
        protected override bool? BoolFunction(Employee entity)
        {
            return entity.EmployeeVacations.Any(i => i.VacationType == 1 && i.BeginDate <= DateTime.Today && i.EndDate >= DateTime.Today);
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("В командировке ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterEm3 : ClauseBoolParameter<Employee>
    {
        protected override bool? BoolFunction(Employee entity)
        {
            return entity.EmployeeTrips.Any(i => i.BeginDate <= DateTime.Today && i.EndDate >= DateTime.Today);
        }
    }
}
