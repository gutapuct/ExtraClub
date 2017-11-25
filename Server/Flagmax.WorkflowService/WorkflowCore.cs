﻿using System;
using System.Collections.Generic;
using System.Linq;
using TonusClub.Entities;
using TonusClub.ServiceModel;
using TonusClub.ServerCore;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Diagnostics;
using System.Data.Objects.SqlClient;
using System.Threading;
using System.Data.Objects;
using Microsoft.Win32;
using System.Data;
using System.ServiceModel;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ClaimServiceContract;

namespace Flagmax.WorkflowService
{
    public static class WorkflowCore
    {
        private static object _syncObj = new object();
        private static ChannelFactory<IClaimService> factory = new ChannelFactory<IClaimService>("ClaimServiceEndpoint");

        private static bool IsSyncBlocked { get; set; }

        public static Guid InternalUserId = Guid.Empty;
        public static DateTime DailyLastStart
        {
            get
            {
                var key = Registry.LocalMachine.CreateSubKey("Software\\Flagmax\\");

                if(key != null)
                {
                    var res = DateTime.Parse(key.GetValue("DailyLastStartTonusclub", DateTime.MinValue).ToString());
                    try
                    {
                        //Logger.Log("Дата в реестре не найдена");
                        //Logger.Log(new StackTrace());

                        if(res == DateTime.MinValue)
                        {
                            key.SetValue("DailyLastStartTonusclub", DateTime.Today.AddDays(-1));
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.Log("Ошибка при назначении даты в реестре");
                        Logger.Log(ex);
                    }
                    return res;
                }
                throw new NullReferenceException("Software\\Flagmax is null");
            }
            set
            {
                Logger.Log("Установка даты в реестре");
                Logger.Log(new StackTrace());
                var key = Registry.LocalMachine.CreateSubKey("Software\\Flagmax\\");
                if(key != null)
                {
                    key.SetValue("DailyLastStartTonusclub", value);
                }
            }
        }

        private static int HourlyLastStart
        {
            get;
            set;
        }

        private static DateTime _syncLastStart = DateTime.Today.AddDays(-1);
        private static DateTime _syncLastCompleted = DateTime.Today.AddDays(-1);

        public static void StartWorkflow(CancelThreadInfo e)
        {
            using(var context = new TonusEntities())
            {
                var divs = GetDivisionsEnumerable(context).ToList();
                if(e.Cancel) return;
                new EventsProcessor().Execute(divs);
                if(e.Cancel) return;
                var w = Stopwatch.StartNew();
                ProcessSolariumEvents(context, divs);
                if(e.Cancel) return;
                w.Stop();
                Thread t1 = null;
                Debug.WriteLine("Sol: " + w.ElapsedMilliseconds);
                Log.WriteLine(String.Format("Daily Test {0} : DailyLastStart {1}", DateTime.Now, DailyLastStart));

                if(DailyLastStart.Date != DateTime.Today && DateTime.Now.Hour > 1)
                {
                    Log.WriteLine("Запуск ежедневных задач");
                    lock(_syncObj)
                    {
                        DailyLastStart = DateTime.Today;
                        //if (context.Companies.Count() == 1)
                        {
                            Log.WriteLine("Запуск ежедневных задач - 10");
                            ChargeSmartTickets(context, divs);
                            Log.WriteLine("Запуск ежедневных задач - 1");
                            ProcessTicketClosure(context, divs);
                            if (e.Cancel) return;
                            Log.WriteLine("Запуск ежедневных задач - 2");
                            CheckTicketsAutoActivate(context, divs);
                            if (e.Cancel) return;
                            Log.WriteLine("Запуск ежедневных задач - 3");
                            CheckTicketsActivity(context, divs);
                            if (e.Cancel) return;
                            Log.WriteLine("Запуск ежедневных задач - 4");
                            ProcessCustomerTasks(context, divs);
                            if (e.Cancel) return;
                            Log.WriteLine("Запуск ежедневных задач - 5");
                            ProcessDivisionTasks(context, divs);
                            if (e.Cancel) return;
                            //Log.WriteLine("Запуск ежедневных задач - 6");
                            //ProcessLicenseNotifications(context);
                            if (e.Cancel) return;
                            Log.WriteLine("Запуск ежедневных задач - 7");
                            ProcessCutomerBirthdays(context);
                            if (e.Cancel) return;
                            Log.WriteLine("Запуск ежедневных задач - 8");
                            ProcessEmployeesExit(context, divs);
                            if (e.Cancel) return;
                            //Log.WriteLine("Запуск ежедневных задач - 9"); //убрали анкеты (решение руководства)
                            //ProcessAnketTasks(context, divs);
                            //if (e.Cancel) return;
#if !BEAUTINIKA
                            //Log.WriteLine("Запуск ежедневных задач - 11");
                        }

                        //if(context.Companies.Count() > 1)
                        //{
                            //Log.WriteLine("Запуск ежедневных задач - 12");
                            //new CloseDivisionUpdater(e).Run();
                            //Log.WriteLine("Запуск ежедневных задач - 13 (создать задачу на обзвон клиентам с первым посещением)");
                            //CreateTaskCallToNewCustomers(context);
                            //Log.WriteLine("Запуск ежедневных задач - 14 (рассылка - отправка почты клиентам)");
                            //ClientTicketNotification(context);
                            //Log.WriteLine("Запуск ежедневных задач - 17 (создание смс на ДР, писем на ДР и бонусов на ДР)");
                            //new SmsBirthdayCore().DoSmsGen(new CancelThreadInfo());
                            //Log.WriteLine("Запуск ежедневных задач - 18 (списание бонусов за ДР, если не были потрачены за 3 месяца)");
                            //new SmsBirthdayCore().SubtractionBonuses(new CancelThreadInfo());
                        //}

                        if (DateTime.Today.Day == 1 && context.Companies.Count() == 1)
                        {
                            Log.WriteLine("Запуск ежедневных задач - 15. Клиент в прошлом месяце посетил клуб менее 6 раз");
                            ProcessClientLittleWalk(context);
                        }

                        if (context.Companies.Count() == 1)
                        {
                            var DefaultDivisionId = context.LocalSettings.Select(i => i.DefaultDivisionId).FirstOrDefault();
                            if (DefaultDivisionId.HasValue)
                            {
                                Log.WriteLine("Запуск ежедневных задач - 16. Автовыход клиента, если забыли оформить выход. Клуб: " + DefaultDivisionId);
                                ProcessClientOut(context, DefaultDivisionId);
                            }
                            else
                            {
                                Log.WriteLine("Запуск ежедневных задач - 16. Автовыход клиента, если забыли оформить выход. Клуба по умолчанию в LocalSettings нет!");
                            }
                        }
#endif
                        if (e.Cancel) return;

                        Log.WriteLine("Запуск ежедневных задач - фиксация");
                    }
                }

                //пока поставлю сюда, т.к. до твоего последнего обновления ( 09 июня 2015г ) до сюда воркфлов точно отрабатывал нормально
                //CheckNewCustomers();
                //убрал, как сказал Максим
                //if (e.Cancel) return;

                //if(context.Companies.Count() > 1)
                //{
                    //var claimsCore = new ClaimsCore();
                    //claimsCore.Process();
                    //if(e.Cancel) return;
                    //UpdateAnkets();
                    //if(e.Cancel) return;
                    //SendAnkets();
                    //if(e.Cancel) return;
                    //if (DateTime.Now.Hour > 0)
                    //{
                    //    Log.WriteLine("Отправка отчетов...");
                    //    t1 = new ReportGenerator(e, DateTime.Today).RunAsync();
                    //}
                    //if(e.Cancel) return;
                    //Hourly stuff
//                    if(HourlyLastStart != DateTime.Now.Hour)
//                    {
//#if !BEAUTINIKA
//                        ProcessSmsStuff(e);
//#endif
//                        HourlyLastStart = DateTime.Now.Hour;
//                    }
//                    if(e.Cancel) return;

                    //ProcessEmail(e);
                    //if (e.Cancel) return;
                //}
                //if(CheckSyncNeeded(context))
                //{
                //    Log.WriteLine("Начинаем синхронизацию...");
                //    _syncLastStart = DateTime.Now;
                //    ProcessSync();
                //    if(e.Cancel) return;
                //}

                //try
                //{
                //    ProcessClaimsSync();
                //}
                //catch(Exception ex)
                //{
                //    Logger.Log(ex);
                //}

                //try
                //{
                //    var ls = context.LocalSettings.FirstOrDefault();
                //    if(ls != null && ls.DefaultDivisionId.HasValue)
                //    {
                //        new UpdaterCore(ls.DefaultDivisionId.Value).Update();
                //    }
                //}
                //catch(Exception ex)
                //{
                //    Logger.Log(ex);
                //}
                if(t1 != null)
                {
                    while(t1.IsAlive)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private static void CheckNewCustomers()
        {
            using(var context = new TonusEntities())
            {
                if(!context.LocalSettings.Any())
                {
                    return;
                }

                SyncCore.UpdateCustomersFromSite();
            }
        }

        private static void ProcessSmsStuff(CancelThreadInfo cancelInfo)
        {
            //new MarketingCore().DoSmsGen(cancelInfo);
            //new SmsBirthdayCore().DoSmsGen(cancelInfo);

            var smsCore = new SmsCore();
            smsCore.DoSmsGen(cancelInfo);
            smsCore.CheckPending(cancelInfo);
            smsCore.SendPending(cancelInfo);
        }

        private static void ProcessEmail(CancelThreadInfo cancelInfo)
        {
            var emailCore = new EmailCore();
            emailCore.Send(cancelInfo);
        }


        private static void ProcessClaimsSync()
        {
            var claimStatusIds = new[] { 0, 5, 7 };
            if(new TonusEntities().Claims.Any(i => claimStatusIds.Contains(i.StatusId)))
            {
                SyncCore.SyncronizeClaims();
            }
        }

#if !BEAUTINIKA
        public static void ChargeSmartTickets(TonusEntities context, IEnumerable<Division> divs)
        {
            var yesterday = DateTime.Today.AddDays(-1);
            ChargeSmartTickets(context, divs, yesterday);
        }

        public static void ChargeSmartTickets(Guid divId, DateTime date)
        {
            var context = new TonusEntities();
            var div = context.Divisions.Single(i => i.Id == divId);
            ChargeSmartTickets(context, Enumerable.Repeat(div, 1), date);
        }

        private static string SetUpWords(string income)
        {
            var list = income.Split(' ').Where(x => x != null && x.Length > 0).ToList();
            list.ForEach(x => x = x[0].ToString().ToUpper() + x.Substring(1));
            return string.Join(" ", list);
        }

        public static bool EmailValidate(string email)
        {
            return (email ?? string.Empty).Length > 7 && email.Contains('.') && email.Contains('@');
        }

        private static void CreateTaskCallToNewCustomers(TonusEntities context)
        {
            var dateFrom = DateTime.Today.AddDays(-1);
            var dateTo = DateTime.Today;
            var customers = context.CustomerVisits
                                    .Where(i => i.InTime < dateTo)
                                    .GroupBy(i => i.CustomerId)
                                    .Where(i => i.Count() == 1)
                                    .SelectMany(i => i)
                                    .Where(i => i.InTime >= dateFrom)
                                    .Select(i => i.Customer).ToList();

            var divisionIds = customers.Select(i => i.ClubId).Distinct().ToList();
            var divisions = context.Divisions.Where(i => divisionIds.Contains(i.Id)).ToList();

            var customerIds = customers.Select(j => j.Id).ToList();
            var ticketNames = context.Tickets
                                        .Where(i => customerIds.Contains(i.CustomerId) && !i.TicketType.IsGuest && !i.TicketType.IsVisit && i.IsActive)
                                        .Select(i => new { customerId = i.CustomerId, ticketName = i.TicketType.Name, createdOn = i.CreatedOn })
                                        .ToList();

            var subject = dateFrom.ToString("dd.MM.yyyy") + " - Обзвонить всех по списку и собрать статистику по анкете";
            var text = new StringBuilder("Форма для заполнения: https://docs.google.com/forms/d/e/1FAIpQLSfHOJL7NPPowx97VOaegG8qBS4XGD5VS9XAo5x6-QufIsQNMw/viewform \r\n \r\n");
            foreach (var c in customers)
            {
                var club = divisions.Where(i => i.Id == c.ClubId).FirstOrDefault();
                if (club != null)
                {
                    var ticketName = ticketNames.Where(i => i.customerId == c.Id).OrderByDescending(i => i.createdOn).Select(i => i.ticketName).FirstOrDefault() ?? "Абонемента нет";
                    text.AppendFormat("{0} ({1}) - {2} ({3}). Абонемент - {4}\r\n", c.FullName, c.Phone2, club.Name, club.CityName + " " + club.Street + " " + club.Building, ticketName);
                }
            }

            var channel = factory.CreateChannel();

            using (channel as IDisposable)
            {
                var deadLine = DateTime.Today.AddDays(1).AddMinutes(-1);
                if (deadLine.DayOfWeek == DayOfWeek.Saturday) deadLine = deadLine.AddDays(2);
                if (deadLine.DayOfWeek == DayOfWeek.Sunday) deadLine = deadLine.AddDays(1);

                channel.CreateTask(new ClaimServiceContract.TaskInfo
                {
                    ManagerCheckingEmail = ConfigurationManager.AppSettings.Get("ManagerCheckingEmail"),
                    ManagerExecutorEmail = ConfigurationManager.AppSettings.Get("ManagerExecutorEmail"),
                    DateDeadline = deadLine,
                    IsNormative = false,
                    Subject = subject,
                    MaxScore = customers.Count * 2,
                    Result = "Совершен обзвон по всем клиентам; собрана статистика в Google-форму",
                    Text = text.ToString(),
                });
            }
        }

        private static void ClientTicketNotification(TonusEntities context)
        {
            // Даты, по которым проверяются купленные абонементы по разным рассылкам
            var toDay = DateTime.Today;
            //через месяц после покупки абонемента
            var dateToFirst = toDay.AddDays(-30);
            var dateFromFirst = toDay.AddDays(-34);

            //через 3 месяца после покупки абонемента
            var dateToSecond = toDay.AddDays(-92);
            var dateFromSecond = toDay.AddDays(-96);

            //через 8 месяцев после покупки абонемента
            var dateToThird = toDay.AddDays(-244);
            var dateFromThird = toDay.AddDays(-248);
            

            ClientTicketNotification(context, dateFromFirst, dateToFirst, 0);
            ClientTicketNotification(context, dateFromSecond, dateToSecond, 1);
            ClientTicketNotification(context, dateFromThird, dateToThird, 2);

        }

        private static void ClientTicketNotification(TonusEntities context, DateTime dateFrom, DateTime dateTo, int countEmails)
        {
            var tickets = context.Tickets.Where(i =>
                                                i.Emails.Where(k => k.TicketID == i.Id).Count() == countEmails
                                                && i.CreatedOn >= dateFrom
                                                && i.CreatedOn <= dateTo
                                                && !i.TicketType.IsGuest
                                                && !i.TicketType.IsVisit
                                                && i.IsActive
                                                && !i.ReturnDate.HasValue).ToArray();
            foreach (var item in tickets)
            {
                // Проверяем, что email валидный и текущий абонемент первый у клиента
                if (EmailValidate(item.Customer.Email) && !context.Tickets.Where(i =>
                                                                                    i.CreatedOn <= item.CreatedOn
                                                                                    && i.CustomerId == item.CustomerId
                                                                                    && i.Id != item.Id
                                                                                    && !i.TicketType.IsGuest
                                                                                    && !i.TicketType.IsVisit).Any()
                )
                {
                    try
                    {
                        var body = String.Empty;
                        var subject = String.Empty;
                        switch (countEmails)
                        {
                            case 0: //первое оповещение
                                subject = "Добро пожаловать в клуб счастливых женщин!";
                                body = string.Format("<p>Уважаемая {0}!<br/><br/>" +
                                    "{1:dd.MM.yyyy} Вы приобрели абонемент {2}.<br/><br/>" +
                                    "Благодарим Вас за выбор Европейской сети женских клубов ТОНУС-КЛУБ®!<br/><br/>" +
                                    "Мы надеемся, что SMART-тренировки в наших клубах помогут достичь и Ваших индивидуальных " +
                                    "целей, а также подарят радость занятий! Мы действительно гарантируем Вам хорошую фигуру!<br/><br/>" +
                                    "Мы будем очень благодарны, если Вы поделитесь  своими впечатлениями об атмосфере клуба, " +
                                    "сервисе и эффективности занятий, заполнив небольшую анкету: <a href=\"http://goo.gl/forms/KtUQxnDC4N\">http://goo.gl/forms/KtUQxnDC4N</a>. Это " +
                                    "займет не больше трех минут Вашего времени!<br/>" +
                                    "Оставив комментарий, Вы поможете другим покупателям определиться с выбором, а нашей сети совершенствоваться с учетом пожеланий наших клиентов.<br/><br/>" +
                                    "<u>Мы ценим Ваше мнение!</u></p>" +
                                    "Вы можете быстро оценить Ваш клуб: просто выберите один из вариантов:" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=5&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAHoAAAAVCAIAAAAGmW08AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAABRcSURBVFhHVVlJkxzHdX6ZWVl79TYbMEMMAZHiAooKSSSlkGTJB1t2hBX23X/CB198kgOO8O+xLz74oJDDYTpoK0RJXAQSINYZYJbu6bX2qqxMf1k9suyKQmO6u/Lle9/73pbNtCmIWiLFyBiSHcVKk8e56WouqMkr1xsSIxLaUNeRIuIM35NmtMHfHQ0I/9nlFYQQSTIRBDUGS/BNRlS75EA46ZwaIu+oNcR4x0hzIm0YI8cw4vger/gEi6ycDlrhLTMOkUOGWx2Y7sh0pBWpuq0iGQus6EhAUUNG1UxKiMbOjDq89heWQQzX+lrG9oIxeCPtbvN2M5WDm0RuZjrJIshziQkoaZ+G/mX/eKRNbBcxqN0CoV4+cGsY1VYNiqCJqlZOGJEJgVQvvXF6WHr7YIgAdsBC9JZazTTxtoP5xAVsvio2D5r8hHgN+2Fnf1t7ehugKz5XeItVcBUkGnKxEW5jsQaA2rFqC9IZVZ9vsv8mds55yQzjJDU0YhZrK2H7ioX2BljYHe9wN8Ra6GV6rLEdp8w0i3T2TFKOtx3fqquZ6IiBOktNK/gOwvvbgW7QR3NAA0dDW3xldYIBFsrmy7r+HdHM0oJZHBgx6214zGoCWAO8avgdu1hLWmZ1AyfxANgZwROWEBClTxfLfyP6gljd1pa8xrjQrLfMigXUnIwkCqxzKOhpaz8jA2kF5ffr/Fdl/isyzyDO7kWe9dr1BVkSrsbeeNPb5vcaWJAsM3rMe+LVJn+ZpZ/X5adEjzhdCQbHWRh6m7U2cNv1BWPgJAsgSfDPGmkNc3DjYUEZqx93m99Q+pkqH+KtZOAFJDkksBs0qXqV+gs72xd7QeZWT3zQy9luvUg3n7TNgzZ9QGbukv795/bpnjfWKG3xgbH2W2uTAQj2qd9LBiB4fKOyh3XxWZt9RuxKihbo2tt+6/e4eYBa3PvZvZ5aICC+EPjTA8XBgOY0W/xcqU81nXBWSP8ONyOEKjbAP8vwziXjbtMCIxfBd61ErwV4IjQXligV1Vfl8tO6+rVhM8S064WMjVnnAxbI4xwJrcXzULrPUVYbaMmsVYAbnpcAlGnIxOpTvv6wnH8Y0kumUsfbY84uogq8tfyxoY1XxGvAzDarbG2GTLwi6Uhu7IZ4ktS8qx+up7+QbFYVtS8j5u5zEwqs6AmJa7uqZ6DgVjjCAzmCQ8Q2KPGAtH7JqHi0nv1a8tMyn3smdgZDZDfBHKZdCxSCHF6GknYRbrinf3UUiFaQvqLqebn5yqUz0zzMV2DlHCQFx+Bu5DX7sPU+8PF6aHoHAPL+W7AATwr4TLdkMmpP2vyxLhe+qTZX9039nMwGe4Hi/b5achiIVVtCAHmHIebgKZu5QA283aawDamXVH1hNr9O3Odd+qlOH+JD7IiMDsOBch+mIczDEoEA2pKjhxtYQz3cVivasO5FVz5ts6lQabV+ZEpE8IKaDR5AArLU61fhAv3sQnwGftvlPVAgPFLnFgeTU/Wszu6PozxfPkhXnxCd62pmnbrF6vc32NVr1CHZVMRRFvDHhtizq5MP2/RqlAzHnjs/eUjr+6RP4DGrroVJ93AjQDnVjg1f4NsDDTtZQ12GTNvalE3TevbL1fTzAR9ENC6vzjaXX1p3spXNAVACDgZV4DnlWBZ0jmXQNkUpataEnMwbOAGV+ZSWn06ffTxOsPsSGK1mH5N5wtjKhyDorgJqYyoDW5MtCr0yNu44R+xCID5PG6o3ZK6oe3L66KOBiBKWON388vQjap6Qs6KqBt+YRgEscaOOaKyCPqjcTZ+orPktdXobutQgMeanD3/usqdMXd4c83Jzv5v+1vHAa+y9IZ5ZznWAl8S9v/9b4inJlMSS6mfkgkGPKf3l8upXrJsngdO1aZmnqi3CSJAPr9fITcQLEo31E6oBXlEQbZW1SDO+Zs6Ce/DZBbWPKP2kWHzU1i8SLzFGLTdPXFdF8cgmQ6e0qtDalmJuRfREvI5he4HqHjLOisszYo9IfT599oti9WA0FBSKfLPKmtp1OnQojLdcwLsgIdZA1P8KAVQADzo31GYkG/KWBGn5J+uL/yjTR7HDPV9X5WmWnSeCCR81AFUMxRnFc66pEjZz+NektmELYgMu0GVGLCNnQdVTWv3XavavnjsLpFtV1WozcxzmDya9N2bkbAi6Id6RgYx6rNSFUtOmnXXNZb08yVePiJ2adjMK93dvHFK7WS6nZ1erMDkW8g0m9/3hjhuA9Duef8zkG0SJtcf6vWq6c9U94Wq2ePlMqrzOzqg9a9oHvucd7r1P0j+ffpyWsOFYs30vPhxMXmXeTjR4lejQkQfEhz3MEAUOFm1+bsonbXu6LJ+p7pIhUPLLgOrDvYlI4quLy2ladWzAxV4UHo/DY6IDd/iaGx0Lf4eQhRA6HC5Eu7Kpy0tdX6kKvdbTYvOE05TRrEoXr95809sdNqv7Jy9OpcTC25od+uOb8cEgV46Ur46Cd4huEyqNaZBqdTYjfqXMw6J8inRhylWdzqv8xHWeT8ZJOLhbb+onL78wIhwO3y67wfDgjhFjzz9MotvcuSnu/cPfVPXp2flv15tHnpx25ZekTjy3dphwxShwgzJfZfk8DnnXZk21FlIJrylUWRvpR3uCo++2fWrPJVXXl+v15+v5Z66YOjT1+YbTEgEV+H7g7SEWdLtQyI+8lm7jBFpznRZNUQnfvyGcIeOhTSw2qTdNPc82X7HmvmOecjpv6pdNOfc9EUeTtnME84u6Rg6STsdN6ZosdBpHmkUG9RLPiwiUZKGNO+yq0tn0q6p+3taP2+YR715EfuryQlUFM27oiLZN66aQkkmnbU3WsXSdL5qOOWLH9fYE2+kri8LNnHZTvNxsHnQgdfNYVV+ReuE5q6beSCf0nSOjRdutXbeRUhmdd7Ld5JuqqoUABhNmzFP0eZcXv82yJ6EzvTlJqUSu2U0Xqm5oPAp1N62LVewlFAzJCYxB3vEzc+hG398dv2frkomuM4AtWeeb5b8Xm4/H4dKjFdSnOmuyJeOYnHaQbEwzM6wUk4Qcr1KDsjsom1tR8s3h3gekR5DQ1ORgbEAOMQtTfp6+/BdfXLhBCPl1umlUKwCJVlEULWZTh/HJzphcFI+maeu82ZXjn8Sj9wiBogdWKxAcLUQDlz+/OPtPre7vjwopUiqQBFrdsVoFfhS3ZVrVm0ESUugbIMpH50vE8HcHkx9wB9S2u0NUZ0ru1K16ubz4jUm/GMlTP1zbZNimTdkyhCY7aJQxZhkETHgBzFgprzQD1z2eDL/B/LfFvXs/Q76LkwFv1erymcifq3wl+EShygsZJrGQ0pW+KfqZrShms6LSB8nOd8LxtzXtIfujQHZwfA8444kveRLIs2efb+Yv3C5lqjE6dOTIIFpc344stvXoVsv1xaxoulGSvDu68T7psWWQDf++gUa4oB+TrdDr6dnz5fmZZ1pfos0H95EhY9dNMGp4ArVVV2uku7xSgebH41f+mJxbtjU2DgYzm0tQUJyAYXOdl9n5ZnGSLV7wpnSly6QvB7tMoCiFngzQdqqyWKV5mrmD+Duj0XeZ9yZGRNuo9+a1JEqGTf3ET+rFLJ0+lc1SmpTaViS3hNzVzNPccST6IkXFuszLRRF6we3d8VssQMLcEff+7h+JB8Qi38Q6L4r5l9nmpOUVeQ0L0GJ2VdqaKjfdWZO9AKGU3tvb+2l44080g0ugSKNZ2/UdEVrTnk0og/5Ar/P1k6Y9adScuI96VJnKQW+Yp221rtUiqyvh7e3uvxMld7l32LZo4DHt92NE33gjNeF/Hh1GXKbrr/IU/cyJ4JkxDmcSHma0kuw0S59M5+cdH+8e/fngzl8SvU4mMazuqDBODTJwgdkVwjon8pOQofPLl5Cz9vgqL2ad8EpLSM1ZUxYvrhYXbSsD/43x7p+Re5d4Yos3yzVrwHq07IXuJPccgRSEDuN5uviirk4wpBRZUBtXOYpcJUxXFfM8fVSUyvB3k/gDN36L2CF1EzSCkkxMeofiN/fe/pOd0Wtoy4t8Y0zqyJKx0qbJMM5yMGjpyPjo6x+4R98ndVApQIxGBZzkNvzRv2GcANwd+tNddvydMD6s2yavlqpLHVFjVuKidMahUtXs6jzLivHwdnTjfZHcJu1L1+1PPuzdGdMqVEuk3R1qjsTN7x+/8770+XJxVuVXvmCu5wFBeybRZW2z4FIPdg7d0V1b00zSZ3/RdxQoAgiuCmmARGylxW/sHn7TD4fL5TzLl1HihBELvFbysm3ny/WLNFtE8f7k69+l0R1yRrAOU1zf1qOnQ0UhTwQVetYqoPh2/PaPx+M7VVPWTSalF3ihjyHEVFVRpmlaVGv0kLfe/GF89AHJQyIo4GJURvyiMcV/MfFxePSOw/dGwc2daJ8VVbWcNvkSGw4Gx8nwrbw5oINvkRmiw/EdNFyokoHQnmN7WxtuBlOQ7fCAfTS++0ONYusnSeRwnolmXiyek1pHibe7u6uVH936MdVHlKIbSfqWHRCjjYfxle9ojhZTI+xGpCc0foOL3cC/NRreYU6Sns83l3PyBtw7CMNX8tIZjI8pPO5qUAd6oOPCcBR3NDCUaOZioAYxuzqgYkiTu4p5yWiMwsX9EWWbfPZc0JWXtPGAx6OhG9wg/6YNVdl2KHu2u4wFDR1CIbJa2hHMCUmhkt31om8wNw7iIAp8XjfVYsbKYhjGo3gnDPbi8S2Kj4gl/WiD4VdxtIL9/IvYQIPh5/OMy3gw3CXHVU3RVDkzCEnIHwt/UsGMFD2yxRQCsHc/eNtmGWVQ2wMg5EtohAluSBlvWj8a3MDartZFXjc14sGnZMSdsGoEzdCN7lKAps1i1He2+AtBYnG377YVGFNPWpQN96MJRQP0+8bJGr3S5Rxp3o9GXjierzDkuMI2JFtRuKCHFdeL0Q3GXT8gDz1G0qCxcQdudEBykqMlKSt78pDEw9FB24WLNVAFAzwsB9Zb1fpZqc8rKAv4HrIRzjpcpiiJk3AwogA1dJ2ms7bJyGXRYARanE/L/kjKQ83UqFhwuj0g+MOlW6OiSSxCt27LRb5eFat5sTmfXZDnBJOhP/QwztnRy1t2NNW06icICwoUsfb94XKK3HjRK358p22Hs6W72ESXSzk9TXUt/eHh3uGdtMj7oQmDDJ63AqBRn0Pgx/8zqtgzHOMP4ngnJpHN0i8vqi+u2q9OVvdtYYi9g6NXFJbY/LMdt/u7xwSrMXdz1E2rHD7sMDRKdxJP7mixt75sF4WcFc6zszI9U+Qc7ex9U7pHVITUxUCqPybrL/sfPLrq6Dmjc+IrkhUcrxiPxq9SuLfJLy+zk0U5vUzPZstnDVXJ+DAZ3uoqq0kvwIr4/xDhU0+yIF4Xcr4JSbw+nHwQJO9kzWi2Mg133VHQ2ENOCxPYbM95IWR7b5dbk3DZYbfTYTz4Wl5Pzq+82hwN998b7n173YwvUzdvktH+1+xU2lakciyzGbJf2Lc6yEfoTHpx2xwuOhkFNffmGS3LkQzvQppybi3rIKuMEyW2etgLQGN463HvlbKxbw+HkKPwFXSqTK2Yv6P5PoyarmI5eHfn5g+N98bJ1J2nQy+67QUHKP+9NLt/H7zbC/+jvljHYca2k7Cbdx4CfX+VJdONy7xb+6/8IBm9u0jd6VppZ5KMj4oKkz5WWIBQA9AI3rN/oTZj5qDF1ewj7mjVHUnn7u7ej4a3fhLvfi9yD5Z5ylzeGKV4GCUYtHY57XDy7fF/j3UfJCgiqFOQjthTq6tzGK+M39HuZP/u+PXvJTfujuK9jvxl2bQaQT0ZHrxp86CdkiDGDuC9Zvbe0sGK5avN+sOsfmlYwvnrg/inN17962T3T4f+1w130rpsmK7acDh4n4ndfhhBTbMnksgAkAG8wPyuU8jeJDB6pKvVWdtxxz2Ih2/tvvKj+NYfjffe4JQgtZWNLNswTPZFiFEZtgAsqACHI1vCUp/RgKHStErwlMzzi9l94XlZGYzG7904+HHyyk+Tybcd9Hjc10ysMukEb6IYcIaeDHmEY8yxtALWXdc5dH5x+s97I8eYmzI4ogBzoNcfGy1IPa6a87ScdXp04+gviI4Qa63qy4dFZUtOe2xtIbMHdPnT+x8mcTlGagrGmI9sWbXFHelsWW5ONxgs6snxaz8kFDQlmYPihoWoJVbQ9rJ/gmf8xdXZPwm5iOSB675G8lvEdu1+Yk76d3X2u6qZlk28N/kr4R3bY0B7Ya/rstn/WrDle20wGNfT9fqB57TDZJ/8feL7fQwU0Iqa8+Vika6dG4d33cFxn75RklD9PXseCW1Qmuw5cK+YOW+bz05OP7m5fyA4psBjYmM7W7GG6AWp8za7nC5UMP5eMnxdCq/TRnD3Gu4tu6krunzqDmNLUwwbiBd7pjW0P4VJYU8NuyIvVDSCKlBStaqSFiYoA0bDHnv23aOGVfXF84c3bsbkwnLo7WKAr6vSC6AqFEIjX+U5iweHRrkYNAhNN0iEeLMAWSm4kGvsGSZV2exxvLNH3Q5hykdVx4SZV+i7rn8G6S7sL2diTCK0TZFFAztaafZG6t/WXsLgmXYqc5zK8aA2RnyQIM6KwpfaAar2dMWUq8p3xyzYtYbYAIGMbaayQQNkNApF/4HpZmgcB5P9bXRiJ1Xag00e9A7GixYYNt0w7qMWjSkCrocbl0VcGYwFVlvg62OXTFcFl/vbo3FV9RyFVrC2IS+yx1LGGrbdbIvStTQAoduKu6IqCsE96cM2ywm7T1fYX2CuIRV1qbwAnQDIaHex7L4WpGAcQ6AY06SlG++aihjqvCQM8fbcj7NqRX5k3dLnazRF24YU76+lXV9MdU0m7E9V2BfdsGZIwvZXMdu3qk679pfYGhZyBwv7c23wo9ekr7r4FrRGAXCZllVB6HFATtubdB3nKDakEOjQDRfeMGXa1mCqD/xOY+4HyMgjgoj/DzUd2SSIIMgUAAAAAElFTkSuQmCC'></a> ОТЛИЧНО " +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=4&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ХОРОШО " +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=3&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> УДОВЛЕТВОРИТЕЛЬНО" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=2&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ПЛОХО" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=1&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> УЖАСНО<br /><br />" +
                                    "Основатели сети Тонус-клуб,<br/>" +
                                    "Ирина Чирва и Елена Коростылева<br/>" +
                                    "<img src=\"data:image/jpeg;base64,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\"><br/>" +
                                    "8-800-333-01-03<br/>" +
                                    "<a href=\"http://www.tonusclub.ru\">www.tonusclub.ru</a>", SetUpWords(item.Customer.FullName), item.CreatedOn, item.TicketType.Name, item.DivisionId, item.Id);
                                break;
                            case 1: //второе оповещение
                                subject = "Поделитесь мнением о занятиях в ТОНУС-КЛУБ®";
                                body = string.Format("<p>Уважаемая {0}!<br/><br/>" +
                                    "Мы надеемся, что Вы уже оценили преимущества тренажеров клуба, а посещение SMART-тренировок принесло Вам первые положительные результаты!<br/><br/>" +
                                    "Мы будем очень благодарны, если Вы поделитесь  своими впечатлениями об атмосфере клуба, " +
                                    "сервисе и эффективности занятий, заполнив небольшую анкету: <a href=\"http://goo.gl/forms/KtUQxnDC4N\">http://goo.gl/forms/KtUQxnDC4N</a>. Это " +
                                    "не отнимет у Вас много времени!<br/><br/>" +
                                    "Если Вы уже поделились своим мнением ранее, то нам было бы интересно повторно получить Ваш отзыв, " +
                                    "так как спустя три месяца, Ваш опыт тренировок в клубе стал больше. Теперь Вы можете рассказать нам о своих впечатлениях, " +
                                    "поделиться своими замечаниями или предложениями по работе клубов сети ТОНУС-КЛУБ®.<br/><br/>" +
                                    "Каждый день мы развиваемся и просим Вас помочь нам в этом!<br/>" +
                                    "Оставив свой комментарий, Вы поможете другим покупателям определиться с выбором, а нашей сети совершенствоваться с учетом пожеланий наших клиентов.<br/><br/>" +
                                    "<u>Нам очень важно Ваше мнение!</u></p>" +
                                    "Вы можете быстро оценить Ваш клуб: просто выберите один из вариантов:" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=5&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ОТЛИЧНО " +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=4&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ХОРОШО " +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=3&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> УДОВЛЕТВОРИТЕЛЬНО" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=2&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ПЛОХО" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=1&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> УЖАСНО<br /><br />" +
                                    "Основатели сети Тонус-клуб,<br/>" +
                                    "Ирина Чирва и Елена Коростылева<br/>" +
                                    "<img src=\"data:image/jpeg;base64,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\"><br/>" +
                                    "8-800-333-01-03<br/>" +
                                    "<a href=\"http://www.tonusclub.ru\">www.tonusclub.ru</a>", SetUpWords(item.Customer.FullName), item.CreatedOn, item.TicketType.Name, item.DivisionId, item.Id); ;
                                break;
                            case 2: //третье оповещение
                                subject = "Оставьте свой отзыв о занятиях в ТОНУС-КЛУБ®";
                                body = string.Format("<p>Уважаемая {0}!<br/><br/>" +
                                    "Мы уверены, что Вы уже оценили заметные результаты и почувствовали пользу для здоровья во время посещения SMART-тренировок в европейской сети женских клубов ТОНУС-КЛУБ®!<br/><br/>" +
                                    "Мы будем очень благодарны, если Вы поделитесь  своими впечатлениями об атмосфере клуба, " +
                                    "сервисе и эффективности занятий, заполнив небольшую анкету: <a href=\"http://goo.gl/forms/KtUQxnDC4N\">http://goo.gl/forms/KtUQxnDC4N</a>. Это " +
                                    "займет у Вас не более трех минут!<br/><br/>" +
                                    "Если Вы уже ранее поделились своим мнением, то нам было бы интересно повторно получить Ваш отзыв, " +
                                    "так как спустя несколько месяцев посещений ТОНУС-КЛУБ®, Ваш опыт тренировок в клубе стал ещё больше. " +
                                    "Расскажите нам о своих впечатлениях, поделитесь своими замечаниями и предложениями по работе клубов сети ТОНУС-КЛУБ®.<br/><br/>" +
                                    "Каждый день мы развиваемся и просим Вас помочь нам в этом!<br/>" +
                                    "Оставив свой комментарий, Вы поможете другим покупателям определиться с выбором, а нашей сети совершенствоваться с учетом пожеланий наших клиентов.<br/><br/>" +
                                    "<u>Мы будем рады Вашим отзывам и пожеланиям!</u></p>" +
                                    "Вы можете быстро оценить Ваш клуб: просто выберите один из вариантов:" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=5&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ОТЛИЧНО " +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=4&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ХОРОШО " +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=3&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> УДОВЛЕТВОРИТЕЛЬНО" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=2&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> ПЛОХО" +
                                    "<br /><a href='http://asu.flagmax.ru/publicApi/SetupRating?divisionId={3}&rating=1&entityId={4}&entityName=Ticket'><img src='data:image/png;base64,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'></a> УЖАСНО<br /><br />" +
                                    "Основатели сети Тонус-клуб,<br/>" +
                                    "Ирина Чирва и Елена Коростылева<br/>" +
                                    "<img src=\"data:image/jpeg;base64,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\"><br/>" +
                                    "8-800-333-01-03<br/>" +
                                    "<a href=\"http://www.tonusclub.ru\">www.tonusclub.ru</a>", SetUpWords(item.Customer.FullName), item.CreatedOn, item.TicketType.Name, item.DivisionId, item.Id); ;
                                break;
                            default:
                                Log.WriteLine("Ошибка при email-рассылке после покупки абонемента. Рассылка №" + countEmails);
                                throw new Exception("Что-то пошло не так!");
                        } // switch

                        var destination = item.Customer.Email;

                        NotificationCore.SendMessage(destination, subject, body);
                        context.Emails.AddObject(new Email()
                        {
                            ID = Guid.NewGuid(),
                            Body = body,
                            Destination = destination,
                            Subject = subject,
                            TicketID = item.Id,
                        });
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
        }

        public static void ChargeSmartTickets(TonusEntities context, IEnumerable<Division> divs, DateTime date)
        {
            var divIds = divs.Select(i => i.Id).ToArray();
            var charged = context.UnitCharges.Where(i =>
                i.Ticket.TicketType.IsSmart
                && divIds.Contains(i.Ticket.DivisionId)
                && EntityFunctions.TruncateTime(i.Date) == date
                && i.Charge > 0 && i.GuestCharge == 0)
                .GroupBy(i => i.Ticket).Select(i => new { i.Key.Id, i.Key.AuthorId, i.Key.CompanyId, Amount = i.Sum(j => j.Charge) })
                .Where(i => i.Amount % SmartCore.MaxUnitsPerDay != 0)
                .ToArray();
            foreach(var t in charged)
            {
                var uc = new UnitCharge
                {
                    AuthorId = t.AuthorId,
                    Charge = SmartCore.MaxUnitsPerDay - (t.Amount % SmartCore.MaxUnitsPerDay),
                    CompanyId = t.CompanyId,
                    Date = date,
                    GuestCharge = 0,
                    Id = Guid.NewGuid(),
                    TicketId = t.Id,
                    Reason = "Автоматическое списание для смарт-абонементов"
                };
                context.UnitCharges.AddObject(uc);
            }
            context.SaveChanges();
        }
#endif
        private static void ProcessAnketTasks(TonusEntities context, IEnumerable<Division> divs)
        {
            if(DateTime.Today.Day == 1)
            {
                foreach(var div in divs)
                {
                    var eIds = div.Company.Roles
                        .Where(i => i.RoleName.ToLower().Contains("максималь") || i.RoleName.ToLower().Contains("управляющ") || i.RoleName.ToLower().Contains("франчайзи"))
                        .SelectMany(i => i.Users)
                        .Select(i => i.EmployeeId)
                        .Where(i => i.HasValue)
                        .Select(i => i.Value).ToArray();
                    var es = context.Employees.Where(i => eIds.Contains(i.Id));
                    var task = new Task
                    {
                        AuthorId = null,
                        CompanyId = div.CompanyId,
                        CreatedOn = DateTime.Now,
                        ExpiryOn = DateTime.Today.AddDays(3),
                        Id = Guid.NewGuid(),
                        Message = "В начале каждого месяца необходимо заполнить ежемесячную анкету франчайзи за предыдущий период.",
                        Priority = 0,
                        StatusId = 2,
                        Subject = "Ежемесячная анкета франчайзи по клубу " + div.Name
                    };
                    context.Tasks.AddObject(task);

                    foreach(var e in es)
                    {
                        task.Employees.Add(e);
                    }
                }
                context.SaveChanges();
            }
        }

        private static void ProcessClientLittleWalk(TonusEntities context)
        {
            var dateFrom = DateTime.Today.AddMonths(-1);
            var dateTo = DateTime.Today;
            var dateTo2 = DateTime.Today.AddDays(-548);
            var allCustomers = context.Tickets
                                        .Where(i => i.IsActive && !i.ReturnDate.HasValue && i.CreatedOn < dateFrom && i.CreatedOn >= dateTo2)
                                        .Select(i => new
                                        {
                                            Id = i.Customer.Id,
                                            FullName = i.Customer.LastName + " " + i.Customer.FirstName,
                                            Phone = i.Customer.Phone2 ?? i.Customer.Phone1,
                                            CardNumber = i.Customer.CustomerCards.Where(j => j.IsActive).OrderByDescending(j => j.EmitDate).Select(j => j.CardBarcode).FirstOrDefault()
                                        })
                                        .Distinct().ToList();
            var allCustomerIdsWithVisit = context.CustomerVisits.Where(i => i.InTime > dateFrom && i.InTime < dateTo).Select(i => i.CustomerId).ToList();

            var message = String.Empty;
            var countCustomers = 0;
            foreach (var cus in allCustomers)
            {
                if (allCustomerIdsWithVisit.Where(i => i == cus.Id).Count() < 6 )
                {
                    message += String.Format("{0}, телефон: {1} (номер карты: {2})\n", cus.FullName, cus.Phone, cus.CardNumber ?? "нет карты");
                    countCustomers++;
                }
            }

            if (countCustomers > 0)
            {
                var companyId = context.Companies.Select(i => i.CompanyId).First();
                context.Tasks.AddObject(new Task
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    CreatedOn = DateTime.Now,
                    ExpiryOn = DateTime.Today.AddDays(5),
                    Subject = String.Format("Вы скоро потеряете клиентов: {0} !", countCustomers),
                    Message = String.Format("Обратите внимание!\nНиже представлен список клиентов, которые посетили клуб менее 6 раз за прошедший месяц."
                                            + "\nВыясните причину.\nЗапишите на тренировку\n\n"
                                            + "Всех подобных клиентов Вы можете найти по отчёту \"Пропавшие клиенты\""
                                            + "или в отчёте \"Все клиенты\", упорядочив или отфильтровав по столбцу \"С последнего посещения дней\"\n\n{0}", message),
                    StatusId = 0,
                    Priority = 0
                });
                context.SaveChanges();
            }
        }

        private static void ProcessClientOut(TonusEntities context, Guid? DefaultDivisionId)
        {
            var Visits = context.CustomerVisits.Where(i => !i.OutTime.HasValue && i.DivisionId == DefaultDivisionId).ToList();
            var Shelves = context.CustomerShelves.Where(i => !i.ReturnOn.HasValue).ToList();
            var RandomUser = context.Users.Where(i => i.IsActive && i.EmployeeId.HasValue).OrderByDescending(i => i.LastLoginDate).FirstOrDefault();

            foreach (var visit in Visits)
            {
                visit.OutTime = DateTime.Now;
                foreach (var shelve in Shelves.Where(i => i.CustomerId == visit.CustomerId).ToList())
                {
                    shelve.ReturnOn = DateTime.Now;
                    shelve.ReturnBy = RandomUser;
                }
            }
            context.SaveChanges();
        }

        private static void ProcessEmployeesExit(TonusEntities context, IEnumerable<Division> divs)
        {
            var dIds = divs.Select(i => i.Id).ToList();
            var emps = context.Employees
                .Where(i => dIds.Contains(i.MainDivisionId))
                .Where(i => i.EmployeeVisits.OrderByDescending(j => j.CreatedOn).FirstOrDefault().IsIncome)
                .Where(i => i.EmployeeVisits.OrderByDescending(j => j.CreatedOn).FirstOrDefault().CreatedOn < DateTime.Today)
                .Select(i => new { Emp = i, Vis = i.EmployeeVisits.OrderByDescending(j => j.CreatedOn).FirstOrDefault() })
                .ToList();
            foreach(var emp in emps)
            {
                emp.Emp.Init();
                if(emp.Emp.SerializedJobPlacement == null) continue;
                var vis = new EmployeeVisit
                {
                    CompanyId = emp.Vis.CompanyId,
                    CreatedOn = emp.Vis.CreatedOn.Date.Add(emp.Emp.SerializedJobPlacement.Job.WorkEnd),
                    EmployeeId = emp.Emp.Id,
                    Id = Guid.NewGuid(),
                    IsIncome = false
                };
                context.EmployeeVisits.AddObject(vis);
            }
            context.SaveChanges();
        }

        private static void SendAnkets()
        {
            using(var context = new TonusEntities())
            {
                var ankets = context.Ankets.Where(i => i.StatusId > 1 && !i.SentDate.HasValue).ToArray();
                foreach(var anket in ankets)
                {
                    try
                    {
                        Logger.Log("Отсылка анкеты " + anket.Id);
                        SendAnkets(context, anket);

                        anket.SentDate = DateTime.Now;
                        context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
        }

        private static void SendAnkets(TonusEntities context, Anket anket)
        {
            return;
            var rep = TemplatesCore.GenerateAnketReport(anket.Id);
            var div = context.Divisions.Single(i => i.Id == anket.DivisionId);
            NotificationCore.SendMessage(ConfigurationManager.AppSettings.Get("AnketDestination"), "Анкета франчайзи " + div.Name, rep);
        }

        private static void UpdateAnkets()
        {
            using(var context = new TonusEntities())
            {
                var ankets = context.Ankets.Where(i => i.StatusId == 1).ToArray();
                foreach(var anket in ankets)
                {
                    anket.StatusId = 2;

                    using(var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
                    {
                        conn.Open();
                        new SqlCommand
                        {
                            Connection = conn,
                            CommandText = "sp_UpdateCrmSales",
                            CommandType = CommandType.StoredProcedure
                        }
                            .AddParameter<Guid>("divId", anket.DivisionId)
                            .AddParameter<decimal>("amount", anket.TotalCash)
                            .ExecuteScalar();
                        conn.Close();
                    }

                }
                context.SaveChanges();
            }
        }

        private static bool CheckSyncNeeded(TonusEntities context)
        {
            return false;
            if(context.Companies.Count() > 1) return false;

            return _syncLastCompleted.Date != DateTime.Now.AddMinutes(-SyncDelay).Date
                && IsNotWorkTime(context)
                && (DateTime.Now - _syncLastStart).TotalMinutes > 30;
        }

        private static void ProcessCutomerBirthdays(TonusEntities context)
        {
            foreach(var cust in GetDivisionsEnumerable(context).SelectMany(i => i.Tickets).Where(i => i.IsActive).Select(i => i.Customer).Where(i => i.Birthday.HasValue).Distinct())
            {
                var firstOrDefault = cust.Tickets.FirstOrDefault(i => i.IsActive);
                if(firstOrDefault != null)
                {
                    var days = firstOrDefault.Division.CustomerBirthdayDays;
                    if(days < 0) continue;
                    var date = DateTime.Today.AddDays(days);
                    Debug.Assert(cust.Birthday != null, "cust.Birthday != null");
                    if(cust.Birthday.Value.Day != date.Day) continue;
                    if(cust.Birthday.Value.Month != date.Month) continue;
                }
                else
                {
                    continue;
                }
                var ne = new CustomerNotification
                {
                    AuthorId = null,
                    CompanyId = cust.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = cust.Id,
                    Id = Guid.NewGuid(),
                    Message = String.Format("У клиента {0:dd.MM.yyyy} день рождения!", cust.Birthday.Value),
                    Subject = "День рождения клиента",
                    ExpiryDate = new DateTime(DateTime.Now.Year, cust.Birthday.Value.Month, cust.Birthday.Value.Day),
                    Priority = 2
                };
                var perm = context.Permissions.FirstOrDefault(i => i.PermissionKey == "CustomerBirthdayTask");
                if(perm != null)
                {
                    foreach(var emp in perm.Roles.Where(i => i.CompanyId == cust.CompanyId).SelectMany(r => r.Users).Select(i => context.Employees.FirstOrDefault(j => j.Id == i.EmployeeId)).Distinct())
                    {
                        if(emp != null)
                        {
                            ne.Employees.Add(emp);
                        }
                    }
                }
                context.CustomerNotifications.AddObject(ne);
            }
            context.SaveChanges();
        }

        private static void ProcessLicenseNotifications(TonusEntities context)
        {
            var ls = context.LocalSettings.FirstOrDefault();
            if(ls == null || String.IsNullOrEmpty(ls.NotifyAdresses)) return;
            if(ls.NotifyKeyPeriod != 0)
            {
                var keyexp = Core.GetCertificateDate();
                if((keyexp - DateTime.Today).TotalDays < ls.NotifyKeyDays && (int)(Math.Round((keyexp - DateTime.Today).TotalDays)) % ls.NotifyKeyPeriod == 0)
                {
                    NotificationCore.SendLocalMail("Лицензионный ключ действителен до " + keyexp.ToString("dd.MM.yyyy") + ".\r\nДля получения нового лицензионного ключа необходимо провести синхронизацию с центральным сервером.", "Срок действия лицензионного ключа подходит к концу");
                }
            }
            if(ls.NotifyLicensePeriod != 0)
            {
                if(((ls.LicenseExpiry ?? DateTime.MaxValue) - DateTime.Today).TotalDays < ls.NotifyLicenseDays && (int)(Math.Round(((ls.LicenseExpiry ?? DateTime.MaxValue) - DateTime.Today).TotalDays)) % ls.NotifyLicensePeriod == 0)
                {
                    NotificationCore.SendLocalMail("Лицензия действительна до " + (ls.LicenseExpiry ?? DateTime.MaxValue).ToString("dd.MM.yyyy") + ".\r\nДля продления лицензии свяжитесь с отделом франчайзинга.", "Срок действия лицензии подходит к концу");
                }
            }
        }

        private static void ProcessSync()
        {
            if(IsSyncBlocked)
            {
                Log.WriteLine("Синхронизация заблокирована из-за ошибки. Перезапустите workflow.");
                return;
            }
            try
            {
                SyncCore.Syncronize();
                _syncLastCompleted = DateTime.Now;
            }
            catch(FaultException<string> ex)
            {
                Log.WriteLine("Message (FaultException): " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + (ex.InnerException != null ? Environment.NewLine + "Inner: " + ex.InnerException.Message : ""));
                //IsSyncBlocked = true;
                throw;
            }
            catch(SqlException ex)
            {
                Log.WriteLine("Message (SqlException): " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + (ex.InnerException != null ? Environment.NewLine + "Inner: " + ex.InnerException.Message : ""));
                //IsSyncBlocked = true; 
                throw; 
            }
            catch(Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        private static bool IsNotWorkTime(TonusEntities context)
        {
            foreach(var div in GetDivisionsEnumerable(context))
            {
                Log.WriteLine(div.Name);
                if(!div.CloseTime.HasValue)
                {
                    return false;
                }
                if(!div.OpenTime.HasValue)
                {
                    return false;
                }
                if(DateTime.Today.Add(div.OpenTime.Value) < DateTime.Now && DateTime.Today.Add(div.CloseTime.Value).AddMinutes(SyncDelay) > DateTime.Now)
                {
                    return false;
                }
            }
            return true;
        }

        private static IEnumerable<Division> GetDivisionsEnumerable(TonusEntities context)
        {
            var ls = context.LocalSettings.SingleOrDefault();
            if(ls == null)
            {
                //return without metadata
                var res = new List<Division>();
                return res;
                using(var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
                {
                    conn.Open();
                    using(var rdr = new SqlCommand("select Id from Divisions where id not in (select DivisionId from syncmetadata.dbo.MetaCompanies)", conn).ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            var dId = rdr.GetGuid(0);
                            res.Add(context.Divisions.Single(i => i.Id == dId));
                        }
                    }
                    conn.Close();
                }
                return res;
            }
            return context.Divisions.Where(i => i.Id == ls.DefaultDivisionId);
        }

        private static void ProcessDivisionTasks(TonusEntities context, IEnumerable<Division> divs)
        {
            foreach(var div in divs)
            {
                foreach(var sol in div.Solariums)
                {
                    var lastch = sol.LampsExpires ?? sol.CreatedOn;
                    var total =
                        context.SolariumVisits.Where(i => i.CreatedOn >= lastch && i.SolariumId == sol.Id)
                            .Select(i => new { i.Amount })
                            .ToArray();
                    if(total.Any() && (total.Sum(i => i.Amount) * 2) > sol.LapsResource)
                    {
                        //if (sol.LampsExpires.HasValue && (sol.LampsExpires.Value.Date - DateTime.Today).TotalDays == 5)
                        //{
                        var spent = 0;
                        if(sol.LapsResource != 0)
                        {
                            spent = total.Sum(i => i.Amount) / sol.LapsResource * 100;
                        }
                        if(spent > 95)
                        {
                            var msg = String.Format("Солярий: {0}\nДоговор на обслуживание: {1}\nСерийный номер:{2}\nДата поставки: {3}\nГарантия до: {4}\nИзрасходован ресурс ламп: {5:n0}%",
                                sol.Name, sol.DogNumber, sol.SerialNumber, sol.Delivery, sol.GuaranteeExp, spent);
                            var task = new Task
                            {
                                AuthorId = null,
                                CompanyId = div.CompanyId,
                                CreatedOn = DateTime.Now,
                                ExpiryOn = DateTime.Today.AddDays(5),
                                Id = Guid.NewGuid(),
                                Message = msg,
                                Priority = 2,
                                StatusId = 0,
                                Subject = "Замена ламп в солярии",
                                Parameter = sol.Id
                            };
                            context.Tasks.AddObject(task);
                            UserManagement.GetEmployeesWithPermission(context, div.CompanyId, div.Id, "SolariumLampsTask").ForEach(i => task.Employees.Add(i));
                        }
                    }
                }

                if(div.InventoryDay == DateTime.Today.Day)
                {
                    var task = new Task
                    {
                        AuthorId = null,
                        CompanyId = div.CompanyId,
                        CreatedOn = DateTime.Now,
                        ExpiryOn = DateTime.Now.AddDays(1),
                        Id = Guid.NewGuid(),
                        Message = "Инвентаризация",
                        Priority = 2,
                        StatusId = 0,
                        Subject = "Инвентаризация"
                    };
                    context.Tasks.AddObject(task);
                    UserManagement.GetEmployeesWithPermission(context, div.CompanyId, div.Id, "DivisionInventoryTask").ForEach(i => task.Employees.Add(i));
                }
            }
            context.SaveChanges();
        }

        private static void ProcessTicketClosure(TonusEntities context, IEnumerable<Division> divs)
        {
            var dIds = divs.Select(i => i.Id).ToList();
            var tickets = context.Tickets
                .Where(t => dIds.Contains(t.DivisionId) && t.IsActive)
                .Where(t => t.UnitCharges.Sum(j => j.Charge) >= t.UnitsAmount
#if BEAUTINIKA
                 && t.UnitCharges.Sum(j => j.ExtraCharge) >= t.ExtraUnitsAmount
#endif
 && (t.MinutesCharges.Sum(j => (decimal?)j.MinutesCharged) ?? 0) >= t.SolariumMinutes)
                .ToList();
            foreach(var t in tickets)
            {
                Log.WriteLine("Абонемент " + t.Number + " закрыт");
                t.IsActive = false;
            }
            context.SaveChanges();
        }

        private static void CheckTicketsAutoActivate(TonusEntities context, IEnumerable<Division> divs)
        {
            var dIds = divs.Select(i => i.Id).ToList();
            var tickets = context.Tickets
                .Include("TicketType")
                .Where(i => dIds.Contains(i.DivisionId) && !i.IsActive && i.TicketType.AutoActivate.HasValue)
                .Where(i => !i.StartDate.HasValue && !i.TicketType.IsGuest)
                .Where(i => SqlFunctions.DateDiff("day", i.CreatedOn, DateTime.Today) > i.TicketType.AutoActivate)
                .Where(i => i.Division.OpenDate < DateTime.Today || !i.Division.OpenDate.HasValue)
                .ToList();
            foreach(var t in tickets)
            {
                var sd = t.CreatedOn > (t.Division.OpenDate ?? DateTime.MinValue) ? t.CreatedOn : (t.Division.OpenDate ?? DateTime.MinValue);
                if((DateTime.Today - sd).TotalDays < t.TicketType.AutoActivate)
                {
                    continue;
                }
                t.IsActive = true;
                t.StartDate = sd == DateTime.MinValue ? DateTime.Today : sd.AddDays(t.TicketType.AutoActivate.Value).Date;
                Log.WriteLine(t.Number + " сделан активным с " + t.StartDate);
            }
            try
            {
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                Logger.Log("Ошибка при сохранении активации абонементов");
                Logger.Log(ex);
                throw ex;
            }
        }

        private static void CheckTicketsActivity(TonusEntities context, IEnumerable<Division> divs)
        {
            var dIds = divs.Select(i => i.Id).ToList();
            var tickets = context.Tickets
                .Where(i => dIds.Contains(i.DivisionId))
                .Where(t => t.IsActive && t.StartDate.HasValue)
                .Select(i => new
                {
                    i.Id,
                    Freezes = i.TicketFreezes.Where(t => t.TicketFreezeReasonId != Guid.Empty).Sum(f => SqlFunctions.DateDiff("day", f.StartDate, f.FinishDate) + 1) ?? 0,
                    i.Length,
                    StartDate = i.StartDate.Value
                }).ToList();
            foreach(var t in tickets)
            {
                if(t.StartDate.AddDays(t.Length + t.Freezes) < DateTime.Today)
                {
                    var dbTicket = context.Tickets.Single(i => i.Id == t.Id);
                    dbTicket.IsActive = false;
                    Log.WriteLine(dbTicket.Number + " истек и сделан неактивным");
                }
            }
            context.SaveChanges();
        }

        private static void ProcessCustomerTasks(TonusEntities context, IEnumerable<Division> divs)
        {
            var dIds = divs.Select(i => i.Id).ToList();

            //Задолженность
            var tpGroups = context.Tickets
                .Where(i => dIds.Contains(i.DivisionId)).SelectMany(t => t.TicketPayments)
                .Where(i => !i.Ticket.HasNotify && i.Ticket.InstalmentId.HasValue && i.Ticket.LastInstalmentDay.HasValue && i.Ticket.LastInstalmentDay <= DateTime.Today)
                .GroupBy(i => i.Ticket);
            foreach(var tp in tpGroups)
            {
                var ticket = tp.Key;
                var amount = ticket.Cost - tp.Sum(i => i.Amount);
                if(amount > 0)
                {
                    Debug.Assert(ticket.LastInstalmentDay != null, "ticket.LastInstalmentDay != null");
                    var ne = new CustomerNotification
                    {
                        AuthorId = null,
                        CompanyId = ticket.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = ticket.CustomerId,
                        Id = Guid.NewGuid(),
                        Message = String.Format("Абонемент №{0}\nДата платежа: {1:d}\nСумма задолженности {2:c}", ticket.Number, ticket.LastInstalmentDay.Value, amount),
                        Subject = "Задолженность по абонементу",
                        ExpiryDate = ticket.LastInstalmentDay.Value,
                        Priority = 2
                    };
                    ticket.HasNotify = true;
                    context.CustomerNotifications.AddObject(ne);
                }
            }
            //Клиент пропал
            var date = DateTime.Today;
            var custs = context.Tickets
                .Where(i => dIds.Contains(i.DivisionId))
                .Where(i => i.IsActive)
                .Select(i => i.Customer)
                .Where(i => EntityFunctions.TruncateTime(i.CustomerVisits.Max(j => j.InTime)) == SqlFunctions.DateAdd("day", -i.Company.LostCutomerDays, date))
                .Distinct()
                .ToArray();
            foreach(var c in custs)
            {
                var count = c.CreatedOn;
                if(c.CustomerVisits.Any())
                {
                    count = c.CustomerVisits.Max(i => i.InTime);
                }
                if(!context.CustomerNotifications.Any(i => i.CustomerId == i.Id
                    && i.Subject == "Клиент пропал"
                    && DateTime.Today == EntityFunctions.TruncateTime(i.CreatedOn)))
                {
                    var ne1 = new CustomerNotification
                    {
                        AuthorId = null,
                        CompanyId = c.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = c.Id,
                        Id = Guid.NewGuid(),
#if BEAUTINIKA
                        Message = String.Format("У клиента есть активный абонемент, однако он c {0:d} не появляется в студии.", count),
#else
                        Message = String.Format("У клиента есть активный абонемент, однако он c {0:d} не появляется в клубе.", count),
#endif
                        Subject = "Клиент пропал",
                        ExpiryDate = DateTime.Now.AddDays(1),
                        Priority = 3
                    };

                    UserManagement.GetEmployeesWithPermission(context, c.CompanyId, null, "CustomerLostTask").ForEach(i => ne1.Employees.Add(i));

                    context.CustomerNotifications.AddObject(ne1);
                }
            }
            //Истекает абонемент
            var tickets = context.Tickets
                .Where(i => dIds.Contains(i.DivisionId))
                .Where(t => t.IsActive && !t.HasNotify).ToList();
            foreach(var t in tickets)
            {
                t.InitDetails();
                var message = String.Empty;
                if(t.UnitsLeft < 10)
                {
                    message = "осталось менее 10 единиц";
                }
                if(t.FinishDate.HasValue && t.FinishDate < DateTime.Today.AddDays(14))
                {
                    message = "до окончания срока действия осталось менее 14 дней (проверьте, что новый абонемент еще не был куплен)";
                } 
                else if (t.FinishDate.HasValue && t.FinishDate < DateTime.Today.AddDays(30))
                {
                    message = "до окончания срока действия осталось менее 30 дней";
                }
                if(String.IsNullOrEmpty(message)) continue;
                var cn = new CustomerNotification
                {
                    AuthorId = null,
                    CompanyId = t.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = t.CustomerId,
                    Id = Guid.NewGuid(),
                    Message = String.Format("У клиента есть активный абонемент, у которого {0}.\nНомер: {1}\nДата активации: {2:d}\nТип: {3}\nСрок действия, дней: {4}\nДата истечения: {5:d}\nОстаток единиц: {6:n0}", message, t.Number, t.StartDate, t.TicketType.Name, t.Length, t.FinishDate, t.UnitsLeft),
                    Subject = "Истекает абонемент",
                    ExpiryDate = DateTime.Now,
                    Priority = 2
                };

                UserManagement.GetEmployeesWithPermission(context, t.CompanyId, t.DivisionId, "TicketFinishingTask").ForEach(i => cn.Employees.Add(i));

                context.CustomerNotifications.AddObject(cn);
                t.HasNotify = true;

            }

            //Отзывы о первом посещении
            var yesterday = DateTime.Today.AddDays(-1);
            var customersToSet = context.Customers
                .Where(i => EntityFunctions.TruncateTime(i.CustomerVisits.Where(j => dIds.Contains(j.DivisionId)).Min(j => (DateTime?)j.InTime)) == yesterday)
                .Select(i => new { CustomerId = i.Id, i.CompanyId, EmployeeIds = i.Company.Users.Where(j => j.Roles.Any(k => k.Permissions.Any(l => l.PermissionKey == "CallTask"))).Select(j => j.EmployeeId) }).ToArray();
            foreach(var t in customersToSet)
            {
                var cn = new CustomerNotification
                {
                    AuthorId = null,
                    CompanyId = t.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = t.CustomerId,
                    Id = Guid.NewGuid(),
                    Message = "Необходимо узнать как дела у клиента, какие впечатления после первого посещения.",
                    Subject = "Первое посещение клуба",
                    ExpiryDate = DateTime.Today.AddDays(3),
                    Priority = 2
                };
                context.CustomerNotifications.AddObject(cn);
                foreach(var eId in t.EmployeeIds)
                {
                    var e = context.Employees.SingleOrDefault(i => i.Id == eId);
                    if(e != null)
                    {
                        cn.Employees.Add(e);
                    }
                }
            }
            context.SaveChanges();
        }

        private static void ProcessSolariumEvents(TonusEntities context, IEnumerable<Division> divs)
        {
            var dIds = divs.Select(i => i.Id).ToList();

            var events = context.SolariumVisits
                .Where(i => dIds.Contains(i.DivisionId)).Where(e => (e.VisitDate <= DateTime.Now) && (e.Status == (short)SolariumVisitStatus.Planned));
            foreach(var ev in events)
            {
                if(ev.VisitDate.AddMinutes(ev.Amount + 1) > DateTime.Now) continue;

                ev.eStatus = SolariumVisitStatus.Skipped;
            }
            context.SaveChanges();
        }

        public static int SyncDelay
        {
            get
            {
                int i;
                return !Int32.TryParse(ConfigurationManager.AppSettings.Get("SyncDelay"), out i) ? 0 : i;
            }
        }
    }
}
