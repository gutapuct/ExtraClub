using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class EmployeePayment : IInitable
    {
        [DataMember]
        public string SerializedEmployeeName { get; set; }
        [DataMember]
        public string SerializedPaidName { get; set; }

        public void Init()
        {
            SerializedEmployeeName = Employee.BoundCustomer.FullName;
        }

        public string PaymentTypeText
        {
            get
            {
                if(PaymentType==0)
                return "Аванс";
                if (PaymentType == 1)
                    return "Окончательный расчет за месяц";
                if (PaymentType == 2)
                    return "Расчет при увольнении";
                if (PaymentType == 3)
                    return "Оплата больничного/отпускных";
                return "";
            }
        }
    }
}
