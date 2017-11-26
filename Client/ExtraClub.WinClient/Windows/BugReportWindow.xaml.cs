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
using ExtraClub.Infrastructure;
using ExtraClub.UIControls.Windows;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using System.ServiceModel;

namespace ExtraClub.WinClient.Windows
{
    /// <summary>
    /// Interaction logic for BugReportWindow.xaml
    /// </summary>
    public partial class BugReportWindow : Window
    {
        Exception Exception { get; set; }

        public string AdditionalMessage { get; set; }

        public string ResultMessage
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Версия ОС: {0} {1}\n", System.Environment.OSVersion.Platform, System.Environment.OSVersion.Version);
                sb.AppendFormat("Название компьютера: {0}\n", System.Environment.MachineName);
                sb.AppendFormat("Является ли система x64: {0}\n", System.Environment.Is64BitOperatingSystem);
                sb.AppendFormat("Версия .net Framework: {0}\n", System.Environment.Version);
                sb.AppendFormat("Версия АСУ: {0}\n", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
                sb.AppendFormat("Адрес сервера: {0}\n", AppSettingsManager.GetSetting("ServerAddress"));
                if (!String.IsNullOrWhiteSpace(AdditionalMessage))
                {
                    sb.AppendFormat("Сообщение пользователя:\n{0}\n", AdditionalMessage);
                }
                sb.AppendLine();
                AppendException(sb, Exception);
                return sb.ToString();
            }
        }

        private void AppendException(StringBuilder sb, Exception exception)
        {
            sb.AppendLine(exception.GetType().ToString());
            sb.AppendLine(exception.Message);
            sb.AppendLine(exception.StackTrace);
            if (exception.InnerException != null)
            {
                sb.AppendLine("-----------------------------------------------------");
                sb.AppendLine("Inner exception:");
                AppendException(sb, exception.InnerException);
            }
        }

        public BugReportWindow(Exception ex)
        {
            Exception = ex;
            DataContext = this;
            InitializeComponent();
        }

        private void ViewReportContentClick(object sender, RoutedEventArgs e)
        {
            var msg = ResultMessage;
            MessageBox.Show(msg.Substring(0, Math.Min(msg.Length, 1024)), "Содержимое отчета", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProvideAdditionalInfoClick(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Prompt("Дополнительная информация",
                "Пожалуйста, укажите дополнительные подробности, которые, на ваш взгляд, могут помочь воспроизвести ошибку:",
                AdditionalMessage ?? "",
                e1=>EditClosed(e1));
        }


        private void EditClosed(PromptWindow w)
        {
            if (w.DialogResult ?? false)
            {
                AdditionalMessage = w.TextResult;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            //throw Exception;
        }

        private void SendReportClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ChannelFactory<ISyncService> cf = new ChannelFactory<ISyncService>("SyncServiceEndpoint");
                var client = cf.CreateChannel();
                client.PostApplicationErrorReport(ResultMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("К сожалению, отправить отчет не удалось.", "Ошибка при отправке отчета\n" + ex.GetType().ToString() + "\n" + ex.Message, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void CopyReport(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(System.Windows.DataFormats.UnicodeText, ResultMessage.Replace("\n", "\r\n"));
        }
    }
}
