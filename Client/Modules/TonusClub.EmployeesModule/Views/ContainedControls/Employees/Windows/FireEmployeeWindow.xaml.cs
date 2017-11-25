using System;
using System.Linq;
using System.Windows;
using TonusClub.ServiceModel;
using System.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class FireEmployeeWindow
    {
        public Employee Employee { get; set; }


        private string _fireCause;
        public string FireCause
        {
            get
            {
                return _fireCause;
            }
            set
            {
                _fireCause = value;
                OnPropertyChanged("FireCause");
            }
        }


        private decimal _bonus;
        public decimal Bonus
        {
            get
            {
                return _bonus;
            }
            set
            {
                _bonus = value;
                OnPropertyChanged("Bonus");
                OnPropertyChanged("TotalToPay");
            }
        }


        private decimal _ndfl;
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

        public decimal TotalToPay
        {
            get
            {
                return PayAmount + Bonus - NDFL;
            }
        }

        private string _logMessage;

        private DateTime _fireDate;
        public DateTime FireDate
        {
            get
            {
                return _fireDate;
            }
            set
            {
                _fireDate = value;
                var salary = _context.GetFireSalary(Employee.Id, FireDate);
                PayAmount = salary.Key;
                _logMessage = salary.Value;
                OnPropertyChanged("FireDate");
            }
        }

        IReportManager _repMan;


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

        public FireEmployeeWindow(ClientContext context, Employee employee, IReportManager repMan)
        {
            InitializeComponent();
            var cust = context.GetCustomer(employee.BoundCustomerId, true);
            var ticks = context.GetTicketsForPlanning(employee.BoundCustomerId);
            if (cust.ActiveCard != null)
            {
                if (cust.RurDepositValue > 0 || ticks.Any(i => i.Status == TicketStatus.Active || i.Status == TicketStatus.Available))
                {
                    CardReturnBox.Content = "Карта №" + cust.ActiveCard.CardBarcode + " остается в качестве клиентской";
                    CardReturnBox.IsEnabled = false;
                }
                else
                {
                    CardReturnBox.Content = "Карта №" + cust.ActiveCard.CardBarcode + " возвращена";
                }
            }
            else
            {
                CardReturnBox.Content = "Сотруднику не выдавалась смарт-карта";
                CardReturnBox.IsEnabled = false;
            }
            Employee = employee;
            FireDate = DateTime.Today;

            FireDateBox.SelectableDateStart = Employee.SerializedJobPlacement.ApplyDate;
            _repMan = repMan;
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = _context.PostEmployeeFire(Employee.Id, FireDate, FireCause, CardReturnBox.IsChecked ?? false, TotalToPay, Bonus, NDFL, _logMessage);
                if (PrintFireOrderBox.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateEmployeeFireOrder(res));
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
            TonusWindow.Alert("Отчет о формировании суммы расчета", _logMessage);
        }
    }
}
