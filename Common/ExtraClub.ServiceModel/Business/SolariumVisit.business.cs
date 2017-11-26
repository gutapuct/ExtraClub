using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class SolariumVisit : IInitable
    {
        [DataMember]
        public string SerializedDivisionName { get; private set; }

        [DataMember]
        public string SerializedTicketNumber { get; private set; }

        [DataMember]
        public string SerializedCustomerName { get; private set; }

        [DataMember]
        public string SerializedSolariumName { get; private set; }

        [DataMember]
        public decimal SerializedPrice { get; private set; }

        [DataMember]
        public decimal SerializedPriceCoeff { get; private set; }

        public void Init()
        {
            SerializedDivisionName = Division.Name;
            SerializedCustomerName = Customer.FullName;
            Customer.InitEssentials();
            if (Customer.ActiveCard != null)
            {
                SerializedCustomerName = "[" + Customer.ActiveCard.CardBarcode + "] " + SerializedCustomerName;
            }
            SerializedSolariumName = Solarium.Name;
            if (TicketId.HasValue)
            {
                SerializedTicketNumber = Ticket.Number;
            }
            SerializedPrice = Solarium.MinutePrice;
            SerializedPriceCoeff = Solarium.TicketMinutePrice;
        }

        public SolariumVisitStatus eStatus
        {
            get
            {
                return (SolariumVisitStatus)Status;
            }
            set
            {
                Status = (short)value;
            }
        }

        public string StatusText
        {
            get
            {
                switch (eStatus)
                {
                    case SolariumVisitStatus.Canceled:
                        return "Отменен";
                    case SolariumVisitStatus.Completed:
                        return "Посещен";
                    case SolariumVisitStatus.Planned:
                        return "Запланирован";
                    case SolariumVisitStatus.Skipped:
                        return "Пропущен";
                }
                return "Неизвестен";
            }
        }

        public string TimeText
        {
            get
            {
                return VisitDate.ToString("H:mm") + " - " + VisitDate.AddMinutes(Amount).ToString("H:mm");
            }
        }
    }

    public enum SolariumVisitStatus : short
    {
        Planned = 0,
        Canceled = 1,
        Completed = 2,
        Skipped = 3
    }
}
