using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure;
using TonusClub.Infrastructure.Interfaces;

namespace TonusClub.UIControls.BaseClasses
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _dataLoaded;
        private bool _dataLoading;

        protected IDictionaryManager DictManager => ApplicationDispatcher.UnityContainer.Resolve<IDictionaryManager>();

        public bool IsUpdating { get; private set; }

        public event EventHandler<CancelEventArgs> CommittingChanges;

        public event EventHandler UpdateFinished;

        BackgroundWorker _refreshWorker;

        readonly Lazy<ClientContext> _context;

        public ClientContext ClientContext => _context.Value;

        protected ViewModelBase()
        {
            _context = new Lazy<ClientContext>(() => ApplicationDispatcher.UnityContainer.Resolve<ClientContext>());

            IsUpdating = false;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _refreshWorker = new BackgroundWorker();
                _refreshWorker.DoWork += bw_DoWork;
                _refreshWorker.RunWorkerCompleted += DoFinishRefresh;
            }));
        }

        public void SaveChangesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnCommittingChanges())
            {
                SaveChangesInternal(e);
            }
        }

        protected virtual void SaveChangesInternal(ExecutedRoutedEventArgs e)
        {
        }

        protected virtual bool OnCommittingChanges()
        {
            if (CommittingChanges != null)
            {
                var e = new CancelEventArgs(false);
                CommittingChanges(this, e);
                return !e.Cancel;
            }
            return true;
        }

        /// <summary>
        /// Обновление Коллекций и прочей радости должно быть здесь.
        /// Метод выполняется асинхронно, так что все, что связано с UI, лучше помещать в RefreshFinished
        /// </summary>
        protected abstract void RefreshDataInternal();

        protected void CancelAddEdit(ICollectionView collection)
        {
            var view = collection as ListCollectionView;
            if (view != null)
            {
                if (view.IsEditingItem)
                {
                    if (view.CanCancelEdit) view.CancelEdit();
                    else view.CommitEdit();
                }
                if (view.IsAddingNew) view.CancelNew();
            }
        }

        protected bool IsDeleted(object obj)
        {
            if (obj?.GetType().GetProperty("Deleted") != null)
            {
                return !(bool)obj.GetType().GetProperty("Deleted").GetValue(obj, null);
            }
            return true;
        }

        public void EnsureDataLoading()
        {
            if (_dataLoaded || _dataLoading) return;
            RefreshDataAsync();
        }

        public void EnsureDataLoaded()
        {
            if (_dataLoaded) return;
            if (_dataLoading)
            {
                do { Thread.Sleep(300); } while (_refreshWorker.IsBusy);
            }
            else
            {
                if (!_refreshWorker.IsBusy)
                {
                    RefreshDataSync();
                }
            }
        }

        public void RefreshDataAsync(Action onCompleted)
        {

            Application.Current.Dispatcher.BeginInvoke(new Action(() => { Mouse.OverrideCursor = Cursors.AppStarting; }), null);
            if (!_refreshWorker.IsBusy)
            {
                _dataLoading = true;
                _refreshWorker.RunWorkerAsync(onCompleted);
            }
        }

        public void RefreshDataAsync()
        {
            RefreshDataAsync(null);
        }

        public void RefreshDataSync()
        {
            using (new OverrideCursor(Cursors.Wait))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    IsUpdating = true;
                    RefreshDataInternal();
                    RefreshFinished();
                }
                finally
                {
                    IsUpdating = false;
                    Mouse.OverrideCursor = null;
                }
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CultureHelper.FixupCulture();
                IsUpdating = true;
                RefreshDataInternal();
                e.Result = e.Argument;
            }
            catch (FaultException)
            {
                //Application.Current.Shutdown();
            }
            IsUpdating = false;
        }

        void DoFinishRefresh(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Mouse.OverrideCursor = Cursors.AppStarting; }), null);
                if (e.Error != null) throw e.Error;
                RefreshFinished();
                OnUpdateFinished();
                _dataLoaded = true;

                var action = e.Result as Action;
                action?.Invoke();
            }
            finally
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Mouse.OverrideCursor = null; }), null);
            }
        }

        protected void OnUpdateFinished()
        {
            UpdateFinished?.Invoke(this, null);
        }

        /// <summary>
        /// Все операции, так или иначе связанные с UI, помещать сюда.
        /// Метод выполняется в UI потоке сразу после завершения Refresh()
        /// </summary>
        protected virtual void RefreshFinished()
        {
            CultureHelper.FixupCulture();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public static T Clone<T>(object obj)
            where T : class
        {
            if (obj == null) return null;
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            T newObject = (T)dcSer.ReadObject(memoryStream);
            return newObject;
        }

        public void WaitUpdateFinished()
        {
            while (IsUpdating)
            {
                Thread.Sleep(200);
            }
        }

        protected void RefreshAsync<T>(List<T> container, ICollectionView view, Func<IEnumerable<T>> getter, bool updateUi = true)
        {
            container.Clear();

            if (updateUi)
            {
                Application.Current?.Dispatcher.Invoke(new Action(view.Refresh));
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                container.AddRange(getter.Invoke());

                if (updateUi)
                {
                    Application.Current?.Dispatcher.Invoke(new Action(view.Refresh));
                }
            });
        }
    }
}
