using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel
{
    partial class TreatmentConfig : IComparable, IInitable, IDataErrorInfo
    {
        public int CompareTo(object obj)
        {
            if (!(obj is TreatmentConfig))
            {
                return 0;
            }
            return TreatmentType.Name.CompareTo((obj as TreatmentConfig).TreatmentType.Name);
        }

        public bool Helper { get; set; }

        public int FullDuration
        {
            get
            {
                return TreatmentType.Duration * LengthCoeff;
            }
        }

        [DataMember]
        public int SerializedFullDuration { get; private set; }

        [DataMember]
        public string SerializedTreatmentTypeName { get; private set; }

#if BEAUTINIKA
        [DataMember]
        public Dictionary<Guid, decimal> TreatmentConfigGoodIds { get; set; }
#endif

#if BEAUTINIKA
        public string TreatmentKindText
        {
            get
            {
                return IsMainTreatment ? "Основная" : "Дополнительная";
            }
        }
#endif

        public void Init()
        {
            SerializedFullDuration = FullDuration;
            SerializedTreatmentTypeName = TreatmentType.Name;
#if BEAUTINIKA
            TreatmentConfigGoodIds = TreatmentConfigGoods.ToDictionary(i => i.GoodId, i => i.Amount);
#endif
        }

        public string Error
        {
            get
            {
                var error = new StringBuilder();

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
                    case "LengthCoeff":
                        if (LengthCoeff < 1 || LengthCoeff > 15) return "!";
                        break;
                }
                return String.Empty;
            }
        }

        public static IEnumerable<Tuple<int, int>> GetDisabledPeriods(string ages)
        {
            if (String.IsNullOrWhiteSpace(ages))
            {
                return new Tuple<int, int>[0];
            }
            try
            {
                var periods = ages.Replace(" ", "")
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Contains("-") ? i.Split('-') : new string[] { i, i }).Select(i => new Tuple<int, int>(Int32.Parse(i[0]), Int32.Parse(i[1]))).ToArray();

                return periods;
            }
            catch
            {
                return new Tuple<int, int>[0];
            }
        }

#if !BEAUTINIKA
        public IEnumerable<Tuple<int, int>> GetDisabledPeriods55609868()
        {
            return GetDisabledPeriods(DisableAges);
        }
#endif
    }
}
