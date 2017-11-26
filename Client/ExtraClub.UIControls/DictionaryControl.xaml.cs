using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Threading;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.UIControls
{
    /// <summary>
    /// Interaction logic for DictionaryControl.xaml
    /// </summary>
    public partial class DictionaryControl : UserControl
    {
        public List<DictionaryInfo> RegisteredDictionaries { get; private set; }
        public Dictionary<DictionaryInfo, ICollectionView> AssotiatedViews { get; private set; }

        public ICollectionView RegisteredDictionariesView { get; set; }

        private IDictionaryManager _dictManager;
        private ClientContext _context;

        private DictionaryInfo _currentItem;

        [Obsolete("Только для дизайн-тайм!", true)]
        public DictionaryControl() {
            InitializeComponent();
        }

        public void Init(IDictionaryManager dictManager, ClientContext context)
        {
            DataContext = this;
            _dictManager = dictManager;
            _context = context;
            RegisteredDictionaries = new List<DictionaryInfo>();
            RegisteredDictionariesView = CollectionViewSource.GetDefaultView(RegisteredDictionaries);
            AssotiatedViews = new Dictionary<DictionaryInfo, ICollectionView>();

            InitializeComponent();
            
            _dictManager = dictManager;

        }

        public void RegisterDictionary(string entitySetName)
        {
            new Thread(DoWork).Start(entitySetName);
        }

        private void DoWork(object entitySetName)
        {
            try
            {
                var ent = (string)entitySetName;
                DictionaryInfo info = _dictManager.GetDictionaryInfoBySetName(ent);
                RegisteredDictionaries.Add(info);
                AssotiatedViews.Add(info, _dictManager.GetViewSource(ent));
            }
            catch { }
        }

        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DictionaryGrid.CancelEdit();
            if (e.AddedItems.Count == 0)
            {
                _currentItem = null;
                DictionaryGrid.ItemsSource = null;
                return;
            }

            _currentItem = (DictionaryInfo)e.AddedItems[0];
            DictionaryGrid.ItemsSource = AssotiatedViews[_currentItem];
            DictionaryGrid.Focus();
        }


        void RefreshAll()
        {
            foreach (var dictInfo in RegisteredDictionaries)
            {
                _dictManager.RefreshDictionary(dictInfo);
            }
        }

        private void AddNewElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentItem == null) return;
            ExtraWindow.Prompt(UIControls.Localization.Resources.ElementAdding,
                UIControls.Localization.Resources.ProvideElementName,
                "",
                wnd => AddClosed(wnd));
        }
        private void AddClosed(PromptWindow wnd)
        {
            if (wnd.DialogResult ?? false)
            {
                try
                {
                    if (String.IsNullOrEmpty(wnd.TextResult.Trim())) throw new Exception(UIControls.Localization.Resources.EmptyElement);
                    if (_dictManager.ContainsElement(_currentItem, wnd.TextResult.Trim())) throw new Exception(UIControls.Localization.Resources.ElementExists);
                    _dictManager.AddNewElement(_currentItem, wnd.TextResult.Trim());
                }
                catch (Exception ex)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.Error,
                        Content = UIControls.Localization.Resources.UnableAddElement + "\n" + ex.Message,
                        OkButtonContent = UIControls.Localization.Resources.Ok,
                        Owner = Application.Current.MainWindow
                    });
                }
            }
        }

        private void EditElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentItem == null) return;
            if (AssotiatedViews[_currentItem].CurrentItem == null) return;
            ExtraWindow.Prompt(UIControls.Localization.Resources.ElementEdit,
                 UIControls.Localization.Resources.ProvideElementNewName,
                ((DictionaryPair)AssotiatedViews[_currentItem].CurrentItem).Value.Trim(),
                wnd => EditClosed(wnd));
        }

        private void EditClosed(PromptWindow wnd)
        {
            if (wnd.DialogResult ?? false)
            {
                try
                {
                    if (String.IsNullOrEmpty(wnd.TextResult.Trim())) throw new Exception(UIControls.Localization.Resources.EmptyElement);
                    var currentText = ((DictionaryPair)AssotiatedViews[_currentItem].CurrentItem).Value.Trim();
                    if (wnd.TextResult.Trim() == currentText) return;
                    if (_dictManager.ContainsElement(_currentItem, wnd.TextResult.Trim()) && wnd.TextResult.Trim().ToLower() != currentText.ToLower()) throw new Exception(UIControls.Localization.Resources.ElementExists);
                    _dictManager.RenameElement(_currentItem, ((DictionaryPair)AssotiatedViews[_currentItem].CurrentItem).Key, wnd.TextResult.Trim());
                }
                catch (Exception ex)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.Error,
                        Content = UIControls.Localization.Resources.UnableEdit + "\n" + ex.Message,
                        OkButtonContent = UIControls.Localization.Resources.Ok
                    });
                }
            }
        }

        private void DictionaryGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    EditElementButton_Click(null, null);
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentItem == null) return;
            if (AssotiatedViews[_currentItem].CurrentItem == null) return;

            ExtraWindow.Confirm(UIControls.Localization.Resources.Deletion,
                 UIControls.Localization.Resources.DeleteSelectedElement,
                wnd =>
                {
                    if ((wnd.DialogResult ?? false))
                    {
                        try
                        {
                            var res = _dictManager.RemoveElement(_currentItem, ((DictionaryPair)AssotiatedViews[_currentItem].CurrentItem).Key);
                            //Пока не настроили трассировку эксепшенов с сервера
                            if (!String.IsNullOrEmpty(res)) throw new Exception(res);
                        }
                        catch (Exception ex)
                        {
                            ExtraWindow.Alert(new DialogParameters
                            {
                                Header = UIControls.Localization.Resources.Error,
                                Content = UIControls.Localization.Resources.UnableToDelete + "\n" + ex.Message,
                                OkButtonContent = UIControls.Localization.Resources.Ok,
                                Owner = Application.Current.MainWindow
                            });
                        }
                    }
                });
        }
    }
}
