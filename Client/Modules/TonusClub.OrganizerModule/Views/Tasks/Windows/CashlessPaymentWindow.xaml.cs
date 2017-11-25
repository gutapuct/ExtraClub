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
using System.Windows.Shapes;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.OrganizerModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for CashlessPaymentWindow.xaml
    /// </summary>
    public partial class CashlessPaymentWindow
    {
        public Customer Customer { get; set; }
        public BarOrder BarOrder { get; set; }
        public string Comments { get; set; }

        public CashlessPaymentWindow(BarOrder item)
        {
            BarOrder = item;
            Customer = _context.GetCustomer(item.CustomerId);
            DataContext = this;
            InitializeComponent();
        }

        private void MatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Comments))
            {
                TonusWindow.Alert("Ошибка", "Необходимо указать комментарий!");
                return;
            }
            _context.FinalizeCashlessPayment(BarOrder.Id, Comments.Trim(), true);
            DialogResult = true;
            Close();
        }

        private void UnmatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Comments))
            {
                TonusWindow.Alert("Ошибка", "Необходимо указать комментарий!");
                return;
            }
            _context.FinalizeCashlessPayment(BarOrder.Id, Comments.Trim(), false);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
