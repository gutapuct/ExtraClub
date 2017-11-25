using System;
using System.Windows;
using Telerik.Windows.Controls;
using Microsoft.Practices.Unity;
using TonusClub.TurnoverModule.ViewModels;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewConsignment.xaml
    /// </summary>
    public partial class NewEditProviderOrderWindow
    {
        ConsignmentViewModel _model;

        public NewEditProviderOrderWindow(IUnityContainer cont, IReportManager repMan, Consignment consignment)
        {
            var model = cont.Resolve<ConsignmentViewModel>(new ParameterOverride("consignment", consignment), new ParameterOverride("type", 3));
            InitializeComponent();

            ProviderSelector.SelectedId = consignment.ProviderId ?? Guid.Empty;

            if (consignment.IsReadOnly)
            {
                SaveButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            DataContext = _model = model;

            Owner = Application.Current.MainWindow;

            InitializeComponent();

            ConsignmentLinesView.RowValidating += ConsignmentLinesView_RowValidating;
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
                    Content = ex.Message,
                    Owner = this
                });
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (PostConsignment())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
