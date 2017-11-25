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
using TonusClub.UIControls;
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure;

namespace TonusClub.SettingsModule.Views.ContainedControls
{
    public partial class CompanySettingsControl : ModuleViewBase
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CompanySettingsControl()
        {
            InitializeComponent();
#if BEAUTINIKA
            ostImg.Source = new BitmapImage(new Uri("/Beautinika.SettingsModule;component/Views/Resources/formulae.png", UriKind.Relative));
            checks1.Content = "Абонементы действуют во всех студиях франчайзи";
#endif
        }

        private void SaveParametersButton_Click(object sender, RoutedEventArgs e)
        {
            Model.CommitCompany();
        }

        private void DataImportClick(object sender, RoutedEventArgs e)
        {
            ApplicationDispatcher.UnityContainer.Resolve<DataImportWindow>().ShowDialog();
        }
    }
}
