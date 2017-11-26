using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views.Windows
{
    /// <summary>
    /// Interaction logic for DepositOutWindow.xaml
    /// </summary>
    public partial class DepositOutWindow
    {

        private decimal _amount;
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                OnPropertyChanged("Amount");
                OnPropertyChanged("TotalAmount");
            }
        }

        public decimal TotalAmount
        {
            get
            {
                return Amount * (1-_context.CurrentCompany.DepositComissionPercent) - _context.CurrentCompany.DepositComissionRub;
            }
        }

        public Customer Customer { get; set; }

        readonly IReportManager _repMan;

        public DepositOutWindow(Customer customer, IReportManager repMan)
        {
            _repMan = repMan;
            Customer = customer;
            InitializeComponent();
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Customer.RurDepositValue < Amount)
            {
                ExtraWindow.Alert("Ошибка","Невозможно вывести средств больше, чем находится на счету клиента!");
                return;
            }
            var res = _context.RequestDepositOut(Customer.Id, Amount);
            _repMan.ProcessPdfReport(() => _context.GenerateDepositOutStatementReport(res));
            DialogResult = true;
            Close();
        }
    }
}
