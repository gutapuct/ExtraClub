using System.Windows;
using Telerik.Windows.Controls;

namespace ExtraClub.UIControls.Windows
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : WindowBase
    {
        public string OkButtonText { get; set; }
        public string Text { get; set; }

        public AlertWindow(DialogParameters parameters)
            : base(null)
        {
            InitializeComponent();
            Text = parameters.Content.ToString();
            OkButtonText = (parameters.OkButtonContent??UIControls.Localization.Resources.Yes).ToString();
            Header = parameters.Header.ToString();
            this.DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
