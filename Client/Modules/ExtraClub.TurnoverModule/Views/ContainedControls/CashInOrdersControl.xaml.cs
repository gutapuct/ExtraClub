﻿using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Input;
using ExtraClub.ServiceModel;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.TurnoverModule.Views.ContainedControls.Windows;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    public partial class CashInOrdersControl
    {
        private CashierDocumentsViewModel Model => (CashierDocumentsViewModel)DataContext;

        public CashInOrdersControl()
        {
            InitializeComponent();
        }

        private void NewClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewCashInOrderWindow>(() => Model.RefreshCashInOrders());
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (Model.CashInOrders.CurrentItem != null)
            {
                ProcessUserDialog<NewCashInOrderWindow>(() => Model.RefreshCashInOrders(), new ParameterOverride("order", ViewModelBase.Clone<CashInOrder>(Model.CashInOrders.CurrentItem)));
            }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            var order = Model.CashInOrders.CurrentItem as CashInOrder;
            if (order != null)
            {
                ReportManager.ProcessPdfReport(()=>ClientContext.GeneratePkoReport(order.Id));
            }
        }

        private void CashInOrdersView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                ExportClick(null, null);
            }
        }
    }
}
