using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class ScheduleProposal : IComparable, INotifyPropertyChanged
    {
        [DataMember]
        public List<ScheduleProposalElement> List { get; set; }
        [DataMember]
        public DateTime MinTime { get; set; }
        [DataMember]
        public DateTime MaxTime { get; set; }

        [DataMember]
        public Guid ProgramId { get; set; }

        private bool _prefer;

        [DataMember]
        public bool Prefer
        {
            get
            {
                return _prefer;
            }
            set
            {
                if (_prefer == value) return;
                _prefer = value;
                OnPropertyChanged("Prefer");
            }
        }

        public string TimeText
        {
            get
            {
                return String.Format("{0:H:mm} — {1:H:mm}", List.Min(i=>i.StartTime), List.Max(i=>i.EndTime));
            }
        }

        public ScheduleProposal()
        {
            List = new List<ScheduleProposalElement>();
            Duration = new Lazy<int>(() =>
            {
                if (List.Count == 0) return 0;
                return (int)(List.Max(i => i.EndTime) - List.Min(i => i.StartTime)).TotalMinutes;
            });
        }

        public Lazy<int> Duration; 

        public int CompareTo(object obj)
        {
            if (Prefer) return -1;
            if (!(obj is ScheduleProposal)) return 0;
            var o = obj as ScheduleProposal;
            return Duration.Value.CompareTo(o.Duration.Value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public int GetMaxParallel()
        {
            var res = 0;
            for(var i=0;i<List.Count;i++)
            {
                var ires = 0;
                for (var j = 0; j < List.Count; j++)
                {
                    if (DatesIntersects(List[i].StartTime, List[i].EndTime, List[j].StartTime, List[j].EndTime)) ires++;
                }
                res = Math.Max(res, ires);
            }
            return res;
        }

        public static bool DatesIntersects(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return end2 > start1 && end1 > start2;
        }

    }
}
