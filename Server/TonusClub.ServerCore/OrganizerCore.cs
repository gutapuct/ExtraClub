using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;
using TonusClub.Entities;

using TonusClub.ServiceModel.Organizer;
using System.ServiceModel;
using System.Diagnostics;

namespace TonusClub.ServerCore
{
    public static class OrganizerCore
    {
        public static List<OrganizerItem> GetOrganizerItems(DateTime start, DateTime end, bool addNotifications = true)
        {
            end = end.AddDays(1);
            var res = new List<OrganizerItem>();
            using(var context = new TonusEntities())
            {
                var sw = Stopwatch.StartNew();
                var user = UserManagement.GetUser(context, "Roles", "Roles.Permissions");
                var clubs = context.Divisions.ToDictionary(i => i.Id, i => i.Name);

                //Обычные таски
                var tasks = context.Tasks
                    .Include("CreatedBy")
                    .Include("ClosedBy")
                    .Include("Employees")
                    .Where(t => t.CompanyId == user.CompanyId
                        && t.StatusId == 0
                        && !t.ClosedById.HasValue
                        && (t.Employees.Any(e => e.Id == user.EmployeeId) || t.Employees.Count() == 0)
                        && t.CreatedOn < end && t.ExpiryOn >= start)
                    .ToList();
                foreach(var t in tasks)
                {
                    res.Add(OrganizerItem.CreateTaskOrganizerItem(t));
                }

                //if (UserManagement.HasPermission(user, "CustomerTargetTask"))
                //{
                //    context.CustomerTargets
                //        .Include("CustomerTargetType")
                //        .Include("Customer")
                //        .Include("CreatedBy")
                //        .Where(i => i.TargetDate >= start && i.CreatedOn < end
                //            && !i.TargetComplete.HasValue
                //            && i.CompanyId == user.CompanyId)
                //        .ToList()
                //        .ForEach(i => res.Add(OrganizerItem.CreateCustomerTargetOrganizerItem(i, clubs)));
                //}
                Debug.WriteLine("1: " + sw.ElapsedMilliseconds);
                if(UserManagement.HasPermission(user, "TicketReturnTask"))
                {
                    res.AddRange(context.Tickets
                        .Where(i => i.TicketFreezes.Any(j => j.TicketFreezeReason.IsReturnReason)
                            && !i.ReturnDate.HasValue
                            && i.CompanyId == user.CompanyId)
                        .Select(i => new OrganizerItem
                        {
                            Data = i,
                            AppearDate = i.TicketFreezes.FirstOrDefault(j => j.TicketFreezeReasonId == Guid.Empty).CreatedOn,
                            Category = "Возврат абонемента",
                            Status = "Поставлена",
                            Text = "Возврат абонемента №" + i.Number,
                            SerializedCreatedBy = "Логика АСУ",
                            Priority = 1,
                            ExpiryDate = i.TicketFreezes.FirstOrDefault(j => j.TicketFreezeReasonId == Guid.Empty).CreatedOn,
                            SerializedAssignedTo = "Управляющий"
                        }));
                }

                Debug.WriteLine("1.5: " + sw.ElapsedMilliseconds);

                //Для вывода средств с депозита не указан набор прав, разрешаем всем
                context.DepositOuts
                    .Where(i => !i.ProcessedById.HasValue && i.CompanyId == user.CompanyId)
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateDepositOutOrganizerItem(i)));
                Debug.WriteLine("2: " + sw.ElapsedMilliseconds);

                if(UserManagement.HasPermission(user, "CashlessPaymentsTask"))
                {
                    context.BarOrders
                        .Where(i => i.ProviderId.HasValue && !i.PaymentDate.HasValue && i.CompanyId == user.CompanyId)
                        .ToList()
                        .ForEach(i => res.Add(OrganizerItem.CreateCashlessPaymentOrganizerItem(i)));
                }
                Debug.WriteLine("2.5: " + sw.ElapsedMilliseconds);

                if(addNotifications)
                {
                    //Права уже назначены в нотификациях - это для тех нотификаций, для которых не назначено
                    context.CustomerNotifications
                        .Include("CreatedBy")
                        .Include("CompletedBy")
                        .Include("Employees")
                        .Where(i => !i.CompletedById.HasValue
                            && i.CompanyId == user.CompanyId
                            && i.CreatedOn <= DateTime.Now
                            && i.ExpiryDate >= start
                            && i.CreatedOn < end
                            && !i.Employees.Any())
                        .ToList()
                        .ForEach(i => res.Add(OrganizerItem.CreateCustomerNotificationItem(i, clubs)));
                    Debug.WriteLine("3: " + sw.ElapsedMilliseconds);

                    if(user.EmployeeId.HasValue)
                    {

                        var not = context
                            .CustomerNotifications
                            .Where(i => i.Employees.Any(j => j.Id == user.EmployeeId.Value));
                        res.AddRange(
                        not.Where(i =>
                            !i.CompletedById.HasValue
                            && i.CompanyId == user.CompanyId
                            && i.ExpiryDate >= start
                            && i.CreatedOn < end
                            && i.CreatedOn <= DateTime.Now)
                            .Select(cn =>
                        new
                        {
                            Data = cn,
                            AppearDate = cn.CreatedOn,
                            Category = "Оповещение клиента",
                            Status = cn.CompletionComment == "Отозвана" ? 3 : (cn.CompletedById.HasValue ? 1 : 0),
                            Text = cn.Subject == "Задача на обзвон клиентов" ? "Звонок " + (cn.Customer.LastName + " " ?? "") + (cn.Customer.FirstName + " " ?? "") + (cn.Customer.MiddleName ?? "") :
                            cn.Subject + " " + (cn.Customer.LastName + " " ?? "") + (cn.Customer.FirstName + " " ?? "") + (cn.Customer.MiddleName ?? ""),
                            SerializedCreatedBy = cn.AuthorId.HasValue ? cn.CreatedBy.FullName : "Автоматически",
                            Priority = cn.Priority,
                            ExpiryDate = cn.ExpiryDate,
                            //SerializedAssignedTo = OrganizerItem.GetAttachString(cn.Employees),
                            ClosureDate = cn.CompletedOn,
                            SerializedClosedBy = cn.CompletedById.HasValue ? cn.CompletedBy.FullName : "",
                            CompletionComment = "Задача:\n" + cn.Message + "\nОтчет:\n" + (String.IsNullOrEmpty(cn.CompletionComment) ? "Комментарий не указан" : cn.CompletionComment),
                            ClubText = context.Divisions.FirstOrDefault(i => i.Id == cn.Customer.ClubId).Name
                        }).ToList()
                        .Select(cn => new OrganizerItem
                        {
                            Data = cn.Data,
                            AppearDate = cn.AppearDate,
                            Category = "Оповещение клиента",
                            Status = OrganizerItem.GetStatusText(cn.Status),
                            Text = cn.Text,
                            SerializedCreatedBy = cn.SerializedCreatedBy,
                            Priority = cn.Priority,
                            ExpiryDate = cn.ExpiryDate,
                            //SerializedAssignedTo = cn.SerializedAssignedTo,
                            ClosureDate = cn.ClosureDate,
                            SerializedClosedBy = cn.SerializedClosedBy,
                            CompletionComment = cn.CompletionComment,
                            ClubText = cn.ClubText
                        }).ToArray()
                        );
                        Debug.WriteLine("4: " + sw.ElapsedMilliseconds);

                    }
                }
            }
            return res.OrderByDescending(i => i.ExpiryDate).ToList();
        }

        public static string GetNotificationsForUser()
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                if(!user.EmployeeId.HasValue) return String.Empty;
                var employee = context.Employees.SingleOrDefault(i => i.Id == user.EmployeeId.Value);
                if(employee == null) return String.Empty;

                var sb = new StringBuilder();

                //Дни рождений
                var emps = context.Employees.Where(i => i.MainDivisionId == employee.MainDivisionId && i.BoundCustomer.Birthday.HasValue).ToList()
                    .Where(i =>
                    {
                        if(i.BoundCustomer.Birthday.HasValue
                            && i.BoundCustomer.Birthday.Value.Month == 2
                            && i.BoundCustomer.Birthday.Value.Day == 29) i.BoundCustomer.Birthday = i.BoundCustomer.Birthday.Value.AddDays(-1);
                        var d = (new DateTime(DateTime.Today.Year, i.BoundCustomer.Birthday.Value.Month, i.BoundCustomer.Birthday.Value.Day) - DateTime.Today).TotalDays;
                        return d <= 3 && d >= 0;
                    });

                foreach(var i in emps)
                {
                    if(sb.Length != 0) sb.Append('\n');
                    sb.Append(i.BoundCustomer.FullName + ": день рождения " + new DateTime(DateTime.Today.Year, i.BoundCustomer.Birthday.Value.Month, i.BoundCustomer.Birthday.Value.Day).ToString("d.MM"));
                }

                var apps = context.JobPlacements.Where(i => i.Employee.MainDivisionId == employee.MainDivisionId && i.TestPeriod > 0).ToList()
                    .Where(i =>
                    {
                        i.Employee.Init();
                        if(i.Employee.SerializedJobPlacement != null)
                        {
                            if(i.Employee.SerializedJobPlacement.FireDate.HasValue) return false;
                            var days = (i.ApplyDate.AddMonths(i.TestPeriod) - DateTime.Today).TotalDays;
                            return days >= 0 && days <= 10;
                        }
                        else
                        {
                            return false;
                        }
                    });

                foreach(var i in apps)
                {
                    if(sb.Length != 0) sb.Append('\n');
                    sb.Append(i.Employee.BoundCustomer.FullName + ": исп. срок " + i.ApplyDate.AddMonths(i.TestPeriod).ToString("d.MM"));
                }

                var vacs = EmployeeCore.GetCurrentEmployeeVacationsSchedule(employee.MainDivisionId);
                foreach(var vac in vacs)
                {
                    var days = (vac.Start - DateTime.Today).TotalDays;
                    if(days >= 0 && days <= 3)
                    {
                        var e = context.Employees.Single(j => j.Id == vac.EmployeeId);
                        if(sb.Length != 0) sb.Append('\n');
                        sb.Append(e.BoundCustomer.FullName + ": отпуск с " + vac.Start.ToString("d.MM") + " по " + vac.Finish.ToString("d.MM"));
                    }
                }

                if(employee.MainDivision.WorkGraphNotifyDay - DateTime.Today.Day < 0)
                {
                    if(!context.EmployeeWorkGraphs.Where(i => i.DivisionId == employee.MainDivisionId).ToList().Any(i => i.Begin.Year == DateTime.Today.AddMonths(1).Year && i.Begin.Month == DateTime.Today.AddMonths(1).Month))
                    {
                        if(sb.Length != 0) sb.Append('\n');
                        sb.Append("Необходимо составить график работы на следующий месяц!");
                    }
                }

                var dateTomorrow = DateTime.Today.AddDays(1).AddMinutes(-1);
                var tasks = GetOrganizerItems(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(1)).Where(i => i.ExpiryDate < dateTomorrow).Count();
                if(tasks > 0)
                {
                    if(sb.Length != 0) sb.Append('\n');
                    sb.AppendFormat("Невыполненных задач органайзера на сегодня: {0}", tasks);
                }

                return sb.ToString();
            }
        }

        public static void PostNewCall(Guid divisionId, string log, CallResult callResult, Guid? customerId, bool isIncoming, DateTime started, string goal, string result)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var div = context.Divisions.Single(i => i.Id == divisionId);
                var call = new Call
                {
                    AuthorId = user.UserId,
                    CompanyId = div.CompanyId,
                    CustomerId = customerId,
                    DivisionId = divisionId,
                    Id = Guid.NewGuid(),
                    IsIncoming = isIncoming,
                    Log = log,
                    ResultType = (int)callResult,
                    StartAt = started,
                    Goal = goal,
                    Result = result
                };
                context.Calls.AddObject(call);
                context.SaveChanges();
            }
        }

        public static List<Call> GetDivisionCalls(Guid divisionId, DateTime callsStart, DateTime callsEnd)
        {
            using(var context = new TonusEntities())
            {
                callsEnd = callsEnd.Date.AddDays(1);
                return context.Calls.Where(i => i.DivisionId == divisionId && i.StartAt >= callsStart && i.StartAt < callsEnd).OrderByDescending(i => i.StartAt).ToList().Init();
            }
        }

        public static void PostNotificationCompletion(Guid divisionId, Guid notificationId, string comment, string result)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var notify = context.CustomerNotifications.SingleOrDefault(i => i.Id == notificationId);
                if(notify == null) return;

                notify.CompletionComment = comment;
                notify.CompletedById = user.UserId;
                notify.CompletedOn = DateTime.Now;

                var call = new Call
                {
                    AuthorId = user.UserId,
                    CompanyId = notify.CompanyId,
                    CustomerId = notify.CustomerId,
                    DivisionId = divisionId,
                    Id = Guid.NewGuid(),
                    IsIncoming = false,
                    Log = "Задача:\n" + notify.Message + "\n\nОтчет:\n" + comment,
                    ResultType = (int)CallResult.OK,
                    StartAt = notify.CompletedOn.Value,
                    Result = result
                };
                context.Calls.AddObject(call);

                context.SaveChanges();
            }
        }

        public static void PostIncorrectPhoneTask(Guid divisionId, Guid cnId, Guid customerId, string comments)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var div = context.Divisions.Single(i => i.Id == divisionId);
                var cn = new CustomerNotification
                {
                    AuthorId = user.UserId,
                    CompanyId = div.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customerId,
                    Id = Guid.NewGuid(),
                    Message = "Неверный номер телефона\n" + comments,
                    Subject = "Неверный номер телефона",
                    ExpiryDate = DateTime.Now,
                    Priority = 2
                };

                var users = context.Users.Where(i => i.EmployeeId.HasValue).ToList().Where(u => UserManagement.HasPermission(u, "IcorrectPhoneTask"));
                foreach(var u in users)
                {
                    var employee = context.Employees.SingleOrDefault(e => e.Id == u.EmployeeId.Value);
                    if(employee != null)
                    {
                        cn.Employees.Add(employee);
                    }
                }

                var oldCn = context.CustomerNotifications.Single(i => i.Id == cnId);

                Logger.DBLog("Неверный номер телефона - задача для клиента {0}", oldCn.Customer.FullName);


                oldCn.CompletionComment = "Поставлена задача о неверно указанном номере телефона";
                oldCn.CompletedById = user.UserId;
                oldCn.CompletedOn = DateTime.Now;

                context.CustomerNotifications.AddObject(cn);
                context.SaveChanges();
            }
        }

        public static void PostGroupCall(Guid divisionId, Guid[] customers, Guid[] employees, string comments, DateTime runDate, DateTime expiry)
        {
            using(var context = new TonusEntities())
            {
                if(runDate < DateTime.Now) runDate = DateTime.Now;
                var user = UserManagement.GetUser(context);
                var div = context.Divisions.Single(i => i.Id == divisionId);

                var emps = new List<Employee>();
                employees.ToList().ForEach(i => emps.Add(context.Employees.Single(j => j.Id == i)));

                foreach(var i in customers)
                {
                    var cn = new CustomerNotification
                    {
                        AuthorId = user.UserId,
                        CompanyId = div.CompanyId,
                        CreatedOn = runDate,
                        CustomerId = i,
                        Id = Guid.NewGuid(),
                        Message = comments,
                        Subject = "Задача на обзвон клиентов",
                        ExpiryDate = expiry,
                        Priority = 2
                    };
                    foreach(var e in emps)
                    {
                        cn.Employees.Add(e);
                    }
                    context.CustomerNotifications.AddObject(cn);
                }
                context.SaveChanges();
            }
        }

        public static void PostTaskClosing(Guid taskId, bool isCompleted, string comment, DateTime date)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var task = context.Tasks.Single(i => i.Id == taskId);
                task.ClosedById = user.UserId;
                task.ClosedComment = (comment ?? "").Trim();
                task.ClosedOn = DateTime.Now;

                task.StatusId = isCompleted ? 1 : 2;

                if(isCompleted && task.Parameter.HasValue)
                {
                    var sol = context.Solariums.SingleOrDefault(i => i.Id == task.Parameter.Value);
                    if(sol != null)
                    {
                        sol.LampsExpires = date;
                    }
                }

                context.SaveChanges();
            }
        }

        public static void PostNewTask(Guid[] employees, string subject, string comments, DateTime expiryDate, int priority)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var task = new Task
                {
                    AuthorId = user.UserId,
                    CompanyId = user.CompanyId,
                    CreatedOn = DateTime.Now,
                    ExpiryOn = expiryDate,
                    Id = Guid.NewGuid(),
                    Message = comments,
                    Priority = priority,
                    StatusId = 0,
                    Subject = subject
                };

                employees.ToList().ForEach(i => task.Employees.Add(context.Employees.Single(j => j.Id == i)));
                context.Tasks.AddObject(task);
                context.SaveChanges();
            }
        }

        public static List<OrganizerItem> GetOutboxOrganizerItems()
        {
            var res = new List<OrganizerItem>();
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var tasks = context.Tasks.Where(t => t.StatusId == 0 && !t.ClosedById.HasValue && t.AuthorId == user.UserId);
                foreach(var t in tasks)
                {
                    res.Add(OrganizerItem.CreateTaskOrganizerItem(t));
                }

                var clubs = context.Divisions.ToDictionary(i => i.Id, i => i.Name);
                //context.CustomerTargets
                //    .Where(i => !i.TargetComplete.HasValue && i.AuthorId == user.UserId)
                //    .ToList()
                //    .ForEach(i => res.Add(OrganizerItem.CreateCustomerTargetOrganizerItem(i, clubs)));

                //context.Tickets
                //    .Where(i => i.TicketFreezes.Any(j => j.TicketFreezeReason.IsReturnReason && j.AuthorId == user.UserId && !j.Ticket.ReturnDate.HasValue)
                //        && !i.ReturnDate.HasValue
                //        && i.CompanyId == user.CompanyId)
                //    .ToList()
                //    .ForEach(i => { i.InitDetails(); res.Add(OrganizerItem.CreateTicketReturnOrganizerItem(i)); });

                context.DepositOuts
                    .Where(i => !i.ProcessedById.HasValue && i.AuthorId == user.UserId)
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateDepositOutOrganizerItem(i)));

                context.BarOrders
                    .Where(i => i.ProviderId.HasValue && !i.PaymentDate.HasValue && i.AuthorId == user.UserId)
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateCashlessPaymentOrganizerItem(i)));

                context.CustomerNotifications
                    .Where(i => !i.CompletedById.HasValue
                        && i.AuthorId == user.UserId)
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateCustomerNotificationItem(i, clubs)));
            }
            //Для исходящих тасок не подробности не нужны
            res.ForEach(i =>
            {
                if(i.Data is Task) i.Data = (i.Data as Task).Id;
                else if(i.Data is CustomerNotification) i.Data = (i.Data as CustomerNotification).Id;
                else i.Data = null;
            });
            return res.OrderByDescending(i => i.ExpiryDate).ToList();
        }

        public static void PostTaskRecall(Guid elementId)
        {
            using(var context = new TonusEntities())
            {
                var task = context.Tasks.SingleOrDefault(i => i.Id == elementId);
                if(task != null)
                {
                    task.StatusId = 3;
                    context.SaveChanges();
                    return;
                }
                var not = context.CustomerNotifications.SingleOrDefault(i => i.Id == elementId);
                if(not != null)
                {
                    not.CompletionComment = "Отозвана";
                    not.CompletedById = UserManagement.GetUser(context).UserId;
                    not.CompletedOn = DateTime.Now;
                    context.SaveChanges();
                    return;
                }
            }
        }

        public static List<OrganizerItem> GetArchivedOrganizerItems()
        {
            var res = new List<OrganizerItem>();
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var companyId = user.CompanyId;
                //if (!user.EmployeeId.HasValue) return res;
                var dateThreeMonthsAgo = DateTime.Today.AddDays(-15);
                var tasks = context.Tasks.Where(t => t.CompanyId == companyId && (t.ClosedById.HasValue || t.StatusId == 3) && (!t.ClosedOn.HasValue || t.ClosedOn > dateThreeMonthsAgo));// && (t.AuthorId == user.UserId || t.ClosedById == user.UserId));
                foreach(var t in tasks)
                {
                    res.Add(OrganizerItem.CreateTaskOrganizerItem(t));
                }

                var clubs = context.Divisions.ToDictionary(i => i.Id, i => i.Name);
                //context.CustomerTargets
                //    .Where(i => i.TargetComplete.HasValue && i.AuthorId == user.UserId)
                //    .ToList()
                //    .ForEach(i => res.Add(OrganizerItem.CreateCustomerTargetOrganizerItem(i, clubs)));

                context.DepositOuts
                    .Where(i => i.CompanyId == companyId && i.ProcessedById.HasValue && (i.AuthorId == user.UserId || i.ProcessedById == user.EmployeeId))
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateDepositOutOrganizerItem(i)));

                context.BarOrders
                    .Where(i => i.CompanyId == companyId && i.ProviderId.HasValue && i.PaymentDate.HasValue && i.AuthorId == user.UserId)
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateCashlessPaymentOrganizerItem(i)));

                context.CustomerNotifications
                    .Where(i => i.CompanyId == companyId && i.CompletedOn.HasValue && i.CompletedOn.Value > dateThreeMonthsAgo)//&& (i.AuthorId == user.UserId || i.CompletedById == user.UserId))
                    .ToList()
                    .ForEach(i => res.Add(OrganizerItem.CreateCustomerNotificationItem(i, clubs)));
            }
            //Для завершенных тасок не подробности не нужны
            res.ForEach(i =>
            {
                if(i.Data is Task) i.Data = (i.Data as Task).Id;
                else if(i.Data is CustomerNotification) i.Data = (i.Data as CustomerNotification).Id;
                else i.Data = null;
            });
            return res.OrderByDescending(i => i.ExpiryDate).ToList();
        }

        public static void PostTaskReopen(Guid elementId)
        {
            using(var context = new TonusEntities())
            {
                var task = context.Tasks.SingleOrDefault(i => i.Id == elementId);
                if(task != null)
                {
                    task.StatusId = 0;
                    task.ClosedById = null;
                    task.ClosedComment = null;
                    task.ClosedOn = null;
                    context.SaveChanges();
                    return;
                }
                var not = context.CustomerNotifications.SingleOrDefault(i => i.Id == elementId);
                if(not != null)
                {
                    not.CompletionComment = null;
                    not.CompletedById = null;
                    not.CompletedOn = null;
                    context.SaveChanges();
                    return;
                }
            }
        }

        public static List<Call> GetCustomerCalls(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                return context.Calls.Where(i => i.CustomerId == customerId).OrderByDescending(i => i.StartAt).ToList().Init();
            }
        }

        public static List<Claim> GetClaims(Guid companyId, DateTime start, DateTime end, bool showClosedClaims)
        {
            using(var context = new TonusEntities())
            {
                end = end.Date.AddDays(1);
                return context.Claims
                    .Where(i => i.CreatedOn >= start
                        && i.CreatedOn < end
                        && i.CompanyId == companyId
                        && (showClosedClaims || (i.StatusId != 5 && i.StatusId != 6) || i.StatusDescription == "Исполнена - подтвердите"))
                    .ToList();
            }
        }

        public static void PostClaim(Claim claim)
        {
            if(!new int[] { 0, 1, 2, 3 }.Contains(claim.ClaimTypeId)) throw new FaultException<string>("Необходимо обновление клиентского приложения Flagmax Direction!", "Необходимо обновление клиентского приложения Flagmax Direction!");
            using(var context = new TonusEntities())
            {
                Claim dbClaim;
                if(claim.Id == Guid.Empty)
                {
                    dbClaim = claim;
                    dbClaim.Id = Guid.NewGuid();
                    dbClaim.CreatedOn = DateTime.Now;
                    context.Claims.AddObject(dbClaim);
                }
                else
                {
                    dbClaim = context.Claims.Single(i => i.Id == claim.Id);
                    dbClaim.ClaimTypeId = claim.ClaimTypeId;
                    dbClaim.ContactEmail = claim.ContactEmail;
                    dbClaim.ContactInfo = claim.ContactInfo;
                    dbClaim.ContactPhone = claim.ContactPhone;
                    dbClaim.Eq_BuyDate = claim.Eq_BuyDate;
                    dbClaim.Eq_Guaranty = claim.Eq_Guaranty;
                    dbClaim.Eq_Serial = claim.Eq_Serial;
                    dbClaim.Message = claim.Message;
                    dbClaim.PrefFinishDate = claim.PrefFinishDate;
                    dbClaim.Subject = claim.Subject;
                    dbClaim.StatusId = claim.StatusId;
                    dbClaim.Circulation = claim.Circulation;
                }
                if(dbClaim.StatusId == 0)
                {
                    dbClaim.StatusDescription = "Ожидает отправки";
                }
                else
                {
                    dbClaim.StatusDescription = "Черновик";
                }
                context.SaveChanges();
            }
        }

        public static void SubmitClaim(Guid claimId, int ActualScore)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var claim = context.Claims.Single(i => i.Id == claimId);

                claim.StatusId = 5;
                claim.StatusDescription = "Закрыта";
                claim.SubmitDate = DateTime.Now;
                claim.SubmitUser = user.UserId;
                claim.ActualScore = ActualScore;

                context.SaveChanges();
            }
        }

        public static List<AnketInfo> GetAnkets(Guid companyId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                var res = context.Ankets.Where(i => i.CompanyId == companyId && i.Period >= start && i.Period <= end)
                    .Select(i => new AnketInfo { Id = i.Id, Period = i.Period, StatusId = i.StatusId, DivisionName = context.Divisions.FirstOrDefault(j => j.Id == i.DivisionId).Name })
                    .ToList();

                return res;
            }
        }

        public static Anket GenerateAnketDefault(Guid divisionId, DateTime period)
        {
            using(var context = new TonusEntities())
            {
                var sw = Stopwatch.StartNew();
                var div = context.Divisions.Include("Company").Single(i => i.Id == divisionId);
                var user = UserManagement.GetUser(context);
                var periodEnd = period.AddMonths(1);
                var totalWorkdays = DateTime.DaysInMonth(period.Year, period.Month);
                var job = context.Employees.Where(i => i.Id == user.EmployeeId).SelectMany(i => i.JobPlacements).OrderByDescending(i => i.CreatedOn).Select(i => i.Job.Name).FirstOrDefault();
                Logger.Log("Анкета: JobPlacement {0}ms", sw.ElapsedMilliseconds);
                var res = new Anket
                {
                    FilledBy = user.FullName,
                    FilledByPosition = job,
                    CompanyId = div.CompanyId,
                    CreatedBy = user.UserId,
                    CreatedOn = DateTime.Now,
                    DivisionId = divisionId,
                    Period = period,
                    PriceChanges = context.GoodPrices.Any(i => i.DivisionId == divisionId && i.Date >= period && i.Date < periodEnd),
                    TotalWorkdays = totalWorkdays,
                    PlanComplete = SalaryCalculation.Get01TotalSalesPercent(context, divisionId, period, periodEnd),
                };
                Logger.Log("Анкета: Get01TotalSalesPercent+PriceChanges {0}ms", sw.ElapsedMilliseconds);

                res.AvgVisitors = (decimal)context.CustomerVisits.Count(i => i.DivisionId == divisionId && i.InTime >= period && i.InTime < periodEnd) / totalWorkdays;
                Logger.Log("Анкета: AvgVisitors {0}ms", sw.ElapsedMilliseconds);

                res.RecurringTickets = context.Tickets.Where(i => i.DivisionId == divisionId && i.CreatedOn >= period && i.CreatedOn < periodEnd && !i.TicketType.IsGuest && !i.TicketType.IsVisit)
                        .Count(i => i.Customer.Tickets.Any(j => j.CreatedOn < i.CreatedOn && !j.TicketType.IsGuest && !j.TicketType.IsVisit));
                Logger.Log("Анкета: RecurringTickets {0}ms", sw.ElapsedMilliseconds);

                res.TotalCash = context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= period && i.PurchaseDate < periodEnd).Sum(i => (decimal?)i.CashPayment + i.CardPayment) ?? 0;
                Logger.Log("Анкета: TotalCash {0}ms", sw.ElapsedMilliseconds);

                res.SerializedAnketTickets = new List<AnketTicket>();

                foreach(var tt in context.Tickets.Where(i => i.CreatedOn >= period && i.CreatedOn < periodEnd && i.DivisionId == divisionId)
                    .OrderBy(i => i.TicketType.Name).GroupBy(i => i.TicketType).ToDictionary(i => new KeyValuePair<Guid, string>(i.Key.Id, i.Key.Name), i => i.Count()))
                {
                    res.SerializedAnketTickets.Add(new AnketTicket
                    {
                        Amount = tt.Value,
                        AnketId = res.Id,
                        CompanyId = div.CompanyId,
                        Id = Guid.NewGuid(),
                        TicketTypeId = tt.Key.Key,
                        SerializedName = tt.Key.Value,
                    });
                }
                if(res.SerializedAnketTickets.Count < 3)
                {
                    foreach(var tt in div.Company.TicketTypes)
                    {
                        res.SerializedAnketTickets.Add(new AnketTicket
                        {
                            Amount = 0,
                            AnketId = res.Id,
                            CompanyId = div.CompanyId,
                            Id = Guid.NewGuid(),
                            TicketTypeId = tt.Id,
                            SerializedName = tt.Name
                        });
                    }
                }
                Logger.Log("Анкета: AnketTickets {0}ms", sw.ElapsedMilliseconds);

                res.SerializedAnketTreatments = new List<AnketTreatment>();
                var src = context.TreatmentEvents.Where(i => i.VisitDate >= period && i.VisitDate < periodEnd && i.DivisionId == divisionId && i.VisitStatus != 1)
                    .OrderBy(i => i.TreatmentConfig.TreatmentType.Name).GroupBy(i => i.TreatmentConfig.TreatmentType).ToDictionary(i => new KeyValuePair<Guid, string>(i.Key.Id, i.Key.Name), i => i.Count());
                foreach(var tt in src)
                {
                    res.SerializedAnketTreatments.Add(new AnketTreatment
                    {
                        Amount = tt.Value,
                        AnketId = res.Id,
                        CompanyId = div.CompanyId,
                        Id = Guid.NewGuid(),
                        TreatmentTypeId = tt.Key.Key,
                        SerializedName = tt.Key.Value
                    });
                }
                Logger.Log("Анкета: AnketTreatments {0}ms", sw.ElapsedMilliseconds);


                res.AvgTreatments = (decimal)src.Sum(i => i.Value) / totalWorkdays;
                var src2 = context.Tickets.Where(i => i.TicketType.IsVisit && i.DivisionId == divisionId && i.CreatedOn >= period && i.CreatedOn < periodEnd);
                res.TotalTestVisitors = src2.Count();
                res.TotalBuyAfterTest = src2.Select(i => i.Customer).Distinct().Count(i => i.Tickets.Any(j => !j.TicketType.IsGuest && !j.TicketType.IsVisit));

                Logger.Log("Анкета: TotalBuyAfterTest {0}ms", sw.ElapsedMilliseconds);


                res.SerializedAnketAdverts = new List<AnketAdvert>();
                try
                {
                    var calls = context.Customers
                        .Select(i => i.Calls.OrderBy(j => j.StartAt).FirstOrDefault())
                        .Where(i => i.DivisionId == divisionId && i.StartAt >= period && i.StartAt < periodEnd && i.Customer.AdvertTypeId.HasValue)
                        .GroupBy(i => i.Customer.AdvertType.AdvertGroupId)
                        .ToDictionary(i => i.Key, i => i.Count());
                    Logger.Log("Анкета: calls {0}ms", sw.ElapsedMilliseconds);

                    var visits = context.CustomerVisits
                        .Where(i => i.DivisionId == divisionId && i.Customer.AdvertTypeId.HasValue)
                        .GroupBy(i => new { i.CustomerId, i.Customer.AdvertType.AdvertGroupId })
                        .Select(i => new { i.Key.AdvertGroupId, InTime = i.Min(j => j.InTime) })
                        .Where(i => i.InTime >= period && i.InTime < periodEnd)
                        .GroupBy(i => i.AdvertGroupId)
                        .ToDictionary(i => i.Key, i => i.Count());
                    Logger.Log("Анкета: visits {0}ms", sw.ElapsedMilliseconds);

                    var purcahses = context.Customers
                        .Select(i => i.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault())
                        .Where(i => i.DivisionId == divisionId && i.CreatedOn >= period && i.CreatedOn < periodEnd && i.Customer.AdvertTypeId.HasValue)
                        .GroupBy(i => i.Customer.AdvertType.AdvertGroupId)
                        .ToDictionary(i => i.Key, i => i.Count());
                    Logger.Log("Анкета: purcahses {0}ms", sw.ElapsedMilliseconds);

                    foreach(var ag in context.AdvertGroups.OrderBy(i => i.Name).Select(i => new { Id = i.Id, Name = i.Name }).ToArray())
                    {
                        res.SerializedAnketAdverts.Add(new AnketAdvert
                        {
                            AnketId = res.Id,
                            CompanyId = res.CompanyId,
                            Id = Guid.NewGuid(),
                            AdvertGroupName = ag.Name,
                            HasComment = false,
                            HadPlace = calls.ContainsKey(ag.Id) || visits.ContainsKey(ag.Id) || purcahses.ContainsKey(ag.Id),
                            Calls = calls.ContainsKey(ag.Id) ? calls[ag.Id] : 0,
                            Visits = visits.ContainsKey(ag.Id) ? visits[ag.Id] : 0,
                            Purchases = purcahses.ContainsKey(ag.Id) ? purcahses[ag.Id] : 0,
                        });
                    }
                    Logger.Log("Анкета: AdvertGroups {0}ms", sw.ElapsedMilliseconds);


                }
                catch(Exception ex)
                {
                    Logger.Log(ex);
                    res.SerializedAnketAdverts.Clear();
                    foreach(var ag in context.AdvertGroups.OrderBy(i => i.Name).Select(i => new { Id = i.Id, Name = i.Name }).ToArray())
                    {
                        res.SerializedAnketAdverts.Add(new AnketAdvert
                        {
                            AnketId = res.Id,
                            CompanyId = res.CompanyId,
                            Id = Guid.NewGuid(),
                            AdvertGroupName = ag.Name,
                            HasComment = false
                        });
                    }
                    Logger.Log("Анкета: ExceptionAdvertGroups {0}ms", sw.ElapsedMilliseconds);

                }

                res.SerializedAnketAdverts.Add(new AnketAdvert
                {
                    AnketId = res.Id,
                    CompanyId = res.CompanyId,
                    Id = Guid.NewGuid(),
                    AdvertGroupName = "Промо-акции",
                    HasComment = true
                });
                res.SerializedAnketAdverts.Add(new AnketAdvert
                {
                    AnketId = res.Id,
                    CompanyId = res.CompanyId,
                    Id = Guid.NewGuid(),
                    AdvertGroupName = "Внешние сайты",
                    HasComment = true
                });
                res.SerializedAnketAdverts.Add(new AnketAdvert
                {
                    AnketId = res.Id,
                    CompanyId = res.CompanyId,
                    Id = Guid.NewGuid(),
                    AdvertGroupName = "Другое",
                    HasComment = true
                });

                return res;
            }
        }

        public static void PostAnket(Anket anket)
        {
            using(var context = new TonusEntities())
            {
                var dbAnket = context.Ankets.SingleOrDefault(i => i.Id == anket.Id);
                if(dbAnket != null)
                {
                    dbAnket.AnketAdverts.ToList().ForEach(i =>
                    {
                        context.DeleteObject(i);
                    });
                    dbAnket.AnketTreatments.ToList().ForEach(i =>
                    {
                        context.DeleteObject(i);
                    });
                    dbAnket.AnketTickets.ToList().ForEach(i =>
                    {
                        context.DeleteObject(i);
                    });
                    context.DeleteObject(dbAnket);
                    context.SaveChanges();
                }
                else
                {
                    anket.Id = Guid.NewGuid();
                    anket.AnketAdverts.ToList().ForEach(i =>
                    {
                        i.AnketId = anket.Id;
                        i.Name = i.Name ?? "";
                    });
                    anket.AnketTreatments.ToList().ForEach(i =>
                    {
                        i.AnketId = anket.Id;
                    });
                    anket.AnketTickets.ToList().ForEach(i =>
                    {
                        i.AnketId = anket.Id;
                    });

                }
                context.Ankets.AddObject(anket);

                anket.SerializedAnketAdverts.ToList().ForEach(i =>
                {
                    i.AnketId = anket.Id;
                    i.Name = i.Name ?? string.Empty;
                    context.AnketAdverts.AddObject(i);
                });
                anket.SerializedAnketTreatments.ToList().ForEach(i =>
                {
                    i.AnketId = anket.Id;
                    context.AnketTreatments.AddObject(i);
                });
                anket.SerializedAnketTickets.ToList().ForEach(i =>
                {
                    i.AnketId = anket.Id;
                    context.AnketTickets.AddObject(i);
                });
                try
                {
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static Anket GetAnket(Guid anketId)
        {
            using(var context = new TonusEntities())
            {
                var res = context.Ankets.Single(i => i.Id == anketId);
                res.Init();
                return res;
            }
        }

        public static void DeleteAnket(Guid anketId)
        {
            using(var context = new TonusEntities())
            {
                var a = context.Ankets.Single(i => i.Id == anketId);

                if(a.StatusId != 0) return;

                a.AnketAdverts.ToList().ForEach(i => context.DeleteObject(i));
                a.AnketTreatments.ToList().ForEach(i => context.DeleteObject(i));
                a.AnketTickets.ToList().ForEach(i => context.DeleteObject(i));

                context.DeleteObject(a);

                context.SaveChanges();
            }
        }

        public static void ReopenClaim(Guid claimId, string message)
        {
            using(var context = new TonusEntities())
            {
                var claim = context.Claims.Single(i => i.Id == claimId);

                claim.FinishDescription = "Возобновление задачи: " + message;
                claim.StatusId = 7;
                claim.StatusDescription = "Возобновлена";

                context.SaveChanges();
            }
        }

        public static string GetNotificationsForDivision(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                var sb = new StringBuilder();
                var maxCash = context.Divisions.Where(i => i.Id == divisionId).Select(i => i.MaxCash).FirstOrDefault();
                if(maxCash > 0)
                {
                    var year = new DateTime(DateTime.Today.Year, 1, 1);
                    var balance = (context.CashInOrders.Where(i => i.CreatedOn >= year && i.DivisionId == divisionId).Sum(i => (decimal?)i.Amount) ?? 0) -
                        (context.CashOutOrders.Where(i => i.CreatedOn >= year && i.DivisionId == divisionId).Sum(i => (decimal?)i.Amount) ?? 0);
                    if(balance > maxCash * 0.9m)
                    {
                        sb.AppendFormat("\nВ кассе находится {0:c}! Необходимо сдать деньги в банк.", balance);
                    }
                    var date = DateTime.Today.AddDays(-6);
                    if(context.CashOutOrders.Where(i => i.DivisionId == divisionId).OrderByDescending(i => i.CreatedOn).Select(i => i.CreatedOn).FirstOrDefault() < date)
                    {
                        sb.AppendFormat("\nВыручка последний раз сдавалась {0:dd.MM.yyyy}! Необходимо сдать деньги в банк.", date);
                    }
                }
                return sb.ToString();
            }
        }

        public static CustomerNotificationInfo GetCustomerNotificationInfo(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                return context.Customers.Where(i => i.Id == customerId).Select(i => new CustomerNotificationInfo
                {
                    Id = i.Id,
                    FullName = (i.LastName ?? "") + (" " + i.FirstName ?? "") + (" " + i.MiddleName ?? ""),
                    Card = i.CustomerCards.Where(j => j.IsActive).Select(j => j.CardBarcode).FirstOrDefault(),
                    Phone2 = i.Phone2
                }).Single();
            }
        }
    }
}
