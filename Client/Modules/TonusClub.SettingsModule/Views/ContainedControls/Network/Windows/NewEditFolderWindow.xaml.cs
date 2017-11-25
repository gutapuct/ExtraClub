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

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    /// <summary>
    /// Interaction logic for NewEditFolderWindow.xaml
    /// </summary>
    public partial class NewEditFolderWindow
    {
        public SettingsFolder SettingsFolder { get; set; }

        public List<SettingsFolder> SettingsFolders { get; set; }

        public List<CompanyView> Companies { get; set; }

        public bool SelectAllColumns
        {
            get
            {
                return !Companies.Any(i => !i.Helper);
            }
            set
            {
                Companies.ForEach(i => i.Helper = value);
                OnPropertyChanged("SelectAllColumns");
            }
        }

        public NewEditFolderWindow(ClientContext context, SettingsFolder folder, Guid parentId, int categoryId)
        {

            SettingsFolders = _context.GetSettingsFolders(categoryId, false);
            SettingsFolders.Insert(0, new SettingsFolder { Id = Guid.Empty, Name = "" });


            if (folder.Id == Guid.Empty || folder == null)
            {
                SettingsFolder = new SettingsFolder { Id = Guid.NewGuid(), ParentFolderId = parentId, Name = "Новая папка", CategoryId = categoryId };
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
            Companies = context.GetCompaniesList(SettingsFolder.Id);

            DataContext = this;

            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostSettingsFolder(SettingsFolder, Companies.Where(i => i.Helper).Select(i => i.Id).ToArray());
            DialogResult = true;
            Close();
        }
    }
}
