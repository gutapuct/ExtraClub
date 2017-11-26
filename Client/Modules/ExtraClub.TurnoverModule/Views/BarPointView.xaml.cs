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
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Events;
using System.Data;
using Telerik.Windows.Controls;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.Infrastructure;

namespace ExtraClub.TurnoverModule.Views
{
    /// <summary>
    /// Interaction logic for BarPointView.xaml
    /// </summary>
    public partial class BarPointView : UserControl
    {
        public BarPointViewModel _model;

        public CustomerSearchControl Search
        {
            get
            {
                return CustomerSearch;
            }
        }

        public BarPointView(BarPointViewModel model)
        {
            CultureHelper.FixupCulture();
            InitializeComponent();
            DataContext = _model = model;
            IsTabStop = true;
            Focusable = true;
        }

        public void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                _model.AddToBasket(GoodsList.SelectedItem, 1);
            UpdateSumaryText();
        }

        private void UpdateSumaryText()
        {
            _model.UpdateStatus();

            BonusPaymentButton.Visibility = _model.GetBonusVisibility() ? Visibility.Visible : Visibility.Collapsed;
        }

        bool ClientListChanging = false;

        public void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_model.IsBasketEmpty() && e.RemovedItems.Count > 0 && !ClientListChanging && !_model.IsRefreshing)
            {
                var str = UIControls.Localization.Resources.ClearOrderMessage.Replace("\\n", "\n");
                ExtraWindow.Confirm(UIControls.Localization.Resources.NotProcessedOrder,
                    str,
                    e1 =>
                    {
                        if ((e1.DialogResult ?? false))
                        {
                            _model.ClearBasket();
                        }
                        else
                        {
                            //ClientListChanging = true;
                            //ClientList.SelectedItem = e.RemovedItems[0];
                            //ClientListChanging = false;
                        }
                        UpdateSumaryText();
                    }, UIControls.Localization.Resources.ClearOrder, "Оставить");
                ;
            }
        }

        private void BasketRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _model.AddToBasket(((Control)sender).DataContext, -1);
            UpdateSumaryText();
        }

        private void ProcessPayment_Click(object sender, RoutedEventArgs e)
        {
            _model.ProcessPayment(false, _ => { });
        }

        private void BonusPayment_Click(object sender, RoutedEventArgs e)
        {
            _model.ProcessPayment(true, pd =>
            {
                if (pd.Success)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.BonusPmtSuccess,
                        Content = String.Format(UIControls.Localization.Resources.BonusOutAmount, pd.BonusPayment),
                        OkButtonContent = UIControls.Localization.Resources.Ok,
                        Owner = Application.Current.MainWindow
                    });
                }
            });
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            _model.AddToCustomersList(_model.ClientContext.GetCustomer(e.Guid, true));
        }

        private void ClientList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_model.CurrentCustomer != null)
            {
                NavigationManager.MakeClientRequest(_model.CurrentCustomer.Id);
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            _model.ClearBasket();
        }

        private void CashlessPayment_Click(object sender, RoutedEventArgs e)
        {
            _model.ProcessPayment(false, _ => { }, true);
        }
    }
}
