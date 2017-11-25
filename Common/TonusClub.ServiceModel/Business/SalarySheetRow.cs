using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class SalarySheetRow : IInitable
    {
        [DataMember]
        public string SerializedEmployeeName { get; set; }

        [DataMember]
        public decimal SerializedAdvance { get; set; }

        public void Init()
        {
            SerializedEmployeeName = Employee.BoundCustomer.FullName;
            var x = Employee.EmployeePayments.Where(i=>i.SalarySheetId==SalarySheetId);
            if (x.Count() > 0) SerializedAdvance = x.Sum(i => i.Amount);
            else SerializedAdvance = 0;
        }

        partial void OnDeserialized()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SalarySheetRow_PropertyChanged);
        }

        void SalarySheetRow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Bonus")
            {
                OnPropertyChanged("SalaryTotal");
                OnPropertyChanged("TotalToPay");
            }
            if (e.PropertyName == "NDFL" || e.PropertyName == "Ved10" || e.PropertyName == "Ved25")
            {
                OnPropertyChanged("TotalToPay");
            }
        }

        public decimal SalaryTotal
        {
            get
            {
                return Salary + Bonus;
            }
        }

        public decimal TotalToPay
        {
            get
            {
                return SalaryTotal - NDFL - Ved10 - Ved25 - SerializedAdvance;
            }
        }

        /// <summary>
        /// Helper property
        /// </summary>
        public bool CanSalaryChange { get; set; }
    }
}
