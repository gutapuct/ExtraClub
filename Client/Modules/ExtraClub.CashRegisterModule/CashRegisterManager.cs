using System;
using System.Collections.Generic;
using System.Linq;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;
using ExtraClub.Infrastructure;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.CashRegisterModule
{
    public class CashRegisterManager
    {
        private ClientContext Context
        {
            get
            {
                return ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();
            }
        }
        private IDictionaryManager DictMgr
        {
            get
            {
                return ApplicationDispatcher.UnityContainer.Resolve<IDictionaryManager>();
            }
        }
        private IReportManager RepMan
        {
            get
            {
                return ApplicationDispatcher.UnityContainer.Resolve<IReportManager>();
            }
        }

        private readonly IFiscalRegistrator _fiscalRegistrator;

        public CashRegisterManager()
        {
            if (AppSettingsManager.GetSetting("UseKKM") == "2")
            {
                _fiscalRegistrator = new Pirit();
            }
            else if (AppSettingsManager.GetSetting("UseKKM") == "1")
            {
                _fiscalRegistrator = new Spark();
            }
            else
            {
                _fiscalRegistrator = new DummyRegistrator();
            }
        }

        public void ProcessPayment(PaymentDetails pmt, IEnumerable<PayableItem> goods, bool isBonusPmt, Customer customer, Guid goodActionId, Action<PaymentDetails> onFinish)
        {
            pmt.DivisionId = Context.CurrentDivision.Id;
            if (!isBonusPmt)
            {
                ProcessMoneyPayment(pmt, goods, customer, goodActionId, onFinish);
            }
            else
            {
                //Если платим бонусами - то просто отмечаем на сервере.
                pmt = Context.ProcessPayment(pmt, goods, Guid.Empty);
                onFinish(pmt);
            }
        }

        private void ProcessMoneyPayment(PaymentDetails pmt, IEnumerable<PayableItem> goods, Customer customer, Guid goodActionId, Action<PaymentDetails> onFinish)
        {

            //Вначале показываем сумму к оплате.
            //Указываем, списывать ли деньги с депозита, или оплата налом - и сколько внесено в кассу.

            PaymentWindow pmtWindow;
            var payableItems = goods as PayableItem[] ?? goods.ToArray();
            var pmtModel = new PaymentWindowModel(Context, DictMgr, pmt, payableItems, customer);
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                pmtWindow = new PaymentWindow(pmtModel) { PaymentDetails = pmt };
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            pmtWindow.Closed = () =>
            {
                if (pmtWindow.DialogResult ?? false)
                {
                    try
                    {
                        //Загоняем в кассу чек в режиме кеширования
                        if (Context.CurrentDivision.RCashRegister && !pmt.Cashless && pmtModel.PayWithFR)
                        {
                            if (!(pmt.CardPayment > 0 && !Context.CurrentDivision.RReceiptOnBank))
                            {
                                if (!_fiscalRegistrator.ProcessOrder(pmt, payableItems, Context.CurrentUser.FullName)) throw new Exception("Другой чек уже оформляется");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        pmt.Success = false;
                        pmt.Description = UIControls.Localization.Resources.BillPrintError + ":\n" + e.Message;
                        ExtraWindow.Alert(new DialogParameters
                        {
                            Header = UIControls.Localization.Resources.Error,
                            Content = UIControls.Localization.Resources.UnableToProcessPayment + ":\n" + e.Message,
                            OkButtonContent = UIControls.Localization.Resources.Ok,
                            Owner = Application.Current.MainWindow
                        });
                        onFinish(pmt);
                        return;
                    }
                    //Шлем на сервер оплату
                    //при успешном ответе сервера печатаем чек, при неуспешном - отмена чека.
                    try
                    {

                        pmt = Context.ProcessPayment(pmt, payableItems, goodActionId);
                    }
                    catch (Exception e)
                    {
                        if (Context.CurrentDivision.RCashRegister && !pmt.Cashless && pmtModel.PayWithFR)
                        {
                            if (!(pmt.CardPayment > 0 && !Context.CurrentDivision.RReceiptOnBank))
                            {
                                _fiscalRegistrator.CancelOrder();
                            }
                        }
                        ExtraWindow.Alert(new DialogParameters
                        {
                            Header = UIControls.Localization.Resources.Error,
                            Content = UIControls.Localization.Resources.UnablePmtServer + "\n" + e.Message,
                            OkButtonContent = UIControls.Localization.Resources.Ok,
                            Owner = Application.Current.MainWindow
                        });
                    }
                    if (Context.CurrentDivision.RCashRegister && !pmt.Cashless && pmtModel.PayWithFR)
                    {
                        if (!(pmt.CardPayment > 0 && !Context.CurrentDivision.RReceiptOnBank))
                        {
                            if (pmt.RequestedAmountTotal > 0 || (!(_fiscalRegistrator is Pirit)))
                            {
                                if (pmt.Success)
                                {
                                    _fiscalRegistrator.FinishOrder();
                                }
                                else
                                {
                                    _fiscalRegistrator.CancelOrder();
                                }
                            }
                            else
                            {
                                _fiscalRegistrator.ClearDocumentStatus();
                            }
                        }
                    }
                    if (pmt.Success)
                    {
                        if (pmtWindow.PrintTov.IsChecked ?? false)
                        {
                            RepMan.ProcessPdfReport(() => Context.GenerateCashMemoReport(pmt.OrderNumber));
                        }
                        if (pmt.Cashless)
                        {
                            RepMan.ProcessPdfReport(() => Context.GenerateOrderBillReport(pmt.OrderNumber));
                            RepMan.ProcessPdfReport(() => Context.GenerateOrderContractReport(pmt.OrderNumber));
                        }
                    }
                }
                onFinish(pmt);
            };

            pmtWindow.ShowDialog();
        }

        public PaymentDetails ProcessReturn(PaymentDetails pmt, IEnumerable<PayableItem> items)
        {
            //try
            //{
            //    //Загоняем в кассу чек в режиме кеширования
            //    //if (!Spark.ProcessReturn(pmt, items, _context.CurrentUser.FullName)) throw new Exception("Другой чек уже оформляется");
            //}
            //catch (Exception e)
            //{
            //    pmt.Success = false;
            //    pmt.Description = UIControls.Localization.Resources.BillError + ":\n" + e.Message;
            //    return pmt;
            //}

            //Шлем на сервер оплату
            //при успешном ответе сервера печатаем чек, при неуспешном - отмена чека.
            try
            {
                pmt.DivisionId = Context.CurrentDivision.Id;
                pmt = Context.ProcessReturn(pmt, items);

            }
            catch (Exception e)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnablePmtServer + ":\n" + e.Message,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = Application.Current.MainWindow
                });
            }

            //if (pmt.Success)
            //{
            //    //Spark.FinishOrder();
            //}
            //else
            //{
            //    //Spark.CancelOrder();
            //}

            return pmt;
        }

        public bool PrintText(List<string> text)
        {
            try
            {
                _fiscalRegistrator.PrintText(text, UIControls.Localization.Resources.Cashier1);
            }
            catch (Exception e)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnablePrintBill + ":\n" + e.Message,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = Application.Current.MainWindow
                });
                return false;
            }
            return true;
        }

        public void ProcessPayment(PayableItem item, Customer customer, Action<PaymentDetails> onFinish, bool cashless = false)
        {
            ProcessPayment(new PaymentDetails(customer.Id, item.Cost, cashless), new[] { item }, false, customer, Guid.Empty, onFinish);
        }

        public void OpenShift()
        {
            try
            {
                _fiscalRegistrator.OpenShift(Context.CurrentUser.FullName);
            }
            catch (Exception e)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnableToOpenShift + ":\n" + e.Message
                });
            }
        }

        public void CloseShift()
        {
            try
            {
                _fiscalRegistrator.CloseShift();
            }
            catch (Exception e)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnableToCloseShift + ":\n" + e.Message
                });
            }
        }

        public void PrintReport(int repNum)
        {
            try
            {
                _fiscalRegistrator.PrintReport(Context.CurrentUser.FullName, repNum);
            }
            catch (Exception e)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnableToTakeReport + ":\n" + e.Message
                });
            }
        }

        public void PrintReturn(decimal amount, string clerkName, int sectionNumber)
        {
            try
            {
                _fiscalRegistrator.PrintReturn(clerkName, amount, sectionNumber);
            }
            catch (Exception e)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnableToPrintRefund + ".\n" + e.Message
                });
            }
        }
    }
}
