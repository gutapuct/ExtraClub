using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(Treatment))]
    public class TreatmentColumns
    {
        public Treatment entity { get; set; }

        public TreatmentColumns(Treatment value)
        {
            entity = value;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Тип")]
        public string field1 { get { return entity.TreatmentType.Name; } }
        [Description("Название")]
        public string field2 { get { return entity.DisplayName; } }
        [Description("Максимум посетителей")]
        public int field3 { get { return entity.MaxCustomers; } }
        [Description("Номер договора")]
        public string field4 { get { return entity.DogNumber; } }
        [Description("Серийный номер")]
        public string field5 { get { return entity.SerialNumber; } }
        [Description("Дата поставки")]
        public string field6 { get { return entity.Delivery; } }
        [Description("Истечение гарантии")]
        public string field7 { get { return entity.GuaranteeExp; } }
        [Description("Истечение срока службы")]
        public string field8 { get { return entity.UseExp; } }
        [Description("Комментарий")]
        public string field9 { get { return entity.Comment; } }
        [Description("Франчайзи")]
        public string field10 { get { return entity.Division.Company.CompanyName; } }

        [Description("Клуб")]
        public string field11 { get { return entity.Division.Name; } }

        [Description("Число процедур всего")]
        public int field12
        {
            get
            {
                return entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
            }
        }

        [Description("Число процедур за год")]
        public int field13
        {
            get
            {
                var date = DateTime.Today.AddYears(-1);
                return entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
            }
        }

        [Description("Среднесуточная загрузка")]
        public decimal field14
        {
            get
            {
                var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
                var days = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays;
                if (days < 1) return 0;
                return procs / days;
            }
        }
        [Description("Среднесуточная загрузка за год")]
        public decimal field15
        {
            get
            {
                var date = DateTime.Today.AddYears(-1);
                var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
                var days = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays;
                if (days > 365) days = 365;
                if (days < 1) return 0;
                return procs / days;
            }
        }
        [Description("Средненедельная загрузка")]
        public decimal field16
        {
            get
            {
                var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
                var weeks = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays / 7;
                if (weeks < 1) return 0;
                return procs / weeks;
            }
        }
        [Description("Средненедельная загрузка за 364 дня")]
        public decimal field17
        {
            get
            {
                var date = DateTime.Today.AddDays(-364);
                var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
                var weeks = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays / 7;
                if (weeks > 52) weeks = 52;
                if (weeks < 1) return 0;
                return procs / weeks;
            }
        }
        [Description("Среднемесячная загрузка")]
        public decimal field18
        {
            get
            {
                var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
                var months = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays / 30.5m;
                if (months < 1) return 0;
                return procs / months;
            }
        }
        [Description("Среднемесячная загрузка за год")]
        public decimal field19
        {
            get
            {
                var date = DateTime.Today.AddYears(-1);
                var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
                var months = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays / 30.5m;
                if (months > 12) months = 12;
                if (months < 1) return 0;
                return procs / months;
            }
        }

        [Description("Стоимость оборудования")]
        public decimal? field20
        {
            get
            {
                return entity.Cost;
            }
        }
    }
}