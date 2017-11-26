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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    /// <summary>
    /// Interaction logic for NewEditTemplateWindow.xaml
    /// </summary>
    public partial class NewEditTemplateWindow
    {
        public ReportTemplate Templ { get; set; }

        public NewEditTemplateWindow(ClientContext context, ReportTemplate template, ISettingsManager setMan)
        {
            this.DataContext = this;
            Owner = Application.Current.MainWindow;

            Templ = ViewModelBase.Clone<ReportTemplate>(template);
            InitializeComponent();
            setMan.RegisterWindow(this);
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostReportTemplate(Templ);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
