using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;

namespace ExtraClub.WorkflowService
{
    public sealed class SiteCustomersImport
    {
        private readonly ExtraEntities _context;

        public SiteCustomersImport(ExtraEntities context)
        {
            _context = context;
        }

        public void Execute()
        {
            return;
            var candidates = _context.wf_GetSiteCustomers().ToArray();
            var companies = candidates.Select(i=>i.CompanyId).Distinct().ToArray();
            var userIds = _context.Users
                .Where(i => companies.Contains(i.CompanyId))
                .GroupBy(i => i.CompanyId)
                .Select(i => new { i.Key, i.FirstOrDefault().UserId }).ToDictionary(i => i.Key, i => i.UserId);
            foreach(var candidate in candidates)
            {
                var customer = new Customer
                {
                    AuthorId = userIds[candidate.CompanyId],
                    ClubId=candidate.ClubId,
                    Comments = "Клиент с сайта www.ExtraClub.ru",
                    CompanyId=candidate.CompanyId,
                    CreatedOn = DateTime.Today,
                    Email = candidate.Email,
                    FirstName = candidate.Name,
                    FromSite = true,
                    Id = Guid.NewGuid(),
                    LastName="",
                    MiddleName="",
                    Phone2 = candidate.Phone,
                    SmsList=true
                };
                var notification = new CustomerNotification
                {
                    AuthorId = userIds[candidate.CompanyId],
                    CompanyId=candidate.CompanyId,
                    CreatedOn=DateTime.Today,
                    CustomerId=customer.Id,
                    ExpiryDate=DateTime.Today,
                    Id=Guid.NewGuid(),
                    Message="Необходимо проконсультировать клиента, воспользовавшегося калькулятором стройности на сайте",
                    Priority=0,
                    Subject="Калькулятор стройности"
                };
                _context.Customers.AddObject(customer);
                _context.CustomerNotifications.AddObject(notification);
            }
            _context.SaveChanges();
        }
    }
}
