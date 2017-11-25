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
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    public partial class NewEditCompanyFolderWindow
    {
        public CompanySettingsFolder SettingsFolder { get; set; }

        public List<CompanySettingsFolder> SettingsFolders { get; set; }

        public NewEditCompanyFolderWindow(ClientContext context, CompanySettingsFolder folder, Guid parentId, int categoryId)
        {
            DataContext = this;

            SettingsFolders = _context.GetCompanySettingsFolders(categoryId);
            SettingsFolders.Insert(0, new CompanySettingsFolder { Id = Guid.Empty, Name = "" });


            if (folder.Id == Guid.Empty || folder == null)
            {
                SettingsFolder = new CompanySettingsFolder { Id = Guid.NewGuid(), CompanyId = context.CurrentCompany.CompanyId, ParentFolderId = parentId, Name = "Новая папка", CategoryId = categoryId };
            }
            else
            {
                SettingsFolder = folder;
                var f = SettingsFolders.SingleOrDefault(i => i.Id == folder.Id);
                if (f != null && SettingsFolders.Contains(f))
                {
                    SettingsFolders.Remove(f);
                }
            }

            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostCompanySettingsFolder(SettingsFolder);
            DialogResult = true;
            Close();
        }
    }

}
