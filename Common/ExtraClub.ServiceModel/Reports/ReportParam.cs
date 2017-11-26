using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports
{
    [DataContract]
    public class ReportParamInt : INotifyPropertyChanged
    {
        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string InternalName { get; set; }

        [DataMember]
        public ReportParameterType Type { get; set; }

        public event EventHandler InstanceValueChanged;

        object _instanceValue;
        [DataMember]
        public object InstanceValue
        {
            get
            {
                return _instanceValue;
            }
            set
            {
                _instanceValue = value;
                if (InstanceValueChanged != null) InstanceValueChanged.Invoke(this, new EventArgs());
                OnPropertyChanged("InstanceValue");
            }
        }

        [DataMember]
        public string GetValuesDelegateType { get; set; }


        private object _list;
        public object List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                if (_list is List<SelectListItem>)
                {
                    var l = _list as List<SelectListItem>;
                    l.ForEach(i => i.Changed += changed);
                }
                OnPropertyChanged("List");
            }
        }

        private void changed(object sender, EventArgs e)
        {
            var l = _list as List<SelectListItem>;
            l.ForEach(i => i.Changed += changed);

            InstanceValue = String.Join(";", l.Where(i => i.Helper).Select(i => i.Key.ToString()).ToArray());
        }

        public static ReportParamInt CreateFromContext(ReportParameter i)
        {
            return new ReportParamInt
            {
                DisplayName = i.Name,
                InternalName = i.IntName,
                Type = (ReportParameterType)i.Type
            };
        }

        public CultureInfo MonthCulture
        {
            get
            {
                CultureInfo cultureInfo = new CultureInfo("ru-RU");
                DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
                dateInfo.ShortDatePattern = "MMMM yyyy";
                //dateInfo.ShortTimePattern = "HH:mm";
                cultureInfo.DateTimeFormat = dateInfo;
                cultureInfo.DateTimeFormat.MonthNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь", "" };
                cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек", "" };
                return cultureInfo;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum ReportParameterType : int
    {
        Date = 0,
        Division = 1,
        Month = 2,
        Boolean = 3,
        String = 4,
        CustomDropdown = 5,
        Good = 6,
        Employee = 7,
        Company = 8,
        Employees = 9
    }
}