using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class File
    {
        public string CategoryText
        {
            get
            {
                if (Category == 0) return "Входящий звонок - новый клиент";
                if (Category == 1) return "Входящий звонок - старый клиент";
                return "Сценарий";
            }
        }
    }
}
