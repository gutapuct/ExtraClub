using System;
using System.Windows;
using Microsoft.Practices.Unity;
using TonusClub.ServiceModel;
using Telerik.Windows.Controls;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;
using TonusClub.TurnoverModule.ViewModels;
using System.ServiceModel;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewEditDeductionConsignment.xaml
    /// </summary>
    public partial class NewEditDeductionConsignment
    {
        ConsignmentViewModel _model;

        IReportManager _repMan;

        public NewEditDeductionConsignment(IUnityContainer cont, ClientContext context, IReportManager repMan, Consignment consignment)
            : base(context)
        {
            var model = cont.Resolve<ConsignmentViewModel>(new ResolverOverride[] { new ParameterOverride("consignment", consignment), new ParameterOverride("type", 2) });
            InitializeComponent();
            DataContext = _model = model;
            _repMan = repMan;

            Owner = Application.Current.MainWindow;

            InitializeComponent();

            if (consignment.IsAsset)
            {
                PrintButton.Visibility = System.Windows.Visibility.Visible;
                PrintCheckbox.Visibility = System.Windows.Visibility.Collapsed;
                AssetButton.Visibility = System.Windows.Visibility.Collapsed;
                SaveButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            ConsignmentLinesView.RowValidating += new EventHandler<GridViewRowValidatingEventArgs>(ConsignmentLinesView_RowValidating);
        }

        void ConsignmentLinesView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var i = e.Row.Item as ConsignmentLine;
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
                TonusWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.UnableToCreateCons,
                    Content = ex.Message
                });
                return false;
            }
            catch (FaultException ex)
            {
                TonusWindow.Alert(new DialogParameters
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
            _repMan.ProcessPdfReport(() => _context.GenerateConsignmentReport(_model.Consignment.Id));
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
