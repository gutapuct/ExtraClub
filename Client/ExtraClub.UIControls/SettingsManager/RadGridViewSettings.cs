using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Runtime.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using GridViewColumn = Telerik.Windows.Controls.GridViewColumn;
using ExtraClub.Infrastructure.Interfaces;
using System.Diagnostics;

namespace ExtraClub.UIControls.SettingsManager
{
    internal class RadGridViewSettings : IStateContainer
    {
        public RadGridViewSettings()
        {
            //
        }

        public class RadGridViewApplicationSettings : Dictionary<string, object>
        {
            private RadGridViewSettings settings;

            private DataContractSerializer serializer = null;

            public RadGridViewApplicationSettings()
            {
                //
            }

            public RadGridViewApplicationSettings(RadGridViewSettings settings)
            {
                this.settings = settings;

                List<Type> types = new List<Type>();
                types.Add(typeof(List<ColumnSetting>));
                types.Add(typeof(List<GroupSetting>));
                types.Add(typeof(List<SortSetting>));
                types.Add(typeof(List<PropertySetting>));

                this.serializer = new DataContractSerializer(typeof(RadGridViewApplicationSettings), types);
            }

            public string PersistID
            {
                get
                {
                    if (!ContainsKey("PersistID") && settings.grid != null)
                    {
                        this["PersistID"] = settings.grid.Name;
                    }

                    return (string)this["PersistID"];
                }
            }

            public int FrozenColumnCount
            {
                get
                {
                    if (!ContainsKey("FrozenColumnCount"))
                    {
                        this["FrozenColumnCount"] = 0;
                    }

                    return (int)this["FrozenColumnCount"];
                }
                set
                {
                    this["FrozenColumnCount"] = value;
                }
            }

            public List<ColumnSetting> ColumnSettings
            {
                get
                {
                    if (!ContainsKey("ColumnSettings"))
                    {
                        this["ColumnSettings"] = new List<ColumnSetting>();
                    }

                    return (List<ColumnSetting>)this["ColumnSettings"];
                }
            }

            public List<SortSetting> SortSettings
            {
                get
                {
                    if (!ContainsKey("SortSettings"))
                    {
                        this["SortSettings"] = new List<SortSetting>();
                    }

                    return (List<SortSetting>)this["SortSettings"];
                }
            }

            public List<GroupSetting> GroupSettings
            {
                get
                {
                    if (!ContainsKey("GroupSettings"))
                    {
                        this["GroupSettings"] = new List<GroupSetting>();
                    }

                    return (List<GroupSetting>)this["GroupSettings"];
                }
            }

            public void Reload()
            {
                try
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain())
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(PersistID, FileMode.OpenOrCreate, file))
                        {
                            if (stream.Length > 0)
                            {
                                RadGridViewApplicationSettings loaded = (RadGridViewApplicationSettings)serializer.ReadObject(stream);

                                FrozenColumnCount = loaded.FrozenColumnCount;

                                ColumnSettings.Clear();
                                foreach (ColumnSetting cs in loaded.ColumnSettings)
                                {
                                    ColumnSettings.Add(cs);
                                }

                                GroupSettings.Clear();
                                foreach (GroupSetting gs in loaded.GroupSettings)
                                {
                                    GroupSettings.Add(gs);
                                }

                                SortSettings.Clear();
                                foreach (SortSetting ss in loaded.SortSettings)
                                {
                                    SortSettings.Add(ss);
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            public void Reset()
            {
                try
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain())
                    {
                        file.DeleteFile(PersistID);
                    }
                }
                catch
                {
                    //
                }
            }

            public void Save()
            {
                try
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain())
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(PersistID, FileMode.Create, file))
                        {
                            serializer.WriteObject(stream, this);
                        }
                    }
                }
                catch
                {
                    //
                }
            }
        }

        private RadGridView grid = null;

        public RadGridViewSettings(RadGridView grid)
        {
            this.grid = grid;
            Attach();
        }

        public virtual void LoadState()
        {
            _isLoaded = true;
            Debug.WriteLine("LoadState for " + grid.Name);
            try
            {
                Settings.Reload();
            }
            catch
            {
                Settings.Reset();
            }

            if (this.grid != null)
            {
                grid.FrozenColumnCount = Settings.FrozenColumnCount;

                if (Settings.ColumnSettings.Count > 0)
                {

                    if (grid.Columns.Count == Settings.ColumnSettings.Count)
                    {
                        foreach (ColumnSetting setting in Settings.ColumnSettings)
                        {
                            GridViewColumn column = grid.Columns[setting.UniqueName];
                            if (column == null)
                            {
                                column = grid.Columns.FirstOrDefault<GridViewColumn>(gwc => gwc.Name == setting.UniqueName);
                            }
                            if (column == null)
                            {
                                column = grid.Columns.FirstOrDefault<GridViewColumn>(gwc => gwc.Header != null &&
                                    gwc.Header.Equals(setting.Header) && !String.IsNullOrEmpty(gwc.Header as string));
                            }
                            if (column != null && setting.Width.HasValue && setting.Width.Value > 20 && !column.Width.IsStar &&!column.Width.IsAuto)
                            {
                                column.Width = new GridViewLength(setting.Width.Value);

                                if (setting.DisplayIndex != null && (setting.DisplayIndex ?? -1) >= 0)
                                {
                                    column.DisplayIndex = setting.DisplayIndex.Value;
                                }
                            }
                        }
                    }

                    //foreach (ColumnSetting setting in Settings.ColumnSettings)
                    //{
                    //    ColumnSetting currentSetting = setting;

                    //    GridViewColumn column = (from c in grid.Columns.OfType<GridViewColumn>()
                    //                             where c.UniqueName == currentSetting.UniqueName
                    //                             select c).FirstOrDefault();

                        //if (currentSetting.DisplayIndex != null && column != null && (currentSetting.DisplayIndex ?? -1) >= 0)
                        //{
                        //    column.DisplayIndex = currentSetting.DisplayIndex.Value;
                        //}
                    //}
                }
                using (grid.DeferRefresh())
                {
                    if (Settings.SortSettings.Count > 0)
                    {
                        grid.SortDescriptors.Clear();

                        foreach (SortSetting setting in Settings.SortSettings)
                        {
                            if (setting.PropertyName.Contains("&"))
                            {
                                ColumnSortDescriptor d = new ColumnSortDescriptor();
                                d.Column = grid.Columns[setting.PropertyName.Replace("&","")];
                                d.SortDirection = setting.SortDirection;

                                grid.SortDescriptors.Add(d);
                            }
                            else
                            {

                                Telerik.Windows.Data.SortDescriptor d = new Telerik.Windows.Data.SortDescriptor();
                                d.Member = setting.PropertyName;
                                d.SortDirection = setting.SortDirection;

                                grid.SortDescriptors.Add(d);
                            }
                        }
                    }

                    if (Settings.GroupSettings.Count > 0)
                    {
                        grid.GroupDescriptors.Clear();

                        foreach (GroupSetting setting in Settings.GroupSettings)
                        {
                            Telerik.Windows.Data.GroupDescriptor d = new Telerik.Windows.Data.GroupDescriptor();
                            d.Member = setting.PropertyName;
                            d.SortDirection = setting.SortDirection;
                            d.DisplayContent = setting.DisplayContent;

                            grid.GroupDescriptors.Add(d);
                        }
                    }
                }
            }
        }

        public virtual void ResetState()
        {
            Settings.Reset();
        }

        bool _isLoaded = false;

        public virtual void SaveState()
        {
            if (!_isLoaded) return;
            //if (grid.ItemsSource == null) return;
            Debug.WriteLine("SaveState for " + grid.Name);

            Settings.Reset();

            if (grid != null)
            {
                if (grid.Columns != null)
                {
                    Settings.ColumnSettings.Clear();

                    foreach (GridViewColumn column in grid.Columns)
                    {
                        ColumnSetting setting = new ColumnSetting();
                        if (column is GridViewDataColumn)
                        {
                            setting.PropertyName = ((GridViewDataColumn)column).DataMemberBinding.Path.Path;
                        }
                        setting.UniqueName = column.UniqueName ?? column.Name;
                        if (String.IsNullOrEmpty(setting.UniqueName)) setting.UniqueName = setting.PropertyName;
                        if (String.IsNullOrEmpty(setting.UniqueName)) continue;
                        setting.Header = column.Header;
                        setting.Width = column.ActualWidth;
                        setting.DisplayIndex = column.DisplayIndex;
                        Settings.ColumnSettings.Add(setting);
                    }
                }

                if (grid.SortDescriptors != null)
                {
                    Settings.SortSettings.Clear();

                    foreach (var d in grid.SortDescriptors)
                    {
                        if (d is Telerik.Windows.Data.SortDescriptor)
                        {
                            SortSetting setting = new SortSetting();

                            setting.PropertyName = ((Telerik.Windows.Data.SortDescriptor)d).Member;
                            setting.SortDirection = ((Telerik.Windows.Data.SortDescriptor)d).SortDirection;

                            Settings.SortSettings.Add(setting);
                        }
                        if (d is ColumnSortDescriptor)
                        {
                            SortSetting setting = new SortSetting();

                            setting.PropertyName = "&" + ((ColumnSortDescriptor)d).Column.UniqueName;
                            setting.SortDirection = ((ColumnSortDescriptor)d).SortDirection;

                            Settings.SortSettings.Add(setting);

                            
                        }
                    }
                }

                if (grid.GroupDescriptors != null)
                {
                    Settings.GroupSettings.Clear();

                    foreach (Telerik.Windows.Data.GroupDescriptor d in grid.GroupDescriptors)
                    {
                        GroupSetting setting = new GroupSetting();

                        setting.PropertyName = d.Member;
                        setting.SortDirection = d.SortDirection;
                        setting.DisplayContent = d.DisplayContent;

                        Settings.GroupSettings.Add(setting);
                    }
                }

                Settings.FrozenColumnCount = grid.FrozenColumnCount;
            }

            Settings.Save();
        }

        private void Attach()
        {
            if (this.grid != null)
            {
                //this.grid.LayoutUpdated += new EventHandler(LayoutUpdated);
                this.grid.Loaded += Loaded;
                Application.Current.Exit += Current_Exit;
            }
        }

        void Current_Exit(object sender, EventArgs e)
        {
            SaveState();
        }

        void Loaded(object sender, EventArgs e)
        {
            LoadState();
            this.grid.Loaded -= Loaded;
        }

        void LayoutUpdated(object sender, EventArgs e)
        {
            if (grid.Parent == null)
            {
                SaveState();
            }
        }

        private RadGridViewApplicationSettings gridViewApplicationSettings = null;

        protected virtual RadGridViewApplicationSettings CreateRadGridViewApplicationSettingsInstance()
        {
            return new RadGridViewApplicationSettings(this);
        }

        protected RadGridViewApplicationSettings Settings
        {
            get
            {
                if (gridViewApplicationSettings == null)
                {
                    gridViewApplicationSettings = CreateRadGridViewApplicationSettingsInstance();
                }
                return gridViewApplicationSettings;
            }
        }
    }

    public class PropertySetting
    {
        public string PropertyName { get; set; }
    }

    public class SortSetting : PropertySetting
    {
        public ListSortDirection SortDirection { get; set; }
    }

    public class GroupSetting : PropertySetting
    {
        public object DisplayContent { get; set; }

        public ListSortDirection? SortDirection { get; set; }
    }

    public class ColumnSetting : PropertySetting
    {
        public string UniqueName { get; set; }
        public object Header { get; set; }

        public double? Width { get; set; }
        public int? DisplayIndex { get; set; }
    }
}