using System;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for EmitCardWindow.xaml
    /// </summary>
    public partial class EmitCardWindow
    {

        private string _NewCardNumber;
        public string NewCardNumber
        {
            get
            {
                return _NewCardNumber;
            }
            set
            {
                if (_NewCardNumber != value)
                {
                    _NewCardNumber = value;
                    var msg = _context.GetEmployeeCardStatusMessage(_NewCardNumber);
                    if (!String.IsNullOrEmpty(msg))
                    {
                        CardInfoBox.Text = msg;
                        CardInfoDiv.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        CardInfoBox.Text = String.Empty;
                        CardInfoDiv.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public Employee Employee { get; set; }

        public EmitCardWindow(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            this.DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(NewCardNumber))
            {
                TonusWindow.Alert("Ошибка", "Необходимо указать номер карты!");
                return;
            }
            _context.PostEmployeeCard(Employee.Id, NewCardNumber);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
