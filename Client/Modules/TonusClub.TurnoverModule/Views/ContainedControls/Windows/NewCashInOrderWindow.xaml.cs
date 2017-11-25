using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.TurnoverModule.Views.ContainedControls.Windows
{
    public partial class NewCashInOrderWindow
    {
        public CashInOrder Order { get; set; }
        public List<User> Users { get; set; }
        public List<string> Debets { get; set; }

        readonly IReportManager _repMan;

        public NewCashInOrderWindow(IReportManager repMan, CashInOrder order)
        {
            _repMan = repMan;
            InitializeComponent();

            Users = _context.GetUsers();
            Debets = new List<string> { "50.01", "71.01" };
            if (order != null && order.Id != Guid.Empty)
            {
                Order = order;
            }
            else
            {
                Order = new CashInOrder
                {
                    CreatedOn = DateTime.Today,
                    CreatedById = _context.CurrentUser.UserId,
                    Amount = _context.GetCashAmount(DateTime.Today),
                    Id = Guid.NewGuid()
                };
            }
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Order.CreatedOn > DateTime.Today)
            {
                TonusWindow.Alert("Ошибка", "Укажите дату! Будущим ПКО оформлять нельзя.");
                return;
            }
            if (String.IsNullOrEmpty(Order.Debet))
            {
                TonusWindow.Alert("Ошибка", "Укажите дебет!");
                return;
            }
            if (Order.Amount <= 0)
            {
                TonusWindow.Alert("Ошибка", "Укажите сумму!");
                return;
            }
            if (Order.ReceivedById == Guid.Empty)
            {
                TonusWindow.Alert("Ошибка", "Укажите получателя!");
                return;
            }
            if (String.IsNullOrEmpty(Order.Reason))
            {
                TonusWindow.Alert("Ошибка", "Укажите основание!");
                return;
            }

            _context.PostCashInOrder(Order.Id, Order.CreatedOn, Order.Debet, Order.Amount, Order.CreatedById, Order.ReceivedById, Order.Reason);
            DialogResult = true;
            _repMan.ProcessPdfReport(()=>_context.GeneratePkoReport(Order.Id));
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GenPicker_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Order.Amount = _context.GetCashAmount(Order.CreatedOn.Date);
        }
    }
}
