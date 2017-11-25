using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;

namespace TonusClub.CashRegisterModule
{
    interface IFiscalRegistrator
    {
        bool ProcessOrder(PaymentDetails pmt, IEnumerable<PayableItem> goods, string OperatorName);
        bool FinishOrder();
        void CancelOrder();
        bool PrintReturn(string OperatorName, decimal amount, int sectionNumber);
        void PrintText(List<string> text, string OperatorName);
        void OpenShift(string userName);
        void CloseShift();

        void ClearDocumentStatus();

        /// <param name="repNum">z = 3</param>
        void PrintReport(string userName, int repNum);
    }
}
