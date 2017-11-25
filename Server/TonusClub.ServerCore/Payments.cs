using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;
using TonusClub.Entities;

namespace TonusClub.ServerCore
{
    public static class Payments
    {
        internal static void ProcessBonusPayment(TonusEntities context, PaymentDetails details)
        {
                var customer = context.Customers.FirstOrDefault(c => c.Id == details.CustomerId);

                if (customer == null)
                {
                    details.Success = false;
                    details.Description = "Wrong customer ID";
                    return;
                }

                customer.InitDepositValues();

                if (customer.BonusDepositValue < details.RequestedBonusAmount)
                {
                    details.Success = false;
                    details.Description = String.Format(Localization.Resources.NoBonusesErr, details.RequestedBonusAmount, customer.BonusDepositValue);
                    return;
                }

                details.Success = true;
                details.BonusPayment = (decimal)details.RequestedBonusAmount;
                details.PurchaseDate = DateTime.Now;

                var ba = new BonusAccount
                {
                    Id = Guid.NewGuid(),
                    Amount = -details.RequestedBonusAmount.Value,
                    AuthorId = details.UserId,
                    CreatedOn = details.PurchaseDate,
                    CustomerId = details.CustomerId,
                    Description = details.OrderNumber.ToString(),
                    CompanyId = customer.CompanyId
                };

                context.BonusAccounts.AddObject(ba);
            
        }

        internal static void ProcessMoneyPayment(TonusEntities context, PaymentDetails details)
        {
                var customer = context.Customers.FirstOrDefault(c => c.Id == details.CustomerId);

                if (customer == null)
                {
                    details.Success = false;
                    details.Description = "Wrong customer ID.";
                    return;
                }

                customer.InitDepositValues();

                if (customer.RurDepositValue < (decimal)details.DepositPayment)
                {
                    details.Success = false;
                    details.Description = String.Format(Localization.Resources.NoDepositErr, details.DepositPayment, customer.RurDepositValue);
                    return;
                }

                details.Success = true;
                details.PurchaseDate = DateTime.Now;

                if (details.DepositPayment > 0)
                {
                    var da = new DepositAccount
                    {
                        Id = Guid.NewGuid(),
                        Amount = -(decimal)details.DepositPayment,
                        AuthorId = details.UserId,
                        CreatedOn = details.PurchaseDate,
                        CustomerId = details.CustomerId,
                        Description = String.Format(Localization.Resources.OrderPmtNum, details.OrderNumber),
                        CompanyId = customer.CompanyId
                    };

                    context.DepositAccounts.AddObject(da);
                }
        }
    }
}
