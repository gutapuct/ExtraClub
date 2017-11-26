using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ExtraClub.ServiceModel.Dictionaries;

namespace ExtraClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(Customer))]
    public class CustomerColumns
    {
        public Customer entity { get; set; }
        public CustomerColumns(Customer value)
        {
            entity = value;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Франчайзи")]
        [Include("Company")]
        public string Fr
        {
            get
            {
                return entity.Company.CompanyName;
            }
        }

        [Description("Первый клуб")]
        [Include("Company","Company.Divisions")]
        public string Club
        {
            get
            {
                return entity.Company.Divisions.Where(i => i.Id == entity.ClubId).Select(i => i.Name).FirstOrDefault();
            }
        }

        [Description("Клуб последнего аб-та")]
        [Include("Tickets", "Tickets.Division")]
        public string Club2
        {
            get
            {
                if (!entity.Tickets.Any()) return null;
                return entity.Tickets.OrderByDescending(i => i.CreatedOn).First().Division.Name;
            }
        }



        [Description("Дата внесения")]
        public DateTime CreatedOn
        {
            get
            {
                return entity.CreatedOn;
            }
        }

        [Description("Дата рождения")]
        public DateTime? DR
        {
            get
            {
                return entity.Birthday;
            }
        }


        [Description("Возраст")]
        public int? Age
        {
            get
            {
                if (!entity.Birthday.HasValue) return null;
                return (int)Math.Floor((DateTime.Today - entity.Birthday.Value).TotalDays / 365);
            }
        }

        [Description("Фамилия")]
        public string LastName
        {
            get
            {
                return entity.LastName;
            }
        }
        [Description("Имя")]
        public string FirstName
        {
            get
            {
                return entity.FirstName;
            }
        }
        [Description("Отчество")]
        public string MiddleName
        {
            get
            {
                return entity.MiddleName;
            }
        }
        [Description("Коэффициент активности")]
        [Include("TreatmentEvents", "TreatmentEvents.TreatmentConfig", "TreatmentEvents.TreatmentConfig.TreatmentType", "TreatmentEvents.TreatmentConfig.TreatmentType.TreatmentTypeGroup")]
        public decimal? KAct
        {
            get
            {
                if (entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count() == 0) return null;
                var actives = entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Where(e => e.TreatmentConfig.TreatmentType.TreatmentTypeGroupId.HasValue && e.TreatmentConfig.TreatmentType.TreatmentTypeGroup.Name == "Активный");
                if (actives.Count() == 0) return 0;
                return (decimal)actives.Count() / (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
            }
        }
        [Description("Средний чек")]
        [Include("BarOrders", "BarOrders.GoodSales")]
        public decimal? KAct1
        {
            get
            {
                var sales = entity.BarOrders.SelectMany(i => i.GoodSales);
                if (sales.Count() == 0) return null;
                decimal days = entity.BarOrders.Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
                if (days == 0) return null;

                return sales.Sum(i => i.Cost) / days;
            }
        }
        [Description("Средний чек за последний месяц")]
        [Include("BarOrders", "BarOrders.GoodSales")]
        public decimal? KAct2
        {
            get
            {
                var date = DateTime.Now.AddMonths(-1);
                var sales = entity.BarOrders.Where(i => i.PurchaseDate >= date).SelectMany(i => i.GoodSales);
                if (sales.Count() == 0) return null;
                var days = entity.BarOrders.Where(i => i.PurchaseDate >= date).Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
                if (days == 0) return null;

                return sales.Sum(i => i.Cost) / (decimal)days;
            }
        }
        [Description("Средний чек за последний год")]
        [Include("BarOrders", "BarOrders.GoodSales")]
        public decimal? KAct3
        {
            get
            {
                var date = DateTime.Now.AddYears(-1);
                var sales = entity.BarOrders.Where(i => i.PurchaseDate >= date).SelectMany(i => i.GoodSales);
                if (sales.Count() == 0) return null;
                var days = entity.BarOrders.Where(i => i.PurchaseDate >= date).Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
                if (days == 0) return null;

                return sales.Sum(i => i.Cost) / (decimal)days;
            }
        }
        [Description("Средний чек с момента активации последнего аб-та")]
        [Include("BarOrders", "BarOrders.GoodSales", "Tickets")]
        public decimal? KAct4
        {
            get
            {
                var ticks = entity.Tickets.Where(i => i.StartDate.HasValue);
                if (ticks.Count() == 0) return null;
                var date = ticks.Max(i => i.StartDate);
                var sales = entity.BarOrders.Where(i => i.PurchaseDate >= date).SelectMany(i => i.GoodSales);
                if (sales.Count() == 0) return null;
                var days = entity.BarOrders.Where(i => i.PurchaseDate >= date).Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
                if (days == 0) return null;

                return sales.Sum(i => i.Cost) / (decimal)days;
            }
        }

        [Description("Серия и номер паспорта")]
        public string pasp1
        {
            get
            {
                return entity.PasspNumber;
            }
        }
        [Description("Кем выдан паспорт")]
        public string pasp2 { get { return entity.PasspEmitPlace; } }
        [Description("Когда выдан паспорт")]
        public DateTime? pasp3 { get { return entity.PasspEmitDate; } }
        [Description("Дом. телефон")]
        public string field1 { get { return entity.Phone1; } }
        [Description("Эл. почта")]
        public string field2 { get { return entity.Email; } }
        [Description("Моб.телефон")]
        public string field3 { get { return entity.Phone2; } }
        [Description("Индекс")]
        public string field4 { get { return entity.AddrIndex; } }
        [Description("Город")]
        public string field5 { get { return entity.AddrCity; } }
        [Description("Улица")]
        public string field6 { get { return entity.AddrStreet; } }
        [Description("Дом, квартира")]
        public string field7 { get { return entity.AddrOther; } }
        [Description("Метро")]
        public string field8 { get { return entity.AddrMetro; } }
        [Description("Рекламная группа")]
        [Include("AdvertType", "AdvertType.AdvertGroup")]
        public string field8a { get { return entity.AdvertTypeId.HasValue ? entity.AdvertType.AdvertGroup.Name : null; } }
        [Description("Рекламный канал")]
        [Include("AdvertType")]
        public string field8b { get { return entity.AdvertTypeId.HasValue ? entity.AdvertType.Name : null; } }
        [Description("Дата покупки первой карты")]
        [Include("CustomerCards")]
        public DateTime? field9
        {
            get
            {
                if (!entity.CustomerCards.Any()) return null;
                return entity.CustomerCards.Min(i => i.EmitDate);
            }
        }
        [Description("Тип активной карты")]
        [Include("CustomerCards", "CustomerCards.CustomerCardType")]
        public string field10
        {
            get
            {
                if (!entity.CustomerCards.Any()) return null;
                return entity.CustomerCards.OrderByDescending(i => i.EmitDate).First().CustomerCardType.Name;
            }
        }
        [Description("Дата последней замены карты")]
        [Include("CustomerCards")]
        public DateTime? field11
        {
            get
            {
                if (entity.CustomerCards.Count < 2) return null;
                return entity.CustomerCards.Max(i => i.EmitDate);
            }
        }
        [Description("Всего аб-тов")]
        [Include("Tickets")]
        public int field12
        {
            get
            {
                return entity.Tickets.Count;
            }
        }
        [Description("Последнияя дата истечения аб-та*")]
        [Include("Tickets")]
        public DateTime? field13
        {
            get
            {
                if (!entity.Tickets.Any(i => { i.InitDetails(); return i.FinishDate.HasValue; })) return null;
                return entity.Tickets.Max(i => { i.InitDetails(); return i.FinishDate; });
            }
        }

        [Description("Наличие активного аб-та")]
        [Include("Tickets")]
        public string field14
        {
            get
            {
                return entity.Tickets.Any(i => i.IsActive) ? "Да" : "Нет";
            }
        }

        [Description("Дней до оконч. последнего проданного аб-та")]
        [Include("Tickets", "Tickets.TicketType")]
        public int? field15
        {
            get
            {
                if (!entity.Tickets.Any()) return null;
                var ticket = entity.Tickets.OrderByDescending(i => i.CreatedOn).First();
                ticket.InitDetails();
                return (int)((ticket.FinishDate ?? ticket.CreatedOn.AddDays((ticket.TicketType.AutoActivate ?? 0) + ticket.Length)) - DateTime.Now).TotalDays;
            }
        }

        [Description("Остаток ед. последнего проданного активного аб-та*")]
        [Include("Tickets", "Tickets.UnitCharges")]
        public decimal? field16
        {
            get
            {
                if (!entity.Tickets.Any(i => i.IsActive)) return null;
                var ticket = entity.Tickets.Where(i => i.IsActive).OrderByDescending(i => i.CreatedOn).First();
                return ticket.UnitsAmount - (ticket.UnitCharges.Sum(i => (int?)i.Charge) ?? 0);
            }
        }

        [Description("Дата последней активации аб-та")]
        [Include("Tickets")]
        public DateTime? field17
        {
            get
            {
                if (!entity.Tickets.Any(i => i.StartDate.HasValue)) return null;
                return entity.Tickets.Max(i => i.StartDate);
            }
        }

        [Description("Дата покупки первого аб-та")]
        [Include("Tickets")]
        public DateTime? field18
        {
            get
            {
                if (!entity.Tickets.Any()) return null;
                return entity.Tickets.Min(i => i.CreatedOn);
            }
        }

        [Description("Дата покупки последнего аб-та")]
        [Include("Tickets")]
        public DateTime? field19
        {
            get
            {
                if (!entity.Tickets.Any()) return null;
                return entity.Tickets.Max(i => i.CreatedOn);
            }
        }

        [Description("Тип последнего приобретенного аб-та")]
        [Include("Tickets", "Tickets.TicketType")]
        public string field20
        {
            get
            {
                if (!entity.Tickets.Any()) return null;
                return entity.Tickets.OrderByDescending(i => i.CreatedOn).First().TicketType.Name;
            }
        }

        [Description("Тип последнего активиорванного аб-та")]
        [Include("Tickets", "Tickets.TicketType")]
        public string field21
        {
            get
            {
                if (!entity.Tickets.Any(i => i.StartDate.HasValue)) return null;
                return entity.Tickets.OrderByDescending(i => i.StartDate).First().TicketType.Name;
            }
        }

        [Description("Дата последней покупки в баре")]
        [Include("BarOrders", "BarOrders.GoodSales")]
        public DateTime? field22
        {
            get
            {
                if (!entity.BarOrders.Any(i => i.GoodSales.Any())) return null;
                return entity.BarOrders.Where(i => i.GoodSales.Any()).Max(i => i.PurchaseDate);
            }
        }

        [Description("Последняя покупка в баре")]
        [Include("BarOrders", "BarOrders.GoodSales")]
        public string field23
        {
            get
            {
                if (!entity.BarOrders.Any(i => i.GoodSales.Any())) return null;
                return entity.BarOrders.Where(i => i.GoodSales.Any()).OrderByDescending(i => i.PurchaseDate).First().GoodSales.First().Good.Name;
            }
        }

        [Description("Бонусный счет")]
        [Include("BonusAccounts", "DepositAccounts")]
        public decimal field24
        {
            get
            {
                entity.InitDepositValues();
                return entity.BonusDepositValue;
            }
        }

        [Description("Депозит")]
        [Include("BonusAccounts", "DepositAccounts")]
        public decimal field25
        {
            get
            {
                entity.InitDepositValues();
                return entity.RurDepositValue;
            }
        }

        [Description("Всего бонусов потрачено")]
        [Include("BonusAccounts")]
        public decimal field26
        {
            get
            {
                if (entity.BonusAccounts.Where(i => i.Amount < 0).Count() == 0) return 0;
                return entity.BonusAccounts.Where(i => i.Amount < 0).Sum(i => i.Amount);
            }
        }

        [Description("Всего депозита потрачено")]
        [Include("DepositAccounts")]
        public decimal field27
        {
            get
            {
                if (entity.DepositAccounts.Where(i => i.Amount < 0).Count() == 0) return 0;
                return entity.DepositAccounts.Where(i => i.Amount < 0).Sum(i => i.Amount);
            }
        }

        [Description("Действует ли заморозка")]
        [Include("Tickets", "Tickets.TicketFreezes")]
        public string field28
        {
            get
            {
                return entity.Tickets.SelectMany(i => i.TicketFreezes).Any(i => i.StartDate <= DateTime.Now && i.FinishDate >= DateTime.Now) ? "Да" : "Нет";
            }
        }

        [Description("Заполнены противопоказания")]
        [Include("ContraIndications")]
        public string field29
        {
            get
            {
                return entity.NoContraIndications.HasValue || entity.ContraIndications.Any() ? "Да" : "Нет";
            }
        }

        [Description("Любимая процедура*")]
        [InitAttribude]
        public string field30
        {
            get
            {
                return entity.LikedTreatments;
            }
        }

        [Description("Любимая программа*")]
        [InitAttribude]
        public string field31
        {
            get
            {
                return entity.LikedPrograms;
            }
        }

        [Description("Любимый товар*")]
        [InitAttribude]
        public string field31a
        {
            get
            {
                return entity.LikedGoods;
            }
        }

        [Description("Противопоказания")]
        [Include("ContraIndications")]
        public string field31b
        {
            get
            {
                if (!entity.NoContraIndications.HasValue) return null;
                if (entity.NoContraIndications.Value) return "Нет";
                var sb = new StringBuilder();
                entity.ContraIndications.OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(i.Name);
                });
                return sb.ToString();
            }
        }

        [Description("Статусы")]
        [Include("CustomerStatuses")]
        public string field32
        {
            get
            {
                return String.Join(", ", entity.CustomerStatuses.OrderBy(i=>i.Name).Select(i => i.Name));
            }
        }

        [Description("Ставились ли цели")]
        [Include("CustomerTargets")]
        public string field33
        {
            get
            {
                return entity.CustomerTargets.Any() ? "Да" : "Нет";
            }
        }

        [Description("Есть ли достигнутые цели")]
        [Include("CustomerTargets")]
        public string field34
        {
            get
            {
                return entity.CustomerTargets.Any(i => i.TargetComplete ?? false) ? "Да" : "Нет";
            }
        }

        [Description("Есть ли текущие цели")]
        [Include("CustomerTargets")]
        public string field35
        {
            get
            {
                return entity.CustomerTargets.Any(i => i.CreatedOn <= DateTime.Now && i.TargetDate >= DateTime.Now) ? "Да" : "Нет";
            }
        }

        [Description("Поставил последнюю цель")]
        [Include("CustomerTargets")]
        public string field36
        {
            get
            {
                if (!entity.CustomerTargets.Any()) return null;
                return entity.CustomerTargets.OrderByDescending(i => i.CreatedOn).First().CreatedBy.FullName;
            }
        }
        [Description("Если ли антропометрия")]
        [Include("Anthropometrics")]
        public string field37
        {
            get
            {
                return entity.Anthropometrics.Any() ? "Да" : "Нет";
            }
        }
        [Description("Если ли посещения врача")]
        [Include("DoctorVisits")]
        public string field38
        {
            get
            {
                return entity.DoctorVisits.Any() ? "Да" : "Нет";
            }
        }
        [Description("Если ли дневник питания")]
        [Include("Nutritions")]
        public string field39
        {
            get
            {
                return entity.Nutritions.Any() ? "Да" : "Нет";
            }
        }

        [Description("Если ли контрольный замер")]
        [Include("CustomerMeasures")]
        public string field39a
        {
            get
            {
                return entity.CustomerMeasures.Any() ? "Да" : "Нет";
            }
        }

        [Description("Снимал последнюю антропометрию")]
        [Include("Anthropometrics", "Anthropometrics.CreatedBy")]
        public string field40
        {
            get
            {
                if (!entity.Anthropometrics.Any()) return null;
                return entity.Anthropometrics.OrderByDescending(i => i.CreatedOn).First().CreatedBy.FullName;
            }
        }

        [Description("Записал последний в дневник питания")]
        [Include("Nutritions", "Nutritions.CreatedBy")]
        public string field41
        {
            get
            {
                if (!entity.Nutritions.Any()) return null;
                return entity.Nutritions.OrderByDescending(i => i.CreatedOn).First().CreatedBy.FullName;
            }
        }
        [Description("Снимал последний контрольный замер")]
        [Include("CustomerMeasures", "CustomerMeasures.CreatedBy")]
        public string field42
        {
            get
            {
                if (!entity.CustomerMeasures.Any()) return null;
                return entity.CustomerMeasures.OrderByDescending(i => i.CreatedOn).First().CreatedBy.FullName;
            }
        }
        [Description("ФИО последнего врача")]
        [Include("DoctorVisits")]
        public string field43
        {
            get
            {
                if (!entity.DoctorVisits.Any()) return String.Empty;
                return entity.DoctorVisits.OrderBy(i => i.Date ?? i.CreatedOn).First().Doctor ?? "";
            }
        }

        [Description("Количество детей")]
        public int? field44 { get { return entity.Kids; } }
        [Description("Социальный статус")]
        public string field45 { get { return CusomerDictionaries.GetSocialStatus(entity.SocialStatusId); } }
        [Description("Место работы")]
        public string field46 { get { return entity.WorkPlace; } }
        [Description("Должность")]
        public string field47 { get { return entity.Position; } }
        [Description("Рабочий телефон")]
        public string field48 { get { return entity.WorkPhone; } }

        [Description("Количество дней до дня рождения")]
        public int? field49
        {
            get
            {
                if (!entity.Birthday.HasValue) return null;
                try
                {
                    var bd = new DateTime(DateTime.Today.Year, entity.Birthday.Value.Month, entity.Birthday.Value.Day);
                    var days = (int)((bd - DateTime.Today).TotalDays);
                    if (days < 0) return days + 365;
                    return days;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }

        [Description("Всего единиц на активных аб-х")]
        [Include("Tickets", "Tickets.UnitCharges")]
        public int? field50
        {
            get
            {
                var ticks = entity.Tickets.Where(i => i.IsActive || !i.StartDate.HasValue).ToList();
                if (!ticks.Any()) return 0;
                ticks.ForEach(i => i.InitUnitsOut());
                return ticks.Sum(i => (int)i.UnitsLeft);
            }
        }
        [Description("Смс-рассылка")]
        public string field51
        {
            get
            {
                return entity.SmsList ? "Да" : "Нет";
            }
        }

        [Description("Дата последнего посещения")]
        [Include("CustomerVisits")]
        public DateTime? field52
        {
            get
            {
                var l = entity.CustomerVisits.OrderByDescending(i => i.InTime).FirstOrDefault();
                if (l == null) return null;
                return l.InTime;
            }
        }
        [Description("Комментарии")]
        public string field53
        {
            get
            {
                return entity.Comments;
            }
        }
    }
}