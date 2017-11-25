using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Modularity;
using TonusClub.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.Regions;
using TonusClub.Infrastructure.Constants;

namespace TonusClub.DictionaryManager
{
    public class Module : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public Module(IRegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        public void Initialize()
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            _container.RegisterType<IDictionaryManager, DictionaryManager>(new ContainerControlledLifetimeManager());
            System.Diagnostics.Debug.WriteLine(this.GetType().ToString() + " initialization takes " + w.ElapsedMilliseconds + " ms.");
        }

    }
}
