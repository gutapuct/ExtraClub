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
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.UIControls.BaseClasses;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network
{
    public partial class CallScrenario
    {

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CallScrenario()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCallScrenarioFormWindow>(() => Model.RefreshCallScrenario());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CallScrenarioView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditCallScrenarioFormWindow>(() => Model.RefreshCallScrenario(), new ParameterOverride("form", ViewModelBase.Clone<IncomingCallForm>(Model.CallScrenarioView.CurrentItem)));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CallScrenarioView.CurrentItem == null) return;
            TonusWindow.Confirm("Удаление", "Вы действительно хотите удалить выделенную форму?", e1 =>
            {
                if (e1.DialogResult ?? false)
                {
                    ClientContext.DeleteObject("IncomingCallForms", ((IncomingCallForm)Model.CallScrenarioView.CurrentItem).Id);
                    Model.RefreshCallScrenario();
                }
            });
        }

        private void CallScrenarioGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("NetScensMgmt"))
            {
                EditButton_Click(null, null);
            }
        }
    }
}
