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
using TonusClub.TurnoverModule.ViewModels;
using Microsoft.Practices.Unity;
using TonusClub.TurnoverModule.Views.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using ViewModelBase = TonusClub.UIControls.BaseClasses.ViewModelBase;

namespace TonusClub.TurnoverModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for GoodsControl.xaml
    /// </summary>
    public partial class GoodsControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public GoodsControl()
        {
            InitializeComponent();
        }

        private void NewGoodClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditGoodWindow>(() => Model.RefreshGoods());
        }

        private void EditGoodClick(object sender, RoutedEventArgs e)
        {
            if (Model.GoodsView != null && Model.GoodsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditGoodWindow>(() => Model.RefreshGoods(), new ResolverOverride[] { new ParameterOverride("good", ViewModelBase.Clone<Good>(Model.GoodsView.CurrentItem)) });
            }
        }

        private void CopyGoodClick(object sender, RoutedEventArgs e)
        {
            if (Model.GoodsView != null && Model.GoodsView.CurrentItem != null)
            {
                var good = ViewModelBase.Clone<Good>(Model.GoodsView.CurrentItem);
                good.Id = Guid.NewGuid();
                good.Name = UIControls.Localization.Resources.Copy + " " + good.Name;
                ProcessUserDialog<NewEditGoodWindow>(() => Model.RefreshGoods(), new ResolverOverride[] { new ParameterOverride("good", good) });
            }
        }

        private void HideGoodClick(object sender, RoutedEventArgs e)
        {
            if (Model.GoodsView != null && Model.GoodsView.CurrentItem != null)
            {
                TonusWindow.Confirm(UIControls.Localization.Resources.HideGood,
                    UIControls.Localization.Resources.HideGoodMessage,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ClientContext.HideGood(((Good)Model.GoodsView.CurrentItem).Id);
                            Model.RefreshGoods();
                        }
                    });
            }
        }

        private void GoodsGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditGoodClick(null, null);
            }
        }
    }
}
