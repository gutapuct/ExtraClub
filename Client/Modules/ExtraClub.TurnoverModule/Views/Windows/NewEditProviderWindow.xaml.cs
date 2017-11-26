using System;
using System.Collections.Generic;
using System.Windows;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using ExtraClub.UIControls;

namespace ExtraClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewEditProviderWindow.xaml
    /// </summary>
    public partial class NewEditProviderWindow
    {
        public Provider Provider { get; set; }

        public List<ProviderFolder> ProviderFolders { get; set; }

        public ICollectionView OrganizationTypes { get; set; }

        public NewEditProviderWindow(ClientContext context, Provider provider, Guid folderId, IDictionaryManager dictMan)
        {
            DataContext = this;

            OrganizationTypes = dictMan.GetViewSource("OrganizationTypes");

            ProviderFolders = _context.GetProviderFolders();
            ProviderFolders.Insert(0, new ProviderFolder { Id = Guid.Empty, Name = UIControls.Localization.Resources.Contragents });


            if (provider.Id == Guid.Empty || provider == null)
            {
                Provider = new Provider { Id = Guid.NewGuid(), ProviderFolderId = folderId, IsVisible = true };
            }
            else
            {
                Provider = provider;
            }

            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _context.PostProvider(Provider);
            Close();
        }
    }
}
