using System;
using System.Windows;
using TonusClub.ServiceModel;
using System.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class IllWindow
    {
        public Employee Employee { get; set; }
        IReportManager _repMan;


        private DateTime _BeginDate;
        public DateTime BeginDate
        {
            get
            {
                return _BeginDate;
            }
            set
            {
                _BeginDate = value;
                OnPropertyChanged("BeginDate");
                OnPropertyChanged("IllLength");
                var salary = _context.GetEmployeeIllnessPmt(Employee.Id, BeginDate, EndDate);
                PayAmount = salary.Key;
                _LogMessage = salary.Value;

            }
        }


        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                OnPropertyChanged("EndDate");
                OnPropertyChanged("IllLength");
                var salary = _context.GetEmployeeIllnessPmt(Employee.Id, BeginDate, EndDate);
                PayAmount = salary.Key;
                _LogMessage = salary.Value;
           }
        }

        public int IllLength
        {
            get
            {
                return (int)((EndDate - BeginDate).TotalDays + 1);
            }
        }


        private decimal _NDFL;
        public decimal NDFL
        {
            get
            {
                return _NDFL;
            }
            set
            {
                _NDFL = value;
                OnPropertyChanged("NDFL");
                OnPropertyChanged("TotalToPay");
            }
        }

        public decimal TotalToPay
        {
            get
            {
                return PayAmount - NDFL;
            }
        }

        private string _LogMessage;

        private decimal _PayAmount;
        public decimal PayAmount
        {
            get
            {
                return _PayAmount;
            }
            set
            {
                _PayAmount = value;
                OnPropertyChanged("PayAmount");
                OnPropertyChanged("TotalToPay");
            }
        }


        public IllWindow(ClientContext context, Employee employee, IReportManager repMan)
            : base(context)
        {
            _repMan = repMan;
            Employee = employee;
            BeginDate = DateTime.Today.AddDays(1);
            EndDate = DateTime.Today.AddDays(14);
            DataContext = this;
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            try {
            var vacId = _context.PostEmployeeVacation(Employee.Id, BeginDate, EndDate, 1, TotalToPay, NDFL, _LogMessage);

            DialogResult = true;
            Close();
            }
            catch (FaultException ex)
            {
                TonusWindow.Alert("Ошибка", ex.Message);
            }

        }

        private void ShowLogClick(object sender, RoutedEventArgs e)
        {
            TonusWindow.Alert("Результаты расчета", _LogMessage);
        }
    }
}
