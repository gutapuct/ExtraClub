using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.CashRegisterModule
{
    class DummyRegistrator : IFiscalRegistrator
    {
        public void Init()
        {
        }

        public void Deinit()
        {
        }

        public bool ProcessOrder(ServiceModel.PaymentDetails pmt, IEnumerable<ServiceModel.PayableItem> goods, string OperatorName)
        {
            return true;
        }

        public bool FinishOrder()
        {
            return true;
        }

        public void CancelOrder()
        {
        }

        public bool PrintReturn(string OperatorName, decimal amount, int sectionNumber)
        {
            return true;
        }

        public void PrintText(List<string> text, string OperatorName)
        {
        }

        public void OpenShift(string userName)
        {
        }

        public void CloseShift()
        {
        }

        public void PrintReport(string userName, int repNum)
        {
        }


        public void ClearDocumentStatus()
        {
        }
    }
}
