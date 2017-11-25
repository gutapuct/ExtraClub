using System;
using System.Collections.Generic;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.ClientDal.Wizards.NewClub
{
    public partial class NewEditStorehouseWindow
    {
        public Storehouse Storehouse { get; set; }
        public List<CompanySettingsFolder> SettingsFolders { get; set; }


        public NewEditStorehouseWindow(ClientContext context, Storehouse storehouse, Guid divId)
        {
            SettingsFolders = context.GetCompanySettingsFolders(5);

            DataContext = this;

            if (storehouse == null || storehouse.Id == Guid.Empty)
            {
                Storehouse = new Storehouse
                {
                    DivisionId = divId,
                    IsActive = true,
                    BarSale = true
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
