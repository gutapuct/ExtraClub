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
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.UIControls;

namespace ExtraClub.OrganizerModule.Views.Calls.Windows
{
    /// <summary>
    /// Interaction logic for CallDetailsWindow.xaml
    /// </summary>
    public partial class CallDetailsWindow
    {
        public Call Call { get; set; }

        public CallDetailsWindow(Call call)
            : base(null)
        {
            Call = call;
            InitializeComponent();
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Call.CustomerId.HasValue)
            {
                NavigationManager.MakeClientRequest(Call.CustomerId.Value);
                Close();
            }
        }
    }
}
