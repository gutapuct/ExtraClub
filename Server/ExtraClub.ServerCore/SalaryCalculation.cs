using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;
using System.Net;

namespace ExtraClub.ServerCore
{
    public static class SalaryCalculation
    {
        public static decimal Get01TotalSalesPercent(ExtraEntities context, Guid? divisionId, DateTime calcStart, DateTime calcEnd, Guid? companyId = null)
        {
            var month = new DateTime(calcStart.Year, calcStart.Month, 1);
            if(divisionId.HasValue)
            {
                var plan = context.SalesPlans.FirstOrDefault(i => i.DivisionId == divisionId && i.Month == month);
                if(plan == null || plan.Value == 0) return 0;
                if(calcEnd == DateTime.MinValue) calcEnd = calcStart.AddMonths(1);

                var data = context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd)
                    .Sum(x => (decimal?)(x.CashPayment + x.CardPayment)) ?? 0;
                var data1 = context.TicketPayments.Where(i => divisionId == i.Ticket.DivisionId && i.PaymentDate >= calcStart && i.PaymentDate < calcEnd && !i.BarOrderId.HasValue && i.Ticket.CreditInitialPayment.HasValue)
                    .Sum(i => (decimal?)i.Amount) ?? 0;
                var returns = context.GoodSales.Where(i => i.ReturnDate >= calcStart && i.ReturnDate < calcEnd && i.BarOrder.DivisionId == divisionId).Sum(i => i.PriceMoney) ?? 0;
                var returns2 = context.Tickets.Where(i => i.ReturnDate >= calcStart && i.ReturnDate < calcEnd && i.DivisionId == divisionId).Sum(i => i.ReturnCost) ?? 0;
                var sum = data + data1 - returns - returns2;


                //var sum = context.TicketPayments.Where(i => i.Ticket.DivisionId == divisionId && i.PaymentDate >= calcStart && i.PaymentDate < calcEnd && !i.Ticket.Customer.CorporateId.HasValue).Sum(i => (decimal?)i.Amount) ?? 0;

                //Logger.Log(String.Format("Абонементы {0:c}", sum));

                //var gss = context.GoodSales.Where(i => i.BarOrder.DivisionId == divisionId && i.BarOrder.PurchaseDate >= calcStart && i.BarOrder.PurchaseDate < calcEnd);
                //if (gss.Any())
                //{
                //    var x = gss.Select(i => new { i.Amount, PriceMoney = i.PriceMoney ?? 0 }).ToList().Sum(i => (decimal)i.Amount * i.PriceMoney);
                //    sum += x;
                //    Logger.Log(String.Format("Продажи {0:c}", x));

                //}

                //var sol = context.SolariumVisits.Where(i => i.DivisionId==divisionId && i.Cost.HasValue && i.VisitDate >= calcStart && i.VisitDate < calcEnd && !i.Customer.CorporateId.HasValue);
                //if (sol.Any())
                //{
                //    var x = sol.Sum(i => i.Cost ?? 0);
                //    sum += x;
                //    Logger.Log(String.Format("Ослярий {0:c}", x));
                //}


                ////var codes = new List<int>(){2,5,6,8};

                //var c1c = context.BarOrders.Where(i => i.DivisionId == divisionId
                //    && !i.Customer.CorporateId.HasValue
                //    && i.PurchaseDate >= calcStart
                //    && i.PurchaseDate < calcEnd
                //    && (i.Kind1C==2 ||i.Kind1C==5 ||i.Kind1C==6 ||i.Kind1C==8));
                //Logger.Log(String.Format("Кодов {0}", c1c.Count()));

                //if (c1c.Any())
                //{
                //    var x = c1c.Sum(i => (i.CashPayment) + (i.DepositPayment) + (i.CardPayment));
                //    sum += x;
                //    Logger.Log(String.Format("Коды {0:c}", x));
                //}

                return sum / plan.Value * 100;

                //var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && !i.Customer.CorporateId.HasValue);
                //if (ls.Count() == 0) return 0;
                //return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment) / plan.CorpValue * 100;
            }
            else
            {
                var plans = context.SalesPlans.Where(i => i.CompanyId == companyId && i.Month == month);
                if(plans.Count() == 0 || plans.Sum(i => i.Value) == 0) return 0;
                if(calcEnd == DateTime.MinValue) calcEnd = calcStart.AddMonths(1);
                var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.CompanyId == companyId);
                if(ls.Count() == 0) return 0;
                return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment) / plans.Sum(i => i.Value) * 100;
            }
        }

        public static decimal Get01aTotalCorporateSalesPercent(ExtraEntities context, Guid? divisionId, DateTime calcStart, DateTime calcEnd, Guid? companyId = null)
        {
            var month = new DateTime(calcStart.Year, calcStart.Month, 1);
            if(divisionId.HasValue)
            {
                var plan = context.SalesPlans.FirstOrDefault(i => i.DivisionId == divisionId && i.Month == month);
                if(plan == null || plan.CorpValue == 0) return 0;
                if(calcEnd == DateTime.MinValue) calcEnd = calcStart.AddMonths(1);

                var data = context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd)
                    .Sum(x => (decimal?)(x.CashPayment + x.CardPayment)) ?? 0;
                var data1 = context.TicketPayments.Where(i => divisionId == i.Ticket.DivisionId && i.PaymentDate >= calcStart && i.PaymentDate < calcEnd && !i.BarOrderId.HasValue && i.Ticket.CreditInitialPayment.HasValue)
                    .Sum(i => (decimal?)i.Amount) ?? 0;
                var returns = context.GoodSales.Where(i => i.ReturnDate >= calcStart && i.ReturnDate < calcEnd && i.BarOrder.DivisionId == divisionId).Sum(i => i.PriceMoney) ?? 0;
                var returns2 = context.Tickets.Where(i => i.ReturnDate >= calcStart && i.ReturnDate < calcEnd && i.DivisionId == divisionId).Sum(i => i.ReturnCost) ?? 0;
                var sum = data + data1 - returns - returns2;


                //var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.CorporateId.HasValue);
                //if (ls.Count() == 0) return 0;
                return sum / plan.CorpValue * 100;
            }
            else
            {
                var plans = context.SalesPlans.Where(i => i.CompanyId == companyId && i.Month == month);
                if(plans.Count() == 0 || plans.Sum(i => i.CorpValue) == 0) return 0;
                if(calcEnd == DateTime.MinValue) calcEnd = calcStart.AddMonths(1);
                var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.CompanyId == companyId && i.Customer.CorporateId.HasValue);
                if(ls.Count() == 0) return 0;
                return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment) / plans.Sum(i => i.CorpValue) * 100;
            }
        }

        internal static decimal Get02GroupSales(ExtraEntities context, Guid divisionId, DateTime calcStart, int group)
        {
            var calcEnd = calcStart.AddMonths(1);

            //0-goods
            if(group == 0)
            {
                var ls = context.GoodSales.Where(i => i.BarOrder.PurchaseDate >= calcStart && i.BarOrder.PurchaseDate < calcEnd && divisionId == i.BarOrder.DivisionId);
                if(ls.Count() == 0) return 0;
                return ls.ToList().Sum(i => i.Cost);
            }
            //2-sol
            if(group == 2)
            {
                var ls = context.SolariumVisits.Where(i => i.VisitDate >= calcStart && i.VisitDate < calcEnd && i.DivisionId == divisionId);
                if(ls.Count() == 0) return 0;
                return ls.Sum(i => i.Cost ?? 0);
            }
            //1-tickets - только реальные денежные поступления!
            var ls1 = context.TicketPayments.Where(i => i.PaymentDate >= calcStart && i.PaymentDate < calcEnd && divisionId == i.Ticket.DivisionId);
            if(ls1.Count() == 0) return 0;
            var sum = ls1.Sum(i => i.Amount);
            if(group == 1)
            {
                return sum;
            }
            //3-all-abon
            var ls2 = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId);
            if(ls2.Count() == 0) return 0;
            return ls2.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment) - sum;
        }

        internal static decimal Get03NewTicketsAmount(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);

            return context.Tickets.Count(i => i.CreatedOn >= calcStart && i.CreatedOn < calcEnd && i.DivisionId == divisionId && !i.Customer.Tickets.Any(j => j.CreatedOn < i.CreatedOn));
        }

        internal static decimal Get04DicsountedTickets(ExtraEntities context, Guid divisionId, DateTime calcStart, decimal discount)
        {
            var calcEnd = calcStart.AddMonths(1);
            var discount100 = discount / 100;
            return context.Tickets.Count(i => i.CreatedOn >= calcStart && i.CreatedOn < calcEnd && i.DivisionId == divisionId && i.DiscountPercent == discount || i.DiscountPercent == discount100);
        }

        internal static decimal Get05RetryTicketsAmount(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);
            return context.Tickets.Count(i => i.CreatedOn >= calcStart && i.CreatedOn < calcEnd && i.DivisionId == divisionId && i.Customer.Tickets.Any(j => j.CreatedOn < i.CreatedOn));
        }

        internal static decimal Get06TicketType(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid ticketType)
        {
            var calcEnd = calcStart.AddMonths(1);
            return context.Tickets.Count(i => i.CreatedOn >= calcStart && i.CreatedOn < calcEnd && i.DivisionId == divisionId && i.TicketTypeId == ticketType);
        }

        internal static decimal Get07CardType(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid cardType)
        {
            var calcEnd = calcStart.AddMonths(1);
            return context.CustomerCards.Count(i => i.EmitDate >= calcStart && i.EmitDate < calcEnd && i.DivisionId == divisionId && i.CustomerCardTypeId == cardType);
        }

        internal static decimal Get08GoodGroup(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid goodCategory)
        {
            var calcEnd = calcStart.AddMonths(1);
            var ls = context.GoodSales.Where(i => i.BarOrder.PurchaseDate >= calcStart && i.BarOrder.PurchaseDate < calcEnd && i.BarOrder.DivisionId == divisionId && i.Good.GoodsCategoryId == goodCategory);
            if(ls.Count() == 0) return 0;
            return ls.Sum(i => (int)i.Amount);
        }

        internal static decimal Get09Good(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid goodId)
        {
            var calcEnd = calcStart.AddMonths(1);
            var ls = context.GoodSales.Where(i => i.BarOrder.PurchaseDate >= calcStart && i.BarOrder.PurchaseDate < calcEnd && i.BarOrder.DivisionId == divisionId && i.GoodId == goodId);
            if(ls.Count() == 0) return 0;
            return ls.Sum(i => (int)i.Amount);
        }

        internal static decimal Get10Action(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid actionId)
        {
            var calcEnd = calcStart.AddMonths(1);
            return context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.GoodActionId == actionId).Select(i => i.Customer).Distinct().Count();
        }

        internal static decimal Get11AfterGuest(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);
            try
            {
                return context.CustomerCards.Count(i => i.EmitDate >= calcStart && i.EmitDate < calcEnd && i.DivisionId == divisionId && !i.CustomerCardType.IsGuest && !i.CustomerCardType.IsVisit
                    && i.Customer.CustomerCards.Any(j => j.EmitDate >= calcStart && j.EmitDate < calcEnd && j.DivisionId == divisionId && i.CustomerCardType.IsGuest)) /
                    context.CustomerCards.Count(i => i.EmitDate >= calcStart && i.EmitDate < calcEnd && i.DivisionId == divisionId && !i.CustomerCardType.IsGuest && !i.CustomerCardType.IsVisit) * 100;
            }
            catch
            {
                return 0;
            }
        }

        internal static decimal Get12AfterVisit(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);
            try
            {
                return context.CustomerCards.Count(i => i.EmitDate >= calcStart && i.EmitDate < calcEnd && i.DivisionId == divisionId && !i.CustomerCardType.IsVisit && !i.CustomerCardType.IsGuest
                    && i.Customer.CustomerCards.Any(j => j.EmitDate >= calcStart && j.EmitDate < calcEnd && j.DivisionId == divisionId && i.CustomerCardType.IsVisit)) /
                    context.CustomerCards.Count(i => i.EmitDate >= calcStart && i.EmitDate < calcEnd && i.DivisionId == divisionId && !i.CustomerCardType.IsGuest && !i.CustomerCardType.IsVisit) * 100;
            }
            catch
            {
                return 0;
            }
        }

        internal static decimal Get13AvgSpent(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);
            return (context.GetAvgSpendings(calcStart, calcEnd, divisionId).First()) ?? 0;
        }

        internal static decimal Get14ClubVisit(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);
            var div = context.Divisions.Single(i => i.Id == divisionId);
            if(!div.CloseTime.HasValue || !div.OpenTime.HasValue) return 0;
            var totalHrs = (div.CloseTime.Value - div.OpenTime.Value).TotalHours
                * DateTime.DaysInMonth(calcStart.Year, calcStart.Month)
                * div.Treatments.Where(i => i.IsActive && i.CreatedOn <= calcStart).Count();
            var usedHrs = context.TreatmentEvents.Where(i => i.DivisionId == divisionId
                && i.VisitDate >= calcStart && i.VisitDate < calcEnd
                && i.VisitStatus >= 2)
                .ToList().Init()
                .Sum(i => (i.EndTime - i.VisitDate).TotalHours);
            return (decimal)(usedHrs / totalHrs) * 100;
        }

        internal static decimal Get15TreatmentTypeVisit(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid treatmentTypeId)
        {
            var calcEnd = calcStart.AddMonths(1);
            var div = context.Divisions.Single(i => i.Id == divisionId);
            if(!div.CloseTime.HasValue || !div.OpenTime.HasValue) return 0;
            var totalHrs = (div.CloseTime.Value - div.OpenTime.Value).TotalHours
                * DateTime.DaysInMonth(calcStart.Year, calcStart.Month)
                * div.Treatments.Where(i => i.IsActive && i.CreatedOn <= calcStart && i.TreatmentTypeId == treatmentTypeId).Count();
            var usedHrs = context.TreatmentEvents.Where(i => i.DivisionId == divisionId
                && i.VisitDate >= calcStart && i.VisitDate < calcEnd
                && i.VisitStatus >= 2 && i.Treatment.TreatmentTypeId == treatmentTypeId)
                .Sum(i => (i.EndTime - i.VisitDate).TotalHours);
            return (decimal)(usedHrs / totalHrs) * 100;

        }

        internal static decimal Get16TreatmentVisit(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid treatmentId)
        {
            var calcEnd = calcStart.AddMonths(1);
            var div = context.Divisions.Single(i => i.Id == divisionId);
            if(!div.CloseTime.HasValue || !div.OpenTime.HasValue) return 0;
            var totalHrs = (div.CloseTime.Value - div.OpenTime.Value).TotalHours
                * DateTime.DaysInMonth(calcStart.Year, calcStart.Month);
            var usedHrs = context.TreatmentEvents.Where(i => i.DivisionId == divisionId
                && i.VisitDate >= calcStart && i.VisitDate < calcEnd
                && i.VisitStatus >= 2 && i.TreatmentId == treatmentId)
                .Sum(i => (i.EndTime - i.VisitDate).TotalHours);
            return (decimal)(usedHrs / totalHrs) * 100;
        }

        internal static decimal Get18CertByGroup(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid catId)
        {
            var calcEnd = calcStart.AddMonths(1);
            return context.Certificates.Count(i => i.SellDate.HasValue && i.SellDate.Value >= calcStart && i.SellDate.Value < calcEnd && i.DivisionId == divisionId && i.CategoryId == catId);
        }

        internal static decimal Get22AmTicketsFromCardType(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid cardType)
        {
            var calcEnd = calcStart.AddMonths(1);
            var list = new List<Customer>();
            var ls = context.CustomerCards.Where(i => i.EmitDate < calcEnd).GroupBy(i => i.Customer);
            foreach(var i in ls)
            {
                var t = i.OrderByDescending(j => j.EmitDate).FirstOrDefault();
                if(t.CustomerCardTypeId == cardType)
                {
                    list.Add(i.Key);
                }
            }
            if(list.Count() == 0) return 0;
            return list.Sum(i => i.Tickets.Count(j => j.CreatedOn >= calcStart && j.CreatedOn < calcEnd));
        }

        internal static decimal Get23CostTicketsFromCardType(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid cardType)
        {
            var calcEnd = calcStart.AddMonths(1);
            var list = new List<Customer>();
            var ls = context.CustomerCards.Where(i => i.EmitDate < calcEnd).GroupBy(i => i.Customer);
            foreach(var i in ls)
            {
                var t = i.OrderByDescending(j => j.EmitDate).FirstOrDefault();
                if(t.CustomerCardTypeId == cardType)
                {
                    list.Add(i.Key);
                }
            }
            if(list.Count() == 0) return 0;
            return list.Sum(i => i.Tickets.SelectMany(j => j.TicketPayments).Where(j => j.PaymentDate >= calcStart && j.PaymentDate < calcEnd).Sum(j => j.Amount));
        }

        internal static decimal Get24GoodsFromCardType(ExtraEntities context, Guid divisionId, DateTime calcStart, Guid cardType)
        {
            var calcEnd = calcStart.AddMonths(1);
            var list = new List<Customer>();
            var ls = context.CustomerCards.Where(i => i.EmitDate < calcEnd).GroupBy(i => i.Customer);
            foreach(var i in ls)
            {
                var t = i.OrderByDescending(j => j.EmitDate).FirstOrDefault();
                if(t.CustomerCardTypeId == cardType)
                {
                    list.Add(i.Key);
                }
            }
            if(list.Count() == 0) return 0;
            return list.Sum(i => i.BarOrders.Where(j => j.PaymentDate >= calcStart && j.PaymentDate < calcEnd && j.DivisionId == divisionId).SelectMany(o => o.GoodSales).Sum(o => o.Cost));
        }

        internal static decimal Get17ParameterizedVisit(ExtraEntities context, Guid divisionId, DateTime calcStart, int dayBits, TimeSpan openTime, TimeSpan closeTime)
        {
            var calcEnd = calcStart.AddMonths(1);

            var div = context.Divisions.Single(i => i.Id == divisionId);

            var selectedDaysCount = GetSelectedDays(calcStart, calcEnd, dayBits);

            var totalHrs = (closeTime - openTime).TotalHours
                * selectedDaysCount
                * div.Treatments.Where(i => i.IsActive && i.CreatedOn <= calcStart).Count();
            var usedHrs = context.TreatmentEvents.Where(i => i.DivisionId == divisionId
                && i.VisitDate >= calcStart && i.VisitDate < calcEnd
                && i.VisitStatus >= 2).
                ToList()
                .Where(i => IsDaySet(dayBits, (int)i.VisitDate.DayOfWeek) && Core.DatesIntersects(i.VisitDate, i.VisitDate.AddMinutes(i.TreatmentConfig.FullDuration), i.VisitDate.Date.Add(openTime), i.VisitDate.Date.Add(closeTime)))
                .Sum(i => (i.EndTime - i.VisitDate).TotalHours);
            return (decimal)(usedHrs / totalHrs) * 100;
        }

        private static int GetSelectedDays(DateTime calcStart, DateTime calcEnd, int dayBits)
        {
            var res = 0;
            var cd = calcStart;
            while(cd < calcEnd)
            {
                cd = cd.AddDays(1);
                if(IsDaySet(dayBits, (int)cd.DayOfWeek)) res++;
            }
            return res;
        }

        /// <summary>
        /// monday==0
        /// </summary>
        /// <param name="container"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool IsDaySet(int container, int index)
        {
            return (container & (1 << index - 2)) != 0;
        }

        internal static decimal Get19NewCorporates(ExtraEntities context, Guid divisionId, DateTime calcStart)
        {
            var calcEnd = calcStart.AddMonths(1);
            return context.CustomerCards.Count(i => i.DivisionId == divisionId && i.EmitDate >= calcStart && i.EmitDate < calcEnd && i.Customer.CorporateId.HasValue);
        }

        internal static decimal Get20CategorySales(ExtraEntities context, Guid divisionId, DateTime calcStart, int type, Guid catId)
        {
            var calcEnd = calcStart.AddMonths(1);
            if(type == 0)
            {
                var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.AdvertTypeId == catId);
                if(ls.Count() == 0) return 0;
                return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment);
            }
            else
                if(type == 1)
                {
                    var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.CustomerStatuses.Any(j => j.Id == catId));
                    if(ls.Count() == 0) return 0;
                    return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment);
                }
                else
                {
                    var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.CorporateId == catId);
                    if(ls.Count() == 0) return 0;
                    return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment);
                }
        }

        internal static decimal Get21CategorySalesNum(ExtraEntities context, Guid divisionId, DateTime calcStart, int type, Guid catId)
        {
            var calcEnd = calcStart.AddMonths(1);
            if(type == 0)
            {
                return context.BarOrders.Count(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.AdvertTypeId == catId);
            }
            else
                if(type == 1)
                {
                    return context.BarOrders.Count(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.CustomerStatuses.Any(j => j.Id == catId));
                }
                else
                {
                    return context.BarOrders.Count(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd && i.DivisionId == divisionId && i.Customer.CorporateId == catId);
                }
        }

        internal static decimal Get25KPD(ExtraEntities context, DateTime calcStart, Employee employee)
        {
            var calcEnd = calcStart.AddMonths(1);
            if(String.IsNullOrWhiteSpace(employee.BoundCustomer.Email)) return 0;
            var reqStr = String.Format("http://ftm.sendika.ru/External/Index?email={0}&start={1:dd.MM.yyyy}&end={2:dd.MM.yyyy}", employee.BoundCustomer.Email, calcStart, calcEnd);
            var wc = new WebClient();
            try
            {
                var str = wc.DownloadString(reqStr);
                decimal res = 0;
                if(Decimal.TryParse(str, out res))
                {
                    return res;
                }
                return 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
