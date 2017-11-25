﻿using System;
using System.Windows;
using TonusClub.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using TonusClub.UIControls;
using System.Windows.Input;
using System.ComponentModel;
using TonusClub.UIControls.Windows;

using FlagmaxControls;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.WinClient
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {

            InitializeComponent();
            WpfEngine.Init(root);
        }

        private readonly MainContent _mc;
        private readonly ClientContext _context;
        private readonly IReportManager _repMan;

        public MainWindow(IUnityContainer container, ISettingsManager setMan, IReportManager repMan, ClientContext context)
            : this()
        {
            _context = context;
            _repMan = repMan;

            root.Content = _mc = container.Resolve<MainContent>();

            new System.Threading.Tasks.Task(() => _mc.LoadUiAsync()).Start();

            setMan.RegisterWindow(this);
            DataContext = this;

            Title = "ON Flagmax [" + _context.CurrentDivision.Name + "] — " + _context.CurrentUser.FullName;
        }

        #region old
        bool _runOnceFlag;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (_runOnceFlag) return;
            _runOnceFlag = true;

            Mouse.OverrideCursor = Cursors.Wait;

            if (_context.NeedAct)
            {
                TonusWindow.Confirm(UIControls.Localization.Resources.Statement,
                    UIControls.Localization.Resources.AsuStatement, e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ActGenerator.GenerateAct(_context, _repMan);
                        }
                        else
                        {
                            Application.Current.Shutdown();
                        }
                    });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


    }
}
