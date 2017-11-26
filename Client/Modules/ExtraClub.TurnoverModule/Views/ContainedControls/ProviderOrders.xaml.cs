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
using ExtraClub.TurnoverModule.Views.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls.Windows;
using Telerik.Windows.Controls;
using System.ServiceModel;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ProviderOrders.xaml
    /// </summary>
    public partial class ProviderOrders
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public ProviderOrders()
        {
            InitializeComponent();
        }

        private void NewProviderOrderClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditProviderOrderWindow>(() => Model.RefreshProviderOrders());
        }

        private void EditProviderOrderClick(object sender, RoutedEventArgs e)
        {
            if (Model.ProviderOrdersView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditProviderOrderWindow>(() => Model.RefreshProviderOrders(), new ResolverOverride[] { new ParameterOverride("consignment", ViewModelBase.Clone<Consignment>(Model.ProviderOrdersView.CurrentItem)) });
            }
        }

        private void PaymentProviderOrderClick(object sender, RoutedEventArgs e)
        {
            if (Model.ProviderOrdersView.CurrentItem != null)
                ProcessUserDialog<Views.Windows.NewProviderPaymentWindow>(() =>
                {
                    Model.RefreshProviderOrders();
                    Model.RefreshPayments();
                }, new ResolverOverride[] { new ParameterOverride("order", Model.ProviderOrdersView.CurrentItem) });

        }

        private void NewIncomeClick(object sender, RoutedEventArgs e)
        {
            if (Model.ProviderOrdersView.CurrentItem != null)
            {
                var doc = ViewModelBase.Clone<Consignment>(Model.ProviderOrdersView.CurrentItem);
                doc.ProviderOrderId = doc.Id;
                doc.Id = Guid.NewGuid();
                doc.Number = 0;
                doc.SerializedConsignmentLines.Clear();
                ((Consignment)Model.ProviderOrdersView.CurrentItem).SerializedConsignmentLines.ForEach(i =>
                {
                    doc.SerializedConsignmentLines.Add(new ConsignmentLine
                    {
                        AuthorId = i.AuthorId,
                        Comment = i.Comment,
                        CompanyId = i.CompanyId,
                        ConsignmentId = i.ConsignmentId,
                        GoodId = i.GoodId,
                        Id = Guid.NewGuid(),
                        Position = i.Position,
                        Price = i.Price,
                        Quantity = i.Quantity
                    });
                });
                doc.DocType = 0;
                doc.Date = DateTime.Today;
                ProcessUserDialog<NewConsignment>(() =>
                {
                    Model.RefreshConsignments();
                    Model.RefreshProviderOrders();
                }, new ResolverOverride[] { new ParameterOverride("consignment", doc) });

            }
        }

        //Sie rufen einen Klempner?
        //private void DeleteProviderOrderClick(object sender, RoutedEventArgs e)
        //{
        //    if (Model.ProviderOrdersView.CurrentItem != null)
        //    {
        //        var order = Model.ProviderOrdersView.CurrentItem as Consignment;
        //        if (order.IsReadOnly)
        //        {
        //            ExtraWindow.Alert(new DialogParameters { Header = "Удаление невозможно", Content = "Невозможно удалить заказ, по которому есть оплаты или приходные накладные." });
        //            return;
        //        }
        //        if (!ExtraWindow.Confirm(new DialogParameters { Header = "Удаление заказа", Content = "Удалить выделенный заказ?" })) return;
        //        try
        //        {
        //            ClientContext.DeleteOrder(order.Id);
        //            Model.RefreshProviderOrders();
        //        }
        //        catch (FaultException<string> ex)
        //        {
        //            ExtraWindow.Alert(new DialogParameters { Header = "Удаление невозможно", Content = ex.Message });
        //        }
        //    }
        //}

        private void ProviderOrdersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditProviderOrderClick(null, null);
            }
        }

        private void CancelProviderOrderClick(object sender, RoutedEventArgs e)
        {
            if (Model.ProviderOrdersView.CurrentItem != null)
            {
                ExtraWindow.Confirm(UIControls.Localization.Resources.RemoveOrder, UIControls.Localization.Resources.RemoveOrderMessage,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            var res = Model.ClientContext.HideProviderOrder(((Consignment)Model.ProviderOrdersView.CurrentItem).Id);
                            if (!String.IsNullOrEmpty(res))
                            {
                                ExtraWindow.Alert(UIControls.Localization.Resources.Error, res);
                            }
                            else
                            {
                                Model.RefreshProviderOrders();
                            }
                        }
                    });
            }
        }
    }
}
