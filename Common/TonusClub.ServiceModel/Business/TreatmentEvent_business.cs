using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    public partial class TreatmentEvent : IInitable
    {
        public bool IsCorrectionEnabled
        {
            get
            {
                return VisitStatus == 0;
            }
        }

        public void Init()
        {
            if (Ticket != null)
            {
                SerializedTicketNumber = Ticket.Number;
            }
            else
            {
                SerializedTicketNumber = "Резерв";
            }

            Price = TreatmentConfig.Price;

#if BEAUTINIKA
            IsMainTreatment = TreatmentConfig.IsMainTreatment;
            SerializedRoomName = Room.Name;
            SerializedEmployeeName = Employee.BoundCustomer.ShortName;
#endif

            EndTime = VisitDate.AddMinutes(TreatmentConfig.FullDuration);
            SerializedTreatmentTypeName = Treatment.TreatmentType.Name;
            SerializedTreatmentName = Treatment.DisplayName;
            SerializedDuration = TreatmentConfig.FullDuration;
            SerializedCost = (int)TreatmentConfig.Price;
            SerializedTreatmentTypeId = TreatmentConfig.TreatmentTypeId;
            SerializedCustomerInfo = Customer.FullName;
            SerializedCustomerPhone = Customer.Phone2;

            Customer.InitActiveCard();
            if (Customer.ActiveCard != null) {
                SerializedCustomerInfo = "[" + Customer.ActiveCard.CardBarcode + "] " + SerializedCustomerInfo;
                SerializedCardNumber = Customer.ActiveCard.CardBarcode;
            }
            if (ProgramId.HasValue)
            {
                SerializedProgramName = TreatmentProgram.ProgramName;
            }
        }

#if BEAUTINIKA
        [DataMember]
        public bool IsMainTreatment { get; set; }
        [DataMember]
        public string SerializedRoomName { get; set; }
        [DataMember]
        public string SerializedEmployeeName { get; set; }
        [DataMember]
        public string HasGoodCharges { get; set; }

        public string MainTreatmentType
        {
            get
            {
                return IsMainTreatment?"Осн":"Доп";
            }
        }

        [DataMember]
        public int CostExtra { get; set; }
#endif

        [DataMember]
        public string SerializedCustomerInfo { get; private set; }

        [DataMember]
        public string SerializedCustomerPhone { get; private set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public int Cost { get; set; }

        [DataMember]
        public string SerializedTicketNumber { get; set; }

        [DataMember]
        public string SerializedTreatmentTypeName { get; set; }

        [DataMember]
        public string SerializedTreatmentName { get; set; }

        [DataMember]
        public string SerializedCardNumber { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public int SerializedDuration { get; set; }

        [DataMember]
        public string Employer { get; set; }

        public string StatusText
        {
            get
            {
                return GetStatus(VisitStatus);
            }
        }

        public bool CanBeCancelled
        {
            get
            {
                return VisitStatus == 0;
            }
        }

        public string TimeText
        {
            get
            {
                return VisitDate.ToString("H:mm") + " - " + VisitDate.AddMinutes(SerializedDuration).ToString("H:mm");
            }
        }

        [DataMember]
        public int SerializedCost { get; set; }

        [DataMember]
        public Guid SerializedTreatmentTypeId { get; set; }

        [DataMember]
        public string SerializedProgramName { get; set; }

        public bool Helper { get; set; }

        public static string GetStatus(int visitStatus)
        {
            switch (visitStatus)
            {
                case 0:
                    return "Запланирована";
                case 1:
                    return "Отменена";
                case 2:
                    return "Завершена";
                case 3:
                    return "Прогуляна";
                case -1:
                    return "Автоматически";
            }
            return "";
        }
    }

    public enum TreatmentEventStatus : short
    {
        Planned = 0,
        Canceled = 1,
        Completed = 2,
        Missed = 3
    }
}
