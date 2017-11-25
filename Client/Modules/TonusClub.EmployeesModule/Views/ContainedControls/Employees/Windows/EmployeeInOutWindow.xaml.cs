using System.Windows;
using System.Windows.Input;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for EmployeeInOutWindow.xaml
    /// </summary>
    public partial class EmployeeInOutWindow
    {

        private Employee _Employee;
        public Employee Employee
        {
            get
            {
                return _Employee;
            }
            set
            {
                _Employee = value;
                OnPropertyChanged("Employee");
            }
        }


        private string _CardNumber;
        public string CardNumber
        {
            get
            {
                return _CardNumber;
            }
            set
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    if (Employee != null && !_context.CheckPermission("EmployeeInOutSelf") && _context.CurrentUser.EmployeeId == Employee.Id)
                    {
                        TonusWindow.Alert("Ошибка", "Недостаточно прав для регистрации входа/выхода самого себя.");
                    }
                    else
                    {
                        _CardNumber = value;
                        Employee = _context.GetEmployeeByCard(_CardNumber);
                    }
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
                OnPropertyChanged("CardNumber");
            }
        }

        public EmployeeInOutWindow(ClientContext context)
            : base(context)
        {
            InitializeComponent();
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RegisterInButtonClick(object sender, RoutedEventArgs e)
        {
            if (Employee == null) return;
            _context.PostEmployeeVisit(Employee.Id, true);
            DialogResult = true;
            Close();
        }

        private void RegisterOutButtonClick(object sender, RoutedEventArgs e)
        {
            if (Employee == null) return;
            _context.PostEmployeeVisit(Employee.Id, false);
            DialogResult = true;
            Close();
        }
    }
}