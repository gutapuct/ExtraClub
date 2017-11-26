using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class Employee : IInitable
    {
        [DataMember]
        public Customer SerializedCustomer { get; set; }

        [DataMember]
        public JobPlacement SerializedJobPlacement { get; set; }

        [DataMember]
        public string SerializedContractText { get; set; }

        [DataMember]
        public string SerializedCardNumber { get; set; }

        [DataMember]
        public bool SerializedHasJobPlacementDraft { get; set; }

        [DataMember]
        public string SerializedInOutStatus { get; set; }

        [DataMember]
        public bool IsAtWorkplace { get; set; }

        public JobPlacement FirstPlacement
        {
            get
            {
                return JobPlacements.Where(i => i.IsAsset).OrderBy(i => i.ApplyDate).FirstOrDefault();
            }
        }

        bool isInit = false;
        public void Init()
        {
            if (isInit) return;
            isInit = true;
            SerializedCustomer = BoundCustomer;
            var jobPlacement = JobPlacements.Where(i => i.IsAsset).OrderByDescending(i => i.CreatedOn).FirstOrDefault();
            if (jobPlacement != null)
            {
                jobPlacement.Init();
                SerializedJobPlacement = jobPlacement;
                SerializedContractText = jobPlacement.EmployeeCategory.IsPupilContract ? "Ученический" : "Трудовой";
            }
            SerializedHasJobPlacementDraft = JobPlacements.Any(i => !i.IsAsset);
            BoundCustomer.InitActiveCard();
            if (BoundCustomer.ActiveCard != null)
            {
                SerializedCardNumber = BoundCustomer.ActiveCard.CardBarcode;
            }
            var last = EmployeeVisits.OrderByDescending(i => i.CreatedOn).FirstOrDefault();
            if (last != null)
            {
                string date;
                if (last.CreatedOn.Date == DateTime.Today)
                {
                    date = last.CreatedOn.ToString("HH:mm");
                }
                else
                {
                    date = last.CreatedOn.ToString("dd.MM.yyyy HH:mm");
                }
                if (IsAtWorkplace = last.IsIncome)
                {
                    SerializedInOutStatus = "Сотрудник на работе с " + date;
                }
                else
                {
                    SerializedInOutStatus = "Сотрудник не на работе с " + date;
                }
            }
            else
            {
                IsAtWorkplace = false;
                SerializedInOutStatus = "Нет данных о посещении";
            }
        }
    }
}
