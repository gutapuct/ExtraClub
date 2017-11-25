using System;
using System.Windows;
using TonusClub.UIControls.Windows;

namespace TonusClub.UIControls
{
    public partial class PasswordChangeWindow
    {
        public string Cause { get; set; }

        public PasswordChangeWindow(string cause)
        {
            Cause = cause;
            DataContext = this;
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password != Password2.Password)
            {
                TonusWindow.Alert(Localization.Resources.Error, Localization.Resources.PasswordChangeDiff);
                return;
            }
            if (Password.Password.Length < 4)
            {
                TonusWindow.Alert(Localization.Resources.Error, Localization.Resources.PasswordSimple);
                return;
            }
            var res = _context.ChangePassword(OldPassword.Password, Password.Password);
            if (!String.IsNullOrEmpty(res))
            {
                TonusWindow.Alert(Localization.Resources.Error, res);
                return;
            }
            TonusWindow.Alert(Localization.Resources.PasswordChange, Localization.Resources.PasswordChangeOk);
            DialogResult = true;
            Close();
        }

    }
}
