using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel.Dictionaries
{
    public static class CusomerDictionaries
    {
        static CusomerDictionaries()
        {
            SocialStatuses = new Dictionary<int, string>();
            SocialStatuses.Add(0, "Домохозяйка/временно не работает");
            SocialStatuses.Add(1, "Пенсионер, не работает");
            SocialStatuses.Add(2, "Пенсионер, работает");
            SocialStatuses.Add(3, "Работает");
        }

        public static Dictionary<int, string> GetSocialStatuses()
        {
            return SocialStatuses;
        }

        public static Dictionary<int, string> SocialStatuses { get; private set; }

        internal static string GetSocialStatus(int? statusId)
        {
            if (!statusId.HasValue) return null;
            return SocialStatuses[statusId.Value];
        }
    }
}
