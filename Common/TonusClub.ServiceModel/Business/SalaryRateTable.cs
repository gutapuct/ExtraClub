using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TonusClub.ServiceModel
{
    partial class SalaryRateTable : IDataErrorInfo
    {
        private bool _IsFirst;
        public bool IsFirst
        {
            get
            {
                return _IsFirst;
            }
            set
            {
                _IsFirst = value;
                OnPropertyChanged("IsFirst");
            }
        }

        public string Error
        {
            get
            {
                if (FromValue.HasValue && ToValue.HasValue)
                {
                    if (ToValue.Value < FromValue.Value) return "Неверные значения!";
                }
                return null;
            }
        }

        public string this[string columnName]
        {
            get {
                if (columnName == "FromValue" || columnName == "ToValue")
                {
                    if (FromValue.HasValue && ToValue.HasValue)
                    {
                        if (ToValue.Value < FromValue.Value) return "Неверные значения!";
                    }
                }
                return null;
            }
        }
    }
}
