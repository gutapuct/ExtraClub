using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.Interfaces;
using System.Windows.Data;
using System.Collections;
using TonusClub.ServiceModel;
using System.ComponentModel;
//using TonusClub.Entities;
using System.Threading;
using System.Diagnostics;
using TonusClub.UIControls;

namespace TonusClub.DictionaryManager
{
    public class DictionaryManager: IDictionaryManager, IInitializeable
    {
        readonly IUnityContainer _container;
        private ClientContext Context
        {
            get
            {
                return _container.Resolve<ClientContext>();
            }
        }
        private Dictionary<string, DictionaryInfo> _infos;
        private Dictionary<string, List<DictionaryPair>> _loaded;
        private Dictionary<string, List<ICollectionView>> _registeredVS;

        private object lck = new object();

        public DictionaryManager(IUnityContainer container)
        {
            _container = container;
            _loaded = new Dictionary<string, List<DictionaryPair>>();
            _registeredVS = new Dictionary<string, List<ICollectionView>>();
        }

        public ICollectionView GetViewSource(string tableName)
        {
            TestInitialize();
            lock (lck)
            {
                //if (!_infos.ContainsKey(tableName)) return null;
                if (!_loaded.ContainsKey(tableName)) LoadDictionaryAsync(tableName);
                return RegisterCollectionViewSource(tableName, _loaded[tableName]);
            }
        }

        private ICollectionView RegisterCollectionViewSource(string tableName, object source)
        {
            TestInitialize();

            lock (lck)
            {

                var iCollectionView = CollectionViewSource.GetDefaultView(source);
                //iCollectionView.Source = source;
                if (!_registeredVS.ContainsKey(tableName)) _registeredVS.Add(tableName, new List<ICollectionView>());
                if (!_registeredVS[tableName].Contains(iCollectionView)) _registeredVS[tableName].Add(iCollectionView);
                return iCollectionView;
            }
        }

        private void LoadDictionaryAsync(string tableName)
        {
            TestInitialize();
            _loaded.Add(tableName, new List<DictionaryPair>());
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(LoadDictionaryAsync_Main);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadDictionaryAsync_Completed);
            bw.RunWorkerAsync(tableName);
        }

        void LoadDictionaryAsync_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (lck)
            {
                if (!_registeredVS.ContainsKey((string)e.Result)) return;
                foreach (var view in _registeredVS[(string)e.Result])
                {
                    view.Refresh();
                }
            }
        }

        void LoadDictionaryAsync_Main(object sender, DoWorkEventArgs e)
        {
            LoadDictionary((string)e.Argument);
            e.Result = e.Argument;
        }

        private void LoadDictionary(string tableName)
        {
            try
            {
                TestInitialize();
                lock (lck)
                {

                    if (!_loaded.ContainsKey(tableName))
                    {
                        _loaded.Add(tableName, new List<DictionaryPair>());
                    }
                    else
                    {
                        _loaded[tableName].Clear();
                    }

                    var lst = _loaded[tableName];
                    foreach (var pair in Context.GetDictionaryList(tableName))
                    {
                        lst.Add(new DictionaryPair(pair.Key, pair.Value));
                    }
                    lst.Sort();
                }
            }
            catch { throw; }
        }

        public DictionaryInfo GetDictionaryInfoBySetName(string entitySetName)
        {
            TestInitialize();
            lock (lck)
            {

                while (_infos == null) { Thread.Sleep(100); };
                return _infos[entitySetName];
            }
        }

        public void RefreshDictionary(DictionaryInfo dictInfo)
        {
            TestInitialize();
            lock (lck)
            {

                _loaded[dictInfo.EntitySetName].Clear();
                foreach (var pair in Context.GetDictionaryList(dictInfo.EntitySetName))
                {
                    _loaded[dictInfo.EntitySetName].Add(new DictionaryPair(pair.Key, pair.Value));
                }

                foreach (var reg in _registeredVS[dictInfo.EntitySetName])
                {
                    reg.Refresh();
                }
            }
        }

        private bool HasChanges(string setName)
        {
            if (!_loaded.ContainsKey(setName)) return false;
            return _loaded[setName].Any(p => p.Modified);
        }

        public bool HasChanges(DictionaryInfo di)
        {
            return HasChanges(di.EntitySetName);
        }

        public void TestInitialize()
        {
            if (!Initialized) TryInitialize();
            if (!Initialized) throw new TypeInitializationException("DictionaryManager", null);
        }

        public void TryInitialize()
        {
            //загружаем все DictionaryInfo
            Initialized = true;
            //LoadInfoAsync();
            LoadInfoAsync_Work(null, null);
        }

        private void LoadInfoAsync()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(LoadInfoAsync_Work);
            bw.RunWorkerAsync();
        }

        void LoadInfoAsync_Work(object sender, DoWorkEventArgs e)
        {
            if (_infos == null)
            {
                _infos = Context.GetAllDictionaryInfos();
            }
        }

        public bool Initialized
        {
            get;
            private set;
        }

        public string GetValue(string Dictionary, Guid? Key)
        {
            if (Key == null) return null;
            if (!_loaded.ContainsKey(Dictionary)) LoadDictionary(Dictionary);
            var pair = _loaded[Dictionary].FirstOrDefault(p => p.Key == Key);
            if (pair == null) return null;
            return pair.Value;
        }

        public bool ContainsElement(DictionaryInfo dictionary, string elementName)
        {
            if (String.IsNullOrEmpty(elementName) || String.IsNullOrEmpty(elementName.Trim())) return false;
            if (!_loaded.ContainsKey(dictionary.EntitySetName)) LoadDictionary(dictionary.EntitySetName);
            return _loaded[dictionary.EntitySetName].Any(p => p.Value.Trim().ToLower() == elementName.Trim().ToLower());
        }

        /// <summary>
        /// Должен запускаться в UI потоке - обновляет вьюхи!
        /// Если совсем припрет, надо или проверку сделать, или параметр добавить.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="elementName"></param>
        public void AddNewElement(DictionaryInfo dictionary, string elementName)
        {
            Context.PostNewDictionaryElement(dictionary.Id, elementName.Trim());
            RefreshDictionary(dictionary);
        }

        public void RenameElement(DictionaryInfo dictionary, Guid elementGuid, string newName)
        {
            Context.PostRenameDictionaryElement(dictionary.Id, elementGuid, newName.Trim());
            RefreshDictionary(dictionary);
        }

        public string RemoveElement(DictionaryInfo dictionary, Guid elementGuid)
        {
            var res = Context.PostRemoveDictionaryElement(dictionary.Id, elementGuid);
            if (String.IsNullOrEmpty(res)) {
                RefreshDictionary(dictionary);
            }
            return res;
        }
    }

}
