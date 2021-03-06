﻿using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Input;
using ExtraClub.ServiceModel;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.TurnoverModule.Views.ContainedControls.Windows;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    public partial class CashOutOrdersControl
    {
        private CashierDocumentsViewModel Model => (CashierDocumentsViewModel)DataContext;

        public CashOutOrdersControl()
        {
            InitializeComponent();
        }

        private void NewClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewCashOutOrderWindow>(() => Model.RefreshCashOutOrders());
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (Model.CashOutOrders.CurrentItem != null)
            {
                ProcessUserDialog<NewCashOutOrderWindow>(() => Model.RefreshCashOutOrders(), new ParameterOverride("order", ViewModelBase.Clone<CashOutOrder>(Model.CashOutOrders.CurrentItem)));
            }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            var order = Model.CashOutOrders.CurrentItem as CashOutOrder;
            if (order != null)
            {
                ReportManager.ProcessPdfReport(() => ClientContext.GenerateRkoReport(order.Id));
            }
        }

        private void CashOutOrdersView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                ExportClick(null, null);
            }
        }

    }
}
