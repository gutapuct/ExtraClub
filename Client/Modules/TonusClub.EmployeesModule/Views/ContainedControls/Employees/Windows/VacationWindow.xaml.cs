using System;
using System.Windows;
using TonusClub.ServiceModel;
using System.ServiceModel;
using TonusClub.UIControls.Windows;
using TonusClub.ServiceModel.Employees;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class VacationWindow
    {
        public Employee Employee { get; set; }
        readonly IReportManager _repMan;


        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get
            {
                return _beginDate;
            }
            set
            {
                _beginDate = value;
                OnPropertyChanged("BeginDate");
                OnPropertyChanged("VacationLength");
                var salary = _context.GetEmployeeVacationPmt(Employee.Id, BeginDate, EndDate);
                PayAmount = salary.Key;
                _logMessage = salary.Value;
            }
        }


        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
                OnPropertyChanged("VacationLength");
                var salary = _context.GetEmployeeVacationPmt(Employee.Id, BeginDate, EndDate);
                PayAmount = salary.Key;
                _logMessage = salary.Value;

           }
        }

        public int VacationLength => (int)((EndDate - BeginDate).TotalDays + 1);


        private decimal _ndfl;
        // ReSharper disable once InconsistentNaming
        public decimal NDFL
        {
            get
            {
                return _ndfl;
            }
            set
            {
                _ndfl = value;
                OnPropertyChanged("NDFL");
                OnPropertyChanged("TotalToPay");
            }
        }

        public decimal TotalToPay => PayAmount - NDFL;

        private string _logMessage;

        private decimal _payAmount;
        public decimal PayAmount
        {
            get
            {
                return _payAmount;
            }
            set
            {
                _payAmount = value;
                OnPropertyChanged("PayAmount");
                OnPropertyChanged("TotalToPay");
            }
        }


        public VacationWindow(ClientContext context, Employee employee, IReportManager repMan, EmployeeScheduleProposalElement schedule)
        {
            _repMan = repMan;

            if (schedule.EmployeeId != Guid.Empty)
            {
                Employee = context.GetEmployeeById(schedule.EmployeeId);
                BeginDate = schedule.Start;
                EndDate = schedule.Finish;
            }
            else
            {
                Employee = employee;
                BeginDate = DateTime.Today.AddDays(1);
                EndDate = DateTime.Today.AddDays(7);
            }
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
            var vacId = _context.PostEmployeeVacation(Employee.Id, BeginDate, EndDate, 0, TotalToPay, NDFL, _logMessage);
            if (PrintVacationOrderBox.IsChecked ?? false)
            {
                _repMan.ProcessPdfReport(() => _context.GenerateEmployeeVacationOrder(vacId));
            }

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
            TonusWindow.Alert("Результаты расчета", _logMessage);
        }
    }
}
