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
using ExtraClub.UIControls;
using ExtraClub.ServiceModel;
using Telerik.Windows.Controls;

namespace ExtraClub.ScheduleModule.Views.Treatments.Windows
{
    public partial class SelectConfigWindow : ExtraClub.UIControls.WindowBase
    {
        public TreatmentConfig TcResult;

        public TreatmentConfig[] TreatmentConfigs { get; private set; }

        public SelectConfigWindow(TreatmentConfig[] tcs)
            : base(null)
        {
            InitializeComponent();
            TreatmentConfigs = tcs;
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TreatmentClicked(object sender, RoutedEventArgs e)
        {
            TcResult = (sender as Button).DataContext as TreatmentConfig;
            Close();
        }
    }
}
