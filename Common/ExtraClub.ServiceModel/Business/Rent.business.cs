using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class Rent : IInitable
    {
        [DataMember]
        public string SerializedGoodName { get; private set; }
        [DataMember]
        public string SerializedStorehouseName { get; private set; }

        public Rent()
        {
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Rent_PropertyChanged);
        }

        partial void OnDeserialized()
        {
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Rent_PropertyChanged);
        }

        public decimal AmountToPay
        {
            get
            {
                return SumToPay + (LostFine ?? 0) + (OverdueFine ?? 0);
            }
        }

        void Rent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReturnDate")
            {
                OnPropertyChanged("Length");
                OnPropertyChanged("Cost");
            }
            if (e.PropertyName == "GoodId")
            {
                OnPropertyChanged("Cost");
            }

            if (e.PropertyName == "Cost" || e.PropertyName == "LostFine" || e.PropertyName == "OverdueFine")
            {
                OnPropertyChanged("AmountToPay");
            }

        }

        public void Init()
        {
            SerializedGoodName = Good.Name;
            SerializedStorehouseName = Storehouse.Name;
        }

        public int Length
        {
            get
            {
                return (int)Math.Max(1, Math.Ceiling((ReturnDate - CreatedOn).TotalDays));
            }
        }

        public decimal Cost
        {
            get
            {
                return Price * Length;
            }
        }

        public string Status
        {
            get
            {
                if (LostFine.HasValue) return Localization.Resources.Missed;
                if (FactReturnDate.HasValue) return Localization.Resources.Returned;
                if (OverdueFine.HasValue) return Localization.Resources.ReturnedOverdue;
                if (ReturnDate < DateTime.Now) return Localization.Resources.Overdue;
                return Localization.Resources.Issued;
            }
        }

        public decimal SumToPay
        {
            get
            {
                return IsPayed ? 0 : Cost;
            }
        }
    }
}
