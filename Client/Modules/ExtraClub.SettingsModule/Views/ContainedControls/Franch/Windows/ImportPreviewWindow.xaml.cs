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
using System.ComponentModel;
using ExtraClub.UIControls;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    /// <summary>
    /// Interaction logic for ImportPreviewWindow.xaml
    /// </summary>
    public partial class ImportPreviewWindow
    {
        public List<Customer> Customers { get; set; }

        BackgroundWorker bw = new BackgroundWorker();

        public ImportPreviewWindow(ClientContext context, List<Customer> customers)
            : base(context)
        {
            Customers = customers;
            DataContext = this;
            InitializeComponent();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = bw.IsBusy;
            base.OnClosing(e);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var i = 0;
            foreach (var c in Customers)
            {
                i++;
                _context.PostCustomer(c);
                Dispatcher.Invoke(new Action(() =>
                {
                    impText.Text = String.Format("Проимпортировано клиентов: {0} из {1}", i, Customers.Count);
                }), new object[0]);
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            bCancel.IsEnabled = false;
            bSave.IsEnabled = false;
            impText.Text = "";
            bw.RunWorkerAsync();
        }
    }
}
