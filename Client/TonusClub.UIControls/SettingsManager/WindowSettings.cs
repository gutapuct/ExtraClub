using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.Infrastructure.Extensions;

namespace TonusClub.UIControls.SettingsManager
{
    internal class WindowSettings : IStateContainer
    {
        public WindowSettings()
        {
            //
        }

        public class WindowApplicationSettings : Dictionary<string, object>
        {
            private WindowSettings settings;

            private DataContractSerializer serializer = null;

            public WindowApplicationSettings()
            {
                //
            }

            public WindowApplicationSettings(WindowSettings settings)
            {
                this.settings = settings;

                List<Type> types = new List<Type>();
                types.Add(typeof(Size));
                types.Add(typeof(Point));
                types.Add(typeof(WindowState));

                this.serializer = new DataContractSerializer(typeof(WindowApplicationSettings), types);
            }

            public string PersistID
            {
                get
                {
                    if (!ContainsKey("PersistID") && settings.wnd != null)
                    {
                        this["PersistID"] = settings.wnd.GetType().FullName;
                    }

                    return (string)this["PersistID"];
                }
            }

            public WindowState WindowState
            {
                get
                {
                    if (!ContainsKey("WindowState"))
                    {
                        this["WindowState"] = WindowState.Normal;
                    }

                    return (WindowState)this["WindowState"];
                }
                set
                {
                    this["WindowState"] = value;
                }
            }

            public double Left
            {
                get
                {
                    if (!ContainsKey("Left"))
                    {
                        this["Left"] = 0.0;
                    }

                    return (double)this["Left"];
                }
                set
                {
                    this["Left"] = value;
                }
            }


            public double Top
            {
                get
                {
                    if (!ContainsKey("Top"))
                    {
                        this["Top"] = 0.0;
                    }

                    return (double)this["Top"];
                }
                set
                {
                    this["Top"] = value;
                }
            }
            public Size Size
            {
                get
                {
                    if (!ContainsKey("Size"))
                    {
                        this["Size"] = new Size(800,600);
                    }

                    return (Size)this["Size"];
                }
                set
                {
                    this["Size"] = value;
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
                                WindowApplicationSettings loaded = (WindowApplicationSettings)serializer.ReadObject(stream);

                                Size = loaded.Size;
                                Left = loaded.Left;
                                Top = loaded.Top;
                                WindowState = loaded.WindowState;
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

        private ContentControl wnd = null;

        public WindowSettings(ContentControl wnd)
        {
            this.wnd = wnd;
            Attach();
        }

        public virtual void LoadState()
        {
            try
            {
                Settings.Reload();
            }
            catch
            {
                Settings.Reset();
            }

            if (this.wnd != null)
            {
                wnd.SetValue("WindowState", Settings.WindowState);
                wnd.Width = Settings.Size.Width;
                wnd.Height = Settings.Size.Height;
                wnd.SetValue("Left", Settings.Left);
                wnd.SetValue("Top", Settings.Top);
            }
        }

        public virtual void ResetState()
        {
            Settings.Reset();
        }

        public virtual void SaveState()
        {
            //if (grid.ItemsSource == null) return;

            Settings.Reset();

            if (wnd != null)
            {
                Settings.WindowState = (WindowState)wnd.GetValue("WindowState");
                Settings.Left = (double)wnd.GetValue("Left");
                Settings.Top = (double)wnd.GetValue("Top");
                Settings.Size = wnd.RenderSize;
            }

            Settings.Save();
        }

        private void Attach()
        {
            if (this.wnd != null)
            {
                //this.wnd.LayoutUpdated += new EventHandler(LayoutUpdated);
                this.wnd.Loaded += Loaded;
                if (wnd is Window) ((Window)wnd).Closed += new EventHandler(Current_Exit);
            }
        }

        void WindowSettings_Closed(object sender, WindowClosedEventArgs e)
        {
            SaveState();
        }

        void Current_Exit(object sender, EventArgs e)
        {
            SaveState();
        }

        void Loaded(object sender, EventArgs e)
        {
            LoadState();
        }

        void LayoutUpdated(object sender, EventArgs e)
        {
            if (wnd != null)
            {
                SaveState();
            }
        }

        private WindowApplicationSettings windowSettings = null;

        protected virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
        {
            return new WindowApplicationSettings(this);
        }

        protected WindowApplicationSettings Settings
        {
            get
            {
                if (windowSettings == null)
                {
                    windowSettings = CreateWindowApplicationSettingsInstance();
                }
                return windowSettings;
            }
        }
    }

}