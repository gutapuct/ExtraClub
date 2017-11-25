using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace TonusClub.ServerCore
{
    public static class FinancesCore
    {
        public static List<CashInOrder> GetCashInOrders(Guid divisionId, DateTime start, DateTime end)
        {
            using (var context = new TonusEntities())
            {
                end = end.Date.AddDays(1);
                return context.CashInOrders.Include("ReceivedBy").Include("CreatedBy")
                    .Where(i => i.DivisionId == divisionId && i.CreatedOn >= start && i.CreatedOn < end).OrderBy(i => i.Number).ToList().Init();
            }
        }

        public static List<CashOutOrder> GetCashOutOrders(Guid divisionId, DateTime start, DateTime end)
        {
            using (var context = new TonusEntities())
            {
                end = end.Date.AddDays(1);
                return context.CashOutOrders/*.Include("ReceivedBy")*/.Include("CreatedBy")
                    .Where(i => i.DivisionId == divisionId && i.CreatedOn >= start && i.CreatedOn < end).OrderBy(i => i.Number).ToList();
            }
        }

        public static decimal GetCashAmount(Guid divisionId, DateTime date)
        {
            using (var context = new TonusEntities())
            {
                date = date.Date;
                var date1 = date.AddDays(1);
                var sales = context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= date && i.PurchaseDate < date1).Sum(i => (decimal?)i.CashPayment) ?? 0;
                sales -= context.CashInOrders.Where(i => i.DivisionId == divisionId && i.CreatedOn >= date && i.CreatedOn < date1).Sum(i => (decimal?)i.Amount) ?? 0;

                return sales;
            }
        }

        public static void PostCashInOrder(Guid divisionId, Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, Guid receivedBy, string reason)
        {
            using (var context = new TonusEntities())
            {
                createdOn = createdOn.Date;
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);

                CashInOrder order;
                if (!context.CashInOrders.Any(i => i.Id == orderId))
                {
                    order = new CashInOrder
                    {
                        CompanyId = cId,
                        DivisionId = divisionId,
                        Id = orderId == Guid.Empty ? Guid.NewGuid() : orderId,
                    };

                    var year = new DateTime(DateTime.Today.Year, 1, 1);
                    var num = (context.CashInOrders.Where(i => i.DivisionId == divisionId && i.CreatedOn >= year).Max(i => (int?)i.Number) ?? 0) + 1;
                    order.Number = num;
                    context.CashInOrders.AddObject(order);
                }
                else
                {
                    order = context.CashInOrders.Single(i => i.Id == orderId);
                }

                order.CreatedById = createdBy;
                order.Amount = amount;
                order.CreatedOn = createdOn;
                order.Debet = debet;
                order.Reason = reason.Trim();
                order.ReceivedById = receivedBy;

                context.SaveChanges();
            }
        }
        public static void PostCashOutOrder(Guid divisionId, Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, string receivedBy, string reason, string responsible)
        {
            using (var context = new TonusEntities())
            {
                createdOn = createdOn.Date;
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);

                CashOutOrder order;

                if (!context.CashOutOrders.Any(i => i.Id == orderId))
                {
                    order = new CashOutOrder
                    {
                        CompanyId = cId,
                        DivisionId = divisionId,
                        Id = orderId == Guid.Empty ? Guid.NewGuid() : orderId,
                    };

                    var year = new DateTime(DateTime.Today.Year, 1, 1);
                    var num = (context.CashOutOrders.Where(i => i.DivisionId == divisionId && i.CreatedOn >= year).Max(i => (int?)i.Number) ?? 0) + 1;
                    order.Number = num;
                    context.CashOutOrders.AddObject(order);
                }
                else
                {
                    order = context.CashOutOrders.Single(i => i.Id == orderId);
                }

                order.Amount = amount;
                order.CreatedById = createdBy;
                order.CreatedOn = createdOn;
                order.Debet = debet;
                order.Reason = (reason ?? "").Trim();
                order.ReceivedByText = receivedBy;
                order.Responsible = responsible;

                context.SaveChanges();
            }
        }

        public static decimal GetCashTodaysAmount(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var year = new DateTime(DateTime.Today.Year, 1, 1);
                return (context.CashInOrders.Where(i => i.CreatedOn >= year && i.DivisionId == divisionId).Sum(i => (decimal?)i.Amount) ?? 0) -
                    (context.CashOutOrders.Where(i => i.CreatedOn >= year && i.DivisionId == divisionId).Sum(i => (decimal?)i.Amount) ?? 0);
            }
        }
    }
}
