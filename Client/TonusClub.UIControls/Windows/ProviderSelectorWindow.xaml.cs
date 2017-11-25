using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows;
using Telerik.Windows.Controls;
using TonusClub.ServiceModel;

namespace TonusClub.UIControls.Windows
{
    /// <summary>
    /// Interaction logic for PriderSelectorWindow.xaml
    /// </summary>
    public partial class ProviderSelectorWindow
    {
        private ProviderFolder _currentProviderFolder;
        public List<ProviderFolder> Folders { get; set; }
        public List<Provider> Providers { get; set; }

        public ICollectionView ProvidersView { get; set; }

        private Provider _selectedProvider;
        public Provider SelectedProvider
        {
            get
            {
                return _selectedProvider;
            }
            set
            {
                _selectedProvider = value;
                if (value != null) SelectedId = value.Id;
                else SelectedId = null;
                OnPropertyChanged("SelectedProvider");
            }
        }

        public ProviderSelectorWindow(List<ProviderFolder> folders, List<Provider> providers)
        {
            Folders = folders;
            Providers = providers;
            DataContext = this;

            ConstructProviderFolders();

            ProvidersView = CollectionViewSource.GetDefaultView(Providers);

            ProvidersView.Filter = delegate(object item)
            {
                if (item == null) return false;
                var id = ((Provider)item).ProviderFolderId;
                return !id.HasValue;
            };

            InitializeComponent();
            ProvidersTree.ExpandAll();
        }


        private void ConstructProviderFolders()
        {
            if (Folders.Any(i => i.Id == Guid.Empty)) return;
            var src = Folders.ToList();
            Folders.Clear();
            Folders.Add(new ProviderFolder { Id = Guid.Empty, Name = "Контрагенты" });
            src.Where(i => !i.ParentFolderId.HasValue).ToList().ForEach(i =>
            {
                Folders[0].Children.Add(i);
                src.Remove(i);
            });
            var cnt = 0;
            while (src.Count != cnt)
            {
                cnt = src.Count;
                foreach (var i in src.ToList())
                {
                    if (i.ParentFolderId != null)
                    {
                        var host = SearchList(Folders, i.ParentFolderId.Value);
                        host.Children.Add(i);
                    }
                    src.Remove(i);
                }
            }
        }

        public ProviderFolder SearchList(List<ProviderFolder> src, Guid targetId)
        {
            foreach (var i in src)
            {
                if (i.Id == targetId)
                {
                    return i;
                }
                var res = SearchList(i.Children, targetId);
                if (res != null) return res;
            }
            return null;
        }

        public Guid? SelectedId { get; set; }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadTreeView_Selected(object sender, RadRoutedEventArgs e)
        {
            _currentProviderFolder = ((RadTreeView) e.Source).SelectedItem as ProviderFolder;
            ProvidersView.Filter = delegate(object item)
            {
                if (item == null) return false;
                var id = ((Provider)item).ProviderFolderId;
                if (_currentProviderFolder == null) return !id.HasValue;
                return (id ?? Guid.Empty) == _currentProviderFolder.Id;
            };
        }
        
        private void ProvidersViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CreateButton_Click(null, null);
        }
    }
}
