using System;
using System.Windows;
using System.Windows.Input;
using TonusClub.OrganizerModule.ViewModels;
using TonusClub.OrganizerModule.Views.Ankets.Windows;
using Microsoft.Practices.Unity;
using TonusClub.ServiceModel.Organizer;

namespace TonusClub.OrganizerModule.Views.Ankets
{
    public partial class AnketsControl
    {
        private OrganizerLargeViewModel Model
        {
            get
            {
                return DataContext as OrganizerLargeViewModel;
            }
        }

        public AnketsControl()
        {
            InitializeComponent();
        }

        private void NewAnket_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewAnketDialog>(() => Model.RefreshAnkets(), new ParameterOverride("anketId", Guid.Empty), new ParameterOverride("isReadonly", false));
        }

        private void AnketsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var item = AnketsGrid.SelectedItem as AnketInfo;
                if (item.StatusId == 0)
                {
                    ProcessUserDialog<NewAnketDialog>(() => Model.RefreshAnkets(), new ParameterOverride("anketId", item.Id), new ParameterOverride("isReadonly", false));
                }
                else
                {
                    ProcessUserDialog<NewAnketDialog>(() => { }, new ParameterOverride("anketId", item.Id), new ParameterOverride("isReadonly", true));
                }
            }
        }

        private void ExportAnket_Click(object sender, RoutedEventArgs e)
        {
            var item = AnketsGrid.SelectedItem as AnketInfo;
            if (item == null) return;
            ReportManager.ProcessPdfReport(() => Model.ClientContext.GenerateAnketReport(item.Id));
        }

        private void DeleteAnket_Click(object sender, RoutedEventArgs e)
        {
            var item = AnketsGrid.SelectedItem as AnketInfo;
            if (item == null) return;
            ClientContext.DeleteAnket(item.Id);
            Model.RefreshAnkets();
        }
    }
}
