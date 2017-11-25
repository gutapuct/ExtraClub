using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TonusClub.ServiceModel
{
    partial class JobPlacement : IDataErrorInfo, IInitable
    {

        [DataMember]
        public string SerializedJobName { get; set; }

        [DataMember]
        public string SerializedWorkTime { get; set; }

        [DataMember]
        public string SerializedCategoryName { get; set; }

        [DataMember]
        public string SerializedFullName { get; set; }

        [DataMember]
        public string SerializedUnit { get; set; }

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
                    case "JobId":
                        if (JobId == Guid.Empty) return "!";
                        break;
                    case "CategoryId":
                        if (CategoryId == Guid.Empty) return "!";
                        break;
                    case "ApplyDate":
                        if (ApplyDate < DateTime.Now.AddYears(-20)) return "Некорректная дата приема";
                        break;
                }
                return null;
            }
        }

        public decimal Salary
        {
            get
            {
                return Job.Salary * EmployeeCategory.SalaryMulti;
            }
        }

        public void Init()
        {
            SerializedJobName = Job.Name;
            SerializedUnit = Job.Unit;
            SerializedCategoryName = EmployeeCategory.Name;
            SerializedWorkTime = Job.WorkStart.Hours.ToString("0") + ":" + Job.WorkStart.Minutes.ToString("00")
                + " - " + Job.WorkEnd.Hours.ToString("0") + ":" + Job.WorkEnd.Minutes.ToString("00");
            SerializedFullName = Employee.BoundCustomer.FullName;
        }

        public bool ContainsDate(DateTime date)
        {
            if (FireDate.HasValue)
            {
                if (FireDocumentId.HasValue)
                {
                    return ApplyDate <= date && FireDate.Value >= date;
                }
                else
                {
                    return ApplyDate <= date && FireDate.Value > date;
                }
            }
            else
            {
                return ApplyDate <= date;
            }
        }

        [XmlIgnore]
        public DateTime EffectiveEndDate
        {
            get
            {
                if (FireDate.HasValue)
                {
                    if (FireDocumentId.HasValue)
                    {
                        return FireDate.Value;
                    }
                    else
                    {
                        return FireDate.Value.AddDays(-1);
                    }
                }
                else
                {
                    return DateTime.MaxValue;
                }

            }
        }
    }
}
