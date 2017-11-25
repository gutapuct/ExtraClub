using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TonusClub.Entities;

namespace Sync.Models
{
    public class DivisionRatingModel
    {
        private List<DataForDivisionRating> _data;
        private DateTime _dateFrom, _dateTo;
        public DivisionRatingModel()
        {
            _data = new List<DataForDivisionRating>();
            _dateFrom = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).AddMonths(-1);
            _dateTo = _dateFrom.AddMonths(1).AddSeconds(-1);
        }

        public List<DataForDivisionRating> GetData()
        {
            using (var context = new TonusEntities())
            {
                var divisionIds = context.Divisions.Select(i => i.Id).ToList();

                var calls = context.Calls.Where(i => i.StartAt >= _dateFrom && i.StartAt <= _dateTo).GroupBy(i => i.DivisionId).ToDictionary(i => i.Key, i => i.Count());
                var referralCustomers = context.Customers.Where(i => i.CreatedOn >= _dateFrom && i.CreatedOn <= _dateTo && i.InvitorId.HasValue).GroupBy(i => i.ClubId).ToDictionary(i => i.Key, i => i.Count());
                var tickets = context.Tickets.Where(i => i.CreatedOn >= _dateFrom
                                                    && i.CreatedOn <= _dateTo
                                                    && !i.TicketType.IsVisit
                                                    && !i.TicketType.IsGuest
                                                    && i.TicketType.SolariumMinutes == 0
                                                    && !i.ReturnDate.HasValue)
                                                    .Select(i => new { IsCustomerFromSite = i.Customer.FromSite, DivisionId = i.DivisionId})
                                                    .ToList();
                
                var leads = context.Customers.Where(i => i.CreatedOn >= _dateFrom && i.CreatedOn <= _dateTo && i.FromSite).GroupBy(i => i.ClubId).ToDictionary(i => i.Key, i => i.Count());
                var amountTicketLeads = tickets.Where(i => i.IsCustomerFromSite).GroupBy(i => i.DivisionId).ToDictionary(i => i.Key, i => i.Count());

                var customersWithTicket = context.Tickets.Where(i => i.IsActive && !i.ReturnDate.HasValue).Select(i => new { i.CustomerId, i.DivisionId}).ToList();
                var customerIdsWithTicket = customersWithTicket.Select(i => i.CustomerId).Distinct().ToList();
                var customerVisits = context.CustomerVisits
                        .Where(i => i.InTime >= _dateFrom && i.InTime <= _dateTo)
                        .Select(i => new
                        {
                            DivisionId = i.DivisionId,
                            CustomerId = i.CustomerId
                        })
                        .Where(i => customerIdsWithTicket.Contains(i.CustomerId))
                        .ToList();
                var customersUsedBar = context.GoodSales
                        .Select(i => new { DivisionId = i.BarOrder.DivisionId, CustomerId = i.BarOrder.CustomerId })
                        .Where(i => customerIdsWithTicket.Contains(i.CustomerId))
                        .ToList();

                foreach (var divId in divisionIds)
                {
                    _data.Add(new DataForDivisionRating
                    {
                        DivisionId = divId,
                        AmountCalls = calls.Where(i => i.Key == divId).Select(i => i.Value).FirstOrDefault(),
                        AmountReferralCustomers = referralCustomers.Where(i => i.Key == divId).Select(i => i.Value).FirstOrDefault(),
                        AmountTickets = tickets.Where(i => i.DivisionId == divId).Count(),
                        AmountStars = (int)(Decimal.Round(new DivisionStarsModel(divId).AvgStars)),
                        PercentConversionLeads = GetPercent(amountTicketLeads.Where(i => i.Key == divId).Select(i => i.Value).FirstOrDefault(), leads.Where(i => i.Key == divId).Select(i => i.Value).FirstOrDefault()),

                        PercentCustomersLittleVisits = customerVisits.Where(i => i.DivisionId == divId).GroupBy(i => i.CustomerId).Any()
                                      ? GetPercent(customerVisits.Where(i => i.DivisionId == divId).GroupBy(i => i.CustomerId).Where(i => i.Count() < 6).Count(),
                                                     customerVisits.Where(i => i.DivisionId == divId).GroupBy(i => i.CustomerId).Count())
                                      : 100,

                        PercentCustomersUsedBar = (LicenseTill(divId))
                            ? GetPercent(
                                      customersUsedBar.Where(i => i.DivisionId == divId).Select(i => i.CustomerId).Distinct().Count(),
                                      customersWithTicket.Where(i => i.DivisionId == divId).Select(i => i.CustomerId).Distinct().Count())
                            : 0,

                        PercentCustomersWithExtendTicket = (LicenseTill(divId))
                            ? GetPercent(
                                      customersWithTicket.Where(i => i.DivisionId == divId).GroupBy(i => i.CustomerId).Where(i => i.Count() > 1).Count(),
                                      customersWithTicket.Where(i => i.DivisionId == divId).GroupBy(i => i.CustomerId).Count())
                            : 0
                    });
                }

                return _data;
            }
        }

        private int GetPercent(int one, int two)
        {
            return (two == 0) ? 0 : (int)(Decimal.Round((decimal)one / (decimal)two * 100));
        }

        private bool LicenseTill (Guid DivisionId)
        {
            using (var context = new SyncMetadataEntities())
            {
                var division = context.MetaCompanies.Where(i => i.DivisionId == DivisionId).FirstOrDefault();
                
                if (division == null)
                {
                    return false;
                }
                else
                {
                    var licenseTill = (division.LicenseTill.HasValue) ? division.LicenseTill.Value.AddDays(7) : DateTime.Today.AddDays(1);
                    return (division.IsLicenseAvailable && licenseTill >= DateTime.Today);
                }
            }
        }
    }

    public class DataForDivisionRating
    {
        public Guid DivisionId { get; set; }
        public int PercentConversionLeads { get; set; }
        public int AmountTickets { get; set; }
        public int PercentCustomersWithExtendTicket { get; set; }
        public int AmountCalls { get; set; }
        public int AmountReferralCustomers { get; set; }
        public int PercentCustomersUsedBar { get; set; }
        public int PercentCustomersLittleVisits { get; set; }
        public int AmountStars { get; set; }
    }
}