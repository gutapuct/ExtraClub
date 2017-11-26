using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OposFiscalPrinter_1_11_Lib;
using ExtraClub.Infrastructure;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Extensions;
using System.Windows;

namespace ExtraClub.CashRegisterModule
{
    class Pirit : IFiscalRegistrator
    {
        bool isDocumentInProgress = false;

        OPOSFiscalPrinter p;

        void Init(string operatorName, ref string Log)
        {
            p = new OPOSFiscalPrinter();
            Log += "Ком-объект создан\n";
            Check(p.Open("pirit"));
            Log += "Ком-объект открыт\n";
            Check(p.ClaimDevice(3000));
            Log += "Ком-объект отвечает\n";
            Log += "Фамилия кассира " + (operatorName ?? "?") + "\n";

            //if (!p.DayOpened)
            {
                if (p.CapSetPOSID)
                {
                    Log += "Назначаем фамилию\n";
                    try
                    {
                        Check(p.SetPOSID("", Convert(operatorName)));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    Log += "Фамилия назначена\n";
                }
                else
                {
                    Log += "Назначение фамилии невозможно\n";
                }
            }


            Log += "Оператор назначен\n";

            p.SetHeaderLine(0, Convert(AppSettingsManager.GetSetting("Line1KKM")), true);
            p.SetHeaderLine(1, Convert(AppSettingsManager.GetSetting("Line2KKM")), true);
            p.SetHeaderLine(2, Convert(AppSettingsManager.GetSetting("Line3KKM")), false);
            p.SetHeaderLine(3, Convert(AppSettingsManager.GetSetting("Line4KKM")), false);
            Log += "Заголовок установлен\n";
        }

        void Deinit()
        {
            if (p == null) return;
            p.ReleaseDevice();
        }

        public bool ProcessOrder(PaymentDetails pmt, IEnumerable<PayableItem> goods, string OperatorName)
        {
            if (pmt.DepositPayment > 0)
            {
                return PrintDepositText(pmt, goods, OperatorName);
            }
            else
            {
                if (isDocumentInProgress) return false;
                isDocumentInProgress = true;
                if (pmt.RequestedAmountTotal <= 0) return true;
                var logstr = "Начало обработки чека";
                try
                {
                    string Log = "";
                    Init(OperatorName, ref Log);
                    Check(p.BeginFiscalReceipt(true));
                    logstr += "Документ открыт\n";
                    foreach (var good in goods)
                    {
                        var goodName = good.Name;
                        if (goodName.Length > 32) goodName = goodName.Substring(0, 32);
                        Check(p.PrintRecItem(Convert(goodName), 0, good.InBasket * 1000, 0, good.Price, ""));
                    }
                    logstr += "Товары посланы\n";

                    if (pmt.CertificateDicsount.HasValue && pmt.CertificateDicsount > 0)
                    {
                        Check(p.PrintRecSubtotalAdjustment(1, Convert("Сертификат"), pmt.CertificateDicsount.Value));
                    }

                    if (pmt.CashPayment > 0)
                    {
                        Check(p.PrintRecTotal(0, (decimal)pmt.CashPayment, "0"));
                    }
                    if (pmt.CardPayment > 0)
                    {
                        Check(p.PrintRecTotal(0, (decimal)pmt.CardPayment, "1"));
                    }
                    logstr += "Платежи посланы\n";
                    return true;
                }
                catch (Exception e)
                {
                    isDocumentInProgress = false;
                    if (p != null)
                    {
                        p.ResetPrinter();
                    }
                    Deinit();
                    throw new Exception("Ошибка при печати чека: \n" + e.Message + "\n" + logstr, e);
                }
            }
        }

        private bool PrintDepositText(PaymentDetails pmt, IEnumerable<PayableItem> goods, string OperatorName)
        {
            try
            {
                var str = new List<string>();
                str.AddRange(String.Format("Заказ №{0}", pmt.OrderNumber).SplitByLen(35));
                var n = 1;
                foreach (var i in goods)
                {
                    str.AddRange(String.Format("{0}. {1} {2} x {3}, {4}", n, i.Name, i.InBasket, i.UnitName, i.Cost).SplitByLen(35));
                    n++;
                }

                str.Add("");
                if (pmt.RequestedAmount != pmt.RequestedAmountTotal)
                {
                    str.Add("Итого: " + pmt.RequestedAmount.ToString("c"));
                    str.Add("Скидка: " + (pmt.RequestedAmount - pmt.RequestedAmountTotal).ToString("c"));
                }
                str.Add("К списанию: " + pmt.RequestedAmountTotal.ToString("c"));
                str.Add("");
                str.Add("Со списанием с депозита согласен:");
                str.Add("____________________________________");

                PrintText(str, OperatorName);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool FinishOrder()
        {
            if (!isDocumentInProgress) return false;
            isDocumentInProgress = false;
            try
            {
                Check(p.EndFiscalReceipt(false));
                return true;
            }
            catch (Exception ex)
            {
                if (p != null)
                {
                    p.ResetPrinter();
                    Deinit();
                }
                throw ex;
            }
        }

        public void CancelOrder()
        {
            if (!isDocumentInProgress) return;
            isDocumentInProgress = false;
            p.ResetPrinter();
            Deinit();
        }

        public bool PrintReturn(string OperatorName, decimal amount, int sectionNumber)
        {
            string Log = "";
            try
            {
                Init(OperatorName, ref Log);
                p.FiscalReceiptType = 7;
                Check(p.BeginFiscalReceipt(true));
                Log += "Начат фискальный документ\n";

                Check(p.PrintRecItem(Convert("Возврат"), 0, 1000, 0, amount, ""));
                p.PrintRecTotal(0, amount, "0");
                Check(p.EndFiscalReceipt(false));
                
                Log += "Закрыт нефискальный документ\n";
                return true;
            }
            catch
            {
                Log += "Сбрасываем принтер...\n";
                p.ResetPrinter();
                Log += "Принтер сброшен\n";
                MessageBox.Show(Log);
                throw;
            }
            finally
            {
                p.FiscalReceiptType = 4;
                Deinit();
            }
        }

        public void PrintText(List<string> text, string OperatorName)
        {
            string Log = "";
            if (isDocumentInProgress) throw new Exception("На ККМ уже печатается чек!");
            try
            {
                Init(OperatorName, ref Log);
                p.BeginNonFiscal();
                Log += "Начат нефискальный документ\n";
                foreach (var l in text)
                {
                    Check(p.PrintNormal(2, Convert(l)));
                    Log += "Распечатана строка\n";

                }
                p.EndNonFiscal();
                Log += "Закрыт нефискальный документ\n";
            }
            catch
            {
                Log += "Сбрасываем принтер...\n";
                p.ResetPrinter();
                Log += "Принтер сброшен\n";
                MessageBox.Show(Log);
                throw;
            }
            finally
            {
                Deinit();
            }
        }

        public void OpenShift(string userName)
        {
            throw new Exception("Не поддерживается ФР!");
        }

        public void CloseShift()
        {
            try
            {
                string Log = "";
                Init("Кассир", ref Log);
                Check(p.PrintZReport());
            }
            finally
            {
                Deinit();
            }
        }

        public void PrintReport(string userName, int repNum)
        {
            try
            {
                string Log = "";
                Init(userName, ref Log);
                if (repNum == 1) Check(p.PrintXReport());
                if (repNum == 2)
                {
                    Check(p.PrintXReport());
                }
                if (repNum == 3)
                {
                    Check(p.PrintZReport());
                }
            }
            finally
            {
                Deinit();
            }
        }

        private void Check(int errCode)
        {
            if (errCode != 0)
            {
                if (p!= null)
                {
                    p.ResetPrinter();
                }
                throw new Exception(errCode.ToString());
            }
        }

        public static string Convert(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return " ";
            value = value.Replace("\"", "").Replace("-", "").Replace("(", "").Replace(")", "");
            Decoder dec = Encoding.Default.GetDecoder();
            byte[] ba = Encoding.GetEncoding(866).GetBytes(value);
            int len = dec.GetCharCount(ba, 0, ba.Length);
            char[] ca = new char[len];
            dec.GetChars(ba, 0, ba.Length, ca, 0);
            return new string(ca);
        }

        public void ClearDocumentStatus()
        {
            isDocumentInProgress = false;
        }
    }
}
