using System;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.Clients.Views.Windows.Tickets
{

    public partial class TicketReturnWindow
    {

        public Customer Customer { get; set; }
        public Ticket Ticket { get; set; }

        private readonly IReportManager _repMan;

        public string Comment { get; set; }

        public TicketReturnWindow(Customer customer, Ticket ticket, IReportManager repMan)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
            Ticket = ticket;
            Customer = customer;
            Ticket.ReturnDate = DateTime.Today;
            _repMan = repMan;

            DataContext = this;
            Closed = TicketReturnWindow_Closed;
        }

        void TicketReturnWindow_Closed()
        {
            if (!(DialogResult ?? false))
            {
                Ticket.ReturnDate = null;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.StartTicketReturn(Ticket.Id, (Comment??"").Trim());
            _repMan.ProcessPdfReport(() => _context.GenerateTicketReturnReport(Ticket.Id));
            DialogResult = true;
            Close();
        }
    }
}
    