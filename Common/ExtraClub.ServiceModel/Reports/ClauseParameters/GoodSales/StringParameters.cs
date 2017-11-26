using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.GoodSales
{
    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Номер карты клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamGS1 : ClauseStringParameter<GoodSale>
    {
        protected override string StringFunction(GoodSale i)
        {
            i.BarOrder.Customer.InitActiveCard();
            if (i.BarOrder.Customer.ActiveCard == null) return String.Empty;
            return i.BarOrder.Customer.ActiveCard.CardBarcode;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("ФИО клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamGS2 : ClauseStringParameter<GoodSale>
    {
        protected override string StringFunction(GoodSale i)
        {
            return i.BarOrder.Customer.FullName ?? "";
        }
    }

}
