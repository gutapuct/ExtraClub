using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Customers
{
    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Статус клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class CustomerStateParameter : ClauseGuidParameter<Customer>
    {
        public override Func<IClientContext_, Dictionary<object, string>> GetValuesFunction
        {
            get { return new Func<IClientContext_, Dictionary<object, string>>(i => i.GetDictionaryList("CustomerStatuses").ToDictionary(j => (object)j.Key, j => j.Value)); }
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            return entity.CustomerStatuses.Select(i => i.Id);
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
