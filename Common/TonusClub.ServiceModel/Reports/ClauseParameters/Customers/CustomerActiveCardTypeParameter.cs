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
    [Description("Тип активной карты")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    [Include("CustomerCards", "CustomerCards.CustomerCardType")]
    public class CustomerActiveCardTypeParameter : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetCustomerCardTypes(true).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            entity.InitActiveCard();
            if (entity.ActiveCard == null) return null;
            return entity.ActiveCard.CustomerCardTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
