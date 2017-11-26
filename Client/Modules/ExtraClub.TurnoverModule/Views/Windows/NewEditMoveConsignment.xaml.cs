using System;
using System.Windows;
using ExtraClub.TurnoverModule.ViewModels;
using Microsoft.Practices.Unity;
using ExtraClub.ServiceModel;
using Telerik.Windows.Controls;
using ExtraClub.UIControls.Windows;
using System.ServiceModel;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewEditMoveConsignment.xaml
    /// </summary>
    public partial class NewEditMoveConsignment
    {
        readonly ConsignmentViewModel _model;
        private readonly IReportManager _reportManager;

        public NewEditMoveConsignment(IUnityContainer cont, Consignment consignment, IReportManager reportManager)
        {
            _reportManager = reportManager;
            var model = cont.Resolve<ConsignmentViewModel>(new ParameterOverride("consignment", consignment), new ParameterOverride("type", 1));
            InitializeComponent();
            DataContext = _model = model;

            Owner = Application.Current.MainWindow;

            InitializeComponent();

            if (consignment.IsAsset)
            {
                PrintButton.Visibility = Visibility.Visible;
                PrintCheckbox.Visibility = Visibility.Collapsed;
                AssetButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Collapsed;
            }

            ConsignmentLinesView.RowValidating += ConsignmentLinesView_RowValidating;
        }

        void ConsignmentLinesView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var i = (ConsignmentLine) e.Row.Item;
            if (i.GoodId == Guid.Empty) e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = UIControls.Localization.Resources.GoodNeeded });
            if ((i.Quantity ?? 0) <= 0) e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = UIControls.Localization.Resources.IncorrectGoodAmount });
            e.IsValid = e.ValidationResults.Count == 0;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool PostConsignment()
        {
            if (!ConsignmentLinesView.CommitEdit()) return false;
            try
            {
                _model.PostConsignment();
            }
            catch (NullReferenceException ex)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.UnableToCreateCons,
                    Content = ex.Message,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return false;
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = "Невозможно создать накладную",
                    Content = ex.Reason,
                    OkButtonContent = "ОК"
                });
                return false;
            }
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (PostConsignment())
            {
                if (PrintCheckbox.IsChecked ?? false)
                {
                    PrintConsignment();
                }
                DialogResult = true;
                Close();
            }
        }

        private void PrintConsignment()
        {
            _reportManager.ProcessPdfReport(() => _context.GenerateConsignmentReport(_model.Consignment.Id));
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _model.AssetConsignment();

            if (PostConsignment())
            {
                if (PrintCheckbox.IsChecked ?? false)
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
