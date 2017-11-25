using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;
using TonusClub.TurnoverModule.ViewModels;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewConsignment.xaml
    /// </summary>
    public partial class NewConsignment
    {
        readonly ConsignmentViewModel _model;

        readonly IReportManager _repMan;

        public NewConsignment(IReportManager repMan, Consignment consignment)
        {
            var model = ApplicationDispatcher.UnityContainer.Resolve<ConsignmentViewModel>(new ParameterOverride("consignment", consignment), new ParameterOverride("type", 0));
            InitializeComponent();
            DataContext = _model = model;
            _repMan = repMan;

            Owner = Application.Current.MainWindow;

            InitializeComponent();

            ProviderSelector.SelectedId = consignment.ProviderId ?? Guid.Empty;

            if(consignment.IsAsset)
            {
                PrintButton.Visibility = Visibility.Visible;
                PrintCheckbox.Visibility = Visibility.Collapsed;
                AssetButton.Visibility = Visibility.Collapsed;
            }

            ConsignmentLinesView.RowValidating += ConsignmentLinesView_RowValidating;
        }

        void ConsignmentLinesView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var i = e.Row.Item as ConsignmentLine;
            if(i.GoodId == Guid.Empty) e.ValidationResults.Add(
               new GridViewCellValidationResult { ErrorMessage = UIControls.Localization.Resources.GoodNeeded });
            if((i.Quantity ?? 0) <= 0) e.ValidationResults.Add(
               new GridViewCellValidationResult { ErrorMessage = UIControls.Localization.Resources.IncorrectGoodAmount });
            if(i.Expiry.HasValue && i.Expiry.Value < DateTime.Today) e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Просроченный товар" });
            e.IsValid = e.ValidationResults.Count == 0;
            if(!e.IsValid)
            {
                TonusWindow.Confirm(UIControls.Localization.Resources.InputError,
                    UIControls.Localization.Resources.Error + ":\n" + e.ValidationResults[0].ErrorMessage + "\n" + UIControls.Localization.Resources.CancelNewLine,
                    e1 =>
                    {
                        if(e1.DialogResult ?? false)
                        {
                            e.Row.CancelEdit();
                            _model.ConsignmentLines.Remove(i);
                            _model.ConsignmentLinesView.Refresh();
                            e.IsValid = true;
                        }
                    });
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if(ctrl != null)
            {
                ctrl.Focus();
            }
            if(ctrl is RadDatePicker && (!((RadDatePicker)ctrl).SelectedDate.HasValue || ((RadDatePicker)ctrl).SelectedDate.Value.Year < 1980))
            {
                ((RadDatePicker)ctrl).SelectedDate = DateTime.Today;
            }
        }

        bool PostConsignment()
        {
            if(!ConsignmentLinesView.CommitEdit()) return false;
            try
            {
                _model.PostConsignment();
            }
            catch(NullReferenceException ex)
            {
                TonusWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.UnableToCreateCons,
                    Content = ex.Message,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return false;
            }
            //catch (ArgumentException ex)
            //{
            //    TonusWindow.Alert(new DialogParameters
            //    {
            //        Header = "Невозможно создать накладную",
            //        Content = "Данного товара недостаточно на складе отправителя:\n" + ex.Message,
            //        OkButtonContent = "ОК"
            //    });
            //    return false;
            //}
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.Consignment.IsAsset)
            {
                if (MessageBox.Show("Вы действительно хотите отредактировать проведенную накладную?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }

            if(PostConsignment())
            {
                if(PrintCheckbox.IsChecked ?? false)
                {
                    PrintConsignment();
                }
                DialogResult = true;
                Close();
            }
        }

        private void PrintConsignment()
        {
            _repMan.ProcessPdfReport(()=>_context.GenerateConsignmentReport(_model.Consignment.Id));
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _model.AssetConsignment();

            if(PostConsignment())
            {
                if(PrintCheckbox.IsChecked ?? false)
                {
                    PrintConsignment();
                }

                DialogResult = true;
                Close();

            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintConsignment();
        }
    }
}
