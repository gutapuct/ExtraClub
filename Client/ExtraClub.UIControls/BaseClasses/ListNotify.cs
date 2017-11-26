using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExtraClub.Infrastructure.BaseClasses
{
    public class ListNotify<T> : List<T>
        where T:INotifyPropertyChanged
    {
        public event EventHandler ItemPropertyChanged;

        private void OnItemPropertyChanged(object sender, EventArgs e)
        {
            if (ItemPropertyChanged != null)
            {
                ItemPropertyChanged(sender, e);
            }
        }

        public new void Insert(int index, T item)
        {
            item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            base.Insert(index, item);
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnItemPropertyChanged(sender, e);
        }

        public new void RemoveAt(int index)
        {
            base[index].PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            base.RemoveAt(index);
        }

        public new void Add(T item)
        {
            item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            base.Add(item);
        }

        public new void Clear()
        {
            base.ForEach(item => item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged));
            base.Clear();
        }

        public new bool Remove(T item)
        {
            item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            var res = base.Remove(item);
            return res;
        }
    }
}
