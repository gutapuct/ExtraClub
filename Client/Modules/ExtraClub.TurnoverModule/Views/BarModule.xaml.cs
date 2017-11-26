using Microsoft.Practices.Unity;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using ExtraClub.Infrastructure;
using ExtraClub.Infrastructure.Events;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.TurnoverModule.Views;
using ExtraClub.TurnoverModule.Views.ContainedControls.Windows;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.TurnoverModule.Views
{
    public partial class BarModule : ILargeSection
    {
        TurnoverLargeViewModel _model;

        IReportManager _repMan;
        ISettingsManager _setMan;


        public BarModule(TurnoverLargeViewModel model, IReportManager repMan, ISettingsManager setMan)
        {
            InitializeComponent();
            DataContext = _model = model;
            _repMan = repMan;
            _setMan = setMan;

            _setMan.RegisterGridView(this, GoodPricesView);
            _setMan.RegisterGridView(this, GoodActionsGrid);
        }

        public override void SetState(object data)
        {
            base.SetState(data);

            if (data != null && data is ResizeEventArgs)
            {
                var e = data as ResizeEventArgs;
                if (!String.IsNullOrEmpty(e.ActiveRegionName))
                {
                    switch (e.ActiveRegionName)
                    {
                        case "GoodPresense":
                            BarTabControl.SelectedItem = GoodPresenseTabItem;
                            break;
                    }
                }
            }
            _model.EnsureDataLoading();
        }

        private void NewGoodActionButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewGoodActionWindow>(() => _model.RefreshActions());
        }

        private void DeleteGoodActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.GoodActionsView.CurrentItem != null)
            {
                ExtraWindow.Confirm("Удаление акции",
                    "Действительно удалить акцию?",
                    e1 =>
                    {
                        if ((e1.DialogResult ?? false))
                        {
                            _model.RemoveCurrentGoodAction();
                        }
                    });
            }
        }

        private void EnableActionButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SetGoodActionEnabled(true);
        }

        private void DisableActionButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SetGoodActionEnabled(false);

        }

        private void NewPriceButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditPriceWindow>(() => _model.RefreshPrices(), new ParameterOverride("isAddingNew", true));
        }

        private void EditPriceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.SelectedGoodPrice == null) return;
            ProcessUserDialog<NewEditPriceWindow>(() => _model.RefreshPrices(), new ParameterOverride("goodPrice", ViewModelBase.Clone<GoodPrice>(_model.SelectedGoodPrice)), new ParameterOverride("isAddingNew", false));
        }

        private void ExcludePriceButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Confirm("Исключение из прайса",
                "Вы действительно хотите исключить выделенный товар из прайса?", w =>
            {
                if (w.DialogResult ?? false)
                {
                    _model.SetPricePresence(false);
                }
            });
        }

        private void EditActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.GoodActionsView.CurrentItem == null) return;
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<NewGoodActionWindow>();
            wnd.ApplyEdit((GoodAction)_model.GoodActionsView.CurrentItem);
            wnd.ShowDialog();
            if (wnd.DialogResult ?? false)
            {
                _model.RefreshActions();
            }
        }

        private void GoodActions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    EditActionButton_Click(null, null);
                }
            }
        }

        private void GoodPricesView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    EditPriceButton_Click(null, null);
                }
            }
        }

        private void PrintPriceButton_Click(object sender, RoutedEventArgs e)
        {
            _repMan.ProcessPdfReport(_model.ClientContext.GeneratePriceListReport);
        }

        private void MarkdownButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.GoodPricesView.CurrentItem != null)
            {
                ProcessUserDialog<MarkdownWindow>(() =>
                {
                    _model.RefreshPrices();
                    _model.RefreshGoods();
                    _model.RefreshDataAsync();
                }, new ParameterOverride("price", _model.GoodPricesView.CurrentItem));

            }
        }

    }
}
