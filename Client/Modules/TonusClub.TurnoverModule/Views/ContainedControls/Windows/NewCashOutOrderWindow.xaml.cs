using System;
using System.Collections.Generic;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.TurnoverModule.Views.ContainedControls.Windows
{
    public partial class NewCashOutOrderWindow
    {
        public CashOutOrder Order { get; set; }
        public List<User> Users { get; set; }
        public List<string> Debets { get; set; }
        public List<string> Responsibles { get; set; }

        readonly IReportManager _repMan;

        public NewCashOutOrderWindow(IReportManager repMan, CashOutOrder order)
        {
            _repMan = repMan;
            InitializeComponent();

            Users = _context.GetUsers();
            Debets = new List<string> { "71.01", "90.01" };
            Responsibles = new List<string> { _context.CurrentCompany.GeneralManagerName, _context.CurrentCompany.GeneralAccountantName };
            if (order != null && order.Id != Guid.Empty)
            {
                Order = order;
            }
            else
            {
                Order = new CashOutOrder
                {
                    CreatedOn = DateTime.Today,
                    CreatedById = _context.CurrentUser.UserId,
                    Responsible = _context.CurrentCompany.GeneralManagerName,
                    Amount = _context.GetCashTodaysAmount(),
                    Id = Guid.NewGuid()
                };
            }
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Order.CreatedOn > DateTime.Today)
            {
                TonusWindow.Alert("Ошибка", "Укажите дату! Будущим РКО оформлять нельзя.");
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
            if (String.IsNullOrEmpty(Order.ReceivedByText))
            {
                TonusWindow.Alert("Ошибка", "Укажите получателя!");
                return;
            }
            //if (String.IsNullOrEmpty(Order.Reason))
            {
                //TonusWindow.Alert("Ошибка", "Укажите основание!");
                //return;
            }

            if (String.IsNullOrEmpty(Order.Responsible))
            {
                TonusWindow.Alert("Ошибка", "Укажите ответственного!");
                return;
            }

            _context.PostCashOutOrder(Order.Id, Order.CreatedOn, Order.Debet, Order.Amount, Order.CreatedById, Order.ReceivedByText, Order.Reason, Order.Responsible);
            DialogResult = true;
            _repMan.ProcessPdfReport(() => _context.GenerateRkoReport(Order.Id));
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
