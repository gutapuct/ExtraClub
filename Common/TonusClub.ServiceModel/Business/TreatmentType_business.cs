using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class TreatmentType : IComparable, IDataErrorInfo, IInitable
    {
        public int CompareTo(object obj)
        {
            if (!(obj is TreatmentType))
            {
                return 0;
            }
            return Name.CompareTo((obj as TreatmentType).Name);
        }

        public bool Helper { get; set; }

        [DataMember]
        public string SerializedGroupName { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!String.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Name":
                        if (String.IsNullOrWhiteSpace(Name)) return "!";
                        break;
                    case "Duration":
                        if (Duration <= 0 || Duration >= 1440) return "!";
                        break;
                }
                return String.Empty;
            }
        }

        public void Init()
        {
            if(TreatmentTypeGroupId.HasValue) {
                SerializedGroupName = TreatmentTypeGroup.Name;
            }
        }
    }
}
