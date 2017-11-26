using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(TreatmentEvent))]
    public class TreatmentEventColumns
    {
        public TreatmentEvent entity { get; set; }

        public TreatmentEventColumns(TreatmentEvent value)
        {
            entity = value;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Тип услуги")]
        public string field1
        {
            get
            {
                return entity.TreatmentConfig.TreatmentType.Name;
            }
        }

        [Description("Группа")]
        public string field2
        {
            get
            {
                return entity.TreatmentConfig.TreatmentType.TreatmentTypeGroupId.HasValue ? entity.TreatmentConfig.TreatmentType.TreatmentTypeGroup.Name : null;
            }
        }

        [Description("Несколько занимающихся")]
        public string field4
        {
            get
            {
                return entity.TreatmentConfig.TreatmentType.AllowsMultiple ? "Да" : "Нет";
            }
        }

        [Description("Название услуги")]
        public string field5
        {
            get
            {
                return entity.TreatmentConfig.Name;
            }
        }

        [Description("Длительность")]
        public int field6
        {
            get
            {
                return entity.TreatmentConfig.FullDuration;
            }
        }

        [Description("Цена")]
        public decimal field7
        {
            get
            {
                return entity.TreatmentConfig.Price;
            }
        }

        [Description("Франчайзи")]
        public string field8
        {
            get
            {
                return entity.Division.Company.CompanyName;
            }
        }

        [Description("Клуб")]
        public string field9
        {
            get
            {
                return entity.Division.Name;
            }
        }

        [Description("Оборудование")]
        public string field10
        {
            get
            {
                return entity.Treatment.DisplayName;
            }
        }

        [Description("Статус")]
        public string field11
        {
            get
            {
                return entity.StatusText;
            }
        }


        [Description("Номер карты")]
        public string field12
        {
            get
            {
                entity.Customer.InitActiveCard();
                if (entity.Customer.ActiveCard == null) return null;
                return entity.Customer.ActiveCard.CardBarcode;
            }
        }

        [Description("Тип карты")]
        public string field13
        {
            get
            {
                entity.Customer.InitActiveCard();
                if (entity.Customer.ActiveCard == null) return null;
                return entity.Customer.ActiveCard.CustomerCardType.Name;
            }
        }

        [Description("Абонемент")]
        public string field14
        {
            get
            {
                if (!entity.TicketId.HasValue) return null;
                return entity.Ticket.Number;
            }
        }

        [Description("Тип абонемента")]
        public string field15
        {
            get
            {
                if (!entity.TicketId.HasValue) return null;
                return entity.Ticket.TicketType.Name;
            }
        }

        [Description("Клиент")]
        public string field16
        {
            get
            {
                return entity.Customer.FullName;
            }
        }

        [Description("Статусы")]
        public string field17
        {
            get
            {
                if (!entity.Customer.CustomerStatuses.Any()) return null;
                var sb = new StringBuilder();
                entity.Customer.CustomerStatuses.OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(i.Name);
                });
                return sb.ToString();
            }
        }

        [Description("Программа")]
        public string field18
        {
            get
            {
                if (!entity.ProgramId.HasValue) return null;
                return entity.TreatmentProgram.ProgramName;
            }
        }

        [Description("Дата проведения")]
        public DateTime field3
        {
            get
            {
                return entity.VisitDate;
            }
        }

        [Description("Дата создания")]
        public DateTime field19
        {
            get
            {
                return entity.CreatedOn;
            }
        }

    }
}
