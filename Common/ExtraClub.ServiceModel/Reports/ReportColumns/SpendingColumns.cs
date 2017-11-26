    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(Spending))]
    public class SpendingColumns
    {
        Spending entity { get; set; }
        public SpendingColumns(Spending obj)
        {
            entity = obj;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Франчайзи")]
        public string f1 { get { return entity.Company.CompanyName; } }
        [Description("Клуб")]
        public string f2 { get { return entity.Division.Name; } }
        [Description("Дата")]
        public DateTime f3 { get { return entity.CreatedOn; } }
        [Description("Наименование")]
        public string f4 { get { return entity.Name; } }
        [Description("Создатель")]
        public string f5 { get { return entity.CreatedBy.FullName; } }
        [Description("Способ оплаты/цель")]
        public string f6 { get { return entity.PaymentType; } }
        [Description("Тип затраты")]
        public string f7 { get { return entity.SpendingType.Name; } }
        [Description("Сумма")]
        public decimal f8 { get { return entity.Amount; } }

    }
}
