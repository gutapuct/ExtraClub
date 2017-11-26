using System.Windows;

namespace ExtraClub.UIControls.Windows
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow
    {

        public string YesButtonText { get; set; }
        public string NoButtonText { get; set; }
        public string Text { get; set; }

        public bool IsOuter { get; set; }


        public ConfirmWindow(string header, string content, string yes, string no)
        {
            InitializeComponent();
            Text = content;
            YesButtonText = yes;
            NoButtonText = no;
            Header = header;
            DataContext = this;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close(!IsOuter);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close(!IsOuter);
        }
    }
}
