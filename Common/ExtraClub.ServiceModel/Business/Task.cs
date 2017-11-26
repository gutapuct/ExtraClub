using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class Task
    {
        public string StatusText
        {
            get
            {
                if (StatusId == 0) return "Поставлена";
                else if (StatusId == 1) return "Выполнена";
                else if (StatusId == 2) return "Отказ";
                else return "Отозвана";
            }
        }
    }
}
