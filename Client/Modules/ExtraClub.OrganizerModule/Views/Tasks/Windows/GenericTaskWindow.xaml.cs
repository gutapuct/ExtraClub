﻿using System;
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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.OrganizerModule.Views.Tasks.Windows
{
    /// <summary>
    /// Interaction logic for GenericTaskWindow.xaml
    /// </summary>
    public partial class GenericTaskWindow
    {
        public OrganizerItem Item { get; set; }
        public Task Task { get; set; }

        public DateTime NewDate { get; set; }

        public GenericTaskWindow(OrganizerItem item)
        {
            Item = item;
            Task = item.Data as Task;
            InitializeComponent();
            if (Task.Parameter.HasValue)
            {
                AuthorizationManager.SetElementVisible(SolCfg);
                NewDate = DateTime.Today.AddDays(30);
            }
            if (Task.Subject == "Инвентаризация") AuthorizationManager.SetElementVisible(InventoryCfg);
            DataContext = this;
        }

        private void CommitClick(object sender, RoutedEventArgs e)
        {
            _context.PostTaskClosing(Task.Id, true, Task.ClosedComment, NewDate);
            DialogResult = true;
            Close();
        }

        private void RejectClick(object sender, RoutedEventArgs e)
        {
            _context.PostTaskClosing(Task.Id, false, Task.ClosedComment, NewDate);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
