using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.CustomerTargets
{
    [DataContract]
    [ClauseRelation(typeof(CustomerTarget))]
    [Description("Кто поставил цель")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class CreatedBy : ClauseGuidParameter<CustomerTarget>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get { return (context) => context.GetEmployees(Guid.Empty, false, true).ToDictionary(i => (object)i.Id, i => i.SerializedCustomer.ShortName); }
        }

        protected override Guid? GuidFunction(CustomerTarget entity)
        {
            return entity.CreatedBy.EmployeeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerTarget entity)
        {
            throw new NotImplementedException();
        }
    }
}
