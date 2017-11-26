using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServerCore;

namespace ExtraClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable GetGeneralReport(DateTime start, Guid? divisionId)
        {

            if (!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!!!");
            }
            if (start == DateTime.MinValue)
            {
                throw new Exception("Необходимо указать период!");
            }

            var end = start.AddMonths(1).AddMilliseconds(-1);
            var consultationTreatmentTypeId = Guid.Parse("73EE31C6-379E-49C3-8ADE-EB1F46282523");

            using (var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new DataTable();

                //res.ExtendedProperties.Add("Detailed", true);
                //res.ExtendedProperties.Add("Color", "Red");
                res.Columns.Add("Параметр", typeof(string));
                res.Columns.Add("Значение", typeof(string));
                res.Columns.Add("Комментарий", typeof(string));
                res.Columns.Add(" ", typeof(string));
                res.Columns.Add("  ", typeof(string));
                res.Columns.Add("   ", typeof(string));
                res.Columns.Add("    ", typeof(string));
                res.Columns.Add("     ", typeof(string));
                res.Columns.Add("      ", typeof(string));
                res.Columns.Add("       ", typeof(string));

                #region Выручка от услуг существующим клиентам
                AddNameParametersInRow(res, "Выручка от услуг существующим клиентам");
                res.Rows.Add("1. Продление абонементов");

                //все абонементы 
                var tickets = context.Tickets.Include("TicketType").Include("TicketPayments").Include("Customer").Where(i =>
                                                   !i.TicketType.IsGuest
                                                   && !i.TicketType.IsVisit
                                                   && i.TicketType.SolariumMinutes == 0
                                                   && !i.ReturnDate.HasValue
                                                   && i.DivisionId == divisionId
                                                   ).ToList();

                //количество абонементов закончившихся в этом месяце
                var countTicketEndDate = 0;
                var unitCharges = context.UnitCharges.Where(i => i.Ticket.DivisionId == divisionId).Select(i => new { i.TicketId, i.Charge, i.Date }).ToList();

                //Количество и стоимость абонементов, купленных повторно более чем через 6 мес после окончания предыдущего
                var countTicketsReturnCustomers = 0;
                decimal sumTicketsReturnCustomers = 0m;

                foreach (var t in tickets.Where(i => !i.ReturnDate.HasValue))
                {
                    var endDate = DateTime.MinValue;
                    if (unitCharges.Where(i => i.TicketId == t.Id).Sum(i => i.Charge) >= t.UnitsAmount)
                    {
                        endDate = unitCharges.Where(i => i.TicketId == t.Id).OrderByDescending(i => i.Date).Select(i => i.Date).FirstOrDefault();
                    }
                    else
                    {
                        if (t.StartDate.HasValue)
                        {
                            endDate = t.StartDate.Value.AddDays(t.Length);
                            if (t.TicketFreezes.Any())
                            {
                                endDate = endDate.AddDays(t.TicketFreezes.Sum(i => (i.FinishDate - i.StartDate).TotalDays));
                            }
                        }
                    }

                    if (endDate >= start && endDate <= end)
                    {
                        countTicketEndDate++;

                        if (tickets.Where(i => i.CustomerId == t.CustomerId && i.CreatedOn < t.CreatedOn).Any()
                            && !tickets.Where(i => i.CustomerId == t.CustomerId && i.CreatedOn < t.CreatedOn && i.CreatedOn > t.CreatedOn.AddMonths(-6)).Any())
                        {
                            countTicketsReturnCustomers++;
                            sumTicketsReturnCustomers += t.Price * (1 - t.DiscountPercent);
                        }
                    }
                }
                res.Rows.Add("Количество закончившихся абонементов", countTicketEndDate.ToString(), "Закончились или закончатся в выбранном месяце");

                //средняя стоимость второго, третьего и тд.абонемента
                var nextTicketPrices = tickets.Where(i => i.Id != tickets.Where(j => j.CustomerId == i.CustomerId).OrderBy(j => j.CreatedOn).Select(j => j.Id).FirstOrDefault()).ToList();
                var avgNextTicketPrices = (nextTicketPrices.Any()) ? nextTicketPrices.Average(i => i.Price * (1 - i.DiscountPercent)) : 0m;
                res.Rows.Add("Средняя стоимость второго, третьего и т.д. абонемента", ((int)avgNextTicketPrices).ToString() + " руб.", "За всё время (первый абонемент не учитывается)");

                var countCustomersWithCountTicket = tickets
                            .GroupBy(i => i.CustomerId)
                            .Select(i => new
                            {
                                count = i.Count(),
                                lastTicket = i.OrderByDescending(j => j.CreatedOn).FirstOrDefault()
                            })
                            .ToList();

                //var countCustomersWithTicket = tickets.Select(i => i.CustomerId).Distinct().Count();

                //количество клиентов, продлевающих абонемент после первого абонемента
                var countCustomersWithOneTicket = countCustomersWithCountTicket.Where(i => i.count == 2 && i.lastTicket != null && i.lastTicket.CreatedOn >= start && i.lastTicket.CreatedOn <= end).Count();
                //количество клиентов, продлевающих абонемент после второго абонемента
                var countCustomersWithTwoTicket = countCustomersWithCountTicket.Where(i => i.count == 3 && i.lastTicket != null && i.lastTicket.CreatedOn >= start && i.lastTicket.CreatedOn <= end).Count();
                //количество клиентов, продлевающих абонемент после третьего и т.д.абонемента
                var countCustomersWithThreeTicket = countCustomersWithCountTicket.Where(i => i.count > 3 && i.lastTicket != null && i.lastTicket.CreatedOn >= start && i.lastTicket.CreatedOn <= end).Count();

                //res.Rows.Add("Клиентов, купивших хотя бы один абонемент", countCustomersWithTicket.ToString(), "За всё время");
                res.Rows.Add("Из тех, у кого закончился не более 6 месяцев назад, продлили", (countCustomersWithOneTicket + countCustomersWithTwoTicket + countCustomersWithThreeTicket).ToString(), "За выбранный месяц");
                res.Rows.Add("Клиентов, купивших второй абонемент", countCustomersWithOneTicket.ToString(), "За выбранный месяц");
                res.Rows.Add("Клиентов, купивших третий абонемент", countCustomersWithTwoTicket.ToString(), "За выбранный месяц");
                res.Rows.Add("Клиентов, купивших четвертый и более абонемент", countCustomersWithThreeTicket.ToString(), "За выбранный месяц");

                res.Rows.Add("Доля клиентов, продлевающих абонемент", (GetPercent(countCustomersWithOneTicket, countTicketEndDate) + GetPercent(countCustomersWithTwoTicket, countTicketEndDate) + GetPercent(countCustomersWithThreeTicket, countTicketEndDate)).ToString() + "%", "За выбранный месяц");
                res.Rows.Add("Доля клиентов, продлевающих абонемент после первого абонемента", GetPercent(countCustomersWithOneTicket, countTicketEndDate).ToString() + "%", "За выбранный месяц");
                res.Rows.Add("Доля клиентов, продлевающих абонемент после второго абонемента", GetPercent(countCustomersWithTwoTicket, countTicketEndDate).ToString() + "%", "За выбранный месяц");
                res.Rows.Add("Доля клиентов, продлевающих абонемент после третьего и т.д. абонемента", GetPercent(countCustomersWithThreeTicket, countTicketEndDate).ToString() + "%", "За выбранный месяц");

                //средние продолжительности первого, второго, третьего и т.д. абонементов
                var ticketLengths = tickets.GroupBy(i => i.CustomerId).ToList();
                var avgFirstTicketLengths = (ticketLengths.Any()) ? ticketLengths.Select(i => i.OrderBy(j => j.CreatedOn).Select(j => j.TicketType.Length).First()).Average() : 0d;
                var secondTicketLengths = ticketLengths.Where(i => i.Count() > 1).Select(i => i.OrderBy(j => j.CreatedOn).Skip(1).Select(j => j.TicketType.Length).FirstOrDefault()).ToList();
                var avgSecondTicketLengths = (secondTicketLengths.Any()) ? secondTicketLengths.Average() : 0d;
                var thirdTicketLengths = ticketLengths.Where(i => i.Count() > 2).Select(i => i.OrderBy(j => j.CreatedOn).Skip(2).Select(j => j.TicketType.Length).Last()).ToList();
                var avgThirdTicketLengths = (thirdTicketLengths.Any()) ? thirdTicketLengths.Average() : 0d;
                res.Rows.Add("Средняя продолжительность первого абонемента", GetStringDecimalRound(avgFirstTicketLengths), "Количество дней");
                res.Rows.Add("Средняя продолжительность второго абонемента", GetStringDecimalRound(avgSecondTicketLengths), "Количество дней");
                res.Rows.Add("Средняя продолжительность третьего и т.д. абонемента", GetStringDecimalRound(avgThirdTicketLengths), "Количество дней");
                
                res.Rows.Add("2. Возвращенные клиенты");
                res.Rows.Add("Количество абонементов, купленных повторно более чем через 6 мес после окончания предыдущего", countTicketsReturnCustomers.ToString(), "За выбранный месяц");
                res.Rows.Add("Сумма таких абонементов", GetStringDecimalRoundToRUB(sumTicketsReturnCustomers), "За выбранный месяц");
                #endregion

                #region Выручка от услуг Новым клиентам
                AddNameParametersInRow(res, "Выручка от услуг Новым клиентам (воронка продаж)");

                //Средний чек по абонементам, руб.
                var ticketPrices = tickets.Where(i => i.CreatedOn >= start && i.CreatedOn <= end).Select(i => i.Price * (1 - i.DiscountPercent)).ToList();
                res.Rows.Add("Средний чек по абонементам", ((!ticketPrices.Any()) ? "0" : ((int)ticketPrices.Average()).ToString()) + " руб.", "За выбранный месяц");

                //Количество всех обращений потенциальных клиентов (кто позвонил, оставил заявку на сайте, рефералы)
                var customers = context.Customers.Where(i => i.ClubId == divisionId && i.CreatedOn >= start && i.CreatedOn <= end).ToList();
                //Рекламные каналы
                var advertTypes = context.AdvertTypes.OrderBy(i => i.Name).Select(i => new { Id = i.Id, Name = i.Name }).ToList();

                //Выручка клуба и чистая прибыль

                //оплаты абонементов
                var incomeTickets = context.TicketPayments
                               .Where(i => i.Ticket.DivisionId == divisionId && i.PaymentDate >= start && i.PaymentDate <= end)
                               .Where(i => !i.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == i.BarOrderId && k.CertificateId.HasValue))
                               .Select(i => new { i.Amount, i.Ticket.Customer.AdvertTypeId }).ToList();
                //товары
                var incomeGoods = context.GoodSales.Where(i => i.BarOrder.DivisionId == divisionId
                                                     && (i.BarOrder.PaymentDate ?? i.BarOrder.PurchaseDate) >= start
                                                     && (i.BarOrder.PaymentDate ?? i.BarOrder.PurchaseDate) <= end
                                                     && i.BarOrder.DepositPayment == 0)
                                                     .Select(i => new { Amount = i.Amount, Priсe = i.PriceMoney, i.BarOrder.Customer.AdvertTypeId }).ToList();
                //Сертификаты - использование
                var incomeCertificatesUse = context.Certificates.Where(i => i.DivisionId == divisionId
                                                        && i.UsedOrderId.HasValue
                                                        && i.UsedInOrder.PurchaseDate >= start
                                                        && i.UsedInOrder.PurchaseDate < end)
                                                        .Select(i => new { Amount = -i.Amount, i.Customer.AdvertTypeId}).ToList();
                //пакеты товаров
                var incomeGoodPackages = context.BarOrders
                        .Where(i => i.DivisionId == divisionId && i.Kind1C == 10 && (i.PaymentDate ?? i.PurchaseDate) >= start && (i.PaymentDate ?? i.PurchaseDate) <= end).ToList()
                        .Select(i => new { Lines = i.GetContent(), i.Customer.AdvertTypeId }).ToList()
                        .SelectMany(i => i.Lines.Select(j => new { Line = j.Price, i.AdvertTypeId }))
                        .Select(i => new { i.Line, i.AdvertTypeId }).ToList();
                //сертификаты покупка
                var incomeCertificatesSell = context.Certificates.Where(i => i.SellDate.HasValue && i.SellOrderId.HasValue && i.DivisionId == divisionId && i.SellDate >= start && i.SellDate <= end).Select(i => new { i.Amount, i.Customer.AdvertTypeId }).ToList();
                //пополнение депозита
                var incomeDeposits = context.BarOrders.Where(i => i.Kind1C == 9 && i.DivisionId == divisionId && i.PurchaseDate >= start && i.PurchaseDate <= end)
                        .Select(i => new { Amount = i.CashPayment + i.CardPayment, i.Customer.AdvertTypeId }).Select(i => new { i.Amount, i.AdvertTypeId }).ToList();
                //Доходы прочие
                var incomeAnothers = context.Incomes.Where(j => j.DivisionId == divisionId && j.CreatedOn >= start && j.CreatedOn <= end).Select(i => i.Amount).ToList();

                var proceeds = incomeTickets.Select(i => i.Amount).Sum()
                                + (incomeGoods.Sum(i => (decimal)i.Amount * i.Priсe)) ?? 0m
                                + incomeGoodPackages.Select(i => i.Line).Sum()
                                + incomeCertificatesUse.Select(i => i.Amount).Sum()
                                + incomeCertificatesSell.Select(i => i.Amount).Sum()
                                + incomeDeposits.Select(i => i.Amount).Sum()
                                + incomeAnothers.Sum();

                res.Rows.Add();
                res.Rows.Add("Рекламный канал", "Количество звонков", "Количетво созданных клиентов", "Количество перспективных клиентов (результат события не \"Отказ\")", "Количество записанных на консультацию", "Количество прошедших консультацию", "Количество купленных абонементов", "Количество разовых", "Средний срок от занесения в базу до консультации", "Выручка");

                var calls = context.Calls
                    .Where(i => i.StartAt >= start && i.StartAt <= end && i.Customer.AdvertTypeId != null)
                    .Select(i => new { i.CustomerId, i.Result, advertTypeId = i.Customer.AdvertTypeId })
                    .ToList();
                var consultations = context.TreatmentEvents
                    .Where(i => i.VisitDate >= start && i.VisitDate <= end && (i.VisitStatus == 2 || i.VisitStatus == 3) && i.TreatmentConfig.TreatmentTypeId == consultationTreatmentTypeId && i.Customer.AdvertTypeId != null)
                    .Select(i => new { i.VisitStatus, i.VisitDate, advertType = i.Customer.AdvertTypeId, CustomerCreatedOn = i.Customer.CreatedOn, CustomerId = i.CustomerId })
                    .ToList();

                foreach (var advertType in advertTypes)
                {
                    var atCalls = calls.Where(i => i.advertTypeId == advertType.Id).Count();
                    var atCustomers = customers.Where(i => i.AdvertTypeId == advertType.Id).Count();
                    var atPerspective = calls.Where(i => i.advertTypeId == advertType.Id && i.Result != "Отказ").Select(i => i.CustomerId).Distinct().Count();
                    var atConsultation = consultations.Where(i => i.advertType == advertType.Id).Count();
                    var atConsultationSuccess = consultations.Where(i => i.VisitStatus == 2 && i.advertType == advertType.Id).ToList();
                    var atTickets = tickets.Where(i => i.CreatedOn >= start && i.CreatedOn <= end && i.Customer.AdvertTypeId == advertType.Id).ToList();
                    var atOneTimeTickets = tickets.Where(i => i.CreatedOn >= start && i.CreatedOn <= end && i.Customer.AdvertTypeId == advertType.Id && i.TicketType.IsSmart && i.TicketType.IsActive && !i.TicketType.IsVisit && !i.TicketType.IsGuest && i.TicketType.Units == 8).Count();
                    var avgTotalDaysFromCreateCustomerToConsultation = (atConsultationSuccess.Any())
                        ? atConsultationSuccess.Average(i => (decimal)((i.VisitDate - i.CustomerCreatedOn).TotalDays))
                        : 0m;
                    var proceedsByAdvertTypeId = incomeTickets.Where(i => i.AdvertTypeId == advertType.Id).Select(i => i.Amount).Sum()
                        + (incomeGoods.Where(i => i.AdvertTypeId == advertType.Id).Sum(i => (decimal)i.Amount * i.Priсe)) ?? 0m
                        + incomeGoodPackages.Where(i => i.AdvertTypeId == advertType.Id).Select(i => i.Line).Sum()
                        + incomeCertificatesUse.Where(i => i.AdvertTypeId == advertType.Id).Select(i => i.Amount).Sum()
                        + incomeCertificatesSell.Where(i => i.AdvertTypeId == advertType.Id).Select(i => i.Amount).Sum()
                        + incomeDeposits.Where(i => i.AdvertTypeId == advertType.Id).Select(i => i.Amount).Sum();

                    if (atCalls > 0 || atCustomers > 0 || atConsultation > 0 || atTickets.Count() > 0 || atOneTimeTickets > 0)
                    {
                        res.Rows.Add(
                            advertType.Name, //Рекламный канал,
                            atCalls, // Количество звонков
                            atCustomers, //Количетво созданных клиентов
                            atPerspective, //Количество перспективных клиентов(результат события не "Отказ")
                            atConsultation, //Количество записанных на консультацию
                            atConsultationSuccess.Count(), //Количество прошедших консультацию
                            atTickets.Count(), //Количество купленных абонементов
                            atOneTimeTickets, //Количество разовых
                            avgTotalDaysFromCreateCustomerToConsultation != 0 ? (int)avgTotalDaysFromCreateCustomerToConsultation + " дней" : "---", //Средний срок от занесения в базу до консультации
                            GetStringDecimalRoundToRUB(proceedsByAdvertTypeId) // Выручка
                        );
                    }
                }
                res.Rows.Add("Итого", calls.Count(), customers.Count(), calls.Where(i => i.Result != "Отказ").Select(i => i.CustomerId).Distinct().Count(), consultations.Count(), consultations.Where(i => i.VisitStatus == 2).ToList().Count(), tickets.Where(i => i.CreatedOn >= start && i.CreatedOn <= end).ToList().Count(), tickets.Where(i => i.CreatedOn >= start && i.CreatedOn <= end && i.TicketType.IsSmart && i.TicketType.IsActive && !i.TicketType.IsVisit && !i.TicketType.IsGuest && i.TicketType.Units == 8).Count(), "", GetStringDecimalRoundToRUB(proceeds - incomeAnothers.Sum()));

                #region legacy
                //var incomingCallCustomerIds = context.Calls.Where(i =>
                //                                                i.IsIncoming
                //                                                && i.DivisionId == divisionId
                //                                                && i.ResultType == (int)ServiceModel.Organizer.CallResult.NewCustomer
                //                                                && i.StartAt >= start
                //                                                && i.StartAt <= end).Select(i => i.CustomerId).Distinct().ToList();
                //var referralCustomerIds = customers.Where(i => i.InvitorId.HasValue).Select(i => i.Id).ToList();
                //var leadCustomerIds = customers.Where(i => i.FromSite).Select(i => i.Id).ToList();
                //var countLeadCustomerIds = leadCustomerIds.Count();
                //var amountPotentialCustomers = incomingCallCustomerIds.Count() + referralCustomerIds.Count() + countLeadCustomerIds;

                //res.Rows.Add("Количество всех обращений потенциальных клиентов", amountPotentialCustomers.ToString(), "Звонки, рекомендации, лиды (за выбранный месяц)");
                //res.Rows.Add("Количество входящих звонков", incomingCallCustomerIds.Count().ToString(), "Звонок окончился созданием нового клиента (за выбранный месяц)");
                //res.Rows.Add("Количество рефералов", referralCustomerIds.Count().ToString(), "Привела подруга / Рекомендация (за выбранный месяц)");
                //res.Rows.Add("Количество лидов с сайта", countLeadCustomerIds.ToString(), "За выбранный месяц");

                //var ticketPotentialCustomers = context.Tickets.Where(i => // абонементы, созданные по потенциальным клиентам
                //                                                        incomingCallCustomerIds.Contains(i.CustomerId)
                //                                                        || referralCustomerIds.Contains(i.CustomerId)
                //                                                        || leadCustomerIds.Contains(i.CustomerId)
                //                                                        ).Select(i => new
                //                                                        {
                //                                                            CustomerId = i.CustomerId,
                //                                                            IsVisit = i.TicketType.IsVisit,
                //                                                            IsGuest = i.TicketType.IsGuest
                //                                                        }).ToList();
                //Количество продаж по потенциальным клиентам за выбранный месяц
                // AmountPotentialCustomersToCustomers = ticketPotentialCustomers.Select(i => i.CustomerId).Distinct().Count();
                //res.Rows.Add("Количество продаж по потенциальным клиентам", AmountPotentialCustomersToCustomers.ToString(), "Из всех обращений за выбранный месяц - " + start.ToString("MMMM yyyy"));
                //Конверсия из всех обращений в продажи за выбранный месяц
                //var percentPotentialCustomersToCustomers = GetPercent(AmountPotentialCustomersToCustomers, amountPotentialCustomers);
                //res.Rows.Add("Доля из всех обращений потенциальных клиентов в продажи", (percentPotentialCustomersToCustomers == 0) ? "0%" : percentPotentialCustomersToCustomers.ToString() + "%", "По потенциальным клиентам за выбранный месяц - " + start.ToString("MMMM yyyy"));

                #region _PrevMonth
                //var start_PrevMonth = start.AddMonths(-1);
                //var end_PrevMonth = start_PrevMonth.AddMonths(1).AddMilliseconds(-1);

                //Количество всех обращений потенциальных клиентов (кто позвонил, оставил заявку на сайте, рефералы) за прошлый месяц 
                //var customers_PrevMonth = context.Customers.Where(i => i.ClubId == divisionId && i.CreatedOn >= start_PrevMonth && i.CreatedOn <= end_PrevMonth).ToList();

                //var incomingCallCustomerIds_PrevMonth = context.Calls.Where(i =>
                //                                                i.IsIncoming
                //                                                && i.DivisionId == divisionId
                //                                                && i.ResultType == (int)ServiceModel.Organizer.CallResult.NewCustomer
                //                                                && i.StartAt >= start_PrevMonth
                //                                                && i.StartAt <= end_PrevMonth).Select(i => i.CustomerId).Distinct().ToList();

                //var referralCustomerIds_PrevMonth = customers_PrevMonth.Where(i => i.InvitorId.HasValue).Select(i => i.Id).ToList();
                //var leadCustomerIds_PrevMonth = customers_PrevMonth.Where(i => i.FromSite).Select(i => i.Id).ToList();
                //var countLeadCustomerIds_PrevMonth = leadCustomerIds_PrevMonth.Count();
                //var amountPotentialCustomers_PrevMonth = incomingCallCustomerIds_PrevMonth.Count() + referralCustomerIds_PrevMonth.Count() + countLeadCustomerIds_PrevMonth;

                //var ticketPotentialCustomers_PrevMonth = context.Tickets.Where(i => // абонементы, созданные по потенциальным клиентам
                //                                                        incomingCallCustomerIds_PrevMonth.Contains(i.CustomerId)
                //                                                        || referralCustomerIds_PrevMonth.Contains(i.CustomerId)
                //                                                        || leadCustomerIds_PrevMonth.Contains(i.CustomerId)
                //                                                        ).Select(i => new
                //                                                        {
                //                                                            CustomerId = i.CustomerId,
                //                                                            IsVisit = i.TicketType.IsVisit,
                //                                                            IsGuest = i.TicketType.IsGuest
                //                                                        }).ToList();

                //Количество продаж по потенциальным клиентам за прошлый месяц
                //var AmountPotentialCustomersToCustomers_PrevMonth = ticketPotentialCustomers_PrevMonth.Select(i => i.CustomerId).Distinct().Count();
                //res.Rows.Add("Количество продаж по потенциальным клиентам", AmountPotentialCustomersToCustomers_PrevMonth.ToString(), "Из всех обращений за прошлый месяц - " + start_PrevMonth.ToString("MMMM yyyy"));
                //Конверсия из всех обращений в продажи за прошлый месяц
                //var percentPotentialCustomersToCustomers_PrevMonth = GetPercent(AmountPotentialCustomersToCustomers_PrevMonth, amountPotentialCustomers_PrevMonth);
                //res.Rows.Add("Доля из всех обращений потенциальных клиентов в продажи", (percentPotentialCustomersToCustomers_PrevMonth == 0) ? "0%" : percentPotentialCustomersToCustomers_PrevMonth.ToString() + "%", "По потенциальным клиентам за прошлый месяц - " + start_PrevMonth.ToString("MMMM yyyy"));

                #endregion

                //Количество пробных занятий по лидам
                //var leadCustomersToVisits = ticketPotentialCustomers.Where(i => i.IsVisit && leadCustomerIds.Contains(i.CustomerId)).Select(i => i.CustomerId).Distinct().ToList();
                //res.Rows.Add("Количество пробных занятий по лидам", leadCustomersToVisits.Count().ToString(), "За выбранный месяц");
                //Доля лидов, записанных на пробное занятие
                //var percentLeadCustomersToVisit = GetPercent(leadCustomersToVisits.Count(), countLeadCustomerIds);
                //res.Rows.Add("Доля лидов, записанных на пробное занятие", (percentLeadCustomersToVisit == 0) ? "0%" : percentLeadCustomersToVisit.ToString() + "%", "За выбранный месяц");

                //Количество лидов купивших абонемент после пробного занятия
                //var amountVisitToCustomer = context.Tickets.Where(i =>
                //                                    !i.TicketType.IsGuest
                //                                    && !i.TicketType.IsVisit
                //                                    && !i.ReturnDate.HasValue
                //                                    && leadCustomersToVisits.Contains(i.CustomerId)
                //                                    ).Select(i => i.CustomerId).Distinct().Count();
                //res.Rows.Add("Количество лидов купивших абонемент после пробного занятия", amountVisitToCustomer.ToString(), "За выбранный месяц");
                //Доля лидов, купивших абонемент после пробного занятия
                //var percentVisitToCustomer = GetPercent(amountVisitToCustomer, leadCustomersToVisits.Count());
                //res.Rows.Add("Доля лидов, купивших абонемент после проведения пробного занятия", (percentVisitToCustomer == 0) ? "0%" : percentVisitToCustomer.ToString() + "%", "За выбранный месяц");
                #endregion
                #endregion

                #region Выручка от продажи товаров
                AddNameParametersInRow(res, "Выручка от продажи товаров");
                //Выручка за продажи из бара
                var goodPrices = context.GoodSales
                                        .Where(i => i.BarOrder.DivisionId == divisionId && i.BarOrder.PurchaseDate >= start && i.BarOrder.PurchaseDate <= end && !i.ReturnDate.HasValue && i.PriceMoney.HasValue && !i.PriceBonus.HasValue && i.BarOrder.DepositPayment == 0)
                                        .GroupBy(i => i.BarOrder.CustomerId)
                                        .Select(i => new { CustomerId = i.Key, Amount = i.Sum(j => j.Amount * (double)j.PriceMoney.Value) });
                res.Rows.Add("Выручка за продажи из бара", ((!goodPrices.Any()) ? "0" : goodPrices.Sum(i => i.Amount).ToString()) + " руб.", "За выбранный месяц");

                //Средний чек по товарам, потраченный клиентами
                res.Rows.Add("Средний чек по продажам из бара", (!goodPrices.Any() ? "0" : GetStringDecimalRoundToRUB(goodPrices.Average(i => i.Amount))), "Всеми клиентами, пользующимися баром за выбранный месяц");

                //доля клиентов, пользующаяся услугами бара
                var percentCustomersUsedBar = context.GoodSales.Where(i => i.BarOrder.DivisionId == divisionId).Select(i => i.BarOrder.CustomerId).Distinct().Count();
                res.Rows.Add("Доля клиентов, пользующаяся услугами бара", GetPercent(percentCustomersUsedBar, tickets.Select(i => i.CustomerId).Distinct().Count()).ToString() + "%", "Клиентами (имели когда-либо абонемент) за всё время");
                #endregion

                #region Расчет прибыли
                AddNameParametersInRow(res, "Расчет прибыли");

                var plan = context.SalesPlans.Where(i => i.DivisionId == divisionId && i.Month == start.Date).Select(i => i.Value + i.CorpValue).FirstOrDefault();
                res.Rows.Add("План выручки", (plan != 0) ? GetStringDecimalRoundToRUB(plan) : "План не выставлен", "За выбранный месяц");
  
                res.Rows.Add("Выручка клуба", GetStringDecimalRoundToRUB(proceeds), "");

                //расходы ручные
                var spendingsAnother = context.Spendings.Where(i => i.CreatedOn >= start
                                                       && i.CreatedOn <= end
                                                       && i.DivisionId == divisionId)
                                                       .Select(i => new { i.Amount, TypeName = i.SpendingType.Name }).ToList();
                //вывод средст с депозита
                var returnDeposits = context.DepositOuts.Where(i => (i.Customer.ClubId ?? i.Customer.CustomerCards.FirstOrDefault().DivisionId) == divisionId && i.ProcessedOn.HasValue && i.ProcessedOn >= start && i.ProcessedOn <= end)
                       .Select(i => i.Amount).ToList().Sum();

                var spendings = spendingsAnother.Select(i => i.Amount).ToList().Sum() + returnDeposits;
                res.Rows.Add("Расходы", GetStringDecimalRoundToRUB(spendings), "");

                foreach (var sp in spendingsAnother.GroupBy(i => i.TypeName.Trim()).OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Sum(j => j.Amount)))
                {
                    res.Rows.Add(
                        String.Format("Расходы (Категория: {0})", !String.IsNullOrWhiteSpace(sp.Key) ? sp.Key : "не указана"),
                        GetStringDecimalRoundToRUB(sp.Value),
                        "");
                }

                if (returnDeposits > 0)
                {
                    res.Rows.Add("Расходы (Вывод средст с депозита)", GetStringDecimalRoundToRUB(returnDeposits), "");
                }


                var clearIncome = proceeds - spendings;
                res.Rows.Add("Чистая прибыль", GetStringDecimalRoundToRUB(clearIncome), "");

                res.Rows.Add("Рентабельность клуба", GetPercent((int)clearIncome, (int)proceeds) + "%", "Чистая прибыль / Расходы");

                // все оплаты абонементов за выбранный месяц
                var ticketPayments = tickets.Where(i => i.TicketPayments.Where(j => j.PaymentDate >= start && j.PaymentDate <= end).Any())
                                            .Select(i => new
                                            {
                                                TicketId = i.Id,
                                                CustomerId = i.CustomerId,
                                                Amount = i.TicketPayments.Where(j => j.PaymentDate >= start && j.PaymentDate <= end).Sum(j => j.Amount) - (i.ReturnCost ?? 0),
                                                Length = i.Length,
                                                CreatedOn = i.CreatedOn
                                            }).ToList();
                // существующие клиенты
                var oldCustomerIds = tickets.Where(i => i.CreatedOn < start).Select(i => i.CustomerId).Distinct().ToList();
                var incomeTicketOldCustomers = ticketPayments.Where(i => oldCustomerIds.Contains(i.CustomerId)).Sum(i => i.Amount);
                res.Rows.Add("Выручка по оплатам абонементов существующим клиентам", GetStringDecimalRoundToRUB(incomeTicketOldCustomers), "\"Существующий клиент\" - это клиент, который раньше имел абонемент");
                var rentabilityTicketOldCustomers = GetPercent((int)incomeTicketOldCustomers, (int)proceeds);
                res.Rows.Add("Рентабельность абонементов существующих клиентов", rentabilityTicketOldCustomers.ToString() + "%", "От общей выручки");

                // новые клиенты
                var newCustomerIds = tickets.Where(i => i.CreatedOn >= start && !oldCustomerIds.Contains(i.CustomerId)).Select(i => i.CustomerId).Distinct().ToList();
                var incomeTicketNewCustomers = ticketPayments.Where(i => newCustomerIds.Contains(i.CustomerId)).Sum(i => i.Amount);
                res.Rows.Add("Выручка по оплатам абонементов НОВЫМ клиентам", GetStringDecimalRoundToRUB(incomeTicketNewCustomers), "\"Новый клиент\" - это клиент, у которого раньше не было абонемента");
                var rentabilityTicketNewCustomers = GetPercent((int)incomeTicketNewCustomers, (int)proceeds);
                res.Rows.Add("Рентабельность абонементов новых клиентов", rentabilityTicketNewCustomers.ToString() + "%", "От общей выручки");

                // возвращенные клиенты = ticket.startDate.adddays(length).addmonths(-6)
                var returnCustomerIds = new List<Guid>();
                foreach (var t in ticketPayments)
                {
                    if (!tickets.Where(i => i.CustomerId == t.CustomerId && i.CreatedOn > t.CreatedOn.AddMonths(-6).AddDays(-t.Length) && i.CreatedOn < end && i.Id != t.TicketId).Any() // не было активных абонементов последние 6 месяцев)
                        && tickets.Where(i => i.CustomerId == t.CustomerId && i.CreatedOn < t.CreatedOn.AddMonths(-6).AddDays(-t.Length)).Any()) // были абонементы раньше
                    {
                        returnCustomerIds.Add(t.CustomerId);
                    }
                }
                var incomeTicketReturnCustomers = ticketPayments.Where(i => returnCustomerIds.Contains(i.CustomerId)).Sum(i => i.Amount);
                res.Rows.Add("Выручка по оплатам абонементов ВОЗВРАЩЕННЫХ клиентов", GetStringDecimalRoundToRUB(sumTicketsReturnCustomers), "Не было абонемента более 6 месяцев с даты окончания абонемента");
                var rentabilityTicketReturnCustomers = GetPercent((int)incomeTicketReturnCustomers, (int)proceeds);
                res.Rows.Add("Рентабельность абонементов возвращенных клиентов", rentabilityTicketReturnCustomers.ToString() + "%", "От общей выручки");
                #endregion

                #region Другие ПОКАЗАТЕЛИ
                AddNameParametersInRow(res, "Другие показатели");

                //количество клиентов, посетивших клуб за месяц 
                var customerVisits = context.CustomerVisits.Where(i => i.InTime >= start && i.InTime <= end && i.DivisionId == divisionId).Select(i => new { CustomerId = i.CustomerId, InTime = i.InTime }).ToList();
                var customerIdVisits = customerVisits.Select(i => i.CustomerId).ToList();

                //средняя посещаемость клиента в месяц
                var amountCustomerVisits = customerIdVisits.GroupBy(i => i).Select(i => new { CustomerId = i.Key, AmountVisits = i.Count() }).ToList();
                var avgCustomerVisits = (!amountCustomerVisits.Any()) ? "0" : GetStringDecimalRound(amountCustomerVisits.Average(i => i.AmountVisits));

                var percentCustomersLittleVisits = GetPercent(amountCustomerVisits.Where(i => i.AmountVisits < 6).Count(), amountCustomerVisits.Count);
                res.Rows.Add("Доля клиентов, посещающих клуб МЕНЕЕ 6 раз", percentCustomersLittleVisits.ToString() + "%", "За выбранный месяц");
                res.Rows.Add("Количество клиентов, посетивших клуб за месяц", customerIdVisits.Distinct().Count().ToString(), "Считается количество уникальных клиентов");

                var amountDayInMonth = (start.AddMonths(1) - start).TotalDays;
                res.Rows.Add("Среднее количество  клиентов в день", GetStringDecimalRound(customerIdVisits.Count / (decimal)amountDayInMonth), "Количество дней в выбранном месяце - " + amountDayInMonth);
                res.Rows.Add("Среднее количество посещений одного клиента в месяц", avgCustomerVisits.ToString(), "Среднее количество раз");

                var amountOutgoingCalls = context.Calls.Where(i => i.DivisionId == divisionId && i.StartAt >= start && i.StartAt <= end && !i.IsIncoming).Count();
                res.Rows.Add("Количество исходящих звонков клиентам", amountOutgoingCalls.ToString(), "За выбранный месяц");

                //Количествуо посещений и средняя посещаемость утром, днем и вечером
                var avgVisitsMorning = customerVisits.Where(i => i.InTime.Hour < 12).Count();
                var avgVisitsAfternoon = customerVisits.Where(i => i.InTime.Hour >= 12 && i.InTime.Hour < 16).Count();
                var avgVisitsEvening = customerVisits.Where(i => i.InTime.Hour >= 16).Count();
                res.Rows.Add("Количество посещений утром", avgVisitsMorning.ToString(), "Вход клиента до 12:00 (за выбранный месяц)");
                res.Rows.Add("Количество посещений днем", avgVisitsAfternoon.ToString(), "Вход клиента с 12:00 до 16:00 (за выбранный месяц)");
                res.Rows.Add("Количество посещений вечером", avgVisitsEvening.ToString(), "Вход клиента после 16:00 (за выбранный месяц)");
                res.Rows.Add("Средняя посещаемость утром", GetPercent(avgVisitsMorning, customerVisits.Count).ToString() + "%", "Вход клиента до 12:00 (за выбранный месяц)");
                res.Rows.Add("Средняя посещаемость днем", GetPercent(avgVisitsAfternoon, customerVisits.Count).ToString() + "%", "Вход клиента с 12:00 до 16:00 (за выбранный месяц)");
                res.Rows.Add("Средняя посещаемость вечером", GetPercent(avgVisitsEvening, customerVisits.Count).ToString() + "%", "Вход клиента после 16:00 (за выбранный месяц)");

                //Количествуо посещений и средняя посещаемость в будни, в субботу и в воскресенье
                var weekdays = new int[] { 1, 2, 3, 4, 5 };
                var avgVisitsWeekdays = customerVisits.Where(i => weekdays.Contains((int)i.InTime.DayOfWeek)).Count();
                var avgVisitsSaturday = customerVisits.Where(i => i.InTime.DayOfWeek == DayOfWeek.Saturday).Count();
                var avgVisitsSunday = customerVisits.Where(i => i.InTime.DayOfWeek == DayOfWeek.Sunday).Count();
                res.Rows.Add("Количество посещений в будние дни", avgVisitsWeekdays.ToString(), "За выбранный месяц");
                res.Rows.Add("Количество посещений по субботам", avgVisitsSaturday.ToString(), "За выбранный месяц");
                res.Rows.Add("Количество посещений по воскресеньям", avgVisitsSunday.ToString(), "За выбранный месяц");
                res.Rows.Add("Средняя посещаемость по в будние дни", GetPercent(avgVisitsWeekdays, customerVisits.Count).ToString() + "%", "За выбранный месяц");
                res.Rows.Add("Средняя посещаемость по субботам", GetPercent(avgVisitsSaturday, customerVisits.Count).ToString() + "%", "За выбранный месяц");
                res.Rows.Add("Средняя посещаемость по воскресеньям", GetPercent(avgVisitsSunday, customerVisits.Count).ToString() + "%", "За выбранный месяц");
                #endregion

                #region Посещаемость услуг
                var treatmentEvents = context.TreatmentEvents.Where(i =>
                            i.DivisionId == divisionId
                            && i.VisitStatus != 1
                            && i.VisitStatus != 0
                            && i.VisitDate >= start
                            && i.VisitDate <= end)
                            .GroupBy(i => i.Treatment.TreatmentType.Name).OrderByDescending(i => i.Count()).ToList();
                if (treatmentEvents.Any())
                {
                    AddNameParametersInRow(res, "Посещаемость услуг (количество за период)");

                    foreach (var t in treatmentEvents)
                    {
                        res.Rows.Add(t.Key.Substring(0, (t.Key.Contains("(")) ? t.Key.LastIndexOf('(') : t.Key.Length - 1),
                                     String.Format("{0} ({1}/{2})",
                                        t.Count(),
                                        t.Where(i => i.VisitStatus == 2).Count(),
                                        t.Where(i => i.VisitStatus == 3).Count()),
                                     "Всего (посещено/прогуляно)");
                    }
                }
                #endregion

                #region Количество проданных абонементов
                AddNameParametersInRow(res, "Количество проданных абонементов");
                res.Rows.Add("Тип абонемента", start.ToString("MMMM yyyy"), "Итого");
                var ticketSales = new CustomReports().TicketSales(start, end.Date, divisionId.Value, false, false, false, false);
                foreach (DataRow ts in ticketSales.Rows)
                {
                    res.Rows.Add(ts.ItemArray);
                }
                #endregion

                #region Количество проданных товаров
                AddNameParametersInRow(res, "Количество проданных товаров");
                res.Rows.Add("Параметр", "Товар", start.ToString("MMMM yyyy"), "Итого");
                var goodSales = new CustomReports().GoodSales(start, end.Date, divisionId.Value, false, false, false);
                foreach (DataRow gs in goodSales.Rows)
                {
                    res.Rows.Add(gs.ItemArray);
                }
                #endregion

                #region РАБОТА ПЕРСОНАЛА
                AddNameParametersInRow(res, "РАБОТА ПЕРСОНАЛА");
                res.Rows.Add("ФИО", "Исходящих звонков", "Количество консультаций", "Количество смен", "Количетсво проданных абонементов", "Сумма оплат абонементов", "Средняя цена абонементов", "Сумма продаж по бару", "Конверсия");

                var emploeeVisits = context.EmployeeVisits
                    .Where(i => i.Employee.MainDivisionId == divisionId && i.CreatedOn >= start && i.CreatedOn <= end && i.IsIncome)
                    .Select(i => i.EmployeeId)
                    .ToList();

                var employeeGoodSales = context.GoodSales
                    .Where(i => i.BarOrder.PurchaseDate >= start && i.BarOrder.PurchaseDate <= end && !i.ReturnDate.HasValue && i.PriceMoney.HasValue)
                    .Select(i => new
                    {
                        CreatedById = i.BarOrder.AuthorId,
                        Summary = (int)i.Amount * i.PriceMoney.Value
                    }).ToList();

                var employeeConsultations = context.TreatmentEvents
                            .Where(j => j.VisitDate >= start && j.VisitDate <= end && (j.VisitStatus == 2 || j.VisitStatus == 3) && j.TreatmentConfig.TreatmentTypeId == consultationTreatmentTypeId)
                            .Select(j => j.AuthorId)
                            .ToList();

                var employees = context.Users
                    .Where(i => i.EmployeeId.HasValue && i.Company.Divisions.Select(j => j.Id).Contains(divisionId.Value) && i.IsActive)
                    .Select(i => new
                    {
                        EmployeeId = i.EmployeeId,
                        UserId = i.UserId,
                        FullName = i.FullName,
                        CountOutgoingCalls = i.Calls.Where(j => j.StartAt >= start && j.StartAt <= end && !j.IsIncoming).Count(),
                    })
                    .OrderBy(i => i.FullName)
                    .ToList();

                foreach (var employee in employees)
                {
                    var coc = employee.CountOutgoingCalls;
                    var ec = employeeConsultations.Where(i => i == employee.UserId).Count();
                    var ev = emploeeVisits.Where(j => j == employee.EmployeeId).Count();
                    var et = tickets.Where(j => j.AuthorId == employee.UserId && j.CreatedOn >= start && j.CreatedOn <= end).Count();
                    var ets = GetStringDecimalRound(tickets.SelectMany(i => i.TicketPayments).Where(i => i.AuthorId == employee.UserId && i.PaymentDate >= start && i.PaymentDate <= end).Sum(i => i.Amount));
                    var eta = tickets.Where(j => j.AuthorId == employee.UserId && j.CreatedOn >= start && j.CreatedOn <= end).Any()
                                    ? GetStringDecimalRound(tickets.Where(j => j.AuthorId == employee.UserId && j.CreatedOn >= start && j.CreatedOn <= end).Select(j => j.Cost * (1 - j.DiscountPercent)).Average())
                                    : "0";
                    var egs = GetStringDecimalRound(employeeGoodSales.Where(j => j.CreatedById == employee.UserId).Sum(j => j.Summary));
                    var econv = ec == 0 ? "0" : Decimal.Round((decimal)((decimal)et / (decimal)ec) * 100, 2) + "%";

                    if (coc > 0 || ec > 0 || ev > 0 || et > 0 || ets != "0" || eta != "0" || egs != "0")
                    {
                        res.Rows.Add(employee.FullName, coc, ec, ev, et, ets, eta, egs, econv);
                    }
                }

                #endregion

                #region РЕКЛАМА
                AddNameParametersInRow(res, "РЕКЛАМА");

                res.Rows.Add("Наименование рекламного канала", "Сумма покупок в баре клиентами с этого канала", "Количество новых клиентов с этого канала", "Комментарий");

                var customerAdvertTypes = context.Customers
                                                 .Where(i => i.ClubId == divisionId && i.CreatedOn >= start && i.CreatedOn <= end)
                                                 .GroupBy(i => i.AdvertTypeId)
                                                 .ToList();
                var goodSalesCustomerAdvertTypes = context.GoodSales
                                                          .Where(i => i.BarOrder.DivisionId == divisionId && i.BarOrder.PurchaseDate >= start && i.BarOrder.PurchaseDate <= end && i.BarOrder.Customer.AdvertTypeId.HasValue)
                                                          .GroupBy(i => i.BarOrder.Customer.AdvertTypeId)
                                                          .ToDictionary(i => i.Key, i => i.Sum(j => j.BarOrder.CashPayment + j.BarOrder.CardPayment));

                foreach (var advertType in advertTypes)
                {
                    var customerAdvertType = customerAdvertTypes.Where(i => i.Key == advertType.Id).Count();
                    var goodSalesCustomerAdvertType = goodSalesCustomerAdvertTypes.Where(i => i.Key == advertType.Id).Sum(i => i.Value);

                    if (customerAdvertType > 0 || goodSalesCustomerAdvertType > 0)
                    {
                        res.Rows.Add(advertType.Name, GetStringDecimalRound(goodSalesCustomerAdvertType), customerAdvertType.ToString(), "За выбранный месяц");
                    }
                }
                #endregion
                return res;
            }
        }

        private void AddNameParametersInRow(DataTable res, string parameter)
        {
            if (String.IsNullOrWhiteSpace(parameter)) return;
            if (res.Rows.Count != 0) res.Rows.Add("", "", "");
            res.Rows.Add("          " + parameter.ToUpper());
        }

        private decimal GetPercent(int one, int two)
        {
            return (two == 0) ? 0 : Decimal.Round((decimal)one / (decimal)two * 100, 0);
        }

        private string GetStringDecimalRound(decimal value)
        {
            return Decimal.Round(value, 0).ToString();
        }

        private string GetStringDecimalRound(double value)
        {
            return GetStringDecimalRound((decimal)value);
        }

        private string GetStringDecimalRoundToRUB(decimal value)
        {
            return Decimal.Round(value, 0).ToString() + " руб.";
        }

        private string GetStringDecimalRoundToRUB(double value)
        {
            return GetStringDecimalRoundToRUB((decimal)value);
        }
    }
}
