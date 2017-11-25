using System;
using System.Linq;
using TonusClub.Entities;
using System.Data;
using TonusClub.ServiceModel;
using System.Reflection;
using System.ServiceModel;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace TonusClub.ServerDataModule.UserManagement
{
    public static class UserManagement
    {
        public static User GetUserByName(string userName)
        {
            using (var context = new TonusEntities/*.GetInstance*/())
            {
                var user = context.Users.FirstOrDefault(u => string.Equals(u.UserName, userName));
                if (user == null) return null;
                //context.Detach(user);
                return user;
            }
        }

        public static User GetUser(TonusEntities context)
        {
            var user = context.Users.SingleOrDefault(u => u.UserName.ToLower() == OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }
            return user;
        }

        public static User GetUser()
        {
            using (var context = new TonusEntities())
            {
                return GetUser(context);
            }
        }

        public static bool HasPermission(User user, string permissionKey)
        {
            return user.Roles.SelectMany(i => i.Permissions).Any(i => i.PermissionKey == permissionKey);
        }

        public static List<Employee> GetEmployeesWithPermission(TonusEntities context, Guid? companyId, Guid? divsionId, string permissionKey)
        {
            var res = new List<Employee>();
            var users = context.Users.Where(i => i.EmployeeId.HasValue && (i.CompanyId == companyId || !companyId.HasValue)).ToList().Where(u => UserManagement.HasPermission(u, permissionKey));
            foreach (var u in users)
            {
                var employee = context.Employees.SingleOrDefault(e => e.Id == u.EmployeeId.Value && (e.MainDivisionId==divsionId || !divsionId.HasValue));
                if (employee != null)
                {
                    res.Add(employee);
                }
            }
            return res;
        }

        public static string CalculateSHA1(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

            return hash;
        }
    }
}
