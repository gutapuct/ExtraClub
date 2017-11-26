using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Media;

namespace ExtraClub.ServiceModel.Employees
{
    public class EmployeeWorkScheduleItem : INotifyPropertyChanged
    {
        [DataMember]
        public JobPlacement JobPlacement { get; set; }

        [DataMember]
        public DatesDictionary Dates { get; set; }

        public int TotalDays
        {
            get
            {
                return Dates.Count(o => o.Value.IsSet);
            }
        }

        public EmployeeWorkScheduleItem()
        {
            Dates = new DatesDictionary();
            Init();
        }

        public void Init()
        {
            Dates.ItemAdded += new EventHandler<DateDataArgs>(Dates_ItemAdded);
            foreach (var i in Dates)
            {
                i.Value.PropertyChanged += DateData_PropertyChanged;
            }
        }


        void Dates_ItemAdded(object sender, DateDataArgs e)
        {
            e.DateData.PropertyChanged += new PropertyChangedEventHandler(DateData_PropertyChanged);
        }

        void DateData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSet")
            {
                OnPropertyChanged("TotalDays");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class DateData : INotifyPropertyChanged
    {
        public bool IsHoliday;

        [DataMember]
        public DateTime Date { get; set; }

        public Brush Background
        {
            get
            {
                if (IsHoliday || Date.DayOfWeek == DayOfWeek.Sunday || Date.DayOfWeek == DayOfWeek.Saturday) return Brushes.Salmon;
                return Brushes.Transparent;
            }
        }

        [DataMember]
        public bool IsEnabled { get; set; }

        private bool _IsSet;
        [DataMember]
        public bool IsSet
        {
            get
            {
                return _IsSet;
            }
            set
            {
                _IsSet = value;
                OnPropertyChanged("IsSet");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class DatesDictionary : IDictionary<DateTime, DateData>
    {
        public event EventHandler<DateDataArgs> ItemAdded;

        private Dictionary<DateTime, DateData> dict = new Dictionary<DateTime, DateData>();
        
        public void Add(DateTime key, DateData value)
        {
            dict.Add(key, value);
            if (ItemAdded != null)
            {
                ItemAdded.Invoke(this, new DateDataArgs { DateData = value });
            }
        }

        public bool ContainsKey(DateTime key)
        {
            return dict.ContainsKey(key);
        }

        public ICollection<DateTime> Keys
        {
            get { return dict.Keys; }
        }

        public bool Remove(DateTime key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(DateTime key, out DateData value)
        {
            throw new NotImplementedException();
        }

        public ICollection<DateData> Values
        {
            get { return dict.Values; }
        }

        public DateData this[DateTime key]
        {
            get
            {
                return dict[key];
            }
            set
            {
                dict[key] = value;
            }
        }

        public void Add(KeyValuePair<DateTime, DateData> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(KeyValuePair<DateTime, DateData> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<DateTime, DateData>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return dict.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<DateTime, DateData> item)
        {
            return dict.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<DateTime, DateData>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }

    public class DateDataArgs : EventArgs
    {
        public DateData DateData { get; set; }
    }
}
