using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using TonusClub.ServiceModel;

namespace TonusClub.Infrastructure
{
    public class TreatmentPlan : INotifyPropertyChanged
    {
        public TreatmentPlan()
        {
        }

        public IEnumerable<TreatmentConfig> TreatmentConfigs { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DurationText { get; set; }

        public virtual System.Guid TreatmentConfigId
        {
            get { return _treatmentConfigId; }
            set
            {
                if (_treatmentConfigId != value)
                {
                    _treatmentConfigId = value;
                    var tt = TreatmentConfigs.FirstOrDefault(i => i.Id == _treatmentConfigId);
                    if (tt != null)
                    {
                        DurationText = tt.SerializedFullDuration.ToString() + " мин.";
                        OnPropertyChanged("DurationText");
                    }
                    OnPropertyChanged("TreatmentConfigId");
                }
            }
        }
        private System.Guid _treatmentConfigId;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
