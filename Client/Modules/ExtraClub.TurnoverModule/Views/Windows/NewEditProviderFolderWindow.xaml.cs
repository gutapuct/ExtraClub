using System;
using System.Collections.Generic;
using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewEditProviderFolderWindow.xaml
    /// </summary>
    public partial class NewEditProviderFolderWindow
    {
        public ProviderFolder ProviderFolder { get; set; }

        public List<ProviderFolder> ProviderFolders { get; set; }

        public NewEditProviderFolderWindow(ClientContext context, ProviderFolder folder, Guid parentId)
        {
            DataContext = this;

            ProviderFolders = _context.GetProviderFolders();
            ProviderFolders.Insert(0, new ProviderFolder { Id = Guid.Empty, Name = UIControls.Localization.Resources.Contragents });


            if (folder.Id == Guid.Empty || folder == null)
            {
                ProviderFolder = new ProviderFolder { Id = Guid.NewGuid(), ParentFolderId = parentId, Name= UIControls.Localization.Resources.NewGroup};
            }
            else
            {
                ProviderFolder = folder;
            }

            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostProviderFolder(ProviderFolder);
            DialogResult = true;
            Close();
        }
    }
}
