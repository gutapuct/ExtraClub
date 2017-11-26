using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;

namespace ExtraClub.OrganizerModule.Views.Tasks.Windows.NewCallMaster
{
    /// <summary>
    /// Interaction logic for SelectCustomerStatusesWindow.xaml
    /// </summary>
    public partial class SelectCustomerStatusesWindow
    {
        public List<StatusView> Statuses { get; set; }
        public List<Guid> Result { get; set; }

        public SelectCustomerStatusesWindow()
            : base(null)
        {
            Statuses = new List<StatusView>();
            InitializeComponent();
        }

        public void AppendCollection<T>(IEnumerable<T> collection, Func<T, Guid> idFunc, Func<T, string> valueFunc)
        {
            foreach (T i in collection)
            {
                Statuses.Add(new StatusView { Id = idFunc.Invoke(i), IsChecked = false, Name = valueFunc.Invoke(i) });
            }
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = Statuses.Where(i => i.IsChecked).Select(i => i.Id).ToList();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class StatusView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
