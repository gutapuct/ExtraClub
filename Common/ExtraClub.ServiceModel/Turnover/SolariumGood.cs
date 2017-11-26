using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel.Turnover
{
    [DataContractAttribute]
    public class SolariumGood : PayableItem
    {
        [DataMember]
        public Guid solariumVisitId { get;  set; }

        private SolariumGood() { }

        public SolariumGood(Division div, SolariumVisit visit)
        {
            solariumVisitId = visit.Id;
            InBasket = visit.Amount;
            Name = Localization.Resources.SolariumVisit;
            Price = visit.SerializedPrice;
            UnitName = Localization.Resources.Mins;
        }
    }
}
