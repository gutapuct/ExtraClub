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
using ExtraClub.SettingsModule.ViewModels;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Network
{
    /// <summary>
    /// Interaction logic for OrgSolarium.xaml
    /// </summary>
    public partial class OrgSolarium
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public OrgSolarium()
        {
            InitializeComponent();
        }

        private void SaveWarningsButton_Click(object sender, RoutedEventArgs e)
        {
            SolariumWarningsGrid.CommitEdit();
            Model.PostSolariumWarnings();
        }

        private void ProvidersView_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            Model.SolariumWarningsModified = true;
        }
    }
}
