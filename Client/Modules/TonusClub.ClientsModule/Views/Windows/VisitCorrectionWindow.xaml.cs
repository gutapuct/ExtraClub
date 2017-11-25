using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.Clients.Views.Windows
{
    /// <summary>
    /// Interaction logic for VisitCorrectionWindow.xaml
    /// </summary>
    public partial class VisitCorrectionWindow
    {
        public List<TreatmentEvent> TreatmentEvents { get; set; }
        public Customer Customer { get; set; }

        public VisitCorrectionWindow(ClientContext context, Customer customer)
            : base(context)
        {
            InitializeComponent();
            if (customer != null)
            {
                TreatmentEvents = context.GetCustomerEvents(customer.Id, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1), false).Where(i => i.VisitStatus == 0 || i.VisitStatus == 2).OrderBy(i => i.VisitDate).ToList();
                TreatmentEvents.ForEach(i => { if (i.VisitStatus == 2) i.Helper = true; });
            }
            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var visited = TreatmentEvents.Where(i => i.Helper && i.VisitStatus == 0).Select(i => i.Id).ToArray();
            _context.MarkTreatmentsVisited(visited);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
