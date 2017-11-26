using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.Infrastructure.Extensions;
using ExtraClub.ServiceModel;
//using SPARKAX3Lib;
using ExtraClub.Infrastructure;
using System.IO;


namespace ExtraClub.CashRegisterModule
{
    internal class Spark : IFiscalRegistrator
    {
        dynamic spark;


        void Init()
        {
            var logstr = String.Empty.Log("Начало инициализации устройства");
            try
            {
                if (spark == null)
                {
                    //spark = (ISpark617TF)Activator.CreateComInstanceFrom("Interop.SPARKAX3Lib.dll", "SPARKAX3Lib.Spark617TFClass").Unwrap();
                    var t = System.Type.GetTypeFromProgID("KKS.Spark617TF.1");
                    spark = Activator.CreateInstance(t);

                    logstr = logstr.Log("Объект spark успешно создан\n");
                    ThrowError(spark.SetAccessKey(AppSettingsManager.GetSetting("SparkAccessKey")));
                    logstr = logstr.Log("Ключ доступа задан\n");
                    int comPortNumber = 1;
                    Int32.TryParse(AppSettingsManager.GetSetting("SparkPortNumber"), out comPortNumber);
                    logstr = logstr.Log("Com порт прочитан " + comPortNumber.ToString() + "\n");
                    ThrowError(spark.SetDeviceOpt(1, comPortNumber));//Номер COM-порта
                    ThrowError(spark.InitDevice());
                    logstr = logstr.Log("Инициализация устройства\n");
                    ThrowError(spark.SetDeviceOpt(2, 1));//Включение кеширования чека
                    logstr = logstr.Log("Включение кеширования чека\n");

                    //ThrowError(spark.SetDeviceOpt(7, 0));//Отключение цепочек
                    //logstr += "Отключение цепочек\n";
                    //ThrowError(spark.SetDeviceOpt(4, 0));//Отключение печати НДС
                    //logstr += "Отключение печати НДС\n";
                    //ThrowError(spark.SetDeviceOpt(3, 0));//Отключение печати налогов
                    //logstr += "Отключение печати налогов\n";
                    
                    //ThrowError(spark.SetDeviceOpt(8, 1));//Количество покупок
                    
                    ThrowError(spark.SetDriverOpt(8, 0));//

                    logstr = logstr.Log("Настройки устройства посланы\n");
                    ThrowError(spark.SetOrderHeader(1, AppSettingsManager.GetSetting("Line1KKM"), 1));
                    ThrowError(spark.SetOrderHeader(2, AppSettingsManager.GetSetting("Line2KKM"), 1));
                    ThrowError(spark.SetOrderHeader(3, AppSettingsManager.GetSetting("Line3KKM"), 0));
                    ThrowError(spark.SetOrderHeader(4, AppSettingsManager.GetSetting("Line4KKM"), 0));
                    logstr = logstr.Log("Заголовок чека установлен\n");


                    ThrowError(spark.SetDescriptorText(42, "Заказ №"));

                    logstr = logstr.Log("Инициализация прошла успешно\n");


                    if (spark.GetDeviceInfo(101) != 1) throw new Exception("Ошибка инициализации устройста\n" + logstr);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                try
                {
                    Deinit();
                }
                catch { }
                throw new Exception("Ошибка инициализации устройства: \n" + e.Message + "\n\n" + e.StackTrace + "\n" + logstr, e);
            }
        }

        void Deinit()
        {
            if (spark == null) return;
            ThrowError(spark.DeinitDevice());
            spark = null;
        }

        private void ThrowError(int errNumber)
        {
            if (errNumber == 0) return;
            if (spark == null) throw new NullReferenceException("Spark617TF");
            throw new Exception(spark.GetErrorComment(errNumber));
        }

        private bool _isDocumentInProgress;

        public bool ProcessOrder(ServiceModel.PaymentDetails pmt, IEnumerable<PayableItem> goods, string OperatorName)
        {
            if (pmt.DepositPayment > 0)
            {
                return PrintDepositText(pmt, goods, OperatorName);
            }
            else
            {
                if (pmt.RequestedAmountTotal <= 0) return true;
                var logstr = String.Empty.Log("Начало обработки чека");
                try
                {
                    if (_isDocumentInProgress) return false;
                    _isDocumentInProgress = true;
                    Init();
                    Logger.Log("Начало обработки чека: Отдел {0}, Приведенный отдел {1}, Заказ {2}", pmt.SectionNumber, pmt.SectionNumber < 1 ? 1 : pmt.SectionNumber, pmt.OrderNumber);
                    ThrowError(spark.StartDocument(1, pmt.SectionNumber < 1 ? 1 : pmt.SectionNumber, pmt.OrderNumber, OperatorName));
                    logstr = logstr.Log("Документ открыт\n");

                    ThrowError(spark.SetExtraDocData(pmt.OrderNumber, 0, 0, ""));
                    logstr = logstr.Log("Номер заказа послан\n");

                    foreach (var good in goods)
                    {
                        Logger.Log("Товар {0}, Ценa {1}, Количество {2}", good.Name, (int)(good.Price * 100), good.InBasket * 1000);
                        ThrowError(spark.ItemEx(good.InBasket * 1000, (int)(good.Price * 100), good.Name, 0, pmt.SectionNumber < 1 ? 1 : pmt.SectionNumber));
                    }
                    logstr = logstr.Log("Товары посланы\n");

                    if (pmt.CertificateDicsount.HasValue && pmt.CertificateDicsount > 0)
                    {
                        ThrowError(spark.AbsoluteCorrection((int)Math.Round(-pmt.CertificateDicsount.Value * 100)));
                    }


                    if (pmt.CashPayment > 0)
                    {
                        ThrowError(spark.Tender((int)Math.Round(pmt.CashPayment * 100), 8, "", ""));
                    }
                    if (pmt.CardPayment > 0)
                    {
                        ThrowError(spark.Tender((int)Math.Round(pmt.CardPayment * 100), 5, pmt.CardNumber ?? "", pmt.CardAuth ?? ""));
                    }
                    if (pmt.DepositPayment > 0)
                    {
                        ThrowError(spark.Tender((int)Math.Round(pmt.DepositPayment * 100), 7, "", ""));
                    }
                    logstr = logstr.Log("Платежи посланы\n");
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    _isDocumentInProgress = false;
                    if (spark != null)
                    {
                        spark.CancelDocument();
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
            Logger.Log("Подтверждение чека...");
            if (!_isDocumentInProgress) return false;
            _isDocumentInProgress = false;
            try
            {
                ThrowError(spark.EndDocument());
                return true;
            }
            catch(Exception w)
            {
                Logger.Log("Ошибка при подтверждении чека!");
                Logger.Log(w);
                spark.CancelDocument();
                throw;
            }
            finally
            {
                try
                {
                    Deinit();
                    Logger.Log("Чек подтвержден!");
                }
                catch
                {
                }
            }
        }

        public void CancelOrder()
        {
            Logger.Log("Отмена чека...");

            if (!_isDocumentInProgress) return;
            _isDocumentInProgress = false;
            spark.CancelDocument();
            Deinit();
            Logger.Log("Чек отменен!");

        }

        public bool PrintReturn(string OperatorName, decimal amount, int sectionNumber)
        {
            var logstr = "Начало обработки чека";
            try
            {
                Init();
                ThrowError(spark.SetClerk(OperatorName));
                logstr += "Кассир задан\n";
                ThrowError(spark.StartDocument(6, sectionNumber, 1, OperatorName));
                logstr += "Документ открыт\n";
                //ThrowError(spark.Item(1000, (int)(amount * 100), "Возврат товара", 0));
                //logstr += "Позиция послана\n";
                ThrowError(spark.Tender((int)(amount * 100), 8, "", ""));
                logstr += "Платеж послан\n";
                ThrowError(spark.EndDocument());
                logstr += "Документ закрыт\n";
            }
            catch (Exception ex)
            {
                throw new Exception(logstr + ex.Message);
            }
            return true;
        }

        public void PrintText(List<string> text, string OperatorName)
        {
            if (_isDocumentInProgress) throw new Exception("На ККМ уже печатается чек!");
            Logger.Log("Начало печати текста");
            try
            {
                Init();
                try
                {
                    ThrowError(spark.StartDocument(3, 1, 0, String.IsNullOrEmpty(OperatorName) ? "Кассир" : OperatorName));
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message + "\n" + (String.IsNullOrEmpty(OperatorName) ? "Кассир" : OperatorName));
                }
                foreach (var l in text)
                {
                    ThrowError(spark.TextLine(l));
                }
                spark.EndDocument();
            }
            catch(Exception w)
            {
                Logger.Log("Ошибка при печати текста");
                Logger.Log(w);
                spark.CancelDocument();
                throw;
            }
            finally
            {
                Logger.Log("Окончание печати текста");
                Deinit();
            }
        }

        public void OpenShift(string userName)
        {
            try
            {
                Init();
                ThrowError(spark.SetDeviceOpt(5, 1));
                ThrowError(spark.SetDeviceOpt(6, 1));
                ThrowError(spark.SetDriverOpt(7, 0));
                ThrowError(spark.SetDeviceOpt(9, 1));
                ThrowError(spark.StartSession(userName, 0));
            }
            finally
            {
                Deinit();
            }
        }

        public void CloseShift()
        {
            try
            {
                Init();
                ThrowError(spark.EndSession());
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
                Init();
                ThrowError(spark.SetClerk(userName));
                ThrowError(spark.PrintReport(repNum));
            }
            finally
            {
                Deinit();
            }
        }


        public void ClearDocumentStatus()
        {
        }
    }
}