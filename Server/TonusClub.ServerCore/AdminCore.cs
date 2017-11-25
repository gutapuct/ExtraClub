using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;
using TonusClub.Entities;

using System.Security.Cryptography;
using System.ServiceModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace TonusClub.ServerCore
{
    public static class AdminCore
    {
        public static List<Role> GetRoles()
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = context.Roles.Where(i => i.CompanyId == user.CompanyId).OrderBy(i => i.RoleName).ToList().Init();
                var userperms = user.Roles.SelectMany(r => r.Permissions).ToArray();
                res.ToList().ForEach(r =>
                {
                    foreach (var p in r.Permissions)
                    {
                        if (!userperms.Contains(p))
                        {
                            res.Remove(r);
                            break;
                        }
                    }
                });
                res.ForEach(i => {
                    if (i.CreatedBy.HasValue)
                    {
                        i.CreatedByName = context.Users.Single(j => j.UserId == i.CreatedBy.Value).FullName;
                    }
                    if (i.ModifiedBy.HasValue)
                    {
                        i.ModifiedByName = context.Users.Single(j => j.UserId == i.ModifiedBy.Value).FullName;
                    }
                });
                return res;
            }
        }

        public static List<Permission> GetAllPermissions()
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                return user.Roles.SelectMany(r => r.Permissions).Distinct().OrderBy(i => i.PermissionName).ToList();
            }
        }

        public static void PostRole(Guid roleId, string name, Guid[] permissionIds, string cardDisc, string ticketDisc, string ticketRubDisc, Guid? folderId)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var role = context.Roles.SingleOrDefault(i => i.RoleId == roleId);
                if (role == null)
                {
                    role = new Role
                    {
                        CompanyId = user.CompanyId,
                        RoleId = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        CreatedBy = user.UserId
                    };
                    context.Roles.AddObject(role);
                }
                else
                {
                    role.ModifiedOn = DateTime.Now;
                    role.ModifiedBy = user.UserId;
                }
                role.RoleName = name.Trim();
                role.CardDiscs = cardDisc;
                role.TicketDiscs = ticketDisc;
                role.Permissions.Clear();
                role.SettingsFolderId = folderId;
                role.TicketRubDiscs = ticketRubDisc;
                foreach (var i in permissionIds)
                {
                    var p = context.Permissions.SingleOrDefault(o => o.PermissionId == i);
                    if (p != null)
                    {
                        role.Permissions.Add(p);
                    }
                }
                context.SaveChanges();
            }
        }

        public static void DeleteRole(Guid roleId)
        {
            using (var context = new TonusEntities())
            {
                var role = context.Roles.SingleOrDefault(i => i.RoleId == roleId);
                if (role == null) return;
                role.Permissions.Clear();
                role.Users.Clear();
                context.DeleteObject(role);
                context.SaveChanges();
            }
        }

        public static List<User> GetUsers()
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                return context.Users
                    .Where(i => i.CompanyId == user.CompanyId && i.UserId != Guid.Empty).OrderBy(i => i.FullName)
                    .ToList().Init();
            }
        }

        public static Guid PostNewUser(Guid employeeId, string userName, string fullName, string password, string email)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var emp = context.Employees.SingleOrDefault(i => i.Id == employeeId);

                userName = user.Company.UserPrefix.ToLower().Trim() + userName.ToLower().Trim();

                if (context.Users.Any(i => i.UserName == userName))
                {
                    throw new FaultException<string>("Пользователь с таким именем уже есть в системе!", "Пользователь с таким именем уже есть в системе!");
                }

                var newUser = new User
                {
                    CompanyId = emp.CompanyId,
                    CreatedOn = DateTime.Now,
                    EmployeeId = emp.Id,
                    FullName = fullName,
                    IsActive = true,
                    LastLoginDate = DateTime.Now,
                    PasswordHash = CalculateSHA1(password),
                    UserId = Guid.NewGuid(),
                    UserName = userName,
                    Email = email
                };

                context.Users.AddObject(newUser);
                context.SaveChanges();
                return newUser.UserId;
            }
        }

        public static string CalculateSHA1(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

            return hash;
        }

        public static void PostUser(Guid userId, string fullName, bool isActive, Guid[] roleIds, string email)
        {
            using (var context = new TonusEntities())
            {
                var user = context.Users.SingleOrDefault(i => i.UserId == userId);
                if (user == null) return;
                user.FullName = fullName.Trim();
                user.IsActive = isActive;
                user.Roles.Clear();
                user.Email = email;
                foreach (var rId in roleIds)
                {
                    var role = context.Roles.SingleOrDefault(i => i.RoleId == rId);
                    if (role != null) user.Roles.Add(role);
                }
                context.SaveChanges();
            }
        }

        public static void ResetPassword(Guid userId, string password)
        {
            using (var context = new TonusEntities())
            {
                var user = context.Users.SingleOrDefault(i => i.UserId == userId);
                if (user == null) return;
                user.PasswordHash = CalculateSHA1(password);
                user.LastPasswordChanged = DateTime.Now;
                context.SaveChanges();
            }
        }

        public static string ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            using (var context = new TonusEntities())
            {
                var user = context.Users.SingleOrDefault(i => i.UserId == userId);
                var op = CalculateSHA1(oldPassword);
                var np = CalculateSHA1(newPassword);
                if (user.PasswordHash != op) return Localization.Resources.OldPassErr;
                if (op == np) return Localization.Resources.OldNewPassEqs;
                user.PasswordHash = np;
                user.LastPasswordChanged = DateTime.Now;
                context.SaveChanges();
                return String.Empty;
            }
        }

        public static List<SettingsFolder> GetSettingsFolders(int categoryId, bool companyOnly)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                return context.SettingsFolders.Where(i => i.CategoryId == categoryId).ToList().Where(i => (!companyOnly) || user.Company.AvailSettingsFolders.Any(j => j.Id == i.Id)).ToList();
            }
        }

        public static List<CompanySettingsFolder> GetCompanySettingsFolders(int categoryId, Guid divId)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                return context.CompanySettingsFolders.Where(i => i.CategoryId == categoryId && i.CompanyId == user.CompanyId && (!i.DivisionId.HasValue || i.DivisionId.Value == divId)).ToList();
            }
        }

        public static List<CompanyView> GetCompaniesListForFolder(Guid folderId)
        {
            using (var context = new TonusEntities())
            {
                return context.Companies
                    .OrderBy(i => i.CompanyName)
                    .Select(i => new CompanyView
                    {
                        Id = i.CompanyId,
                        Name = i.CompanyName,
                        Helper = i.AvailSettingsFolders.Any(j => j.Id == folderId)
                    })
                    .ToList();
            }
        }

        public static void PostPermissions(string[] auths)
        {
            using (var context = new TonusEntities())
            {
                foreach (var str in auths)
                {
                    if (!context.Permissions.Any(i => i.PermissionKey == str))
                    {
                        var perm = new Permission
                        {
                            CreatedOn = DateTime.Now,
                            PermissionId = Guid.NewGuid(),
                            PermissionKey = str,
                            PermissionName = str
                        };
                        context.Permissions.AddObject(perm);
                    }
                }
                context.SaveChanges();
            }
        }

        public static List<Company> GetCompanies()
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                if (!user.Roles.Any(r => r.Permissions.Any(p => p.PermissionKey == "FranchTab"))) return new List<Company> { user.Company };
                return context.Companies.OrderBy(i => i.CompanyName).ToList();
            }
        }

        public static void PostNewCompany(string name, string login, string password, Guid roleId, string reportEmail, int utcCorr, string userPrefix)
        {
            using (var context = new TonusEntities())
            {
                login = login.Trim().ToLower();
                if (context.Users.Any(i => i.UserName.ToLower() == login))
                {
                    throw new FaultException<string>("Пользователь с таким именем уже существует!", "Пользователь с таким именем уже существует!");
                }
                userPrefix=userPrefix.Trim().ToLower();
                if (context.Companies.Any(i => i.UserPrefix.ToLower().Trim() == userPrefix))
                {
                    throw new FaultException<string>("Компания с таким префиксом уже существует!", "Компания с таким префиксом уже существует!");
                }
                var company = new Company
                {
                    CompanyId = Guid.NewGuid(),
                    CompanyName = name,
                    ReportEmail = reportEmail,
                    UtcCorr = (short)utcCorr,
                    UserPrefix = userPrefix
                };
                context.Companies.AddObject(company);

                var user = new User
                {
                    UserName = userPrefix.ToLower().Trim() + login.ToLower().Trim(),
                    PasswordHash = UserManagement.CalculateSHA1(password),
                    UserId = Guid.NewGuid(),
                    CompanyId = company.CompanyId,
                    LastLoginDate = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };

                context.Users.AddObject(user);
                var mainRole = context.Roles.Single(r => r.RoleId == roleId);
                

                context.Roles.Where(i => i.IsFixed || i.RoleId == roleId).ToList().ForEach(r =>
                {
                    var role = Clone<Role>(r);
                    role.RoleId = Guid.NewGuid();
                    role.CompanyId = company.CompanyId;
                    role.IsFixed = false;
                    role.CreatedBy = null;
                    role.CreatedOn = DateTime.Now;
                    role.ModifiedBy = null;
                    r.Permissions.Where(p => mainRole.Permissions.Any(x => x == p)).ToList().ForEach(p => role.Permissions.Add(p));
                    context.Roles.AddObject(role);

                    if (r.RoleId == roleId)
                    {
                        user.Roles.Add(role);
                    }
                });

                context.Customers.AddObject(new Customer
                {
                    AuthorId = user.UserId,
                    CompanyId = company.CompanyId,
                    CreatedOn = DateTime.Now,
                    SmsList = false,
                    Birthday = null,
                    LastName = Localization.Resources.Guest,
                    Gender = false,
                    IsActive = false,
                    IsEmployee = false,
                    NoContraIndications = true,
                    Id = Guid.NewGuid()
                });

                context.TreatmentPrograms.Where(i => i.IsFixed).ToList().ForEach(tp =>
                {
                    var program = Clone<TreatmentProgram>(tp);
                    program.IsFixed = false;
                    program.CompanyId = company.CompanyId;
                    program.AuthorId = user.UserId;
                    program.Id = Guid.NewGuid();
                    program.AuthorId = user.UserId;
                    context.TreatmentPrograms.AddObject(program);
                    tp.TreatmentProgramLines.ToList().ForEach(pl =>
                    {
                        var line = Clone<TreatmentProgramLine>(pl);
                        line.CompanyId = company.CompanyId;
                        line.Id = Guid.NewGuid();
                        line.TreatmentProgramId = program.Id;
                        context.TreatmentProgramLines.AddObject(line);
                    });
                });

                context.CustomReports.Where(i => i.IsFixed && i.CompanyId == GeneralCompanyId).ToList().ForEach(cr =>
                {
                    var rep = new CustomReport
                    {
                        Comments = cr.Comments,
                        CompanyId = company.CompanyId,
                        CreatedBy = user.UserId,
                        CustomFields = cr.CustomFields,
                        Id = Guid.NewGuid(),
                        IsFixed = true,
                        Name = cr.Name,
                        BaseTypeName = cr.BaseTypeName,
                        XmlClause = cr.XmlClause
                    };
                    context.CustomReports.AddObject(rep);
                });

                context.ReportTemplates.Where(i => !i.CompanyId.HasValue).ToList().ForEach(ot =>
                {
                    context.ReportTemplates.AddObject(new ReportTemplate
                    {
                        CompanyId=company.CompanyId,
                        Description=ot.Description,
                        DisplayName=ot.DisplayName,
                        DisplayNameEn=ot.DisplayNameEn,
                        HtmlText=ot.HtmlText,
                        HtmlTextEn=ot.HtmlTextEn,
                        Id=Guid.NewGuid(),
                        Name = ot.Name
                    });
                });

                context.SpendingTypes.AddObject(new SpendingType { CompanyId = company.CompanyId, Id = Guid.NewGuid(), IsCommon = true, Name = "Затраты на товар" });
                context.SpendingTypes.AddObject(new SpendingType { CompanyId = company.CompanyId, Id = Guid.NewGuid(), IsCommon = true, Name = "Возврат" });
                context.SpendingTypes.AddObject(new SpendingType { CompanyId = company.CompanyId, Id = Guid.NewGuid(), IsCommon = true, Name = "Выплаты сотрудникам" });
                context.SaveChanges();
            }
        }

        public static T Clone<T>(object obj)
            where T : class
        {
            if (obj == null) return null;
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            T newObject = (T)dcSer.ReadObject(memoryStream);
            return newObject;
        }

        public static void PostNewDivision(Division division)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                division.AuthorId = user.UserId;
                division.CompanyId = user.CompanyId;
                context.Divisions.AddObject(division);
                context.SaveChanges();
            }
        }

        public static Guid GeneralCompanyId = Guid.Parse("66202163-0299-47B3-B89F-8DCD9CCD44C0");

        public static List<AdvertGroup> GetAdvertGroups()
        {
            using (var context = new TonusEntities())
            {
                var res = context.AdvertGroups.Where(i => i.IsActive).OrderBy(i => i.Name).ToList();

                if (Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    res.ForEach(i => i.Name = i.NameEn);
                }

                return res;
            }
        }

        public static void PostAdvertGroup(Guid groupId, string name)
        {
            using (var context = new TonusEntities())
            {
                var group = context.AdvertGroups.FirstOrDefault(i => i.IsActive && (i.Name.ToLower() == name.ToLower().Trim() || i.NameEn.ToLower() == name.ToLower().Trim()));
                if (group != null && group.Id != groupId)
                {
                    throw new FaultException<string>("Группа с указанным именем уже существует", "Группа с указанным именем уже существует");
                }
                group = context.AdvertGroups.SingleOrDefault(i => i.Id == groupId);
                if (group == null)
                {
                    group = new AdvertGroup { Id = groupId, IsActive = true, Name = name, NameEn = name };
                    context.AdvertGroups.AddObject(group);
                    context.SaveChanges();
                }
                else
                {
                    if (Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                    {
                        group.NameEn = name.Trim();
                    }
                    else
                    {
                        group.Name = name.Trim();
                    }
                    context.SaveChanges();
                }
            }
        }

        public static void DeleteAdvertGroup(Guid groupId)
        {
            using (var context = new TonusEntities())
            {
                if (context.AdvertGroups.Count() == 1) return;
                var group = context.AdvertGroups.SingleOrDefault(i => i.Id == groupId);
                group.IsActive = false;
                group.AdvertTypes.ToList().ForEach(i => i.IsAvail = false);
                context.SaveChanges();
            }
        }

        public static void PostAdvertType(Guid typeId, string name, bool commentNeeded, Guid groupId)
        {
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var at = context.AdvertTypes.FirstOrDefault(i => i.IsAvail && i.Name.ToLower() == name.ToLower().Trim() && i.CompanyId == user.CompanyId);
                if (at != null && at.Id != typeId)
                {
                    throw new FaultException<string>("Канал с указанным именем уже существует", "Канал с указанным именем уже существует");
                }
                at = context.AdvertTypes.SingleOrDefault(i => i.Id == typeId);
                if (at == null)
                {
                    at = new AdvertType { Id = typeId, IsAvail = true, CompanyId = user.CompanyId };
                    context.AdvertTypes.AddObject(at);
                }
                at.Name = name.Trim();
                at.CommentNeeded = commentNeeded;
                at.AdvertGroupId = groupId;
                context.SaveChanges();
            }
        }

        public static void DeleteAdvertType(Guid typeId)
        {
            using (var context = new TonusEntities())
            {
                var at = context.AdvertTypes.FirstOrDefault(i => i.Id == typeId);
                if (at != null)
                {
                    at.IsAvail = false;
                    context.SaveChanges();
                }
            }
        }

        public static void SpreadPermission(Guid permisionId, bool Maximum, bool Franch, bool Upravl, bool Admins)
        {
            return;
            using (var context = new TonusEntities())
            {
                if (context.LocalSettings.Any()) throw new FaultException<string>("Операцию можно выполнить только на ЦС!", "Операцию можно выполнить только на ЦС!");
                var perm = context.Permissions.Single(i => i.PermissionId == permisionId);
                var roles = context.Roles.Include("Permissions").Where(i =>
                    (i.RoleName.ToLower().Contains("максимальн") && Maximum) ||
                    (i.RoleName.ToLower().Contains("франчайзи") && Franch) ||
                    (i.RoleName.ToLower().Contains("управляющ") && Upravl) ||
                    (i.RoleName.ToLower().Contains("администрат") && Admins))
                    .Where(i => !i.Permissions.Any(j => j.PermissionId == permisionId)).ToArray();
                foreach (var r in roles)
                {
                    r.Permissions.Add(perm);
                }
                context.SaveChanges();
            }
        }
    }
}
