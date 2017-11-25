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
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for NewEditStorehouseWindow.xaml
    /// </summary>
    public partial class NewEditStorehouseWindow
    {
        public Storehouse Storehouse { get; set; }
        public List<CompanySettingsFolder> SettingsFolders { get; set; }


        public NewEditStorehouseWindow(ClientContext context, Storehouse storehouse)
            : base(context)
        {
            SettingsFolders = context.GetCompanySettingsFolders(5);

            this.DataContext = this;
            Owner = Application.Current.MainWindow;

            if (storehouse == null || storehouse.Id == Guid.Empty)
            {
                Storehouse = new Storehouse
                {
                    DivisionId = context.CurrentDivision.Id
                };
            }
            else
            {
                Storehouse = storehouse;
            }
            InitializeComponent();
        }


        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostStorehouse(Storehouse);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
