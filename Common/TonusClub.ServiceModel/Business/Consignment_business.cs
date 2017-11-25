using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    public partial class Consignment : IInitable
    {
        [DataMember]
        public string SerializedSource { get; set; }
        [DataMember]
        public string SerializedStatus { get; set; }
        [DataMember]
        public string SerializedDestination { get; set; }
        [DataMember]
        public string SerializedCreatedBy { get; set; }
        [DataMember]
        public bool IsReadOnly { get; set; }
        [DataMember]
        public decimal SerializedTotalPayment { get; set; }
        [DataMember]
        public DateTime? SerializedLastPaymentDate { get; set; }


        [DataMember]
        public List<ConsignmentLine> SerializedConsignmentLines { get; set; }

        public void Init()
        {
            IsReadOnly = IsAsset;
            if(DocType == 0 || DocType == 3)
            {
                SerializedSource = Provider.Name;
                SerializedDestination = DestinationStorehouse.Name;
            }
            else if(DocType == 1)
            {
                SerializedSource = SourceStorehouse.Name;
                SerializedDestination = DestinationStorehouse.Name;
            }
            else if(DocType == 2)
            {
                SerializedSource = SourceStorehouse.Name;
            }
            if(DocType == 3)
            {
                SerializedStatus = "Заказано";
                IsReadOnly = ProviderPayments.Count > 0 || Consignments1.Count > 0;
                if(ProviderPayments.Count > 0)
                {
                    SerializedTotalPayment = ProviderPayments.Sum(i => i.Amount);
                    SerializedLastPaymentDate = ProviderPayments.Max(i => i.Date);
                }
                if(Consignments1.Count > 0)
                {
                    SerializedStatus = "Частично получено";
                    var gd = new Dictionary<Guid, double>();
                    foreach(var i in ConsignmentLines)
                    {
                        if(gd.ContainsKey(i.GoodId)) gd[i.GoodId] += i.Quantity ?? 0;
                        else
                        {
                            gd.Add(i.GoodId, i.Quantity ?? 0);
                        }
                    }
                    foreach(var i in Consignments1)
                    {
                        foreach(var j in i.ConsignmentLines)
                        {
                            if(gd.ContainsKey(j.GoodId))
                            {
                                gd[j.GoodId] -= j.Quantity ?? 0;
                            }
                        }
                    }
                    var flag = true;
                    foreach(var i in gd)
                    {
                        if(i.Value > 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if(flag)
                    {
                        SerializedStatus = "Получено";
                    }
                }
            }
            SerializedConsignmentLines = ConsignmentLines.ToList();
            SerializedConsignmentLines.ForEach(l => l.Init());
            SerializedCreatedBy = CreatedBy.FullName;
        }

        partial void OnDeserialized()
        {
            ((FixupCollection<ConsignmentLine>)ConsignmentLines).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Consignment_CollectionChanged);
        }

        void Consignment_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems == null) return;
            if(e.NewItems.Count == 1 && !((ConsignmentLine)(e.NewItems[0])).Position.HasValue)
            {
                ((ConsignmentLine)(e.NewItems[0])).Position = (short)ConsignmentLines.Count;
            }
        }

        public decimal Amount
        {
            get
            {
                if(ConsignmentLines != null && ConsignmentLines.Count > 0)
                {
                    return ConsignmentLines.Sum(l => l.Cost);
                }
                else if(SerializedConsignmentLines != null && SerializedConsignmentLines.Count > 0)
                {
                    return SerializedConsignmentLines.Sum(l => l.Cost);
                }
                else return 0;
            }
        }

        public decimal Balance
        {
            get
            {
                return SerializedTotalPayment - Amount;
            }
        }

        internal void UpdateAmount()
        {
            OnPropertyChanged("Amount");
        }

        public string DocTypeText
        {
            get
            {
                switch(DocType)
                {
                    case 0:
                        return "Приход";
                    case 1:
                        return "Перемещение";
                    case 2:
                        return "Списание";
                }
                return "";
            }
        }

        public string AssetText
        {
            get
            {
                return IsAsset ? "Проведен" : "Не проведен";
            }
        }

        public int TotalAmount
        {
            get
            {
                if(ConsignmentLines != null && ConsignmentLines.Count > 0)
                    return (int)ConsignmentLines.Sum(i => i.Quantity);
                return 0;
            }
        }

        public decimal TotalCost
        {
            get
            {
                if(ConsignmentLines != null && ConsignmentLines.Count > 0)
                    return ConsignmentLines.Sum(i => i.Cost);
                return 0;
            }
        }
    }
}
