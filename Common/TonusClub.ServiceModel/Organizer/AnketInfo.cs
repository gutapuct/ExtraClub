using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Organizer
{
    [DataContract]
    public class AnketInfo
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int StatusId { get; set; }

        [DataMember]
        public DateTime Period { get; set; }

        [DataMember]
        public string DivisionName { get; set; }

        public string StatusDescription
        {
            get
            {
                if (StatusId == 0) return "Черновик";
                if (StatusId == 1) return "Отправлена";
                if (StatusId == 2) return "Получена франчайзором";
                return "Некорректный статус";
            }
        }
    }
}
