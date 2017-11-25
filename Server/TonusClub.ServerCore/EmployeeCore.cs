using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;
using TonusClub.Entities;

using System.ServiceModel;
using TonusClub.ServiceModel.Employees;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Data;

namespace TonusClub.ServerCore
{
    public static class EmployeeCore
    {
        public static void PostEmployeeCategory(EmployeeCategory category, List<Guid> jobIds)
        {
            using (var context = new TonusEntities())
            {
                EmployeeCategory newCat;
                if (category.Id == Guid.Empty)
                {
                    newCat = new EmployeeCategory
                    {
                        CompanyId = category.CompanyId,
                        CreatedOn = DateTime.Now,
                        Id = Guid.NewGuid(),
                    };
                    context.EmployeeCategories.AddObject(newCat);
                }
                else
                {
                    newCat = context.EmployeeCategories.Single(i => i.Id == category.Id);
                }
                bool allowChangeCurrent = newCat.JobPlacements.Count == 0;
                if (allowChangeCurrent)
                {
                    newCat.Description = (category.Description ?? "").Trim();
                    newCat.IsActive = true;
                    newCat.IsPupilContract = category.IsPupilContract;
                    newCat.Name = category.Name.Trim();
                    newCat.SalaryMulti = category.SalaryMulti;

                    newCat.Jobs.Clear();
                    foreach (var i in jobIds)
                    {
                        newCat.Jobs.Add(context.Jobs.Single(j => j.Id == i));
                    }
                }
                else
                {
                    //Save it as a new, mark old as deleted and move all employees to new from today
                    newCat.IsActive = false;
                    var justCat = new EmployeeCategory
                    {
                        CompanyId = newCat.CompanyId,
                        CreatedOn = DateTime.Now,
                        Description = category.Description,
                        Id = Guid.NewGuid(),
                        IsActive = true,
                        IsPupilContract = category.IsPupilContract,
                        Name = category.Name,
                        PrevCategoryId = newCat.Id,
                        SalaryMulti = category.SalaryMulti
                    };

                    foreach (var i in jobIds)
                    {
                        justCat.Jobs.Add(context.Jobs.Single(j => j.Id == i));
                    }


                    context.EmployeeCategories.AddObject(justCat);

                    foreach (var jp in context.JobPlacements.Where(i => i.CategoryId == newCat.Id && !i.FiredById.HasValue).ToList())
                    {
                        PostEmployeeCategoryChange(jp.EmployeeId, justCat.Id, context, false);
                        context.SaveChanges();
                    }
                }
                context.SaveChanges();
            }
        }

        public static List<EmployeeCategory> GetEmployeeCategories(Guid companyId)
        {
            using (var context = new TonusEntities())
            {
                return context.EmployeeCategories.Where(i => i.CompanyId == companyId && i.IsActive).OrderBy(i => i.Name).ToList().Init();
            }
        }

        public static List<Job> GetJobs(Guid divisionId, TonusEntities context)
        {
            context = context ?? new TonusEntities();
            return context.Jobs.Where(i => i.DivisionId == divisionId && !i.HiddenOn.HasValue).OrderBy(i => i.Name).ToList().Init();
        }

        public static List<string> GetJobUnits(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                return context.Jobs.Select(i => i.Unit).Distinct().OrderBy(i => i).ToList();
            }
        }

        public static void PostJob(Job job, List<Guid> categoryIds)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                Job newJob;
                if (job.Id == Guid.Empty)
                {
                    newJob = new Job
                    {
                        CompanyId = job.CompanyId,
                        CreatedOn = DateTime.Now,
                        DivisionId = job.DivisionId,
                        Id = Guid.NewGuid()
                    };
                    context.Jobs.AddObject(newJob);
                }
                else
                {
                    newJob = context.Jobs.Single(i => i.Id == job.Id);
                }
                bool allowChangeCurrent = newJob.JobPlacements.Count == 0;
                if (allowChangeCurrent)
                {
                    newJob.Name = job.Name;
                    newJob.Duties = job.Duties;
                    newJob.Unit = job.Unit;
                    newJob.Salary = job.Salary;
                    newJob.Vacansies = job.Vacansies;
                    newJob.ParallelVacansies = job.ParallelVacansies;
                    newJob.IsMainWorkplace = job.IsMainWorkplace;
                    newJob.WorkGraph = job.WorkGraph;
                    newJob.WorkStart = job.WorkStart;
                    newJob.WorkEnd = job.WorkEnd;
                    newJob.SalarySchemeId = job.SalarySchemeId;

                    newJob.EmployeeCategories.Clear();
                    foreach (var i in categoryIds)
                    {
                        newJob.EmployeeCategories.Add(context.EmployeeCategories.Single(j => j.Id == i));
                    }
                }
                else
                {
                    //Save it as a new, mark old as deleted and move all employees to new from today
                    newJob.HiddenOn = DateTime.Now;
                    var yetJob = new Job
                    {
                        Name = job.Name,
                        Duties = job.Duties,
                        Unit = job.Unit,
                        Salary = job.Salary,
                        Vacansies = job.Vacansies,
                        ParallelVacansies = job.ParallelVacansies,
                        IsMainWorkplace = job.IsMainWorkplace,
                        WorkGraph = job.WorkGraph,
                        WorkStart = job.WorkStart,
                        WorkEnd = job.WorkEnd,
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        CompanyId = job.CompanyId,
                        DivisionId = job.DivisionId,
                        SalarySchemeId = job.SalarySchemeId
                    };
                    foreach (var i in categoryIds)
                    {
                        yetJob.EmployeeCategories.Add(context.EmployeeCategories.Single(j => j.Id == i));
                    }
                    context.Jobs.AddObject(yetJob);
                    foreach (var employee in context.JobPlacements.Where(i => i.JobId == newJob.Id && !i.FiredById.HasValue).Select(i => i.Employee).Distinct().ToList())
                    {
                        employee.Init();
                        PostJobPlacementChange(new JobPlacement
                        {
                            CategoryId = employee.SerializedJobPlacement.CategoryId,
                            CompanyId = employee.CompanyId,
                            EmployeeId = employee.Id,
                            JobId = yetJob.Id,
                            ApplyDate = DateTime.Today
                        }, context, false);
                        context.SaveChanges();
                    }
                }
                context.SaveChanges();
            }
        }

        public static string GetBaselineStatus(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var last = context.Jobs.Where(i => i.DivisionId == divisionId && !i.HiddenOn.HasValue).OrderByDescending(i => i.CreatedOn).FirstOrDefault();
                if (last == null || !last.BaselinedOn.HasValue) return String.Empty;
                return String.Format("Утвердил {0} {1:dd.MM.yyyy}", last.Baseliner.FullName, last.BaselinedOn.Value);
            }
        }

        public static void BaselineJobs(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var last = context.Jobs.Where(i => i.DivisionId == divisionId && !i.HiddenOn.HasValue).OrderByDescending(i => i.CreatedOn).FirstOrDefault();
                if (last == null || last.BaselinedOn.HasValue) return;
                last.BaselinedOn = DateTime.Now;
                last.Baseliner = UserManagement.GetUser(context);
                context.SaveChanges();
            }
        }

        public static List<Employee> GetEmployees(Guid divisionId, bool presentOnly, bool asuOnly)
        {
            using (var context = new TonusEntities())
            {
                var res = context.Employees
                    .Where(i => i.MainDivisionId == divisionId && (!asuOnly || context.Users.Any(u => u.EmployeeId == i.Id)))
                    .ToList()
                    .Init()
                    .OrderBy(i => i.SerializedCustomer.FullName)
                    .ToList();
                if (!presentOnly) return res;
                foreach (var e in res.ToList())
                {
                    if (e.SerializedJobPlacement == null || e.SerializedJobPlacement.FiredById.HasValue)
                    {
                        res.Remove(e);
                    }
                }
                return res;
            }
        }

        public static Guid PostEmployeeForm(Employee employee, Customer customer)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                if (employee.Id == Guid.Empty)
                {
                    employee.Id = Guid.NewGuid();
                    customer.CreatedOn = DateTime.Now;
                    customer.Id = Guid.NewGuid();
                    employee.IsActive = true;
                    employee.AuthorId = user.UserId;
                    customer.AuthorId = user.UserId;
                    employee.BoundCustomerId = customer.Id;

                    employee.Number = context.Employees.Where(i => i.CompanyId == employee.CompanyId).Max(i => i.Number + 1, 1);

                    customer.IsEmployee = true;

                    context.Customers.AddObject(customer);
                    context.Employees.AddObject(employee);
                }
                else
                {
                    var old = context.Employees.Single(i => i.Id == employee.Id);

                    old.FactCity = employee.FactCity;
                    old.FactIndex = employee.FactIndex;
                    old.FactOther = employee.FactOther;
                    old.FactStreet = employee.FactStreet;
#if BEAUTINIKA
                    old.IsCosmetologist = employee.IsCosmetologist;
#endif

                    old.BoundCustomer.AddrCity = customer.AddrCity;
                    old.BoundCustomer.AddrIndex = customer.AddrIndex;
                    old.BoundCustomer.AddrOther = customer.AddrOther;
                    old.BoundCustomer.AddrStreet = customer.AddrStreet;
                    old.BoundCustomer.Comments = customer.Comments;
                    old.BoundCustomer.FirstName = customer.FirstName;
                    old.BoundCustomer.MiddleName = customer.MiddleName;
                    old.BoundCustomer.LastName = customer.LastName;
                    old.BoundCustomer.Gender = customer.Gender;
                    old.BoundCustomer.Birthday = customer.Birthday;
                    old.BoundCustomer.PasspEmitDate = customer.PasspEmitDate;
                    old.BoundCustomer.PasspEmitPlace = customer.PasspEmitPlace;
                    old.BoundCustomer.PasspNumber = customer.PasspNumber;
                    old.BoundCustomer.Phone1 = customer.Phone1;
                    old.BoundCustomer.Phone2 = customer.Phone2;
                    old.BoundCustomer.Email = customer.Email;
                    old.BoundCustomer.IsEmployee = true;
                }

                context.SaveChanges();
                return employee.Id;
            }
        }

        public static List<Job> GetAvailableJobs(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var allJobs = context.Jobs.Where(i => i.DivisionId == divisionId && !i.HiddenOn.HasValue).OrderBy(i => i.Name).ToList().Init().ToDictionary(i => i.Id, i => i);
                var allPls = context.JobPlacements.Where(i => i.Job.DivisionId == divisionId && i.IsAsset && !i.FireDate.HasValue);

                foreach (var jp in allPls)
                {
                    if (allJobs.ContainsKey(jp.JobId))
                    {
                        var j = allJobs[jp.JobId];
                        j.Vacansies--;
                        if (j.Vacansies <= 0)
                        {
                            allJobs.Remove(jp.JobId);
                        }
                    }
                }
                return allJobs.Values.ToList();
            }
        }

        public static Guid PostJobPlacement(JobPlacement placement)
        {
            using (var context = new TonusEntities())
            {
                if (placement.Id != Guid.Empty)
                {
                    var pl = context.JobPlacements.Single(i => i.Id == placement.Id);
                    context.JobPlacements.Detach(pl);
                    context.JobPlacements.Attach(placement);
                    context.ObjectStateManager.ChangeObjectState(placement, System.Data.EntityState.Modified);
                }
                else
                {
                    placement.Id = Guid.NewGuid();
                    placement.CreatedOn = DateTime.Now;
                    context.JobPlacements.AddObject(placement);

                    var num = context.EmployeeDocuments.Where(i => i.CompanyId == placement.CompanyId).Max(i => i.Number + 1, 1);
                    var doc = new EmployeeDocument
                    {
                        CompanyId = placement.CompanyId,
                        DocType = (short)DocumentTypes.JobApply,
                        Id = Guid.NewGuid(),
                        ReferenceId = placement.Id,
                        Number = num,
                        CreatedOn = DateTime.Now
                    };
                    context.EmployeeDocuments.AddObject(doc);
                    placement.DocumentId = doc.Id;
                }

                context.SaveChanges();
                return placement.Id;
            }
        }

        public static JobPlacement GetJobPlacementDraft(Guid employeeId)
        {
            using (var context = new TonusEntities())
            {
                return context.JobPlacements.Where(i => !i.IsAsset && i.EmployeeId == employeeId).FirstOrDefault();
            }
        }

        public static Employee GetEmployeeById(Guid id)
        {
            using (var context = new TonusEntities())
            {
                var res = context.Employees.FirstOrDefault(i => i.Id == id);
                res.Init();
                return res;
            }
        }

        public static string GetEmployeeCardStatusMessage(string cardNumber)
        {
            using (var context = new TonusEntities())
            {
                var card = context.CustomerCards.FirstOrDefault(i => i.CardBarcode == cardNumber && i.IsActive);
                if (card == null) return String.Empty;
                return String.Format("Карта в данный момент оформлена на сотрудника {0}. Если провести назначение, карта будет переоформлена.", card.Customer.FullName);
            }
        }

        public static void PostEmployeeCard(Guid employeeId, string newCardNumber)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var cards = context.CustomerCards.Where(i => i.CardBarcode == newCardNumber && i.CompanyId == user.CompanyId);
                var employee = context.Employees.Single(i => i.Id == employeeId);
                foreach (var card in cards)
                {
                    card.IsActive = false;
                }
                var newCard = new CustomerCard
                {
                    AuthorId = user.UserId,
                    CardBarcode = newCardNumber,
                    CompanyId = employee.CompanyId,
                    CustomerCardTypeId = Guid.Empty,
                    CustomerId = employee.BoundCustomerId,
                    Discount = 0,
                    DivisionId = employee.MainDivisionId,
                    EmitDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Price = 0
                };
                context.CustomerCards.AddObject(newCard);
                context.SaveChanges();
            }
        }

        public static Guid PostJobPlacementChange(JobPlacement jobPlacement, TonusEntities context = null, bool save = true)
        {
            context = context ?? new TonusEntities();
            var user = UserManagement.GetUser(context);
            var employee = context.Employees.Single(i => i.Id == jobPlacement.EmployeeId);
            employee.Init();
            if (employee.SerializedJobPlacement == null || !employee.SerializedJobPlacement.IsAsset)
                throw new FaultException<string>("Сотрудник еще не принят на работу или его принятие не проведено!",
                    "Сотрудник еще не принят на работу или его принятие не проведено!");
            employee.SerializedJobPlacement.FiredById = user.UserId;
            employee.SerializedJobPlacement.FireDate = jobPlacement.ApplyDate;
            employee.SerializedJobPlacement.FireCause = "Перевод на другую должность";
            var newPlacement = new JobPlacement
            {
                ApplyDate = jobPlacement.ApplyDate,
                AuthorId = user.UserId,
                CategoryId = jobPlacement.CategoryId,
                CompanyId = jobPlacement.CompanyId,
                CreatedOn = DateTime.Now,
                EmployeeId = jobPlacement.EmployeeId,
                Id = Guid.NewGuid(),
                IsAsset = true,
                JobId = jobPlacement.JobId,
                Seniority = 0
            };
            context.JobPlacements.AddObject(newPlacement);

            var num = context.EmployeeDocuments.Where(i => i.CompanyId == newPlacement.CompanyId).Max(i => i.Number + 1, 1);
            var doc = new EmployeeDocument
            {
                CompanyId = newPlacement.CompanyId,
                DocType = (short)DocumentTypes.JobChange,
                Id = Guid.NewGuid(),
                ReferenceId = newPlacement.Id,
                Number = num,
                CreatedOn = DateTime.Now
            };
            context.EmployeeDocuments.AddObject(doc);
            newPlacement.DocumentId = doc.Id;

            if (save) context.SaveChanges();
            return newPlacement.Id;
        }

        public static Guid PostEmployeeFire(Guid employeeId, DateTime fireDate, string fireCause, bool cardReturned, decimal totalToPay, decimal bonus, decimal ndfl, string logMessage)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var employee = context.Employees.Single(i => i.Id == employeeId);
                employee.Init();
                if (employee.SerializedJobPlacement == null || !employee.SerializedJobPlacement.IsAsset)
                    throw new FaultException<string>("Сотрудник еще не принят на работу или его принятие не проведено!",
                        "Сотрудник еще не принят на работу или его принятие не проведено!");

                employee.SerializedJobPlacement.FireCause = fireCause;
                employee.SerializedJobPlacement.FireDate = fireDate.Date;
                employee.SerializedJobPlacement.FiredById = user.UserId;

                if (cardReturned)
                {
                    employee.BoundCustomer.InitActiveCard();
                    employee.BoundCustomer.ActiveCard.IsActive = false;
                }

                var num = context.EmployeeDocuments.Where(i => i.CompanyId == employee.SerializedJobPlacement.CompanyId).Max(i => i.Number + 1, 1);
                var doc = new EmployeeDocument
                {
                    CompanyId = employee.SerializedJobPlacement.CompanyId,
                    DocType = (short)DocumentTypes.JobFire,
                    Id = Guid.NewGuid(),
                    ReferenceId = employee.SerializedJobPlacement.Id,
                    Number = num,
                    CreatedOn = DateTime.Now
                };
                context.EmployeeDocuments.AddObject(doc);
                employee.SerializedJobPlacement.FireDocumentId = doc.Id;

                var spType = context.SpendingTypes.SingleOrDefault(i => i.Name == "Выплаты сотрудникам" && i.CompanyId == employee.CompanyId);
                if (spType == null)
                {
                    using (var con = new TonusEntities())
                    {
                        var spType1 = new SpendingType
                        {
                            CompanyId = user.CompanyId,
                            Id = Guid.NewGuid(),
                            IsCommon = true,
                            Name = "Выплаты сотрудникам"
                        };
                        con.SpendingTypes.AddObject(spType1);
                        con.SaveChanges();
                    }
                    spType = context.SpendingTypes.SingleOrDefault(i => i.Name == "Выплаты сотрудникам" && i.CompanyId == employee.CompanyId);
                }
                var n = context.Spendings.Where(i => i.DivisionId == employee.MainDivisionId).Max(i => i.Number, 0) + 1;

                var spending = new Spending
                {

                    Amount = totalToPay + bonus - ndfl,
                    AuthorId = user.UserId,
                    CompanyId = employee.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = employee.MainDivisionId,
                    Id = Guid.NewGuid(),
                    Name = "Расчет при увольнении",
                    Number = n,
                    PaymentType = "Выплаты сотрудникам",
                    SpendingTypeId = spType.Id
                };
                context.Spendings.AddObject(spending);
                context.SaveChanges();

                var pmt = new EmployeePayment
                {
                    CreatedOn = DateTime.Now,
                    Amount = spending.Amount,
                    CompanyId = employee.CompanyId,
                    EmployeeId = employeeId,
                    Id = Guid.NewGuid(),
                    PaymentType = 2,
                    Period = fireDate.AddDays(-fireDate.Day + 1),
                    SalarySheetId = null,
                    SpendingId = spending.Id,
                    Log = logMessage + "\nПремия: " + bonus.ToString("c") + "\nБухгалтерия+НДФЛ: " + ndfl.ToString("c")
                };
                context.EmployeePayments.AddObject(pmt);
                context.SaveChanges();

                return employee.SerializedJobPlacement.Id;
            }
        }

        public static Guid PostEmployeeVacation(Guid employeeId, DateTime beginDate, DateTime endDate, byte vacationType, decimal totalToPay, decimal ndfl, string logMessage)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var employee = context.Employees.Single(i => i.Id == employeeId);

                foreach (var vac in employee.EmployeeVacations.Where(i => i.VacationType == vacationType))
                {
                    if (Core.DatesIntersects(beginDate, endDate, vac.BeginDate, vac.EndDate))
                    {
                        var str = "В указанный период дат сотрудник уже находится в отпуске/на больничном!";
                        throw new FaultException<string>(str, str);
                    }
                }

                var vacation = new EmployeeVacation
                {
                    AuthorId = user.UserId,
                    BeginDate = beginDate,
                    CompanyId = employee.CompanyId,
                    CreatedOn = DateTime.Now,
                    EmployeeId = employeeId,
                    EndDate = endDate,
                    Id = Guid.NewGuid(),
                    VacationType = vacationType
                };
                context.EmployeeVacations.AddObject(vacation);


                var num = context.EmployeeDocuments.Where(i => i.CompanyId == vacation.CompanyId).Max(i => i.Number + 1, 1);
                var doc = new EmployeeDocument
                {
                    CompanyId = vacation.CompanyId,
                    DocType = vacationType + 4,
                    Id = Guid.NewGuid(),
                    ReferenceId = vacation.Id,
                    Number = num,
                    CreatedOn = DateTime.Now
                };
                context.EmployeeDocuments.AddObject(doc);
                vacation.DocumentId = doc.Id;

                var spType = context.SpendingTypes.SingleOrDefault(i => i.Name == "Выплаты сотрудникам" && i.CompanyId == doc.CompanyId);
                if (spType == null)
                {
                    spType = new SpendingType
                    {
                        CompanyId = user.CompanyId,
                        Id = Guid.Empty,
                        IsCommon = true,
                        Name = "Выплаты сотрудникам"
                    };

                    context.SpendingTypes.AddObject(spType);
                }
                var n = context.Spendings.Where(i => i.DivisionId == employee.MainDivisionId).Max(i => i.Number, 0) + 1;

                if (totalToPay > 0)
                {
                    var spending = new Spending
                    {

                        Amount = totalToPay,
                        AuthorId = user.UserId,
                        CompanyId = employee.CompanyId,
                        CreatedOn = DateTime.Now,
                        DivisionId = employee.MainDivisionId,
                        Id = Guid.NewGuid(),
                        Name = "Выплата больничных/отпускных",
                        Number = n,
                        PaymentType = "Выплаты сотрудникам",
                        SpendingTypeId = spType.Id
                    };
                    context.Spendings.AddObject(spending);
                    context.SaveChanges();
                    var pmt = new EmployeePayment
                    {
                        Amount = spending.Amount,
                        CompanyId = employee.CompanyId,
                        EmployeeId = employeeId,
                        Id = Guid.NewGuid(),
                        PaymentType = 3,
                        Period = beginDate.AddDays(-beginDate.Day + 1),
                        SalarySheetId = null,
                        SpendingId = spending.Id,
                        Log = logMessage + "\nБухгалтерия+НДФЛ: " + ndfl.ToString("c"),
                        CreatedOn = DateTime.Now
                    };
                    context.EmployeePayments.AddObject(pmt);
                }
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return vacation.Id;
            }
        }

        public static Guid[] PostEmployeeTrip(Guid[] employeeIds, DateTime beginDate, DateTime endDate, string destination, string target, string tripBase)
        {
            var res = new List<Guid>();
            foreach (var eId in employeeIds)
            {
                using (var context = new TonusEntities())
                {
                    var emp = context.Employees.Single(i => i.Id == eId);
                    //var numK = context.EmployeeTrips.Any(i => i.Employee.MainDivisionId == emp.MainDivisionId) ? context.EmployeeTrips.Where(i => i.Employee.MainDivisionId == emp.MainDivisionId).Max(i => i.Number) + 1 : 1;
                    var user = UserManagement.GetUser(context);
                    var employee = context.Employees.Single(i => i.Id == eId);
                    var trip = new EmployeeTrip
                    {
                        AuthorId = user.UserId,
                        Base = tripBase,
                        BeginDate = beginDate,
                        CompanyId = employee.CompanyId,
                        CreatedOn = DateTime.Now,
                        Destination = destination,
                        EmployeeId = eId,
                        EndDate = endDate,
                        Id = Guid.NewGuid(),
                        Number = 0,
                        Target = target
                    };
                    res.Add(trip.Id);
                    context.EmployeeTrips.AddObject(trip);

                    var num = context.EmployeeDocuments.Where(i => i.CompanyId == trip.CompanyId).Max(i => i.Number + 1, 1);
                    var doc = new EmployeeDocument
                    {
                        CompanyId = trip.CompanyId,
                        DocType = (short)DocumentTypes.Trip,
                        Id = Guid.NewGuid(),
                        ReferenceId = trip.Id,
                        Number = num,
                        CreatedOn = DateTime.Now
                    };
                    context.EmployeeDocuments.AddObject(doc);
                    trip.DocumentId = doc.Id;
                    context.SaveChanges();
                }
            }
            return res.ToArray();

        }

        public static Guid PostEmployeeCategoryChange(Guid employeeId, Guid categoryId, TonusEntities context = null, bool saveChs = true)
        {
            context = context ?? new TonusEntities();

            var user = UserManagement.GetUser(context);
            var employee = context.Employees.Single(i => i.Id == employeeId);
            employee.Init();
            employee.SerializedJobPlacement.FireCause = "Смена категории";
            employee.SerializedJobPlacement.FireDate = DateTime.Today;
            employee.SerializedJobPlacement.FiredById = user.UserId;
            var jp = new JobPlacement
            {
                ApplyDate = DateTime.Today,
                AuthorId = user.UserId,
                CategoryId = categoryId,
                CompanyId = employee.CompanyId,
                CreatedOn = DateTime.Now,
                EmployeeId = employeeId,
                Id = Guid.NewGuid(),
                IsAsset = true,
                JobId = employee.SerializedJobPlacement.JobId
            };
            context.JobPlacements.AddObject(jp);

            var num = context.EmployeeDocuments.Where(i => i.CompanyId == jp.CompanyId).Max(i => i.Number + 1, 1);
            var doc = new EmployeeDocument
            {
                CompanyId = jp.CompanyId,
                DocType = (short)DocumentTypes.CategoryChange,
                Id = Guid.NewGuid(),
                ReferenceId = jp.Id,
                Number = num,
                CreatedOn = DateTime.Now
            };
            context.EmployeeDocuments.AddObject(doc);
            jp.DocumentId = doc.Id;

            if (saveChs) context.SaveChanges();
            return jp.Id;
        }

        public static List<EmployeeDocument> GetEmployeeDocuments(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId);
                return context.EmployeeDocuments.Where(i => i.CompanyId == div.CompanyId).OrderByDescending(i => i.CreatedOn).ToList().Init().Where(i => i.DivisionId == divisionId).ToList();
            }
        }

        public static int GetActiveEmployeesCountForJobId(Guid jobId)
        {
            using (var context = new TonusEntities())
            {
                return context.JobPlacements.Where(i => !i.FiredById.HasValue && i.JobId == jobId).Count();
            }
        }

        public static int GetActiveEmployeesCountForCategoryId(Guid categoryId)
        {
            using (var context = new TonusEntities())
            {
                return context.JobPlacements.Where(i => !i.FiredById.HasValue && i.CategoryId == categoryId).Count();
            }
        }

        public static void HideEmployeeCategoryById(Guid categoryId)
        {
            using (var context = new TonusEntities())
            {
                var cat = context.EmployeeCategories.Single(i => i.Id == categoryId);
                cat.IsActive = false;
                context.SaveChanges();

            }
        }

        public static void HideEmployeeJobById(Guid jobId)
        {
            using (var context = new TonusEntities())
            {
                var job = context.Jobs.Single(i => i.Id == jobId);
                job.HiddenOn = DateTime.Now;
                context.SaveChanges();
            }
        }

        public static Employee GetEmployeeByCard(string cardNumber)
        {
            using (var context = new TonusEntities())
            {
                var card = context.CustomerCards.SingleOrDefault(i => i.CardBarcode == cardNumber && i.IsActive);
                if (card == null) return null;
                if (card.Customer.Employees.Count() == 0) return null;
                card.Customer.Employees.First().Init();
                return card.Customer.Employees.First();
            }
        }

        public static void PostEmployeeVisit(Guid employeeId, bool isIn)
        {
            using (var context = new TonusEntities())
            {
                var employee = context.Employees.Single(i => i.Id == employeeId);
                employee.Init();
                if (employee.IsAtWorkplace == isIn)
                {
                    return;
                }
                var vis = new EmployeeVisit
                {
                    CompanyId = employee.CompanyId,
                    CreatedOn = DateTime.Now,
                    EmployeeId = employeeId,
                    Id = Guid.NewGuid(),
                    IsIncome = isIn
                };
                context.EmployeeVisits.AddObject(vis);
                context.SaveChanges();
                Logger.DBLog("Регистрация посещения сотрудника {0}, вход: {1}", employee.BoundCustomer.FullName, isIn);

            }
        }

        public static List<VacationList> GetVacationHistory(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                return context.VacationLists.Where(i => i.DivisionId == divisionId).OrderByDescending(i => i.CreatedOn).ToList().Init();
            }
        }

        public static List<VacationPreference> GetEmployeePreferences(Guid employeeId)
        {
            using (var context = new TonusEntities())
            {
                return context.VacationPreferences.Where(i => i.EmployeeId == employeeId).OrderByDescending(i => i.CreatedOn).ToList();
            }
        }

        public static void PostEmployeePreference(Guid employeeId, DateTime beginDate, DateTime endDate, short prefType)
        {
            using (var context = new TonusEntities())
            {
                var cId = context.Employees.Single(i => i.Id == employeeId).CompanyId;

                var pref = new VacationPreference
                {
                    CompanyId = cId,
                    CreatedOn = DateTime.Now,
                    EmployeeId = employeeId,
                    EndDate = endDate,
                    Id = Guid.NewGuid(),
                    PrefType = prefType,
                    StartDate = beginDate
                };
                context.VacationPreferences.AddObject(pref);
                context.SaveChanges();
            }
        }

        public static List<EmployeeScheduleProposalElement> GenerateScheduleProposal(Guid divisionId, int year, int recDays)
        {
            using (var context = new TonusEntities())
            {
                var res = new List<EmployeeScheduleProposalElement>();

                //Job-periods
                var dict = new Dictionary<Guid, List<KeyValuePair<DateTime, DateTime>>>();

                var employees = context.JobPlacements
                    .Where(i => !i.FireDate.HasValue && i.Employee.MainDivisionId == divisionId)
                    .Select(i => i.Employee)
                    .OrderBy(i => i.BoundCustomer.LastName)
                    .ThenBy(i => i.BoundCustomer.FirstName);

                var minDate = DateTime.Today.Year >= year ? DateTime.Today : new DateTime(year, 1, 1);

                //28 поделить на рекДейз - именно столько итераций надо провести, чтобы заполнить год.
                for (int k = 0; k < 28 / recDays; k++)
                {
                    foreach (var employee in employees)
                    {
                        employee.Init();
                        if (employee.SerializedJobPlacement == null) continue;
                        var jobId = employee.SerializedJobPlacement.JobId;
                        if (!dict.ContainsKey(jobId))
                        {
                            dict.Add(jobId, new List<KeyValuePair<DateTime, DateTime>>());
                            //Блокируются в том числе и отгулы с командировками, по которым есть приказ
                            context.EmployeeVacations.Where(i => i.Employee.MainDivisionId == divisionId
                                && !i.Employee.JobPlacements.Any(j => j.FireDate.HasValue && j.JobId == jobId)).ToList().ForEach(
                                i => dict[jobId].Add(new KeyValuePair<DateTime, DateTime>(i.BeginDate, i.EndDate))
                                );
                            //TODO: add already asset vacations after mindate
                        }

                        var startDate = minDate;

                        while (true)
                        {
                            var flag = true;
                            var availDays = GetAvailVacationDays(context, employee, startDate, recDays);
                            if (availDays == null) break;
                            foreach (var p in dict[jobId])
                            {
                                if (Core.DatesIntersects(availDays.Value.Key, availDays.Value.Key.AddDays(availDays.Value.Value), p.Key, p.Value))
                                {
                                    startDate = p.Value.AddDays(2);
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                if (availDays.Value.Key.Year == year)
                                {
                                    res.Add(new EmployeeScheduleProposalElement
                                    {
                                        EmployeeId = employee.Id,
                                        EmployeeJob = employee.SerializedJobPlacement.Job.Name,
                                        EmployeeName = employee.BoundCustomer.FullName,
                                        Start = availDays.Value.Key,
                                        Finish = availDays.Value.Key.AddDays(availDays.Value.Value - 1)
                                    });
                                    dict[jobId].Add(new KeyValuePair<DateTime, DateTime>(availDays.Value.Key, availDays.Value.Key.AddDays(availDays.Value.Value - 1)));
                                }
                                break;
                            }
                        }

                    }
                }
                return res.OrderBy(i => i.Start).ToList();
            }
        }

        /// <summary>
        /// start, length
        /// </summary>
        /// <param name="context"></param>
        /// <param name="employee"></param>
        /// <param name="minDate"></param>
        /// <returns></returns>
        private static KeyValuePair<DateTime, int>? GetAvailVacationDays(TonusEntities context, Employee employee, DateTime minDate, int recDays)
        {
            var pref = context.VacationPreferences.Where(i => i.EmployeeId == employee.Id && i.StartDate >= minDate && i.PrefType == 0).OrderBy(i => i.StartDate).FirstOrDefault();
            KeyValuePair<DateTime, int>? res;
            if (pref != null) res = new KeyValuePair<DateTime, int>(GetDateForX(employee, pref.StartDate, pref.Length), pref.Length);
            else res = new KeyValuePair<DateTime, int>(GetDateForX(employee, minDate, recDays), recDays);
            return res.Value.Key.Year == minDate.Year ? res : null;
        }

        private static DateTime GetDateForX(Employee employee, DateTime minDate, int X)
        {
            var apply = employee.JobPlacements.Min(i => i.ApplyDate);
            var avail = (int)Math.Floor(Math.Floor((minDate - apply).TotalDays / 30.4) / 3) * 7;
            var used = employee.EmployeeVacations.Where(i => i.VacationType == 0).ToList().Sum(i => i.VacationLength);
            var availTotal = avail - used;
            if (availTotal >= X) return minDate;
            return minDate.AddMonths((int)Math.Ceiling((X - (double)availTotal) / 7) * 3);
        }

        public static Guid PostEmployeeVacationsSchedule(List<EmployeeScheduleProposalElement> list)
        {
            using (var context = new TonusEntities())
            {
                if (list.Count == 0) return Guid.Empty;
                var fId = list[0].EmployeeId;
                var first = context.Employees.Single(i => i.Id == fId);

                var user = UserManagement.GetUser(context);
                var vlist = new VacationList
                {
                    AuthorId = user.UserId,
                    CompanyId = first.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = first.MainDivisionId,
                    Id = Guid.NewGuid(),
                    Year = list[0].Start.Year
                };
                context.VacationLists.AddObject(vlist);

                foreach (var i in list)
                {
                    var line = new VacationListItem
                    {
                        CompanyId = first.CompanyId,
                        EmployeeId = i.EmployeeId,
                        FinishDate = i.Finish.Date,
                        Id = Guid.NewGuid(),
                        StartDate = i.Start.Date,
                        VacationListId = vlist.Id
                    };
                    context.VacationListItems.AddObject(line);
                }
                context.SaveChanges();
                return vlist.Id;
            }
        }

        public static List<EmployeeScheduleProposalElement> GetCurrentEmployeeVacationsSchedule(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var list = context.VacationLists.Where(i => i.DivisionId == divisionId && i.Year == DateTime.Today.Year).OrderByDescending(i => i.CreatedOn).FirstOrDefault();
                if (list == null) return new List<EmployeeScheduleProposalElement>();
                return GetEmployeeVacationsSchedule(list.Id, context);
            }
        }

        public static List<EmployeeScheduleProposalElement> GetEmployeeVacationsSchedule(Guid listId, TonusEntities context = null)
        {
            context = context ?? new TonusEntities();
            var res = new List<EmployeeScheduleProposalElement>();

            var list = context.VacationLists.SingleOrDefault(i => i.Id == listId);
            if (list != null)
            {
                foreach (var item in list.VacationListItems)
                {
                    item.Employee.Init();
                    if (item.Employee.SerializedJobPlacement != null)
                    {
                        res.Add(new EmployeeScheduleProposalElement
                        {
                            EmployeeId = item.EmployeeId,
                            EmployeeJob = item.Employee.SerializedJobPlacement.Job.Name,
                            EmployeeName = item.Employee.BoundCustomer.FullName,
                            Finish = item.FinishDate,
                            Start = item.StartDate,
                            Unit = item.Employee.SerializedJobPlacement.Job.Unit
                        });
                    }
                }
            }
            return res.OrderBy(i => i.Start).ToList();
        }

        public static List<EmployeeWorkScheduleItem> GetEmployeeWorkSchedule(Guid divisionId, DateTime start, DateTime finish)
        {
            using (var context = new TonusEntities())
            {
                var lastBaselined = context.Jobs.Where(i => i.DivisionId == divisionId && !i.HiddenOn.HasValue && i.BaselinedOn.HasValue).OrderByDescending(i => i.BaselinedOn.Value).FirstOrDefault();
                if (lastBaselined == null) return new List<EmployeeWorkScheduleItem>();
                var baseDateTime = lastBaselined.BaselinedOn.Value;
                var jobs = context.Jobs.Where(i => i.DivisionId == divisionId && !i.HiddenOn.HasValue && i.CreatedOn <= baseDateTime);
                var res = new Dictionary<DateTime, EmployeeWorkScheduleProposalElement>();
                var periodLen = (int)(finish - start).TotalDays;

                for (int i = 0; i <= (finish - start).TotalDays; i++)
                {
                    res.Add(start.AddDays(i), new EmployeeWorkScheduleProposalElement
                    {
                        Date = start.AddDays(i),
                        JobPlacements = new List<JobPlacement>()
                    });
                }

                foreach (var job in jobs)
                {
                    Dictionary<DateTime, int> filling = new Dictionary<DateTime, int>();
                    for (int i = 0; i <= (finish - start).TotalDays; i++)
                    {
                        filling.Add(start.AddDays(i), 0);
                    }

                    if (job.ParallelVacansies > job.Vacansies || job.Vacansies == 0 || job.ParallelVacansies == 0) continue;
                    var jps = context.JobPlacements.Where(i => i.JobId == job.Id && !i.FireDate.HasValue);
                    //if (jps.Count() < job.ParallelVacansies) continue;

                    var workdays = 5;
                    var holidays = 2;

                    var graph = (job.WorkGraph ?? "").Split('/');
                    if (graph.Length == 2)
                    {
                        Int32.TryParse(graph[0], out workdays);
                        Int32.TryParse(graph[1], out holidays);
                    }

                    foreach (var jp in jps)
                    {
                        if (!(workdays == 5 & holidays == 2))
                        {
                            //Сменная работа
                            var sDate = start;
                            if (workdays + holidays == 3 && (finish - start).TotalDays > 1 && filling.Max(i => i.Value) == 1)
                            {
                                sDate = sDate.AddDays(1);
                            }
                            while (true)
                            {
                                for (int i = 0; i < workdays; i++)
                                {
                                    if (sDate > finish) break;

                                    if (filling[sDate] < job.ParallelVacansies
                                        && !jp.Employee.EmployeeVacations.Any(j => j.BeginDate <= sDate && j.EndDate >= sDate)
                                        && !jp.Employee.VacationPreferences.Any(j => j.StartDate <= sDate && j.EndDate >= sDate)
                                        && !jp.Employee.EmployeeTrips.Any(j => j.BeginDate <= sDate && j.EndDate >= sDate))
                                    {
                                        res[sDate].JobPlacements.Add(jp);
                                        filling[sDate]++;
                                    }
                                    else
                                    {
                                        i = -1;
                                    }
                                    sDate = sDate.AddDays(1);
                                }
                                if (sDate > finish) break;
                                sDate = sDate.AddDays(holidays);
                            }
                        }
                        else
                        {
                            //Планктонная работа
                            for (int i = 0; i <= (finish - start).TotalDays; i++)
                            {
                                var d = start.AddDays(i);
                                if (!(d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday || context.Holidays.Any(o => o.Vacation == d)))
                                {
                                    res[d].JobPlacements.Add(jp);
                                }
                            }
                        }
                    }

                    if (!(workdays == 5 & holidays == 2))
                    {
                        //Составили с учетом пожеланий, теперь комплектуем до упора
                        //Не надо это делать для тех, кто работает в режиме планктона (5/2)
                        var press = GetPress(res, jps, job.Id);//jp-days
                        foreach (var i in res)
                        {
                            if (filling[i.Key] < job.ParallelVacansies)
                            {
                                var empl = SelectEmployee(res, jps, press, i.Key);
                                if (empl != null)
                                {
                                    res[i.Key].JobPlacements.Add(empl);
                                    filling[i.Key]++;
                                }
                            }
                        }
                    }
                }

                var ret = new Dictionary<Guid, EmployeeWorkScheduleItem>();

                foreach (var i in res.Values)
                {
                    foreach (var j in i.JobPlacements)
                    {
                        if (!ret.ContainsKey(j.Id))
                        {
                            j.Init();
                            ret.Add(j.Id, new EmployeeWorkScheduleItem
                            {
                                JobPlacement = j
                            });
                            for (int k = 0; k <= (finish - start).TotalDays; k++)
                            {
                                var d = start.AddDays(k);
                                ret[j.Id].Dates.Add(d, new DateData { Date = d, IsEnabled = !j.Employee.EmployeeVacations.Any(o => o.EndDate >= d && o.BeginDate <= d), IsSet = false });
                            }
                        }
                        ret[j.Id].Dates[i.Date].IsSet = true;
                    }
                }

                return ret.Values.ToList();
            }
        }

        private static Dictionary<JobPlacement, int> GetPress(Dictionary<DateTime, EmployeeWorkScheduleProposalElement> total, IEnumerable<JobPlacement> jps, Guid guid)
        {
            var res = jps.ToDictionary(i => i, i => 0);

            foreach (var i in total)
            {
                foreach (var j in i.Value.JobPlacements)
                {
                    if (res.ContainsKey(j)) res[j]++;
                }
            }
            return res;
        }

        private static JobPlacement SelectEmployee(Dictionary<DateTime, EmployeeWorkScheduleProposalElement> res, IEnumerable<JobPlacement> jps, Dictionary<JobPlacement, int> press, DateTime date)
        {
            foreach (var i in press.OrderBy(i => i.Value))
            {
                if (res[date].JobPlacements.Any(j => j.Id == i.Key.Id)) continue;
                if (i.Key.Employee.EmployeeVacations.Any(j => j.BeginDate <= date && j.EndDate >= date) ||
                    i.Key.Employee.EmployeeTrips.Any(j => j.BeginDate <= date && j.EndDate >= date)) continue;
                press[i.Key]++;
                return i.Key;

            }
            return null;
        }

        public static Guid PostEmployeeSchedule(Guid divisionId, Dictionary<Guid, List<DateTime>> schedule)
        {
            using (var context = new TonusEntities())
            {
                if (schedule.Count == 0) return Guid.Empty;
                var firstId = schedule.Keys.First();
                var first = context.JobPlacements.Single(i => i.Id == firstId);

                var user = UserManagement.GetUser(context);

                var minDate = DateTime.MaxValue;
                var maxDate = DateTime.Today;

                foreach (var i in schedule.Values)
                {
                    if (i.Count > 0)
                    {
                        var min = i.Min(j => j.Date);
                        var max = i.Max(j => j.Date);
                        minDate = minDate > min ? min : minDate;
                        maxDate = maxDate < max ? max : maxDate;
                    }
                }

                var sched = new EmployeeWorkGraph
                {
                    AuthorId = user.UserId,
                    Begin = minDate,
                    CompanyId = first.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = first.Employee.MainDivisionId,
                    End = maxDate,
                    Id = Guid.NewGuid(),
                    SerializedData = SerializeWorkGraph(schedule)
                };
                context.EmployeeWorkGraphs.AddObject(sched);
                context.SaveChanges();

                return sched.Id;
            }
        }

        private static byte[] SerializeWorkGraph(Dictionary<Guid, List<DateTime>> schedule)
        {
            XmlSerializer x = new XmlSerializer(typeof(List<SerPair>));
            MemoryStream ms = new MemoryStream();
            var src = new List<SerPair>();
            foreach (var i in schedule)
            {
                src.Add(new SerPair { Key = i.Key, Value = i.Value });
            }
            x.Serialize(ms, src);
            ms.Position = 0;
            var res = new byte[ms.Length];
            ms.Read(res, 0, (int)ms.Length);
            return res;
        }

        public static Dictionary<Guid, List<DateTime>> DeserializeWorkGraph(byte[] schedule)
        {
            XmlSerializer x = new XmlSerializer(typeof(List<SerPair>));
            MemoryStream ms = new MemoryStream(schedule);
            var res = x.Deserialize(ms);
            return (res as List<SerPair>).ToDictionary(i => i.Key, i => i.Value);
        }

        public static List<EmployeeWorkGraph> GetWorkGraphs(Guid divisionId)
        {
            List<EmployeeWorkGraph> res;
            using (var context = new TonusEntities())
            {
                res = context.EmployeeWorkGraphs.Where(i => i.DivisionId == divisionId).OrderByDescending(i => i.CreatedOn).ToList().Init();
            }
            res.ForEach(i => i.SerializedData = null);
            return res;
        }

        public static List<DateTime> GetHolidays(int year)
        {
            using (var context = new TonusEntities())
            {
                return context.Holidays.Where(i => i.Vacation.Year == year).Select(i => i.Vacation).OrderBy(i => i).ToList();
            }
        }

        public static void PostHoliday(DateTime date)
        {
            using (var context = new TonusEntities())
            {
                if (context.Holidays.Any(i => i.Vacation == date)) return;
                context.Holidays.AddObject(new Holiday { Id = Guid.NewGuid(), Vacation = date });
                context.SaveChanges();
            }
        }

        public static void DeleteHoliday(DateTime date)
        {
            using (var context = new TonusEntities())
            {
                if (!context.Holidays.Any(i => i.Vacation == date)) return;
                context.Holidays.DeleteObject(context.Holidays.Single(i => i.Vacation == date));
                context.SaveChanges();
            }
        }

        public static List<SalaryScheme> GetSalarySchemes(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                return context.SalarySchemes.Where(i => i.DivisionId == divisionId).OrderBy(i => i.Name).ToList().Init();
            }
        }

        public static void PostSalaryScheme(Guid divisionId, SalaryScheme salaryScheme)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var div = context.Divisions.Single(i => i.Id == divisionId);

                if (salaryScheme.Late1Fine == 0 || salaryScheme.Late1Minutes == 0
                    || !salaryScheme.Late1Fine.HasValue || !salaryScheme.Late1Minutes.HasValue)
                {
                    salaryScheme.Late1Fine = null;
                    salaryScheme.Late1Minutes = null;
                }
                if (salaryScheme.Late2Fine == 0 || salaryScheme.Late2Minutes == 0
                    || !salaryScheme.Late2Fine.HasValue || !salaryScheme.Late2Minutes.HasValue)
                {
                    salaryScheme.Late2Fine = null;
                    salaryScheme.Late2Minutes = null;
                }

                if (salaryScheme.Id == Guid.Empty)
                {
                    var sch = new SalaryScheme
                    {
                        AuthorId = user.UserId,
                        CompanyId = div.CompanyId,
                        CreatedOn = DateTime.Now,
                        DivisionId = divisionId,
                        Id = Guid.NewGuid(),
                        IsOvertimePaid = salaryScheme.IsOvertimePaid,
                        Name = salaryScheme.Name,
                        Late1Fine = salaryScheme.Late1Fine,
                        Late1Minutes = salaryScheme.Late1Minutes,
                        Late2Fine = salaryScheme.Late2Fine,
                        Late2Minutes = salaryScheme.Late2Minutes
                    };
                    context.SalarySchemes.AddObject(sch);
                    foreach (var coeff in salaryScheme.SerializedSalarySchemeCoefficients)
                    {
                        var c = new SalarySchemeCoefficient
                        {
                            CoeffTypeId = coeff.CoeffTypeId,
                            CompanyId = div.CompanyId,
                            Guid1 = coeff.Guid1,
                            Id = Guid.NewGuid(),
                            Int1 = coeff.Int1,
                            Int2 = coeff.Int2,
                            Money1 = coeff.Money1,
                            SalarySchemeId = sch.Id,
                            TimeSpan1 = coeff.TimeSpan1,
                            TimeSpan2 = coeff.TimeSpan2
                        };
                        context.SalarySchemeCoefficients.AddObject(c);
                        foreach (var rt in coeff.SerializedRateTable)
                        {
                            var r = new SalaryRateTable
                            {
                                CompanyId = div.CompanyId,
                                FromValue = rt.FromValue,
                                Id = Guid.NewGuid(),
                                Result = rt.Result,
                                SalarySchemeCoefficientId = c.Id,
                                ToValue = rt.ToValue
                            };
                            context.SalaryRateTables.AddObject(r);
                        }
                    }
                }
                else
                {
                    var sc = context.SalarySchemes.Single(i => i.Id == salaryScheme.Id);
                    foreach (var i in sc.SalarySchemeCoefficients.ToList())
                    {
                        foreach (var j in i.SalaryRateTables.ToList())
                        {
                            context.DeleteObject(j);
                        }
                        context.DeleteObject(i);
                    }

                    context.Detach(sc);
                    context.SalarySchemes.Attach(salaryScheme);
                    context.ObjectStateManager.ChangeObjectState(salaryScheme, EntityState.Modified);
                    foreach (var coeff in salaryScheme.SerializedSalarySchemeCoefficients)
                    {
                        var c = new SalarySchemeCoefficient
                        {
                            CoeffTypeId = coeff.CoeffTypeId,
                            CompanyId = div.CompanyId,
                            Guid1 = coeff.Guid1,
                            Id = Guid.NewGuid(),
                            Int1 = coeff.Int1,
                            Int2 = coeff.Int2,
                            Money1 = coeff.Money1,
                            SalarySchemeId = salaryScheme.Id,
                            TimeSpan1 = coeff.TimeSpan1,
                            TimeSpan2 = coeff.TimeSpan2
                        };
                        context.SalarySchemeCoefficients.AddObject(c);
                        foreach (var rt in coeff.SerializedRateTable)
                        {
                            var r = new SalaryRateTable
                            {
                                CompanyId = div.CompanyId,
                                FromValue = rt.FromValue,
                                Id = Guid.NewGuid(),
                                Result = rt.Result,
                                SalarySchemeCoefficientId = c.Id,
                                ToValue = rt.ToValue
                            };
                            context.SalaryRateTables.AddObject(r);
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        public static decimal CalculateSalary(Guid employeeId, DateTime calcStart, out string log)
        {
            log = "";
            using (var context = new TonusEntities())
            {
                var calcEnd = calcStart.AddMonths(1).AddDays(-1);
                if (calcEnd > DateTime.Today) calcEnd = DateTime.Today;
                log += String.Format("Период расчета: с {0:d} по {1:d}\n", calcStart, calcEnd);
                var employee = context.Employees.SingleOrDefault(i => i.Id == employeeId);
                if (employee == null) return 0;
                var jps = new List<JobPlacement>();
                foreach (var jp in employee.JobPlacements.Where(i => i.IsAsset))
                {
                    if (Core.DatesIntersects(calcStart, calcEnd, jp.ApplyDate, jp.FireDate ?? DateTime.MaxValue))
                    {
                        jps.Add(jp);
                    }
                }
                var res = 0m;
                for (var n = 0; n <= (calcEnd - calcStart).TotalDays; n++)
                {

                    //Получаем подходящий плейсмент, считаем месячный оклад+премию
                    var jp = jps.FirstOrDefault(i => i.ContainsDate(calcStart.AddDays(n)));
                    if (jp == null)
                    {
                        //log += String.Format("Назначений не найдено\n");
                        continue;//потом можно ставить эн как первая аплайдейт
                    }
                    log += String.Format("Дата начала обсчета: {0:d}\n", calcStart.AddDays(n));
                    log += String.Format("Назначение: {0}, c {1:d} по {2:d}\n", jp.Job.Name, jp.ApplyDate, jp.EffectiveEndDate);

                    var monthSalary = CalculateSalaryForMonth(context, jp, calcStart, ref log);

                    var workTotalHrs = GetWorkHrs(context, jp.Job, calcStart);//число рабочих часов по данной ставке в месяце
                    log += String.Format("Всего рабочих часов по должности в месяце: {0:n2}\n", workTotalHrs);
                    if (workTotalHrs == 0) continue;
                    //выбираем, по какому периоду считаем:
                    var periodStart = calcStart.AddDays(n);
                    var periodEnd = jp.EffectiveEndDate > calcEnd ? calcEnd : jp.EffectiveEndDate;

                    //число рабочих часов, которые сотрудник реально отработал по данной схеме (сумма рабочих часов за период файр-1 минус аплай)
                    decimal fine;
                    var workedHrsForPeriod = GetWorkedHoursForEmployee(employee, periodStart, periodEnd, jp.Job, ref log, out fine);
                    log += String.Format("Всего рабочих часов отработано: {0:n2}\n", workedHrsForPeriod);

                    //прибавляем к суммарной зарплате икс/workTotalHrs от данной суммы
                    var add = monthSalary * (decimal)(workedHrsForPeriod / workTotalHrs);
                    res += add - fine;
                    log += String.Format("Итого начислено по назначению: {0:c}\n", add);


                    n += (int)(periodEnd - periodStart).TotalDays;
                }
                return res;
            }
        }

        private static double GetWorkedHoursForEmployee(Employee employee, DateTime periodStart, DateTime periodEnd, Job job, ref string log, out decimal fine)
        {
            fine = 0;
            periodEnd = periodEnd.AddDays(1);
            var list = employee.EmployeeVisits.Where(i => i.CreatedOn >= periodStart && i.CreatedOn < periodEnd).OrderBy(i => i.CreatedOn).ToList();
            if (list.Count < 2) return 0;
            var res = 0.0;
            var payAddition = job.SalarySchemeId.HasValue ? job.SalaryScheme.IsOvertimePaid : false;
            if (!list[0].IsIncome)
            {
                if (payAddition)
                {
                    res += (list[1].CreatedOn - periodStart).TotalHours;
                }
                else
                {
                    if (list.Count > 1)
                    {
                        var x = (list[1].CreatedOn - list[1].CreatedOn.Date - job.WorkStart).TotalHours;
                        if (x > 0)
                        {
                            res += x;
                        }
                    }
                }
            }
            bool isIn = false;
            var xDate = list.First(i => i.IsIncome).CreatedOn.Date;
            var xRes = 0.0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsIncome != isIn)
                {

                    //Первая запись за день: проверяем на штрафы
                    if (xRes == 0 && job.SalarySchemeId.HasValue && !isIn)
                    {
                        var delta = (list[i].CreatedOn - list[i].CreatedOn.Date - job.WorkStart).TotalMinutes;
                        if (job.SalaryScheme.Late2Minutes.HasValue && job.SalaryScheme.Late2Minutes.Value <= delta)
                        {
                            fine += job.SalaryScheme.Late2Fine ?? 0;
                            log += String.Format("Опоздание {0:d} на {1:n0} минут, депремирование {2:c}\n", list[i].CreatedOn, delta, job.SalaryScheme.Late2Fine);
                        }
                        else if (job.SalaryScheme.Late1Minutes.HasValue && job.SalaryScheme.Late1Minutes.Value <= delta)
                        {
                            fine += job.SalaryScheme.Late1Fine ?? 0;
                            log += String.Format("Опоздание {0:d} на {1:n0} минут, депремирование {2:c}\n", list[i].CreatedOn, delta, job.SalaryScheme.Late1Fine);
                        }
                    }
                    if (isIn)
                    {
                        xRes += (list[i].CreatedOn - list[i - 1].CreatedOn).TotalHours;
                    }

                    if (list[i].CreatedOn.Date != xDate)
                    {
                        xDate = list[i].CreatedOn.Date;
                        if (!payAddition && xRes > job.WorkDayLength) xRes = job.WorkDayLength;
                        res += xRes;
                        xRes = 0;
                    }

                    isIn = list[i].IsIncome;
                }
            }
            if (!payAddition && xRes > job.WorkDayLength) xRes = job.WorkDayLength;
            res += xRes;

            if (isIn)
            {
                res += (periodEnd - list[list.Count - 1].CreatedOn).TotalHours;
            }

            return res;
        }

        private static int GetWorkHrs(TonusEntities context, Job job, DateTime start)
        {
            if (job.WorkGraph == "5/2")
            {
                var res = 0;
                for (int i = 0; i < DateTime.DaysInMonth(start.Year, start.Month); i++)
                {
                    var d = start.AddDays(i);
                    if (d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday) continue;
                    if (context.Holidays.Any(j => j.Vacation == d)) continue;
                    res++;
                }
                return (int)(res * job.WorkDayLength);
            }
            if (!job.WorkGraph.Contains('/')) return 0;
            var wg = job.WorkGraph.Split('/');
            if (wg.Length != 2) return 0;
            var work = Int32.Parse(wg[0]);
            var hol = Int32.Parse(wg[1]);
            var coeff = (decimal)work / (decimal)(work + hol);
            var totDays = DateTime.DaysInMonth(start.Year, start.Month);
            return (int)((double)Math.Floor(totDays * coeff) * job.WorkDayLength);
        }

        private static decimal CalculateSalaryForMonth(TonusEntities context, JobPlacement jp, DateTime calcStart, ref string log)
        {
            var res = jp.Salary;
            log += String.Format("Оклад: {0:c}\n", jp.Salary);

            if (jp.Job.SalarySchemeId.HasValue)
            {
                log += String.Format("Схема премий: {0}\n", jp.Job.SalaryScheme.Name);

                foreach (var coef in jp.Job.SalaryScheme.SalarySchemeCoefficients)
                {
                    var value = 0m;
                    try
                    {
                        switch (coef.CoeffTypeId)
                        {
                            case 1:
                                value = SalaryCalculation.Get01TotalSalesPercent(context, jp.Job.DivisionId, calcStart, DateTime.MinValue);
                                break;
                            case 2:
                                value = SalaryCalculation.Get02GroupSales(context, jp.Job.DivisionId, calcStart, coef.Int1.Value);
                                break;
                            case 3:
                                value = SalaryCalculation.Get03NewTicketsAmount(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 4:
                                value = SalaryCalculation.Get04DicsountedTickets(context, jp.Job.DivisionId, calcStart, coef.Money1.Value);
                                break;
                            case 5:
                                value = SalaryCalculation.Get05RetryTicketsAmount(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 6:
                                value = SalaryCalculation.Get06TicketType(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 7:
                                value = SalaryCalculation.Get07CardType(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 8:
                                value = SalaryCalculation.Get08GoodGroup(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 9:
                                value = SalaryCalculation.Get09Good(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 10:
                                value = SalaryCalculation.Get10Action(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 11:
                                value = SalaryCalculation.Get11AfterGuest(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 12:
                                value = SalaryCalculation.Get12AfterVisit(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 13:
                                value = SalaryCalculation.Get13AvgSpent(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 14:
                                value = SalaryCalculation.Get14ClubVisit(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 15:
                                value = SalaryCalculation.Get15TreatmentTypeVisit(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 16:
                                value = SalaryCalculation.Get16TreatmentVisit(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 17:
                                value = SalaryCalculation.Get17ParameterizedVisit(context, jp.Job.DivisionId, calcStart, coef.Int1.Value, coef.TimeSpan1.Value, coef.TimeSpan2.Value);
                                break;
                            case 18:
                                value = SalaryCalculation.Get18CertByGroup(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 19:
                                value = SalaryCalculation.Get19NewCorporates(context, jp.Job.DivisionId, calcStart);
                                break;
                            case 20:
                                value = SalaryCalculation.Get20CategorySales(context, jp.Job.DivisionId, calcStart, coef.Int1.Value, coef.Guid1.Value);
                                break;
                            case 21:
                                value = SalaryCalculation.Get21CategorySalesNum(context, jp.Job.DivisionId, calcStart, coef.Int1.Value, coef.Guid1.Value);
                                break;
                            case 22:
                                value = SalaryCalculation.Get22AmTicketsFromCardType(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 23:
                                value = SalaryCalculation.Get23CostTicketsFromCardType(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 24:
                                value = SalaryCalculation.Get24GoodsFromCardType(context, jp.Job.DivisionId, calcStart, coef.Guid1.Value);
                                break;
                            case 25:
                                value = SalaryCalculation.Get25KPD(context, calcStart, jp.Employee);
                                break;
                            case 26:
                                value = GetSales(context, calcStart, jp.Employee.MainDivisionId);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        log += "Ошибка при расчете коэффициента\n";
                    }
                    var add = ResolveRateTable(coef.SalaryRateTables, value);
                    if (coef.CoeffTypeId == 26)
                    {
                        //Умножаем выручку на разрешенный процент
                        add = add / 100 * value;
                    }
                    log += String.Format("Коэффициент премии: {0}, значение: {1}, премия: {2:c}\n", coef.TypeText, value, add);
                    res += add;
                }
            }

            return res;
        }

        private static decimal GetSales(TonusEntities context, DateTime calcStart, Guid divisionId)
        {

            calcStart = new DateTime(calcStart.Year, calcStart.Month, 1);
            var calcEnd = calcStart.AddMonths(1);
            var ls = context.BarOrders.Where(i => i.PurchaseDate >= calcStart && i.PurchaseDate < calcEnd
                && i.DivisionId == divisionId);
            if (ls.Count() == 0) return 0;
            return ls.Sum(i => i.CardPayment + i.CashPayment + i.DepositPayment);
        }

        private static decimal ResolveRateTable(ICollection<SalaryRateTable> lines, decimal value)
        {
            foreach (var i in lines)
            {
                if (value > i.FromValue && ((i.ToValue.HasValue && value <= i.ToValue.Value) || (!i.ToValue.HasValue)))
                    return i.Result;
            }
            return 0;
        }

        public static List<SalarySheet> GetSalarySheets(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var x = context.SalarySheets.Where(i => i.DivisionId == divisionId).OrderByDescending(i => i.PeriodStart).GroupBy(i => i.PeriodStart);
                var res = new List<SalarySheet>();
                foreach (var i in x)
                {
                    res.Add(i.OrderByDescending(o => o.CreatedOn).First());
                }
                return res.Init();
            }
        }

        public static List<SalarySheetRow> GenerateSalarySheet(Guid divisionId, DateTime genMonth)
        {
            using (var context = new TonusEntities())
            {
                var res = new List<SalarySheetRow>();
                var div = context.Divisions.Single(i => i.Id == divisionId);
                var emps = context.JobPlacements.Where(i => i.Job.DivisionId == divisionId && ((!i.FireDate.HasValue)
                    || (i.FireDate.HasValue && i.FireDate > genMonth)))
                    .OrderBy(i => i.Job.Unit)
                    .ThenBy(i => i.Job.Name)
                    .ThenBy(i => i.Employee.BoundCustomer.LastName)
                    .ToList()
                    .Where(i => Core.DatesIntersects(genMonth, genMonth.AddMonths(1).AddDays(-1), i.ApplyDate, i.FireDate ?? DateTime.MaxValue))
                    .Select(i => i.Employee).Distinct().ToList();
                foreach (var e in emps)
                {
                    string log;
                    var salary = CalculateSalary(e.Id, genMonth, out log);
                    res.Add(new SalarySheetRow
                    {
                        CompanyId = div.CompanyId,
                        EmployeeId = e.Id,
                        Id = Guid.NewGuid(),
                        Salary = Math.Round(salary, 2),
                        Log = log,
                        SerializedEmployeeName = e.BoundCustomer.FullName
                    });
                }

                return res;
            }
        }

        public static string PostSalarySheet(Guid divisionId, DateTime periodStart, List<SalarySheetRow> lines)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var division = context.Divisions.Single(i => i.Id == divisionId);

                var old = context.SalarySheets.Where(i => i.DivisionId == divisionId && i.PeriodStart == periodStart).OrderByDescending(i => i.CreatedOn).FirstOrDefault();
                if (old != null)
                {
                    var x = old.EmployeePayments.GroupBy(i => i.Employee);
                    foreach (var i in x)
                    {
                        if (i.Count() == 0) continue;
                        var l = lines.FirstOrDefault(j => j.EmployeeId == i.Key.Id);
                        var am = 0m;
                        if (l != null) am = l.TotalToPay;
                        if (i.Sum(j => j.Amount) > am) return i.Key.BoundCustomer.FullName + " уже получил больше денег, чем указано в ведомости (" + i.Sum(j => j.Amount).ToString("c") + ")";
                    }
                }

                var sheet = new SalarySheet
                {
                    AuthorId = user.UserId,
                    CompanyId = division.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = divisionId,
                    Id = Guid.NewGuid(),
                    PeriodStart = periodStart
                };

                context.SalarySheets.AddObject(sheet);

                foreach (SalarySheetRow line in lines)
                {
                    var row = new SalarySheetRow
                    {
                        Bonus = line.Bonus,
                        CompanyId = division.CompanyId,
                        EmployeeId = line.EmployeeId,
                        Id = Guid.NewGuid(),
                        Log = line.Log,
                        NDFL = line.NDFL,
                        Salary = line.Salary,
                        SalarySheetId = sheet.Id,
                        Ved10 = line.Ved10,
                        Ved25 = line.Ved25
                    };
                    context.SalarySheetRows.AddObject(row);
                }
                if (old != null)
                {
                    foreach (var i in old.EmployeePayments.ToList())
                    {
                        i.SalarySheetId = sheet.Id;
                        i.PaymentType = 0;
                    }
                }

                context.SaveChanges();
                return String.Empty;
            }
        }

        public static List<SalarySheetRow> GetSalarySheetLines(Guid sheetId)
        {
            using (var context = new TonusEntities())
            {
                return context.SalarySheetRows.Where(i => i.SalarySheetId == sheetId).ToList().Init().OrderBy(i => i.SerializedEmployeeName).ToList();
            }
        }

        /// <summary>
        /// PaymentType = 0 - advance
        /// PaymentType = 1 - finalCount
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="sheetId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string PostEmployeePayment(Guid employeeId, Guid sheetId, decimal amount)
        {
            using (var context = new TonusEntities())
            {
                if (amount <= 0) return "Нулевая выплата невозможна!";
                var user = UserManagement.GetUser(context);

                var spType = context.SpendingTypes.SingleOrDefault(i => i.Name == "Выплаты сотрудникам" && i.CompanyId == user.CompanyId);
                if (spType == null)
                {
                    spType = new SpendingType
                    {
                        CompanyId = user.CompanyId,
                        Id = Guid.Empty,
                        IsCommon = true,
                        Name = "Выплаты сотрудникам"
                    };

                    context.SpendingTypes.AddObject(spType);
                }
                var employee = context.Employees.Single(i => i.Id == employeeId);
                var sheet = context.SalarySheets.SingleOrDefault(i => i.Id == sheetId);
                var n = context.Spendings.Where(i => i.DivisionId == employee.MainDivisionId).Max(i => i.Number, 0) + 1;
                if (sheet == null) return "Ведомость не найдена!";
                var spending = new Spending
                    {
                        Amount = amount,
                        AuthorId = user.UserId,
                        CompanyId = employee.CompanyId,
                        CreatedOn = DateTime.Now,
                        DivisionId = employee.MainDivisionId,
                        Id = Guid.NewGuid(),
                        Name = String.Format("Выплата для {0} по ведомости за {1:MMM yyyy}", employee.BoundCustomer.FullName, sheet.PeriodStart),
                        Number = n,
                        PaymentType = "Выплаты сотрудникам (нал)",
                        SpendingTypeId = spType.Id
                    };
                context.Spendings.AddObject(spending);

                var payed = 0m;
                var x = context.EmployeePayments.Where(i => i.EmployeeId == employeeId && i.SalarySheetId == sheetId);
                if (x.Count() > 0)
                {
                    payed = x.Sum(i => i.Amount);
                }

                var sheetRow = employee.SalarySheetRows.SingleOrDefault(i => i.SalarySheetId == sheetId);
                sheetRow.Init();

                short ptype = 0;

                if (Math.Round(sheetRow.TotalToPay, 2) < amount) return "Нельзя выплатить больше, чем указано в ведомости!";
                if (Math.Round(sheetRow.TotalToPay, 2) == amount) ptype = 1;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                var payment = new EmployeePayment
                {
                    Amount = amount,
                    CompanyId = employee.CompanyId,
                    EmployeeId = employeeId,
                    Id = Guid.NewGuid(),
                    PaymentType = ptype,
                    SalarySheetId = sheetId,
                    SpendingId = spending.Id,
                    Period = sheet.PeriodStart,
                    CreatedOn = DateTime.Now
                };
                context.EmployeePayments.AddObject(payment);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return string.Empty;
            }
        }

        public static bool CheckSalarySheet(Guid divisionId, DateTime genDate)
        {
            using (var context = new TonusEntities())
            {
                return context.SalarySheets.Any(i => i.PeriodStart == genDate && i.DivisionId == divisionId);
            }
        }

        public static KeyValuePair<decimal, string> GetFireSalary(Guid employeeId, DateTime fireDate)
        {
            using (var context = new TonusEntities())
            {
                string log;
                var salary = CalculateSalary(employeeId, fireDate.Date.AddDays(-fireDate.Day + 1), out log);
                log = "Калькуляция за текущий месяц\n" + log;
                //unpaid previous + total advances for current months
                var employee = context.Employees.Single(i => i.Id == employeeId);
                var unpayedPrev = 0m;
                var e = GetActualRowForEmployee(employee, fireDate.Date.AddDays(-fireDate.Day + 1).AddMonths(-1));//prevSheet.SalarySheetRows.FirstOrDefault(i => i.EmployeeId == employeeId);
                if (e != null)
                {
                    unpayedPrev = e.TotalToPay;
                }
                salary += unpayedPrev;
                log += String.Format("Не выплачено за предыдущий месяц: {0:c}\n", unpayedPrev);
                var x = employee.EmployeePayments.Where(i => i.Period == fireDate.Date.AddDays(-fireDate.Day + 1));
                if (x.Count() > 0)
                {
                    var advance = x.Sum(i => i.Amount);
                    log += String.Format("Аванс за текущий месяц: {0:c}\n", advance);
                    salary -= advance;
                }
                //vacation compensation
                var firstApplyDate = employee.JobPlacements.Min(i => i.ApplyDate);
                var stage = (decimal)Math.Round((fireDate - firstApplyDate).TotalDays / 30);
                log += String.Format("Отпускной стаж: {0} мес\n", stage);
                var processedHolidays = 0;
                if (employee.EmployeeVacations.Count > 0)
                {
                    processedHolidays = (int)employee.EmployeeVacations.Sum(i => (i.EndDate - i.BeginDate).TotalDays + 1);
                }
                log += String.Format("Отгуляно дней: {0}\n", processedHolidays);

                //Medium salary for a year etc
                var mss = 0m;
                var msc = 0;
                for (int i = 0; i < 12; i++)
                {
                    var p = GetActualRowForEmployee(employee, fireDate.Date.AddDays(-fireDate.Day + 1).AddMonths(-i - 1));
                    if (p != null)
                    {
                        mss += p.SalaryTotal;
                        msc++;
                    }
                }
                log += String.Format("Всего месяцев отработано за последние 12 месяцев: {0}\n", msc);
                var mediumSalary = 0m;
                if (msc != 0)
                {
                    mediumSalary = mss / msc;
                }
                log += String.Format("Средняя зарплата за последние 12 месяцев (больничные и отпускные не учитываются!): {0:c}\n", mediumSalary);

                var compensation = (stage * 2.33m - processedHolidays) * (mediumSalary / 29.4m);

                salary += compensation;
                log += String.Format("Компенсация неотгулянного отпуска: {0:c}\n", compensation);

                return new KeyValuePair<decimal, string>(salary, log);
            }
        }

        private static SalarySheetRow GetActualRowForEmployee(Employee employee, DateTime period)
        {
            var res = employee.SalarySheetRows.Where(i => i.SalarySheet.PeriodStart == period).OrderByDescending(i => i.SalarySheet.CreatedOn).FirstOrDefault();
            if (res != null)
            {
                res.Init();
            }
            return res;
        }

        public static KeyValuePair<decimal, string> GetEmployeeVacationPmt(Guid employeeId, DateTime beginDate, DateTime endDate)
        {
            using (var context = new TonusEntities())
            {
                var employee = context.Employees.Single(i => i.Id == employeeId);
                //Medium salary for a year
                var mss = 0m;
                var msc = 0;
                for (int i = 0; i < 12; i++)
                {
                    var p = GetActualRowForEmployee(employee, beginDate.Date.AddDays(-beginDate.Day + 1).AddMonths(-i - 1));
                    if (p != null)
                    {
                        mss += p.SalaryTotal;
                        msc++;
                    }
                }
                var log = String.Format("Всего месяцев отработано за последние 12 месяцев: {0}\n", msc);
                var mediumSalary = 0m;
                if (msc != 0)
                {
                    mediumSalary = mss / msc;
                }
                log += String.Format("Средняя зарплата за последние 12 месяцев (больничные и отпускные не учитываются!): {0:c}\n", mediumSalary);

                var salary = ((int)(endDate - beginDate).TotalDays + 1) * (mediumSalary / 29.4m);

                log += String.Format("Итого отпускных: {0:c}\n", salary);

                return new KeyValuePair<decimal, string>(salary, log);
            }
        }

        public static KeyValuePair<decimal, string> GetEmployeeIllnessPmt(Guid employeeId, DateTime beginDate, DateTime endDate)
        {
            using (var context = new TonusEntities())
            {
                var employee = context.Employees.Single(i => i.Id == employeeId);
                //Medium salary for a year
                var mss = 0m;
                var msc = 0;
                for (int i = 0; i < 24; i++)
                {
                    var p = GetActualRowForEmployee(employee, beginDate.Date.AddDays(-beginDate.Day + 1).AddMonths(-i - 1));
                    if (p != null)
                    {
                        mss += p.SalaryTotal;
                        msc++;
                    }
                }
                var log = String.Format("Всего месяцев отработано за последние 24 месяца: {0}\n", msc);
                var mediumSalary = 0m;
                if (msc != 0)
                {
                    mediumSalary = mss / msc;
                }
                log += String.Format("Средняя зарплата за последние 24 месяца (больничные и отпускные не учитываются!): {0:c}\n", mediumSalary);

                var salary = ((int)(endDate - beginDate).TotalDays + 1) * (mediumSalary / 29.4m);

                var seniority = employee.JobPlacements.Max(i => i.Seniority);
                var firstapply = employee.JobPlacements.OrderBy(i => i.ApplyDate).First();
                var totalSeniority = Math.Floor(((beginDate - firstapply.ApplyDate).TotalDays / 365 + ((double)seniority) / 12));
                log += String.Format("Общий стаж, полных лет: {0:n0}\n", totalSeniority);

                log += String.Format("Больничные без учета стажа: {0:c}\n", salary);

                if (totalSeniority < 5) salary *= 0.5m;
                else if (totalSeniority < 9) salary *= 0.8m;


                log += String.Format("Итого больничные: {0:c}\n", salary);

                return new KeyValuePair<decimal, string>(salary, log);
            }
        }

        public static List<EmployeePayment> GetDivisionEmployeeCashflow(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var res = context.EmployeePayments.Where(i => i.Employee.MainDivisionId == divisionId).ToList().Init();
                res.ForEach(i => i.SerializedPaidName = context.Spendings.Where(j => j.Id == i.SpendingId).Select(j => j.CreatedBy.FullName).FirstOrDefault());
                return res;
            }
        }

        public static void PostSalesPlan(Guid divisionId, DateTime month, decimal amount, decimal amountCorp, DateTime oldMonth)
        {
            using (var context = new TonusEntities())
            {
                month = month.AddDays(-month.Day + 1);
                oldMonth = oldMonth.AddDays(-oldMonth.Day + 1);

                var plan = context.SalesPlans.FirstOrDefault(i => i.DivisionId == divisionId && i.Month == oldMonth);
                if (plan == null)
                {
                    var div = context.Divisions.Single(i => i.Id == divisionId);
                    plan = new SalesPlan
                    {
                        CompanyId = div.CompanyId,
                        DivisionId = divisionId,
                        Id = Guid.NewGuid(),
                        Month = month,
                        Value = amount,
                        CorpValue = amountCorp
                    };
                    context.SalesPlans.AddObject(plan);
                }
                else
                {
                    plan.Value = amount;
                    plan.CorpValue = amountCorp;
                    plan.Month = month;
                }
                context.SaveChanges();
            }
        }

        public static List<SalesPlan> GetSalesPlanForDivsion(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                return context.SalesPlans.Where(i => i.DivisionId == divisionId).OrderByDescending(i => i.Month).ToList();
            }
        }

        public static List<Employee> GetEmployeesWorkingAt(Guid divisionId, DateTime date)
        {
            using (var context = new TonusEntities())
            {
                var wg = context.EmployeeWorkGraphs.Where(i => i.Begin <= date && i.End >= date && i.DivisionId == divisionId).OrderByDescending(i => i.CreatedOn).FirstOrDefault();
                if (wg == null) return new List<Employee>();
                var schedule = DeserializeWorkGraph(wg.SerializedData);
                var res = new List<Employee>();
                foreach (var eId in schedule.Where(i => i.Value.Contains(date)).Select(i => i.Key))
                {
                    var e = context.JobPlacements.SingleOrDefault(i => i.Id == eId);
                    if (e != null)
                    {
                        e.Employee.Init();
                        res.Add(e.Employee);
                    }
                }
                return res;
            }
        }

        public static void PostEmployeeActive(Guid employeeId, bool active)
        {
            using (var context = new TonusEntities())
            {
                var emp = context.Employees.SingleOrDefault(i=>i.Id==employeeId);
                if (emp == null) return;
                emp.IsActive = active;
                context.SaveChanges();
            }
        }
    }

    public class SerPair
    {
        [DataMember]
        public Guid Key { get; set; }
        [DataMember]
        public List<DateTime> Value { get; set; }
    }
}
