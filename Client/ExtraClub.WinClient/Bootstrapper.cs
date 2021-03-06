﻿using System.Windows;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using Microsoft.Practices.Unity;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.UIControls.SettingsManager;
using System.Diagnostics;
using ExtraClub.UIControls;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.ClientDal.Wizards.NewClub;

namespace ExtraClub.WinClient
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var w = Stopwatch.StartNew();

            Window wnd;

            var context = Container.Resolve<ClientContext>();
            if(context.CurrentDivision == null)
            {
                wnd = new NewDivisionWizard(context);
            }
            else
            {
                wnd = Container.Resolve<MainWindow>();
            }
            AuthorizationManager.InitAnchor(wnd);
            Application.Current.MainWindow = wnd;
            Shell = wnd;
            Debug.WriteLine($"Resolve shell takes {w.ElapsedMilliseconds} ms");
            return Shell;
        }

        protected override IModuleCatalog GetModuleCatalog()
        {
            var catalog = new ConfigurationModuleCatalog();
            return catalog;
        }

        protected override IUnityContainer CreateContainer()
        {
            var res = base.CreateContainer();
            Infrastructure.ApplicationDispatcher.UnityContainer = res;

            new Reports.IoCModule().RegisterModule(res);

            res.RegisterType<ISettingsManager, SettingsManager>(new ContainerControlledLifetimeManager());
            res.RegisterType<IClientContext, ClientContext>(new ContainerControlledLifetimeManager());
            return res;
        }
        protected override void InitializeModules()
        {
            var w = Stopwatch.StartNew();
            base.InitializeModules();
            Debug.WriteLine("InitializeModules takes {0} ms", w.ElapsedMilliseconds);
        }

        public DependencyObject Shell { get; private set; }
    }
}
