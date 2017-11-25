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
using System.Windows.Shapes;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    /// <summary>
    /// Interaction logic for NewEditButtonWindow.xaml
    /// </summary>
    public partial class NewEditButtonWindow
    {
        public IncomingCallFormButton ButtonResult { get; set; }
        public object Forms { get; set; }
        public object ButtonTypes { get; set; }

        public NewEditButtonWindow(ClientContext context, IncomingCallFormButton button)
            : base(context)
        {
            InitializeComponent();
            ButtonResult = button;
            Forms = context.GetCallScrenarioForms();
            ButtonTypes = Infrastructure.BaseClasses.ButtonTypes.TypeNames;
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
