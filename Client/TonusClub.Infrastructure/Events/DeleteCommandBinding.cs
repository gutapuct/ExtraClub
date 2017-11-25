using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Telerik.Windows.Controls;
using System.ComponentModel;

namespace TonusClub.Infrastructure.Events
{
    public class DeleteCommandBinding : CommandBinding
    {
        public static ICommand Instance = new RoutedUICommand();

        public DeleteCommandBinding()
            : base(Instance, ExecutedInternal, CanExecuteInternal)
        {
        }

        public static void CanExecuteInternal(object sender, CanExecuteRoutedEventArgs e)
        {
            var grid = sender as RadGridView;
            if (grid == null) return;
            if (grid.IsReadOnly) e.CanExecute = false;
            if (grid.CurrentItem != null)
            {
                e.CanExecute = true;
                e.Handled = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public static void ExecutedInternal(object sender, ExecutedRoutedEventArgs e)
        {
            var grid = sender as RadGridView;
            if (grid == null || grid.IsReadOnly || grid.CurrentItem == null) return;
            if (grid.CurrentItem.GetType().GetProperty("Deleted") != null && grid.ItemsSource is ICollectionView)
            {
                grid.CurrentItem.GetType().GetProperty("Deleted").SetValue(grid.CurrentItem, true, null);
                grid.CancelEdit();
                ((ICollectionView)grid.ItemsSource).Refresh();
                grid.Focus();
                e.Handled = true;
            }

        }
    }
}
