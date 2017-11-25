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
using System.Threading;

namespace NewtonicServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerModel serverModel;

        public MainWindow()
        {
            serverModel = new ServerModel(Dispatcher);
            DataContext = serverModel;
            InitializeComponent();
        }

        private void MyNotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Show();
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                WindowState = System.Windows.WindowState.Normal;
            }
            Activate();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            serverModel.Dispose();
            Thread.Sleep(1500);
            Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized) Hide();
            base.OnStateChanged(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Exit_Click(null, null);
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if(!serverModel.Clients.Any())return;
        //    serverModel.Clients.First().PendingEvent = new TonusClub.ServiceModel.TreatmentEvent { VisitDate = DateTime.Now.AddMinutes(-2), SerializedDuration = 5};
        //    serverModel.hwProcessor.StartTreatment(serverModel.Clients.First(), serverModel.Clients.First().PendingEvent);
        //}
    }
}
