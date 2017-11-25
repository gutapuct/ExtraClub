using System.Linq;
using System.Windows;
using TonusClub.CashRegisterModule;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.OrganizerModule.Views.Tasks.Windows
{
    /// <summary>
    /// Interaction logic for TicketReturnWindow.xaml
    /// </summary>
    public partial class TicketReturnWindow
    {
        public Ticket Ticket { get; set; }
        public Customer Customer { get; set; }

        public string Comment { get; set; }

        public decimal ReturnAmount { get; set; }

        bool _isSigned;
        public bool IsSigned
        {
            get
            {
                return _isSigned;
            }
            set
            {
                if (_isSigned != value)
                {
                    _isSigned = value;
                    OnPropertyChanged("IsSigned");
                }
            }
        }

        readonly CashRegisterManager _cashMan;
        readonly IReportManager _repMan;

        public TicketReturnWindow(ClientContext context, Ticket item, CashRegisterManager cashMan, IReportManager repMan)
        {
            
            _cashMan = cashMan;
            InitializeComponent();
            Ticket = context.GetTicketById(item.Id);
            Customer = context.GetCustomer(item.CustomerId);

            _repMan = repMan;

            var tf = Ticket.SerializedTicketFreezes.OrderByDescending(i => i.CreatedOn).FirstOrDefault();
            if (tf != null)
            {
                Comment = tf.Comment;
            }

            ReturnAmount = Ticket.ReturnAmount;
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var pmt = new PaymentDetails(Customer.Id, -Ticket.ReturnAmount, false);

            pmt = _cashMan.ProcessReturn(pmt, new[] { new TicketReturnPosition{ 
                Name = $"Возврат абонемента №{Ticket.Number}",
                Price = -ReturnAmount,
                TicketId = Ticket.Id
            } });
            if (pmt.Success)
            {
                _repMan.ProcessPdfReport(() => _context.GenerateRkoForTicketReturn(Ticket.Id));
                try
                {
                    DialogResult = true;
                }
                catch
                {
                    // ignored
                }
                Close();
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostReturnReject(Ticket.Id);
            DialogResult = true;
            Close();
        }
    }
}
