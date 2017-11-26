using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ExtraClub.CashRegisterModule;
using ExtraClub.Clients.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.Clients.Views.ContainedControls
{
    public partial class CustomerVisitsControl
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public CustomerVisitsControl()
        {
            InitializeComponent();
        }

        private void ReceiptCopyClick(object sender, RoutedEventArgs e)
        {
            if (Model.VisitsView.CurrentItem == null) return;
            var vis = Model.VisitsView.CurrentItem as CustomerVisit;
            if (String.IsNullOrWhiteSpace(vis.Receipt))
            {
                ExtraWindow.Alert("Ошибка", "Для данного посещения чек не сохранен.");
                return;
            }
            var list = vis.Receipt.Split('\n').ToList();
            list.Insert(0, "Дубликат чека за посещение");
            list.Insert(1, String.Format("{0:dd.MM.yyyy HH:mm}-{1:dd.MM.yyyy HH:mm}", vis.InTime, vis.OutTime));
            ApplicationDispatcher.UnityContainer.Resolve<CashRegisterManager>().PrintText(list);
        }

        private void ReceiptCopyPdfClick(object sender, RoutedEventArgs e)
        {
            if (Model.VisitsView.CurrentItem == null) return;
            var vis = Model.VisitsView.CurrentItem as CustomerVisit;
            if (String.IsNullOrWhiteSpace(vis.Receipt))
            {
                ExtraWindow.Alert("Ошибка", "Для данного посещения чек не сохранен.");
                return;
            }
            var list = vis.Receipt.Split('\n').ToList();
            list.Insert(0, "Дубликат чека за посещение");
            list.Insert(1, String.Format("{0:dd.MM.yyyy HH:mm}-{1:dd.MM.yyyy HH:mm}", vis.InTime, vis.OutTime));
            ApplicationDispatcher.UnityContainer.Resolve<IReportManager>().PrintTextToPdf(list);
        }

        private void BonusGrid_DataLoaded(object sender, EventArgs e)
        {
            int i = 1;
            foreach (var visit in Model.VisitsView.OfType<CustomerVisit>().OrderBy(x => x.InTime))
            {
                visit.Number = i++;
            }
        }
    }
}
