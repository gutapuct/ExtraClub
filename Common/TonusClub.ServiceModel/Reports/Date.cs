using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Reports
{
    public struct Date : IComparable, IComparable<Date>, IEqualityComparer<Date>, IEquatable<Date>
    {
        [DataMember]
        public DateTime DateTime { get; set; }

        public static implicit operator Date(DateTime x)
        {
            return new Date { DateTime = x.Date };
        }

        public static bool operator ==(Date a, Date b)
        {
            return a.DateTime == b.DateTime;
        }
        public static bool operator !=(Date a, Date b)
        {
            return a.DateTime != b.DateTime;
        }

        public override int GetHashCode()
        {
            return DateTime.Date.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Date))
            {
                return false;
            }

            return this == (Date)obj;
        }

        public override string ToString()
        {
            return DateTime.ToString("dd.MM.yyyy");
        }

        public int CompareTo(Date other)
        {
            return DateTime.CompareTo(other.DateTime);
        }

        public int CompareTo(object obj)
        {
            return DateTime.CompareTo(obj);
        }

        public bool Equals(Date other)
        {
            return other.DateTime.Date == this.DateTime.Date;
        }

        public bool Equals(Date x, Date y)
        {
            return x.DateTime.Date == y.DateTime.Date;
        }

        public int GetHashCode(Date obj)
        {
            return obj.DateTime.Date.GetHashCode();
        }
    }
}
