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
using TonusClub.SettingsModule.ViewModels;
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;
using TonusClub.ServiceModel;
using ViewModelBase = TonusClub.UIControls.BaseClasses.ViewModelBase;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    /// <summary>
    /// Interaction logic for AdvertSettings.xaml
    /// </summary>
    public partial class FrAdvertSettings
    {

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public FrAdvertSettings()
        {
            InitializeComponent();
        }

        private void AddElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentAdvertGroup == null) return;
            if (new NewEditAdvertTypeWindow(ClientContext, null, Model.CurrentAdvertGroup.Id).ShowDialog() ?? false)
            {
                Model.RefreshAdvertTypes();
            }
        }

        private void EditElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.AdvertTypesView.CurrentItem == null) return;
            var at = ViewModelBase.Clone<AdvertType>(Model.AdvertTypesView.CurrentItem);
            if (new NewEditAdvertTypeWindow(ClientContext, at).ShowDialog() ?? false)
            {
                Model.RefreshAdvertTypes();
            }
        }

        private void RemoveElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.AdvertTypesView.CurrentItem == null) return;
            TonusWindow.Confirm("Удаление канала", "Удалить выделенный канал?", w =>
            {
                if (w.DialogResult ?? false)
                {
                    Model.ClientContext.DeleteAdvertType((Model.AdvertTypesView.CurrentItem as AdvertType).Id);
                    Model.RefreshAdvertTypes();
                }
            });
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditElementButton_Click(null, null);
        }
    }
}
