using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class Job : IInitable
    {
        public bool Helper { get; set; }

        [DataMember]
        public string SerializedFullName { get; set; }

        [DataMember]
        public string SerializedCategoriesList { get; set; }

        [DataMember]
        public List<Guid> SerializedCategoryIds { get; set; }

        [DataMember]
        public string SerializedSchemaName { get; set; }

        public void Init()
        {
            if (SalarySchemeId.HasValue)
            {
                SerializedSchemaName = SalaryScheme.Name;
            }
            SerializedFullName = Division.Name + "\\" + Name;
            SerializedCategoryIds = new List<Guid>();
            foreach (var i in EmployeeCategories)
            {
                SerializedCategoryIds.Add(i.Id);
                if (!String.IsNullOrEmpty(SerializedCategoriesList))
                {
                    SerializedCategoriesList += "; ";
                }
                SerializedCategoriesList += i.Name;
            }
        }

        public string MainWorkplaceText
        {
            get
            {
                return IsMainWorkplace ? "осн" : "совм";
            }
        }

        public string WorkGraphTimeText
        {
            get
            {
                return string.Format("{0:h\\:mm}", WorkStart) + " - " + string.Format("{0:h\\:mm}", WorkEnd);
            }
        }

        public double WorkDayLength
        {
            get
            {
                return (double)(WorkEnd - WorkStart).TotalHours;
            }
        }
    }
}
