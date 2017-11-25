using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using System.Text.RegularExpressions;
using TonusClub.ServiceModel;

using TonusClub.ServiceModel.Reports;
using TonusClub.ServiceModel.Reports.ClauseParameters;
using System.Xml.Serialization;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Data.Objects.SqlClient;
using System.Data.Objects;

namespace TonusClub.ServerCore
{
    public static class TemplatesCore
    {
        public static Dictionary<int, string> Scores = new Dictionary<int, string>
            {
                {-1, "Не взаимодействовали"},
                {0, "Отлично"},
                {1, "Хорошо"},
                {2, "Удовлетворительно"},
                {3, "Неудовлетворительно"},
                {4, "Плохо"}
            };

        static Regex reg = new Regex("{\\S*}");

        public static string GenerateCardContractReport(string cardNumber, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                var ccard = context.CustomerCards.FirstOrDefault(cc => cc.CardBarcode == cardNumber);
                if (ccard == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Карта с номером " + cardNumber + " в системе не обнаружена!</body></html>";
                ccard.Customer.InitActiveCard();
                ccard.Customer.Init();
                return ProcessTemplate(context, reportKey, ccard);
            }
        }

        public static string GenerateBarOrderReport(int orderId, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var order = context.BarOrders.FirstOrDefault(cc => cc.OrderNumber == orderId && cc.CompanyId == user.CompanyId);
                if (order == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Продажа в системе не обнаружена!</body></html>";
                order.Init();
                if (order.Provider != null) order.Provider.Init();
                order.Customer.Init();
                var sb = new StringBuilder();
                int x = 1;
                foreach (var i in order.GetContent())
                {
                    sb.Append(ProcessTemplate(context, reportKey + "Line", new { i = x, x = i }));
                    x++;
                }
                return ProcessTemplate(context, reportKey, order).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateGoodReport(Guid goodSaleId, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                var goodSale = context.GoodSales.FirstOrDefault(cc => cc.Id == goodSaleId);
                goodSale.Init();
                goodSale.Good.Init();
                goodSale.BarOrder.Customer.Init();
                goodSale.BarOrder.Customer.InitActiveCard();
                if (goodSale == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Продажа в системе не обнаружена!</body></html>";
                return ProcessTemplate(context, reportKey, goodSale);
            }
        }

        public static string GenerateChildRequestReport(Guid childId, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                var child = context.ChildrenRooms.FirstOrDefault(cc => cc.Id == childId);
                if (child == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Ошибка!</body></html>";
                child.Init();
                child.Customer.InitEssentials();
                child.Customer.Init();
                return ProcessTemplate(context, reportKey, child);
            }
        }

        public static string GenerateTicketContractReport(Guid parameter, object parameter1, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                Ticket ticket;
                if (reportKey == "GenerateLastTicketSaleReport" ||
                    reportKey == "GenerateLastTicketChangeReport" ||
                    reportKey == "GenerateTicketVatReport")
                {
                    ticket = context.Tickets.Where(t => t.CustomerId == parameter).OrderByDescending(t => t.CreatedOn).FirstOrDefault();
                    if (ticket == null)
                    {
                        ticket = context.Tickets.SingleOrDefault(i => i.Id == parameter);
                    }
                }
                else
                {
                    ticket = context.Tickets.SingleOrDefault(t => t.Id == parameter);
                }
                ticket.InitDetails();
                ticket.Customer.Init();
                ticket.Customer.InitActiveCard();
                if (ticket == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Абонемент с номером " + parameter + " в системе не обнаружен!</body></html>";
                if (ticket.InheritedFrom != null) ticket.InheritedFrom.InitDetails();

                if (reportKey == "GenerateTicketRebillStatementReport")
                {
                    var customer = context.Customers.SingleOrDefault(c => c.Id == (Guid)parameter1);
                    customer.Init(); customer.InitActiveCard();
                    return ProcessTemplate(context, reportKey, new { ticket = ticket, customer = customer });
                }

                if (reportKey == "GenerateTicketFreezeStatementReport")
                {
                    var freeze = parameter1 as TicketFreeze;
                    freeze.TicketFreezeReason = context.TicketFreezeReasons.SingleOrDefault(i => i.Id == freeze.TicketFreezeReasonId);
                    var cost = freeze.Length
                          * ticket.Customer.Company.FreezePrice
                          * (decimal)ticket.Customer.ActiveCard.SerializedCustomerCardType.FreezePriceCoeff
                          * (decimal)ticket.TicketType.FreezePriceCoeff
                          * ((freeze.TicketFreezeReason == null) ? 1 : (decimal)freeze.TicketFreezeReason.K4);
                    return ProcessTemplate(context, reportKey, new { ticket = ticket, freeze = freeze, cost = cost });
                }

                if (reportKey == "GenerateTicketReturnReport")
                {
                    return ProcessTemplate(context, reportKey, new { ticket = ticket, now = DateTime.Now });
                }

                return ProcessTemplate(context, reportKey, ticket);
            }
        }

        private static string ProcessTemplate(TonusEntities context, string reportKey, dynamic obj)
        {
            var cid = UserManagement.GetCompanyIdOrDefaultId(context);
            var templateItem = context.ReportTemplates.First(rt => rt.Name == reportKey && rt.CompanyId == cid);
            var template = Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "ru-RU" ? templateItem.HtmlText : templateItem.HtmlTextEn;
            var strs = reg.Matches(template);
            var res = template;
            foreach (Match i in strs)
            {
                var replaceTag = ProcessValue(obj, i.Value);
                if (replaceTag != null)
                {
                    res = res.Replace(i.Value, replaceTag);
                }
            }
            return res;
        }

        private static string ProcessValue(object val, string replaceTag)
        {
            if (val == null) return null;
            replaceTag = replaceTag.Replace("{", "").Replace("}", "");
            var fmtPos = replaceTag.IndexOf(':');
            string fmt = "";
            if (fmtPos > 0)
            {
                fmt = replaceTag.Substring(fmtPos + 1);
                replaceTag = replaceTag.Substring(0, fmtPos);
            }
            var attrs = replaceTag.Split('.');

            foreach (var attr in attrs)
            {
                val = val.GetValue(attr);
                if (val == null) return "";
            }

            if (fmt == "long" && val is DateTime)
            {
                return ((DateTime)val).ToString("dd MMMM yyyy");
            }

            if (fmt == "spell" && (val is decimal || val is double))
            {
                return RusCurrency.Str(Convert.ToDouble(val));
            }

            if (fmt == "spell" && val is int)
            {
                return RusNumber.Str((int)val, Gender.Male, "", "", "");
            }

            if (fmt == "grade" && val is int)
            {
                return Scores[(int)val];
            }

            if (!String.IsNullOrWhiteSpace(fmt))
            {
                return String.Format("{0:" + fmt + "}", val);
            }

            if (val is TimeSpan)
            {
                var ts = (TimeSpan)val;
                return ts.Hours.ToString() + ":" + ts.Minutes.ToString("00");
            }

            if (val is bool)
            {
                return ((bool)val) ? "Да" : "Нет";
            }


            return val.ToString();
        }

        public static string GenerateConsignmentReport(Guid consId)
        {
            using (var context = new TonusEntities())
            {
                var cons = context.Consignments.FirstOrDefault(cc => cc.Id == consId);
                if (cons == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>накладная в системе не обнаружена!</body></html>";

                cons.Init();
                var sb = new StringBuilder();

                foreach (var i in cons.ConsignmentLines.OrderBy(j => j.Position))
                {
                    sb.Append(ProcessTemplate(context, "GenerateConsignmentReport" + cons.DocType + "Line", i));
                }

                return ProcessTemplate(context, "GenerateConsignmentReport" + cons.DocType, cons).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateDepositOutStatementReport(Guid statementId)
        {
            using (var context = new TonusEntities())
            {
                var stat = context.DepositOuts.FirstOrDefault(cc => cc.Id == statementId);
                stat.Init();
                stat.Customer.Init();
                stat.Customer.InitActiveCard();
                if (stat == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Ваявление в системе не обнаружено!</body></html>";
                return ProcessTemplate(context, "GenerateDepositOutStatementReport", stat);
            }
        }

        public static string GeneratePriceListReport(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var div = context.Divisions.FirstOrDefault(i => i.Id == divisionId);
                if (div == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Клуб в системе не обнаружен!</body></html>";

                var sb = new StringBuilder();

                var pl = Core.GetAllPrices(divisionId);

                foreach (var i in pl.OrderBy(j => j.SerializedGoodName))
                {
                    sb.Append(ProcessTemplate(context, "GeneratePriceListReportLine", i));
                }


                return ProcessTemplate(context, "GeneratePriceListReport", div).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateEmployeeReport(Guid parameterId, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                if (reportKey == "GenerateEmployeeVacationOrder")
                {
                    var vac = context.EmployeeVacations.FirstOrDefault(i => i.Id == parameterId);
                    vac.Employee.Init();
                    return ProcessTemplate(context, reportKey, vac);
                }
                else if (reportKey == "GenerateEmployeeTripOrder")
                {
                    var trip = context.EmployeeTrips.FirstOrDefault(i => i.Id == parameterId);
                    trip.Employee.Init();
                    return ProcessTemplate(context, reportKey, trip);
                }
                else if (reportKey == "GenerateEmployeeCategoryOrder"
                    || reportKey == "GenerateApplyOrder"
                    || reportKey == "GenerateEmployeeFireOrder")
                {
                    var place = context.JobPlacements.FirstOrDefault(i => i.Id == parameterId);
                    place.Employee.Init();
                    return ProcessTemplate(context, reportKey, place);
                }
                else if (reportKey == "GenerateJobChangeOrder")
                {
                    var place = context.JobPlacements.FirstOrDefault(i => i.Id == parameterId);
                    var placeOld = context.JobPlacements.Where(i => i.EmployeeId == place.EmployeeId && i.CreatedOn < place.CreatedOn).OrderByDescending(i => i.CreatedOn).First();
                    placeOld.Employee.Init();
                    place.Employee.Init();
                    return ProcessTemplate(context, reportKey, new { Current = place, Old = placeOld });
                }

                var employee = context.Employees.FirstOrDefault(i => i.Id == parameterId);

                if (employee == null) return "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>Сотрудник в системе не обнаружен!</body></html>";
                employee.Init();
                var sb = new StringBuilder();

                if (reportKey == "GenerateJobDescription")
                {
                    if (employee.SerializedJobPlacement.Job.Duties == null) throw new Exception("Должностные обязанности для данной должности не указаны!");
                    foreach (var i in employee.SerializedJobPlacement.Job.Duties.Split('\n'))
                    {
                        sb.Append(ProcessTemplate(context, "GenerateJobDescriptionLine", new { duty = i }));
                    }
                }
                if (reportKey == "GenerateJobAgreement" && employee.SerializedJobPlacement.EmployeeCategory.IsPupilContract)
                {
                    reportKey = "GenerateJobStudyAgreement";
                }

                return ProcessTemplate(context, reportKey, employee).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateStateScheduleReport(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var division = context.Divisions.Single(i => i.Id == divisionId);
                var sb = new StringBuilder();
                var jobs = EmployeeCore.GetJobs(divisionId, context);
                foreach (var i in jobs)
                {
                    i.Init();
                    sb.Append(ProcessTemplate(context, "GenerateStateScheduleReportLine", i));
                }

                return ProcessTemplate(context, "GenerateStateScheduleReport", new { Division = division, total = jobs.Sum(i => i.Vacansies), totalSalary = jobs.Sum(i => i.Vacansies * i.Salary) }).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateEmployeeVacationList(Guid listId)
        {
            using (var context = new TonusEntities())
            {
                var list = context.VacationLists.Single(i => i.Id == listId);
                list.Init();
                var sb = new StringBuilder();
                foreach (var i in list.VacationListItems.OrderBy(i => i.StartDate))
                {
                    i.Employee.Init();
                    sb.Append(ProcessTemplate(context, "GenerateEmployeeVacationListLine", i));
                }
                return ProcessTemplate(context, "GenerateEmployeeVacationList", list).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateEmployeeScheduleReport(Guid graphId)
        {
            using (var context = new TonusEntities())
            {
                var graph = context.EmployeeWorkGraphs.Single(i => i.Id == graphId);
                var dict = EmployeeCore.DeserializeWorkGraph(graph.SerializedData);
                var header = new StringBuilder();
                var curr = graph.Begin;
                while (curr <= graph.End)
                {
                    header.Append(ProcessTemplate(context, "GenerateEmployeeScheduleReportHeader", curr));
                    curr = curr.AddDays(1);
                }
                var lines = new StringBuilder();
                foreach (var i in dict)
                {
                    var lins = "";
                    curr = graph.Begin;
                    while (curr <= graph.End)
                    {
                        if (i.Value.Contains(curr))
                        {
                            lins = lins + ProcessTemplate(context, "GenerateEmployeeScheduleReportLineAdd", curr);
                        }
                        else
                        {
                            lins = lins + ProcessTemplate(context, "GenerateEmployeeScheduleReportLineEmptyAdd", curr);
                        }
                        curr = curr.AddDays(1);
                    }
                    var placement = context.JobPlacements.Single(j => j.Id == i.Key);
                    lines.Append(ProcessTemplate(context, "GenerateEmployeeScheduleReportLine", placement).Replace("{ ins}", lins));
                }
                return ProcessTemplate(context, "GenerateEmployeeScheduleReport", graph).Replace("{ head}", header.ToString()).Replace("{ lines}", lines.ToString());
            }
        }

        public static string GenerateCardContractReport(string key, Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var division = context.Divisions.FirstOrDefault(i => i.Id == divisionId);
                division.Init();
                var sb = new StringBuilder();
                foreach (var i in context.EmployeeDocuments.Where(i => i.CompanyId == division.CompanyId).OrderBy(i => i.CreatedOn).ThenBy(i => i.Number))
                {
                    i.Init();
                    if (i.DivisionId != divisionId) continue;
                    sb.Append(ProcessTemplate(context, key + "Line", i));
                }
                return ProcessTemplate(context, key, division).Replace("{ lines}", sb.ToString());
            }
        }

        public static string GenerateFirstRun(KeyValuePair<string, string>[] parameters, Guid divisionId)
        {
            var pars = parameters.ToDictionary(i => i.Key, i => i.Value);
            HardwareConfigWriter.AddMachineConfig(pars, "Server");
            pars.Add("TimeZone", TimeZone.CurrentTimeZone.StandardName);
            pars.Add("Ping", GetPing());
            using (var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId);
                if (!String.IsNullOrEmpty(div.Act)) return div.Act;

                var sb = new StringBuilder(context.ReportTemplates.Single(i => i.Name == "GenerateFirstRun" && i.CompanyId == div.CompanyId).HtmlText);

                foreach (var p in pars)
                {
                    sb.Replace("{" + p.Key + "}", p.Value);
                }

                var res = sb.ToString();
                div.Act = res;
                context.SaveChanges();
                return res;
            }
        }

        private static string GetPing()
        {
            var ping = new Ping();
            var res = ping.Send("asu.flagmax.ru");
            if (res.Status == IPStatus.Success)
            {
                return "Успешно, задержка: " + res.RoundtripTime.ToString();
            }
            else
            {
                return "Не успешно, код: " + res.Status.ToString();
            }
        }

        public static string GenerateTicketReceiptReport(Guid pmtId, string reportKey)
        {
            using (var context = new TonusEntities())
            {
                var pmt = context.TicketPayments.FirstOrDefault(i => i.Id == pmtId);
                pmt.Ticket.InitDetails();
                pmt.Ticket.Customer.Init();
                return ProcessTemplate(context, reportKey, pmt);
            }
        }

        public static string GenerateAnketReport(Guid anketId)
        {
            using (var context = new TonusEntities())
            {
                var anket = context.Ankets.FirstOrDefault(i => i.Id == anketId);
                var tsb = new StringBuilder();
                foreach (var t in anket.AnketTickets)
                {
                    tsb.Append(ProcessTemplate(context, "GenerateAnketTicketLine", t));
                }
                var trsb = new StringBuilder();
                foreach (var tr in anket.AnketTreatments)
                {
                    trsb.Append(ProcessTemplate(context, "GenerateAnketTreatmentLine", tr));
                }
                var asb = new StringBuilder();
                foreach (var a in anket.AnketAdverts.Where(i => i.HadPlace))
                {
                    asb.Append(ProcessTemplate(context, "GenerateAnketAdvertLine", a));
                }
                var div = context.Divisions.Single(i => i.Id == anket.DivisionId);
                return ProcessTemplate(context, "GenerateAnket", anket)
                    .Replace("{ ticketLines}", tsb.ToString())
                    .Replace("{ treatmentLines}", trsb.ToString())
                    .Replace("{ advertLines}", asb.ToString())
                    .Replace("{ DivisionName}", div.Name);

            }
        }

        public static string GeneratePKOReport(Guid cashInOrderId)
        {
            using (var context = new TonusEntities())
            {
                var pko = context.CashInOrders.FirstOrDefault(i => i.Id == cashInOrderId);
                pko.DivNumber = context.Divisions.Where(i => i.Id == pko.DivisionId).SelectMany(i => i.Company.Divisions).OrderBy(i => i.Name).Select(i => i.Id).ToList().IndexOf(pko.DivisionId)+1;

                return ProcessTemplate(context, "GeneratePKO", pko);
            }
        }

        public static string GenerateRKOReport(Guid cashOutOrderId)
        {
            using (var context = new TonusEntities())
            {
                var pko = context.CashOutOrders.FirstOrDefault(i => i.Id == cashOutOrderId);
                pko.DivNumber = context.Divisions.Where(i => i.Id == pko.DivisionId).SelectMany(i => i.Company.Divisions).OrderBy(i => i.Name).Select(i => i.Id).ToList().IndexOf(pko.DivisionId)+1;
                return ProcessTemplate(context, "GenerateRKO", pko);
            }
        }

        public static string GenerateCashierPageReport(Guid divisionId, DateTime date)
        {
            using (var context = new TonusEntities())
            {
                var index = context.Divisions.Where(i => i.Id == divisionId).SelectMany(i => i.Company.Divisions).OrderBy(i => i.Name).Select(i => i.Id).ToList().IndexOf(divisionId)+1;
                var div = context.Divisions.Single(i => i.Id == divisionId);


                var begin = new DateTime(date.Year, 1, 1);
                var startBalance = context.CashInOrders.Where(i => i.CreatedOn > begin && i.CreatedOn < date && i.DivisionId == divisionId).Sum(i => (decimal?)i.Amount) ?? 0;
                startBalance -= context.CashOutOrders.Where(i => i.CreatedOn > begin && i.CreatedOn < date && i.DivisionId == divisionId).Sum(i => (decimal?)i.Amount) ?? 0;
                var number = date.DayOfYear;

                var incomes = context.CashInOrders.Where(i => i.DivisionId == divisionId && EntityFunctions.TruncateTime(i.CreatedOn) == date).ToArray();
                var outcomes = context.CashOutOrders.Where(i => i.DivisionId == divisionId && EntityFunctions.TruncateTime(i.CreatedOn) == date).ToArray();

                var TotalIncome = incomes.Sum(i => (decimal?)i.Amount) ?? 0;
                var TotalOutcome = outcomes.Sum(i => (decimal?)i.Amount) ?? 0;

                var sb = new StringBuilder();
                foreach (var inc in incomes)
                {
                    sb.AppendLine(ProcessTemplate(context, "CashierPageLine", new { Number = index + "/" + inc.Number, AmountIncome = inc.Amount, CreatedBy = "Принято от " + inc.CreatedBy.FullName, inc.Debet }));
                }
                foreach (var outc in outcomes)
                {
                    sb.AppendLine(ProcessTemplate(context, "CashierPageLine", new { Number = index + "/" + outc.Number, AmountOutcome = outc.Amount, CreatedBy = "Выдано " + outc.ReceivedByText, outc.Debet }));
                }

                return ProcessTemplate(context, "CashierPage", new
                {
                    Date = date,
                    DocumentNumber = index + "/" + number,
                    StartBalance = startBalance,
                    TotalIncome,
                    TotalOutcome,
                    IncomeAmount = incomes.Count(),
                    OutcomeAmount = outcomes.Count(),
                    Total = startBalance + TotalIncome - TotalOutcome,
                    Responsible = div.Company.GeneralManagerName,
                    Company = div.Company,
                    CurrentUser = UserManagement.GetUser(context)
                }).Replace("{ OrderLines}", sb.ToString());
            }
        }
    }
}
