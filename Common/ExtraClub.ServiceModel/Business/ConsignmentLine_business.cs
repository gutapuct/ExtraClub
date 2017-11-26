using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    public partial class ConsignmentLine : IComparable, IInitable
    {
        [DataMember]
        public string SerializedGoodName { get; set; }

        public ConsignmentLine()
        {
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ConsignmentLine_PropertyChanged);
        }

        partial void OnDeserialized()
        {
            PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(ConsignmentLine_PropertyChanged);
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ConsignmentLine_PropertyChanged);
        }

        void ConsignmentLine_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Price" || e.PropertyName == "Quantity")
            {
                OnPropertyChanged("Cost");
                if (Consignment != null)
                {
                    this.Consignment.UpdateAmount();
                }
            }
        }

        public decimal Cost
        {
            get
            {
                var res = (Price ?? 0) * (decimal)(Quantity ?? 0);
                return res;
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is ConsignmentLine)) return 0;
            return (Position??0).CompareTo((obj as ConsignmentLine).Position);
        }

        public void Init()
        {
            SerializedGoodName = Good.Name;
        }
    }
}
