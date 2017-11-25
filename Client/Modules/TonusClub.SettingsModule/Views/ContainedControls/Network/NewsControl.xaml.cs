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
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;
using TonusClub.UIControls.BaseClasses;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network
{
    public partial class NewsControl
    {

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public NewsControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditNewsWindow>(() => Model.RefreshNews());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.NewsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditNewsWindow>(() => Model.RefreshNews(), new ParameterOverride("news", ViewModelBase.Clone<News>(Model.NewsView.CurrentItem)));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.NewsView.CurrentItem == null) return;
            TonusWindow.Confirm("Удаление", "Вы действительно хотите удалить выделенную новость?", e1 =>
            {
                if (e1.DialogResult ?? false)
                {
                    ClientContext.DeleteObject("News", ((News)Model.NewsView.CurrentItem).Id);
                    Model.RefreshNews();
                }
            });
        }

        private void CallScrenarioGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditButton_Click(null, null);
            }
        }

    }
}
