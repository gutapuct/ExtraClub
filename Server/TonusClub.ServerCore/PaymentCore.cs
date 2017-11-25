using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServiceModel;
using System.ServiceModel;
using TonusClub.ServiceModel.Turnover;
using System.Data;

using System.Xml.Serialization;
using System.IO;
using System.Data.Objects;

namespace TonusClub.ServerCore
{
    public static class PaymentCore
    {

        public static PaymentDetails ProcessPayment(PaymentDetails details, IEnumerable<PayableItem> basket, Guid goodActionId)
        {
            try
            {
                using(var context = new TonusEntities())
                {
                    var user = UserManagement.GetUser(context);

                    var division = context.Divisions.First(d => d.Id == details.DivisionId);

                    details.UserId = user.UserId;

                    if(details.Cashless && !details.ProviderId.HasValue)
                    {
                        throw new FaultException<string>(Localization.Resources.PayerNeeded, Localization.Resources.PayerNeeded);
                    }

                    if(details.RequestedBonusAmount.HasValue)
                    {
                        Payments.ProcessBonusPayment(context, details);
                    }
                    else
                    {
                        Payments.ProcessMoneyPayment(context, details);
                    }

                    if(details.Success)
                    {
                        var barOrder = new BarOrder
                        {
                            Id = Guid.NewGuid(),
                            AuthorId = user.UserId,
                            CustomerId = details.CustomerId,
                            PurchaseDate = details.PurchaseDate,
                            DivisionId = details.DivisionId,
                            OrderNumber = details.OrderNumber,
                            CashPayment = details.CashPayment - details.Change,
                            DepositPayment = details.DepositPayment,
                            CardPayment = details.CardPayment,
                            CardNumber = details.CardNumber,
                            CardAuth = details.CardAuth,
                            BonusPayment = details.BonusPayment,
                            CompanyId = division.CompanyId,
                            CertificateId = details.CertificateId,
                            ProviderId = details.ProviderId,
                            GoodActionId = goodActionId,
                            SectionNumber = details.SectionNumber
                        };

                        if(details.CertificateId.HasValue)
                        {
                            var cert = context.Certificates.Single(i => i.Id == details.CertificateId.Value);
                            cert.UsedOrderId = barOrder.Id;
                        }

                        context.BarOrders.AddObject(barOrder);

                        var customer = context.Customers.First(c => c.Id == details.CustomerId);

                        if(!details.ProviderId.HasValue)
                        {
                            PaymentCore.ProcessBasket(context, division, barOrder, details, customer, user, basket);
                        }
                        else
                        {
                            PaymentCore.ProcessCashlessBasket(context, division, barOrder, details, customer, user, basket);
                        }

                        barOrder.Content = SerializeBasket(basket.ToArray());
                        context.SaveChanges();
                    }

                    return details;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
        }

        internal static void ProcessBasket(TonusEntities context, Division division, BarOrder barOrder, PaymentDetails details, Customer customer, User user, IEnumerable<PayableItem> basket)
        {

            if(basket.Count() > 0)
            {
                Logger.Log("Оплата " + basket.First().GetType().FullName);
                if(basket.First() is BarPointGood)
                {
                    ProcessBarGoods(context, division, barOrder, details, customer, user, basket);
                }
                else if(basket.First() is TicketPaymentPosition)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Ticket;
                    ProcessTicektPayment(context, division, barOrder, details, customer, user, basket);
                }
                else if(basket.First() is TicketGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Ticket;
                    PaymentCore.ProcessTicketGood(context, basket, user, division, details, customer, false, barOrder);
                }
                else if(basket.First() is TicketChangeGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Change;
                    PaymentCore.ProcessTicketChangeGood(context, basket.First() as TicketChangeGood, user, division, details, customer);
                }
                else if(basket.First() is TicketRebillGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Rebill;
                    PaymentCore.ProcessTicketRebillGood(context, basket.First() as TicketRebillGood, user, division, details, customer);
                }
                else if(basket.First() is TicketFreezeGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Freeze;
                    PaymentCore.ProcessTicketFreeze(context, basket.First() as TicketFreezeGood, user, division, details, customer);
                }
                else if(basket.First() is SolariumGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Solarium;
                    PaymentCore.ProcessSolariumVisit(context, basket.First() as SolariumGood, user, division, details, customer);
                }
                else if(basket.First() is RentPayment)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Rent;
                    PaymentCore.ProcessRentPayment(context, basket.First() as RentPayment, user, division, details, customer);
                }
                else if(basket.First() is CloseRentPayment)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Rent;
                    PaymentCore.ProcessCloseRentPayment(context, basket.First() as CloseRentPayment, user, division, details, customer);
                }
                else if(basket.First() is DepositGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Deposit;
                    PaymentCore.ProcessDepositGood(context, basket.First() as DepositGood, user, division, details, customer);
                }
                else if(basket.First() is ChildrenRoomGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Child;
                    details.Parameter = PaymentCore.ProcessChildrenRoom(context, basket.First() as ChildrenRoomGood, user, division, details, customer);
                }
                else if(basket.First() is CustomerCardGood)
                {
                    barOrder.Kind1C = (int)Kinds1CEnum.Card;
                    ProcessCustomerCard(context, division, barOrder, details, customer, user, basket);
                }
                else
                {
                    throw new FaultException<string>(Localization.Resources.DetTypeErr, Localization.Resources.DetTypeErr);
                }
            }
        }

        private static void ProcessCustomerCard(TonusEntities context, Division division, BarOrder barOrder, PaymentDetails details, Customer customer, User user, IEnumerable<PayableItem> basket)
        {
            var bonus = 0m;
            if(customer.ActiveCard != null)
            {
                bonus = customer.ActiveCard.CustomerCardType.Bonus;
            }
            var ccg = basket.First() as CustomerCardGood;

            context.CustomerCards.Where(i => i.CardBarcode == ccg.CardNumber && i.CompanyId == customer.CompanyId).ToList().ForEach(i => i.IsActive = false);

            customer.CustomerCards.ToList().ForEach(i => i.IsActive = false);

            var cc = new CustomerCard
            {
                Id = Guid.NewGuid(),
                AuthorId = user.UserId,
                CardBarcode = ccg.CardNumber,
                Comment = ccg.Comment,
                CustomerCardTypeId = ccg.CardTypeId,
                CustomerId = details.CustomerId,
                Discount = ccg.DiscountPercent,
                EmitDate = DateTime.Now,
                Price = (decimal)ccg.Price,
                CompanyId = division.CompanyId,
                DivisionId = division.Id,
                IsActive = true,
                PmtTypeId = GetPmtType(details, false)
            };
            context.CustomerCards.AddObject(cc);

            var ctype = context.GetObjectByKey(new EntityKey("TonusEntities.CustomerCardTypes", "Id", ccg.CardTypeId)) as CustomerCardType;

            //Начисление бонусов. если карта не первая, то начисляем только разницу.
            if(ctype.Bonus - bonus > 0)
            {
                var bo = new BonusAccount
                {
                    Amount = ctype.Bonus - bonus,
                    AuthorId = user.UserId,
                    CompanyId = division.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = details.CustomerId,
                    Description = Localization.Resources.BonOnCard,
                    Id = Guid.NewGuid()
                };
                context.BonusAccounts.AddObject(bo);
            }
            //Создание абонемента, если чо как
            if(ctype.IsVisit && customer.CustomerCards.Count == 1)
            {
                var tt = Core.GetTicketTypes(true).FirstOrDefault(i => i.IsVisit);
                if(tt != null)
                {
                    var ct = new Ticket
                    {
                        AuthorId = user.UserId,
                        CompanyId = division.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = details.CustomerId,
                        DivisionId = division.Id,
#if BEAUTINIKA
                        ExtraUnitsAmount = tt.ExtraUnits,
#endif
                        GuestUnitsAmount = tt.GuestUnits,
                        Id = Guid.NewGuid(),
                        IsActive = true,
                        Length = tt.Length,
                        Number = Core.GetTicketNumber(context, division.Company),
                        Price = 0,
                        StartDate = DateTime.Today,
                        TicketTypeId = tt.Id,
                        UnitsAmount = tt.Units
                    };
                    context.Tickets.AddObject(ct);
                }
            }
        }

        private static void ProcessTicektPayment(TonusEntities context, Division division, BarOrder barOrder, PaymentDetails details, Customer customer, User user, IEnumerable<PayableItem> basket)
        {
            try
            {
                var tpp = basket.First() as TicketPaymentPosition;

                var ticket = context.Tickets.Single(i => i.Id == tpp.Ticket.Id);
                Logger.Log("Абонемент " + ticket.Number + " оплата " + tpp.Price.ToString());
                ticket.InitDetails();
                if(ticket.Loan < tpp.Price) throw new FaultException(Localization.Resources.TickPmtHigh);

                var pmtInfo = new TicketPayment
                {
                    Id = Guid.NewGuid(),
                    TicketId = tpp.Ticket.Id,
                    Amount = tpp.Price,
                    AuthorId = user.UserId,
                    PaymentDate = DateTime.Now,
                    CompanyId = division.CompanyId,
                    ReceiptNumber = Core.GetNewReceiptNumber(user.CompanyId),
                    BarOrderId = barOrder.Id
                };
                context.TicketPayments.AddObject(pmtInfo);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
        }

        private static bool ProcessBarGoods(TonusEntities context, Division division, BarOrder barOrder, PaymentDetails details, Customer customer, User user, IEnumerable<PayableItem> basket)
        {
            foreach(BarPointGood good in basket.Where(g => ((BarPointGood)g).InBasket > 0))
            {
                if(context.Goods.Any(i => i.Id == good.GoodId))
                {
                    var act = context.GoodActions.FirstOrDefault(i => i.Id == barOrder.GoodActionId);
                    //Обычный товар, не сертификат
                    var saleInfo = new GoodSale
                    {
                        Id = Guid.NewGuid(),
                        GoodId = good.GoodId,
                        Amount = good.InBasket,
                        BarOrderId = barOrder.Id,
                        CompanyId = division.CompanyId,
                        StorehouseId = good.StorehouseId,
                        Discount = act != null ? act.Discount : (decimal?)null
                    };

                    if(!details.BonusPayment.HasValue)
                    {
                        saleInfo.PriceMoney = (decimal)good.Price;
                    }
                    else
                    {
                        saleInfo.PriceBonus = (decimal?)good.BonusPrice;
                    }
                    context.GoodSales.AddObject(saleInfo);
                }
                else
                {
                    //Сертификат
                    var cert = context.Certificates.SingleOrDefault(i => i.Id == good.GoodId);
                    if(cert != null)
                    {
                        cert.SellDate = DateTime.Now;
                        cert.BuyerId = details.CustomerId;
                        cert.SellOrderId = barOrder.Id;
                        if(details.BonusPayment.HasValue && details.BonusPayment > 0)
                        {
                            cert.IsBonusSell = true;
                        }
                        else
                        {
                            cert.PriceMoney = good.Price;
                        }
                    }
                    else
                    {
                        //Пакет
                        barOrder.Kind1C = 10;
                        var pls = context.PackageLines.Where(i => i.PackageId == good.GoodId).ToList();

                        var gp = Core.GetGoodsPresence(division.Id);
                        var sh = context.Storehouses.Where(i => i.DivisionId == barOrder.DivisionId && i.IsActive && i.BarSale).Select(i => i.Id).ToArray();
                        gp = gp.Where(i => sh.Contains(i.StorehouseId)).ToList();
                        var sId = gp.GroupBy(i => i.StorehouseId).Where(i => pls.All(l => i.Any(p => p.Amount >= l.Amount))).Select(i => i.Key).FirstOrDefault();
                        if(sId != Guid.Empty && sId != null)
                        {
                            foreach(var pl in pls)
                            {
                                context.CustomerGoodsFlows.AddObject(new CustomerGoodsFlow
                                {
                                    Amount = pl.Amount,
                                    CompanyId = pl.CompanyId,
                                    CreatedById = user.UserId,
                                    CreatedOn = DateTime.Now,
                                    CustomerId = customer.Id,
                                    Description = barOrder.OrderNumber.ToString(),
                                    DivisionId = division.Id,
                                    GoodId = pl.GoodId,
                                    StorehouseId = sId,
                                    Id = Guid.NewGuid()
                                });
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static byte[] SerializeBasket(PayableItem[] basket)
        {
            XmlSerializer x = new XmlSerializer(typeof(PayableItem[]), new Type[] { typeof(BarPointGood), typeof(ChildrenRoomGood), typeof(CloseRentPayment), typeof(CustomerCardGood), typeof(GoodSaleReturnPosition), typeof(RentPayment), typeof(SolariumGood), typeof(TicketChangeGood), typeof(TicketFreezeGood), typeof(TicketGood), typeof(TicketPaymentPosition), typeof(TicketRebillGood), typeof(TicketReturnPosition), typeof(DepositGood) });
            MemoryStream ms = new MemoryStream();
            x.Serialize(ms, basket);
            ms.Position = 0;
            var res = new byte[ms.Length];
            ms.Read(res, 0, (int)ms.Length);
            return res;
        }

        public static PaymentDetails ProcessReturn(PaymentDetails details, IEnumerable<PayableItem> items)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                details.UserId = user.UserId;

                //TODO: тут, в принципе, ничего не нужно проверять - возврат - он и есть возврат.
                //Payments.ProcessMoneyPayment(details);

                details.Success = true;
                details.PurchaseDate = DateTime.Now;
                var division = context.Divisions.First(d => d.Id == details.DivisionId);

                if(details.Success)
                {
                    var barOrder = new BarOrder
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = user.UserId,
                        CustomerId = details.CustomerId,
                        PurchaseDate = details.PurchaseDate,
                        DivisionId = details.DivisionId,
                        OrderNumber = details.OrderNumber,
                        CashPayment = (decimal)details.CashPayment,
                        CompanyId = division.CompanyId
                    };

                    context.BarOrders.AddObject(barOrder);

                    if(items.Count() > 0)
                    {
                        if(items.First() is TicketReturnPosition)
                        {

                            var tpp = items.First() as TicketReturnPosition;
                            var ticket = context.Tickets.FirstOrDefault(t => t.Id == tpp.TicketId);
                            if(ticket == null) throw new ArgumentNullException("ticket");
                            ticket.ReturnCost = (decimal)-tpp.Price;
                            ticket.ReturnDate = DateTime.Today;
                            ticket.ReturnUserId = user.UserId;

                            var n = context.Spendings.Where(i => i.DivisionId == details.DivisionId).Max(i => i.Number, 0) + 1;
                            var st = context.SpendingTypes.SingleOrDefault(i => i.Name == Localization.Resources.Refund && i.CompanyId == ticket.CompanyId && i.IsCommon);
                            if(st == null)
                            {
                                using(var c1 = new TonusEntities())
                                {
                                    c1.SpendingTypes.AddObject(st = new SpendingType { CompanyId = ticket.CompanyId, Id = Guid.NewGuid(), IsCommon = true, Name = Localization.Resources.Refund });
                                    c1.SaveChanges();
                                }
                            }
                            var stId = st.Id;

                            var sp = new Spending
                            {
                                Amount = Math.Abs(tpp.Price),
                                AuthorId = user.UserId,
                                CompanyId = ticket.CompanyId,
                                CreatedOn = DateTime.Now,
                                DivisionId = ticket.DivisionId,
                                Id = Guid.NewGuid(),
                                Name = String.Format(Localization.Resources.RefundTicketNum, ticket.Number),
                                Number = n,
                                PaymentType = Localization.Resources.RefundTicket,
                                SpendingTypeId = stId
                            };
                            context.Spendings.AddObject(sp);

                            var task = new Task
                            {
                                AuthorId = user.UserId,
                                ClosedById = user.UserId,
                                ClosedComment = Localization.Resources.TicketRefunded,
                                ClosedOn = DateTime.Now,
                                CompanyId = ticket.CompanyId,
                                CreatedOn = DateTime.Now,
                                ExpiryOn = DateTime.Now,
                                Id = Guid.NewGuid(),
                                Message = String.Format("Возврат абонемента {0} {1} на сумму {2:c}", ticket.Number, ticket.Customer.FullName, ticket.ReturnCost),
                                Priority = 1,
                                StatusId = 1,
                                Subject = Localization.Resources.RefundTicket
                            };
                            context.Tasks.AddObject(task);

                        }
                        else if(items.First() is GoodSaleReturnPosition)
                        {
                            var gsr = items.First() as GoodSaleReturnPosition;
                            var gs = context.GoodSales.Single(i => i.Id == gsr.GoodSaleId);
                            gs.ReturnDate = DateTime.Now;
                            gs.ReturnById = user.UserId;

                            var n = context.Spendings.Where(i => i.DivisionId == details.DivisionId).Max(i => i.Number, 0) + 1;
                            var st = context.SpendingTypes.FirstOrDefault(i => i.Name == Localization.Resources.GoodRefund && i.CompanyId == gs.CompanyId && i.IsCommon);

                            if(st == null)
                            {
                                st = new SpendingType
                                {
                                    CompanyId = gs.CompanyId,
                                    IsCommon = true,
                                    Name = Localization.Resources.GoodRefund,
                                    Id = Guid.NewGuid()
                                };
                                context.SpendingTypes.AddObject(st);
                            }

                            var stId = st.Id;

                            var sp = new Spending
                            {
                                Amount = gs.Cost,
                                AuthorId = user.UserId,
                                CompanyId = gs.CompanyId,
                                CreatedOn = DateTime.Now,
                                DivisionId = gs.BarOrder.DivisionId,
                                Id = Guid.NewGuid(),
                                Name = String.Format(Localization.Resources.GoodRefundNum, gs.Good.Name, gs.BarOrder.OrderNumber),
                                Number = n,
                                PaymentType = Localization.Resources.GoodRefund,
                                SpendingTypeId = stId
                            };
                            context.Spendings.AddObject(sp);
                        }
                        context.SaveChanges();
                    }
                }

                return details;
            }
        }

        internal static void ProcessTicketGood(TonusEntities context, IEnumerable<ServiceModel.PayableItem> basket, User user, Division division, PaymentDetails details, Customer customer, bool isCashless, BarOrder barOrder)
        {
            var tg = basket.First() as TicketGood;
            var invoiceNumber = (context.Tickets.Any(i => i.CompanyId == user.CompanyId) ? context.Tickets.Where(i => i.CompanyId == user.CompanyId).Max(i => i.InvoiceNumber ?? 0) : 0) + 1;
            //new ticket
            var ticket = new Ticket
            {
                AuthorId = user.UserId,
                CompanyId = division.CompanyId,
                CreatedOn = DateTime.Now,
                CustomerId = details.CustomerId,
                Deleted = false,
                DiscountPercent = tg.DiscountPercent,
                DivisionId = details.DivisionId,
#if BEAUTINIKA
                ExtraUnitsAmount = tg.Ticket.ExtraUnitsAmount,
#endif
                GuestUnitsAmount = tg.Ticket.GuestUnitsAmount,
                Id = Guid.NewGuid(),
                InstalmentId = (tg.Instalment == null || tg.Instalment.Id == Guid.Empty) ? (Guid?)null : tg.Instalment.Id,
                IsActive = false,
                Length = tg.Ticket.Length,
                Number = Core.GetTicketNumber(context, division.Company),
                Price = tg.Ticket.Price,
                TicketTypeId = tg.TicketType.Id,
                UnitsAmount = tg.Ticket.UnitsAmount,
                FreezesAmount = GetFreezesAmount(context, division.CompanyId, tg.TicketType.Id),
                SolariumMinutes = tg.TicketType.SolariumMinutes,
                LastInstalmentDay = (tg.Instalment == null || tg.Instalment.Id == Guid.Empty) ? (DateTime?)null : DateTime.Today.AddDays(tg.Instalment.Length + (tg.Instalment.SecondLength ?? 0)),
                FirstPmtTypeId = GetPmtType(details, isCashless),
                InvoiceNumber = invoiceNumber,
                VatAmount = tg.VatAmount,
                CreditInitialPayment = tg.CreditInitialPayment,
                CreditComment = tg.CreditComment,
                Comment = tg.Ticket.Comment
            };
            ticket.PlanningInstalmentDay = ticket.LastInstalmentDay;

            //Начисление бонусов.
            if(tg.TicketType.Bonus > 0 &&
                customer.CustomerCards.OrderByDescending(i => i.EmitDate).Where(i => i.IsActive)
                .Select(i => i.CustomerCardType.GiveBonusForCards).FirstOrDefault())
            {
                var bo = new BonusAccount
                {
                    Amount = tg.TicketType.Bonus,
                    AuthorId = user.UserId,
                    CompanyId = division.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = details.CustomerId,
                    Description = String.Format(Localization.Resources.BonAdd, ticket.Number),
                    Id = Guid.NewGuid()
                };
                context.BonusAccounts.AddObject(bo);
            }


            tg.TicketId = ticket.Id;

            if(tg.Instalment != null && tg.Instalment.Id != Guid.Empty && isCashless)
            {
                throw new FaultException<string>(Localization.Resources.NoInstOnBank, Localization.Resources.NoInstOnBank);
            }

            context.Tickets.AddObject(ticket);

            if(tg.TicketType.IsGuest && customer.InvitedBy != null)
            {
                //Списываем гостевые единицы
#if BEAUTINIKA
                if(!ChargeUnits(context, ticket, customer, user, (int)(tg.Ticket.UnitsAmount + tg.Ticket.ExtraUnitsAmount)))
#else
                if(!ChargeUnits(context, ticket, customer, user, (int)tg.Ticket.UnitsAmount))
#endif
                {
                    throw new FaultException(Localization.Resources.NoGuestEnough);
                }
            }

            //Привязываем запланированные на "ничто" процедуры
            var ticketType = context.TicketTypes.Single(i => i.Id == tg.TicketType.Id);
            foreach(var ev in customer.TreatmentEvents.Where(i => !i.TicketId.HasValue && i.VisitStatus == (short)TreatmentEventStatus.Planned && i.VisitDate >= DateTime.Now))
            {
                if(ticketType.TreatmentTypes.Any(i => i.Id == ev.TreatmentConfig.TreatmentTypeId))
                {
                    ev.VisitStatus = (short)TreatmentEventStatus.Canceled;
                }
                else
                {
                    ev.TicketId = ticket.Id;
                }
            }

            if(!isCashless)
            {
                //Пеймент создастся в момент проведения платежа.
                var pmtInfo = new TicketPayment
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    Amount = tg.PartialPayment ?? (decimal)tg.Cost,
                    AuthorId = user.UserId,
                    PaymentDate = DateTime.Now,
                    CompanyId = division.CompanyId,
                    ReceiptNumber = Core.GetNewReceiptNumber(user.CompanyId),
                    BarOrderId = barOrder.Id
                };
                context.TicketPayments.AddObject(pmtInfo);
            }
        }

        private static int GetPmtType(PaymentDetails details, bool isCashless)
        {
            if(isCashless) return 3;
            if(details.CashPayment > 0) return 0;
            if(details.CardPayment > 0) return 1;
            return 2;
        }

        public static int GetFreezesAmount(TonusEntities context, Guid companyId, Guid ticketTypeId)
        {
            var tt = context.TicketTypes.Single(i => i.Id == ticketTypeId);
            var comp = context.Companies.Single(i => i.CompanyId == companyId);
            var res = tt.MaxFreezeDays;
            if(comp.MaxFreezeUnits.HasValue) res = Math.Min(res, comp.MaxFreezeUnits.Value);
            if(comp.MaxFreezePercent.HasValue && comp.MaxFreezePercent > 0) res = Math.Min(res, (int)(comp.MaxFreezePercent.Value * tt.Length));
            return res;
        }

        private static bool ChargeUnits(TonusEntities context, Ticket ticket, Customer customer, User user, int amount)
        {
            while(amount > 0)
            {
                var ticks = customer.InvitedBy.Tickets.Where(t => t.IsActive && t.CompanyId == ticket.CompanyId).ToList();
                ticks.ForEach(t => t.InitDetails());

                var tick = ticks.Where(t => t.GuestUnitsLeft > 0).OrderBy(t => t.FinishDate).FirstOrDefault();
                if(tick != null)
                {
                    var am = (int)Math.Min(amount, tick.GuestUnitsLeft);
                    amount -= am;
                    var uc = new UnitCharge
                    {
                        AuthorId = user.UserId,
                        CompanyId = customer.CompanyId,
                        Date = DateTime.Now,
                        GuestCharge = am,
                        TicketId = tick.Id,
                        Id = Guid.NewGuid(),
                        Reason = String.Format(Localization.Resources.GuestTick, ticket.Number)
                    };
                    context.UnitCharges.AddObject(uc);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        internal static void ProcessTicketChangeGood(TonusEntities context, TicketChangeGood ticketChangeGood, User user, Division division, PaymentDetails details, Customer customer)
        {
            var ticket = context.Tickets.Include("TicketType").FirstOrDefault(t => t.Id == ticketChangeGood.OldTicketId);
            if(ticket == null) return;
            ticket.InitDetails();

            var newTicketType = context.TicketTypes.FirstOrDefault(tt => tt.Id == ticketChangeGood.NewTicketType.Id);

            var bonusDelta = newTicketType.Bonus - ticket.TicketType.Bonus;

            //Старый абонемент - нихуа не надо делать. Только сделать неактивным.
            ticket.IsActive = false;
            //Создаем новый, наполняем, ставим родителя
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                AuthorId = user.UserId,
                CreatedOn = DateTime.Now,
                CustomerId = ticket.CustomerId,
                DiscountPercent = 0,
                DivisionId = ticket.DivisionId,
#if BEAUTINIKA
                ExtraUnitsAmount = ticket.ExtraUnitsLeft + (newTicketType.ExtraUnits - ticket.ExtraUnitsAmount),
#endif
                GuestUnitsAmount = ticket.GuestUnitsLeft + (newTicketType.GuestUnits - ticket.GuestUnitsAmount),
                InheritedTicketId = ticket.Id,
                Length = newTicketType.Length,
                Price = (newTicketType.Price * (1 - ticketChangeGood.Discount)) - ticket.Cost + ticket.Loan,
                StartDate = ticket.StartDate ?? DateTime.Today,
                TicketTypeId = newTicketType.Id,
                UnitsAmount = ticket.UnitsLeft + (newTicketType.Units - ticket.UnitsAmount),
                CompanyId = ticket.CompanyId,
                InstalmentId = (ticketChangeGood.Instalment == null || ticketChangeGood.Instalment.Id == Guid.Empty) ? (Guid?)null : ticketChangeGood.Instalment.Id,
                Number = Core.GetTicketNumber(context, division.Company),
                LastInstalmentDay = (ticketChangeGood.Instalment == null || ticketChangeGood.Instalment.Id == Guid.Empty) ? (DateTime?)null : DateTime.Today.AddDays(ticketChangeGood.Instalment.Length),
                FreezesAmount = ticket.FreezesLeft + newTicketType.MaxFreezeDays - ticket.FreezesAmount,
                IsActive = true,
                SolariumMinutes = ticket.SolariumMinutesLeft + (newTicketType.SolariumMinutes - ticket.SolariumMinutes),
            };
            context.Tickets.AddObject(newTicket);

            ticket.Comment = String.Format("Заменен с доплатой с номера абонемента {0}", newTicket.Number);
            newTicket.Comment = String.Format("Переоформлен от {0}, номер абонемента {1}", ticket.Customer.FullName, ticket.Number);


            if(bonusDelta != 0)
            {
                context.BonusAccounts.AddObject(new BonusAccount
                {
                    Amount = bonusDelta,
                    AuthorId = user.UserId,
                    CompanyId = ticket.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customer.Id,
                    Description = "Перерасчет при замене абонемента №" + ticket.Number,
                    Id = Guid.NewGuid()
                });
            }

            var pmtInfo = new TicketPayment
            {
                Id = Guid.NewGuid(),
                TicketId = newTicket.Id,
                Amount = (decimal)details.RequestedAmount,
                AuthorId = user.UserId,
                PaymentDate = DateTime.Now,
                CompanyId = division.CompanyId,
                ReceiptNumber = Core.GetNewReceiptNumber(user.CompanyId)
            };
            context.TicketPayments.AddObject(pmtInfo);
        }

        internal static void ProcessTicketRebillGood(TonusEntities context, TicketRebillGood ticketRebillGood, User user, Division division, PaymentDetails details, Customer customer)
        {
            var ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketRebillGood.TicketId);
            ticket.InitDetails();
            //Старый абонемент - нихуа не надо делать. Только снять активность.
            ticket.IsActive = false;
            //Создаем новый, наполняем, ставим родителя
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                AuthorId = user.UserId,
                CreatedOn = DateTime.Now,
                CustomerId = ticketRebillGood.NewCustomerId,
                DiscountPercent = 0,
                DivisionId = ticket.DivisionId,
#if BEAUTINIKA
                ExtraUnitsAmount = ticket.ExtraUnitsLeft,
#endif
                GuestUnitsAmount = ticket.GuestUnitsLeft,
                InheritedTicketId = ticket.Id,
                Length = ticket.LengthLeft,
                Price = ticket.Loan,
                StartDate = DateTime.Today,
                TicketTypeId = ticket.TicketTypeId,
                UnitsAmount = ticket.UnitsLeft,
                IsActive = true,
                CompanyId = ticket.CompanyId,
                FreezesAmount = ticket.FreezesLeft,
                InstalmentId = ticket.InstalmentId,
                LastInstalmentDay = ticket.LastInstalmentDay,
                Number = Core.GetTicketNumber(context, division.Company),
                SolariumMinutes = ticket.SolariumMinutesLeft,
            };
            context.Tickets.AddObject(newTicket);

            var newCustomer = context.Customers.Single(x => x.Id == ticketRebillGood.NewCustomerId);
            ticket.Comment = String.Format("Переоформлен на {0}, номер абонемента {1}", newCustomer.FullName, newTicket.Number);
            newTicket.Comment = String.Format("Переоформлен от {0}, номер абонемента {1}", ticket.Customer.FullName, ticket.Number);
        }

        internal static void ProcessTicketFreeze(TonusEntities context, TicketFreezeGood ticketFreezeGood, User user, Division division, PaymentDetails details, Customer customer)
        {
            var ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketFreezeGood.TicketId);
            ticket.InitDetails();

            if(ticket.FreezesLeft < (ticketFreezeGood.EndDate - ticketFreezeGood.StartDate).TotalDays)
            {
                var str = String.Format(Localization.Resources.MaxFreezeLen, ticket.FreezesLeft + 1);
                throw new FaultException<string>(str, str);
            }

            var treatments = context.TreatmentEvents.Where(i => i.CustomerId == customer.Id && i.TicketId == ticketFreezeGood.TicketId && i.VisitDate >= ticketFreezeGood.StartDate
                && EntityFunctions.TruncateTime(i.VisitDate) <= ticketFreezeGood.EndDate && i.VisitStatus == 0).ToArray();
            foreach(var treatment in treatments)
            {
                treatment.VisitStatus = 1;
                treatment.Comment = "Отмена занятия в связи с заморозкой абонемента";
            }


            var freeze = new TicketFreeze
            {
                AuthorId = user.UserId,
                CompanyId = ticket.CompanyId,
                CreatedOn = DateTime.Now,
                FinishDate = ticketFreezeGood.EndDate,
                Id = Guid.NewGuid(),
                StartDate = ticketFreezeGood.StartDate,
                TicketFreezeReasonId = ticketFreezeGood.ReasonId,
                TicketId = ticketFreezeGood.TicketId
            };
            context.TicketFreezes.AddObject(freeze);
        }

        internal static Guid ProcessChildrenRoom(TonusEntities context, ServiceModel.Turnover.ChildrenRoomGood childrenRoomGood, User user, Division division, PaymentDetails details, Customer customer)
        {
            customer.InitEssentials();
            var cr = new ChildrenRoom
                         {
                             AuthorId = user.UserId,
                             ChildName = childrenRoomGood.ChildName.Trim(),
                             CompanyId = customer.CompanyId,
                             Cost = customer.ActiveCard.CustomerCardType.ChildrenCost,
                             CreatedOn = DateTime.Now,
                             CustomerId = customer.Id,
                             DivisionId = division.Id,
                             HealthStatus = childrenRoomGood.HealthStatus,
                             Id = Guid.NewGuid()
                         };
            context.ChildrenRooms.AddObject(cr);
            return cr.Id;
        }

        internal static void ProcessSolariumVisit(TonusEntities context, ServiceModel.Turnover.SolariumGood solariumGood, User user, Division division, PaymentDetails details, Customer customer)
        {
            customer.InitEssentials();
            var sv = context.SolariumVisits.Single(i => i.Id == solariumGood.solariumVisitId);
            if(sv.Cost.HasValue)
            {
                throw new FaultException<string>("Солярий уже оплачен!", "Солярий уже оплачен!");
            }
            sv.TicketId = null;
            sv.Cost = details.RequestedAmount;
            sv.eStatus = SolariumVisitStatus.Completed;
        }

        internal static void ProcessRentPayment(TonusEntities context, RentPayment rentPayment, User user, Division division, PaymentDetails details, Customer customer)
        {
            var rent = context.Rents.Single(i => i.Id == rentPayment.RentId);
            if(rent.Cost == details.RequestedAmount)
            {
                rent.IsPayed = true;
            }
            else
            {
                var res = Localization.Resources.RentPmtErr;
                throw new FaultException<string>(res, res);
            }
        }

        internal static void ProcessCloseRentPayment(TonusEntities context, CloseRentPayment closeRentPayment, User user, Division division, PaymentDetails details, Customer customer)
        {
            var rent = context.Rents.Single(i => i.Id == closeRentPayment.RentId);
            rent.IsPayed = true;
            rent.LostFine = closeRentPayment.LostFine;
            rent.OverdueFine = closeRentPayment.OverdueFine;
            rent.FactReturnDate = DateTime.Now;
            rent.ReturnById = user.UserId;
            rent.IsManualAmount = closeRentPayment.IsManualAmount;
        }

        internal static void ProcessDepositGood(TonusEntities context, DepositGood depositGood, User user, Division division, PaymentDetails details, Customer customer)
        {
            var amount = depositGood.DepositAmount;
            customer.InitActiveCard();
            if(customer.ActiveCard != null)
            {
                amount *= (1 + customer.ActiveCard.CustomerCardType.BonusPercent);
            }

            var da = new DepositAccount
            {
                Amount = amount,
                AuthorId = user.UserId,
                CompanyId = customer.CompanyId,
                CreatedOn = DateTime.Now,
                CustomerId = customer.Id,
                Description = String.Format(Localization.Resources.DepAdd, details.OrderNumber),
                Id = Guid.NewGuid()
            };
            context.DepositAccounts.AddObject(da);
        }

        internal static void ProcessCashlessBasket(TonusEntities context, Division division, BarOrder barOrder, PaymentDetails details, Customer customer, User user, IEnumerable<PayableItem> basket)
        {
            if(basket.Count() > 0)
            {
                if(basket.First() is BarPointGood)
                {
                    //Делать ничего не надо.
                    //Товар спишется при проведении платежа в бухгалтерии.
                }
                else if(basket.First() is TicketGood)
                {
                    PaymentCore.ProcessTicketGood(context, basket, user, division, details, customer, true, barOrder);
                }
                else
                {
                    throw new FaultException<string>(Localization.Resources.NoCashless, Localization.Resources.NoCashless);
                }
            }
            else
            {
                throw new FaultException<string>(Localization.Resources.OrderIsEmpty, Localization.Resources.OrderIsEmpty);
            }
        }

        public static void FinalizeCashlessPayment(Guid orderId, string comments, bool isSuccessful)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var order = context.BarOrders.Single(i => i.Id == orderId);
                order.Init();

                Logger.DBLog("Отметка безналичной оплаты {0} ({1})", order.OrderNumber, isSuccessful);

                order.PaymentComments = comments;
                order.PaymentDate = DateTime.Now;
                if(!isSuccessful)
                {
                    order.Content = null;
                    order.CashPayment = 0;
                    context.SaveChanges();
                    return;
                }
                else
                {
                    foreach(var i in order.GetContent())
                    {
                        if(i is BarPointGood)
                        {
                            var good = i as BarPointGood;
                            if(context.Goods.Any(j => j.Id == good.GoodId))
                            {
                                //Обычный товар, не сертификат
                                var saleInfo = new GoodSale
                                {
                                    Id = Guid.NewGuid(),
                                    GoodId = good.GoodId,
                                    Amount = good.InBasket,
                                    BarOrderId = order.Id,
                                    CompanyId = order.CompanyId,
                                    StorehouseId = good.StorehouseId
                                };

                                saleInfo.PriceMoney = (decimal)good.Price;

                                context.GoodSales.AddObject(saleInfo);
                            }
                            else
                            {
                                //Сертификат
                                var cert = context.Certificates.Single(j => j.Id == good.GoodId);
                                cert.SellDate = DateTime.Now;
                                cert.BuyerId = order.CustomerId;
                            }
                        }
                        else if(i is TicketGood)
                        {
                            var pmtInfo = new TicketPayment
                            {
                                Id = Guid.NewGuid(),
                                TicketId = (i as TicketGood).TicketId,
                                Amount = order.CashPayment,
                                AuthorId = user.UserId,
                                PaymentDate = DateTime.Now,
                                CompanyId = order.CompanyId,
                                ReceiptNumber = Core.GetNewReceiptNumber(user.CompanyId)
                            };
                            context.TicketPayments.AddObject(pmtInfo);
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        public static void ProcessBankReturn(Guid orderId)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var order = context.BarOrders.Single(i => i.Id == orderId);
                order.Init();
                if(order.NeedClosure)
                {
                    order.PaymentDate = DateTime.Now;
                    context.SaveChanges();
                }
                Logger.DBLog("Отметка возврата банка {0}", order.OrderNumber);
            }
        }

        public static void PostCreditTicketPayment(Guid ticketId, decimal bankComissionRur, DateTime paymentDate)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticket = context.Tickets.SingleOrDefault(i => i.Id == ticketId);
                if(ticket == null) return;
                var loan = ticket.Cost - (ticket.TicketPayments.Any() ? ticket.TicketPayments.Sum(o => o.Amount) : 0);
                var tp = new TicketPayment
                {
                    Amount = loan,
                    AuthorId = user.UserId,
                    CompanyId = user.CompanyId,
                    Id = Guid.NewGuid(),
                    PaymentDate = paymentDate,
                    TicketId = ticketId
                };
                context.TicketPayments.AddObject(tp);
                var st = context.SpendingTypes.FirstOrDefault(i => i.CompanyId == user.CompanyId && i.Name == "Комиссия банка по кредитам");
                if(st == null)
                {
                    st = new SpendingType
                    {
                        CompanyId = user.CompanyId,
                        Id = Guid.NewGuid(),
                        Name = "Комиссия банка по кредитам"
                    };
                    context.SpendingTypes.AddObject(st);
                }
                context.Spendings.AddObject(new Spending
                {
                    Amount = bankComissionRur,
                    AuthorId = user.UserId,
                    CompanyId = user.CompanyId,
                    CreatedOn = paymentDate,
                    DivisionId = ticket.DivisionId,
                    Id = Guid.NewGuid(),
                    Name = String.Format("Комиссия банка по абонементу {0} ({1:p2})", ticket.Number, bankComissionRur / loan),
                    Number = context.Spendings.Where(i => i.DivisionId == ticket.DivisionId).Any() ? context.Spendings.Where(i => i.DivisionId == ticket.DivisionId).Max(i => i.Number) + 1 : 1,
                    PaymentType = "Безнал",
                    SpendingTypeId = st.Id
                });
                ticket.CreditComission = bankComissionRur;
                context.SaveChanges();
            }
        }
    }
}
