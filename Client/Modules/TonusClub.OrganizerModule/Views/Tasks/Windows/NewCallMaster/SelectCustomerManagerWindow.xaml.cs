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
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;

namespace TonusClub.OrganizerModule.Views.Tasks.Windows.NewCallMaster
{
    /// <summary>
    /// Interaction logic for SelectCustomerManagerWindow.xaml
    /// </summary>
    public partial class SelectCustomerManagerWindow
    {
        public List<ManagerView> Managers { get; set; }
        public List<Guid> Result { get; set; }

        public SelectCustomerManagerWindow()
            : base(null)
        {
            Managers = new List<ManagerView>();
            InitializeComponent();
        }

        public void AppendCollection<T>(IEnumerable<T> collection, Func<T, Guid> idFunc, Func<T, string> valueFunc)
        {
            foreach (T i in collection)
            {
                Managers.Add(new ManagerView { Id = idFunc.Invoke(i), IsChecked = false, Name = valueFunc.Invoke(i) });
            }
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = Managers.Where(i => i.IsChecked).Select(i => i.Id).ToList();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ManagerView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
