using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    public partial class TextAction : IDataErrorInfo, IInitable
    {
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
                    case "ActionText":
                        if (String.IsNullOrWhiteSpace(this.ActionText)) return "!";
                        break;
                    case "StartDate":
                        if (StartDate.Date > FinishDate.Date) return "!";
                        break;
                    case "FinishDate":
                        if (StartDate.Date > FinishDate.Date) return "!";
                        break;
                }
                return null;
            }
        }

        public void Init()
        {
            SerializedActiveDivisionIds = Divisions.Select(i => i.Id).ToArray();
        }

        [DataMember]
        public Guid[] SerializedActiveDivisionIds { get; set; }
    }
}