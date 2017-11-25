using System;
using Telerik.Windows.Controls;
using System.Windows;

namespace TonusClub.UIControls.Windows
{
    public static class TonusWindow
    {
        public static void Alert(DialogParameters parameters)
        {
            var dlg = new AlertWindow(parameters);
            dlg.ShowDialog();
        }

        public static void Alert(string header, string message)
        {
            Alert(new DialogParameters
            {
                Header = header,
                Content = message,
                OkButtonContent = UIControls.Localization.Resources.Ok,
                Owner = Application.Current.MainWindow
            });
        }

        public static ConfirmWindow Confirm(string header, string content, Action<ConfirmWindow> closed, string yes = null, string no = null)
        {
            yes = yes ?? TonusClub.UIControls.Localization.Resources.Yes;
            no = no ?? TonusClub.UIControls.Localization.Resources.No;
            var dlg = new ConfirmWindow(header, content, yes, no);
            dlg.ShowDialog();
            dlg.Closed = () => closed(dlg);
            return dlg;
        }

        public static void ConfirmOuter(string header, string content, Action<ConfirmWindow> closed, string yes = null, string no = null)
        {
            yes = yes ?? TonusClub.UIControls.Localization.Resources.Yes;
            no = no ?? TonusClub.UIControls.Localization.Resources.No;
            var dlg = new ConfirmWindow(header, content, yes, no) { IsOuter = true };
            var wnd = new Window
            {
                Title = header,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = Application.Current.MainWindow,
                Content = dlg,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            dlg.Closed = () =>
            {
                wnd.Close();
                closed(dlg);
            };
            wnd.ShowDialog();
        }

        public static void Prompt(string header, string content, string result, Action<PromptWindow> closed, string ok = null, string cancel = null)
        {
            ok = ok ?? TonusClub.UIControls.Localization.Resources.Ok;
            cancel = cancel ?? TonusClub.UIControls.Localization.Resources.Cancel;

            var dlg = new PromptWindow(header, content, result, ok, cancel);
            dlg.ShowDialog();
            dlg.Closed = () =>
            {
                if (!(dlg.DialogResult ?? false))
                {
                    dlg.TextResult = result;
                }
                closed(dlg);
            };
        }
    }
}
