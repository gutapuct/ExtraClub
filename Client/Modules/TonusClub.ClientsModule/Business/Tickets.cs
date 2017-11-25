﻿using System;
using Telerik.Windows.Controls;
using System.Windows;
using TonusClub.CashRegisterModule;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using FlagmaxControls.BaseClasses;
using TonusClub.Clients.Views.Windows.Tickets;
using TonusClub.UIControls;

namespace TonusClub.Clients.Business
{
    public static class Tickets
    {
        internal static void ProcessTicketPayment(Action<bool> close, CashRegisterManager cashMan, ClientContext context, Customer customer, Ticket ticket)
        {
            if (ticket.CreditInitialPayment.HasValue)
            {
                TonusWindow.Confirm("Кредитный абонемент", "Абонемент был продан в кредит. Провести оплату без печати чека?", w =>
                {
                    ProcessTicketPayment2(close, cashMan, context, customer, ticket, w.DialogResult ?? false);
                });
            }
            else
            {
                ProcessTicketPayment2(close, cashMan, context, customer, ticket, false);
            }
        }

        private static void ProcessTicketPayment2(Action<bool> close, CashRegisterManager cashMan, ClientContext context, Customer customer, Ticket ticket, bool withoutKkm)
        {
            TonusWindow.Prompt(UIControls.Localization.Resources.TicketPayment,
                 UIControls.Localization.Resources.ProvideCashForTicket + ticket.Number.ToString() + ":",
                 "",
                e=>
                {
                    if (e.DialogResult ?? false)
                    {
                        decimal pmt = 0;
                        if (Decimal.TryParse(e.TextResult, out pmt) && IsTicketPaymentAccepted(ticket, pmt))
                        {
                            if (withoutKkm)
                            {
                                close(context.PostTicketPayment(ticket.Id, pmt));
                            }
                            else
                            {
                                cashMan.ProcessPayment(new TicketPaymentPosition(ticket, pmt), customer, pd => close(pd.Success));
                            }
                        }
                        else
                        {
                            TonusWindow.Alert(new DialogParameters
                            {
                                Header = UIControls.Localization.Resources.Error,
                                Content = UIControls.Localization.Resources.UnableToApplyPayment,
                                OkButtonContent = UIControls.Localization.Resources.Ok,
                                Owner = Application.Current.MainWindow
                            });
                        }
                    }
                });
        }

        private static bool IsTicketPaymentAccepted(Ticket ticket, decimal pmt)
        {
            return (pmt > 0 && ticket != null && ticket.Loan >= pmt);
        }

        internal static void ProcessCreditTicketPayment(Action close, ClientContext context, Customer customer, Ticket ticket)
        {
            if (!ticket.CreditInitialPayment.HasValue) return;
            var dlg = new CreditPaymentDialog(context, customer, ticket);
            dlg.Closed = close;
            dlg.ShowDialog();
        }


    }
}
