using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Ssh;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.OrganizerModule.ViewModels
{
    public class SshFilesViewModel : ViewModelBase
    {
        public ICollectionView SshFoldersView { get; set; }
        public List<SshFolder> SshFolders = new List<SshFolder>();

        public ICollectionView SshFilesView { get; set; }
        public List<SshFile> SshFiles = new List<SshFile>();

        private SshFile _selectedFile;
        public SshFile SelectedFile
        {
            get
            {
                return _selectedFile;
            }
            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }

        public SshFilesViewModel()
        {
            SshFoldersView = CollectionViewSource.GetDefaultView(SshFolders);
            SshFilesView = CollectionViewSource.GetDefaultView(SshFiles);

            var availableRoots = new HashSet<string>();
            if (ClientContext.CheckPermission("SshBusiness"))
            {
                availableRoots.Add("/business");
            }
            if (ClientContext.CheckPermission("SshStart"))
            {
                availableRoots.Add("/start");
            }
            if (ClientContext.CheckPermission("SshCrysis"))
            {
                availableRoots.Add("/crysismaster");
            }

            SshFilesView.Filter = i =>
            {
                var sf = ((SshFile)i);
                var f = CurrentFolder;
                var isnewb = f != null && f.Path.StartsWith("~b/");
                var isnews = f != null && f.Path.StartsWith("~s/");
                var isnewc = f != null && f.Path.StartsWith("~c/");

                if (String.IsNullOrEmpty(SearchText))
                {
                    if (isnewb)
                    {
                        return sf.Avail && sf.Path.StartsWith("/business");
                    }
                    if (isnews)
                    {
                        return sf.Avail && sf.Path.StartsWith("/start");
                    }
                    if (isnewc)
                    {
                        return sf.Avail && sf.Path.StartsWith("/crysismaster");
                    }
                    if (sf.Avail) return false;
                    if (f != null)
                    {
                        return sf.Path.Substring(0, sf.Path.LastIndexOf('/')) == f.Path + "/" + f.Name;
                    }
                    return false;
                }
                else
                {
                    var path = sf.Path.ToLower();
                    return path.Contains(SearchText.ToLower()) && availableRoots.Any(j => path.StartsWith(j));
                }
            };
        }

        private SshFolder _sf;
        public SshFolder CurrentFolder
        {
            get
            {
                return _sf;
            }
            set
            {
                _sf = value;
                SshFilesView.Refresh();
                OnPropertyChanged("CurrentFolder");
            }
        }

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                SshFilesView.Refresh();
                OnPropertyChanged("SearchText");
            }
        }

        protected override void RefreshDataInternal()
        {
            if (SshFolders.Count == 0)
            {
                SshFolders.Clear();
                SshFolders.AddRange(ClientContext.GetSshFolders());
            }

            SshFiles.Clear();
            SshFiles.AddRange(ClientContext.GetSshFiles());
        }

        bool _flag = true;
        protected override void RefreshFinished()
        {
            if (_flag)
            {
                SshFoldersView.Refresh();
                _flag = false;
            }
            SshFilesView.Refresh();
            base.RefreshFinished();
        }

        internal void Enqueue()
        {
            if (SshFilesView.CurrentItem != null)
            {
                var i = (SshFile)SshFilesView.CurrentItem;
                ClientContext.EnqueueSshFile(i.Id);
            }
            RefreshDataSync();
        }
    }
}
