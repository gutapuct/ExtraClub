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
using System.Globalization;
using Telerik.Windows.Controls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for DivisionSettingsControl.xaml
    /// </summary>
    public partial class DivisionSettingsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public DivisionSettingsControl()
        {
            InitializeComponent();
        }

        private void SaveParametersButton_Click(object sender, RoutedEventArgs e)
        {
            Model.CommitDivision();
        }
    }
}
