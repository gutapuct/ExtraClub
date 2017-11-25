using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace TonusClub.CustomerPortal.Business
{
    public static class AccountBL
    {
        public static Customer Register(TonusEntities context, string phone, string firstname, string lastname, Guid divisionId)
        {
                if (context.Customers.Any(i => i.Phone2 == phone))
                {
                    throw new Exception("Клиент с указанным телефоном уже зарегистрирован в системе!");
                }
                var div = context.Divisions.FirstOrDefault(i => i.Id == divisionId);
                if (div == null)
                {
                    throw new Exception("Укажите, пожалуйста, клуб!");
                }
                var cust = new Customer
                {
                    Id = Guid.NewGuid(),
                    CompanyId = div.CompanyId,
                    ClubId = div.Id,
                    Phone2 = phone,
                    FirstName = firstname,
                    LastName = lastname,
                    CreatedOn = DateTime.Now,
                    AuthorId = context.Users.First(i => i.CompanyId == div.CompanyId).UserId
                };
                context.Customers.AddObject(cust);
                context.SaveChanges();
                return cust;
        }

        public static string GetCurrent()
        {
            var uid = Guid.Parse(HttpContext.Current.User.Identity.Name);
            using (var context = new TonusEntities())
            {
                return context.Customers.Where(i => i.Id == uid).Select(i => i.FirstName + " " + i.LastName).Single();
            }
        }

        public static object GetCurrentDivisionName()
        {
            var uid = Guid.Parse(HttpContext.Current.User.Identity.Name);
            using (var context = new TonusEntities())
            {
                var clubId = context.Customers.Where(i => i.Id == uid).Select(i => i.ClubId).Single();
                if (clubId.HasValue)
                {
                    return context.Divisions.Where(i => i.Id == clubId).Select(i => i.Name).Single();
                }
                else return "";
            }
        }
    }
}