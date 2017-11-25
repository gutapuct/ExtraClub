using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class TicketFreeze
    {
        public TicketFreeze()
        {
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TicketFreeze_PropertyChanged);
        }

        void TicketFreeze_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StartDate" || e.PropertyName == "FinishDate")
                OnPropertyChanged("Length");
        }

        [DataMember]
        public string SerializedName { get; internal set; }

        public int Length
        {
            get
            {
                return ((TimeSpan)(FinishDate - StartDate)).Days + 1;
            }
        }
    }
}
