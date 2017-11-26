using System.Windows;
using System.Windows.Input;
using ExtraClub.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.UIControls;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views
{
    public partial class SettingsLargeView : ModuleViewBase, ILargeSection
    {
        private SettingsLargeViewModel _model;


        public SettingsLargeView(SettingsLargeViewModel model)
        {
            DataContext = _model = model;

            InitializeComponent();

            TargetPrograms.Content = new ContainedControls.Network.TargetConfigsControl();


            SettingsManager.RegisterGridView(this, SeqRestGrid);
            SettingsManager.RegisterGridView(this, IntRestGrid);
            DictionariesControl.Init(DictionaryManager, ClientContext);

            //TODO: Uncomment
            //if (ClientContext.CheckPermission("DisableCentral"))
            //{
            //    RadPanelBar.Items.Remove(GeneralBarItem);
            //}
        }


        private void RefreshExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OrgSolarium.SolariumWarningsGrid.CancelEdit();
            //CompanyCardsSettingsControl.Grid.CancelEdit();
            //CompanyTicketsSettingsControl.Grid.CancelEdit();
            _model.RefreshDataSync();
            Focus();
            e.Handled = true;
        }

        public object GetTransferDataForMinimize()
        {
            return null;
        }

        public object GetTransferDataForRestore()
        {
            return null;
        }

        public override void SetState(object data)
        {
            base.SetState(data);
            _model.EnsureDataLoading();
        }

        private void NewSeqRestButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentSeqRestWindow>(() => _model.RefreshDataSync());
        }

        private void NewIntRestButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentIntRestWindow>(() => _model.RefreshDataSync());
        }

        private void NewAmRestButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentAmRestWindow>(() => _model.RefreshDataSync());
        }

        private void EditTreatmentSRButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentSeqRestView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentSeqRestWindow>(
                    () => _model.RefreshDataSync(),
                    new ResolverOverride[] { new ParameterOverride("treatmentSR", _model.TreatmentSeqRestView.CurrentItem) });
            }
        }

        private void EditTreatmentIRButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentIntRestView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentIntRestWindow>(
                    () => _model.RefreshDataSync(),
                    new ResolverOverride[] { new ParameterOverride("treatmentIR", _model.TreatmentIntRestView.CurrentItem) });
            }
        }

        private void DeleteTreatmentIRButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentIntRestView.CurrentItem == null) return;
            ExtraWindow.Confirm("Удаление",
                "Удалить выделенное ограничение?",
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.DeleteObject("TreatmentSeqRests", ((TreatmentSeqRest)_model.TreatmentIntRestView.CurrentItem).Id);
                        _model.RefreshDataSync();
                    }
                });
        }

        private void DeleteTreatmentSRButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentSeqRestView.CurrentItem == null) return;
            ExtraWindow.Confirm("Удаление",
                 "Удалить выделенное ограничение?",
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.DeleteObject("TreatmentSeqRests", ((TreatmentSeqRest)_model.TreatmentSeqRestView.CurrentItem).Id);
                        _model.RefreshDataSync();
                    }
                });
        }

        private void DeleteTreatmentARButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentAmRestView.CurrentItem == null) return;
            ExtraWindow.Confirm("Удаление",
                 "Удалить выделенное ограничение?",
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.DeleteObject("TreatmentSeqRests", ((TreatmentSeqRest)_model.TreatmentAmRestView.CurrentItem).Id);
                        _model.RefreshDataSync();
                    }
                });
        }

        private void EditTreatmentARButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentAmRestView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentAmRestWindow>(() => _model.RefreshDataSync(), new ResolverOverride[] { new ParameterOverride("treatmentAR", _model.TreatmentAmRestView.CurrentItem) });
            }
        }

        private void IntRestGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("PlanningMgmgt"))
            {
                EditTreatmentIRButton_Click(null, null);
            }
        }

        private void SeqRestGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("PlanningMgmgt"))
            {
                EditTreatmentSRButton_Click(null, null);
            }
        }

        private void AmRestGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("PlanningMgmgt"))
            {
                EditTreatmentARButton_Click(null, null);
            }
        }
    }
}
