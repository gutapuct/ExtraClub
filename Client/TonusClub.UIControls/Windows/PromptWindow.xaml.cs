using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace TonusClub.UIControls.Windows
{
    public partial class PromptWindow : WindowBase
    {
        public string YesButtonText { get; set; }
        public string NoButtonText { get; set; }
        public string Text { get; set; }
        public string TextResult { get; set; }


        public PromptWindow(string header, string content, string result, string ok = null, string cancel = null)
            : base(null)
        {
            InitializeComponent();
            Text = content;
            YesButtonText = ok;
            NoButtonText = cancel;
            Header = header;
            TextResult = result;
            this.DataContext = this;
        }

        public override bool? ShowDialog()
        {
            var res = base.ShowDialog();

            new System.Threading.Tasks.Task(() => {
                Thread.Sleep(300);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    FocusManager.SetFocusedElement(this, inputBox);
                    Keyboard.Focus(inputBox);
                }));
            }).Start();

            return res;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
