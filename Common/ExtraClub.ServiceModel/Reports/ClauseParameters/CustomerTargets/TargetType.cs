using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.CustomerTargets
{
    [DataContract]
    [ClauseRelation(typeof(CustomerTarget))]
    [Description("Тип цели")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class TargetType : ClauseGuidParameter<CustomerTarget>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get { return (context) => context.GetCustomerTargetTypes().ToDictionary(i => (object)i.Key, i => i.Value); }
        }

        protected override Guid? GuidFunction(CustomerTarget entity)
        {
            return entity.TargetTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerTarget entity)
        {
            throw new NotImplementedException();
        }
    }

}
