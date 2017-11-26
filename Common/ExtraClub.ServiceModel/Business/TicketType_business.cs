using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class TicketType : IInitable, IDataErrorInfo
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
                    case "Name":
                        if (String.IsNullOrWhiteSpace(Name)) return "!";
                        break;
                }
                return null;
            }
        }

        public double _Price
        {
            get
            {
                return (double)Price;
            }
            set
            {
                Price = (decimal)value;
                OnPropertyChanged("_Price");
            }
        }
        
        public string TotalUnitsString
        {
            get
            {
                return String.Format("{0:n0}, гостевых - {1:n0}", Units, GuestUnits);
            }
        }

        [DataMember]
        public string RestrictionsText
        {
            get; set;
        }

        public void Init()
        {
            RestrictionsText = String.Empty;
            foreach (var r in TreatmentTypes)
            {
                if (!String.IsNullOrEmpty(RestrictionsText)) RestrictionsText += "; ";
                RestrictionsText += r.Name;
            }
            SerializedTreatmentTypes = TreatmentTypes;
            SerializedCustomerCardTypes = CustomerCardTypes;
            SerializedTicketTypeLimits = TicketTypeLimits.ToList();
        }

        public string VistTimeString
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(VisitStart) && !String.IsNullOrWhiteSpace(VisitEnd)) return "" + VisitStart[0] + VisitStart[1] + ":" + VisitStart[2] + VisitStart[3] + " - " + VisitEnd[0] + VisitEnd[1] + ":" + VisitEnd[2] + VisitEnd[3];
                return String.Empty;
            }
        }

        [DataMember]
        public ICollection<TreatmentType> SerializedTreatmentTypes { get; set; }

        [DataMember]
        public ICollection<CustomerCardType> SerializedCustomerCardTypes { get; set; }

        private bool _Helper;
        [DataMember]
        public bool Helper
        {
            get
            {
                return _Helper;
            }
            set
            {
                _Helper = value;
                OnPropertyChanged("Helper");
            }
        }

        [XmlIgnore]
        public string TextDescription
        {
            get
            {
                var res = new StringBuilder();
                res.Append(Name);
                if (IsGuest) res.Append(" (гостевой)");
                if (IsVisit) res.Append(" (обзорный)");
                if (IsAction) res.Append(" (акционный)");
                if (!String.IsNullOrWhiteSpace(Comments))
                {
                    res.Append("\n");
                    res.Append(Comments);
                }
                if (Price > 0)
                {
                    res.AppendFormat("\nЦена: {0:c}", Price);
                }
                res.AppendFormat("\nСрок действия в днях: {0}", Length);

                res.AppendFormat("\nЧасы посещения клуба: {0}", VistTimeString);

                res.AppendFormat("\nЕдиниц: {0}", TotalUnitsString);

                if (SolariumMinutes > 0)
                {
                    res.AppendFormat("\nМинут солярия: {0:n0}", SolariumMinutes);
                }
                return res.ToString();
            }
        }

        [DataMember]
        public List<TicketTypeLimit> SerializedTicketTypeLimits { get; set; }
    }
}
