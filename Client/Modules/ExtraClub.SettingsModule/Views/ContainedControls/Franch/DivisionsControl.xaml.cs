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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class DivisionsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public DivisionsControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //if (ClientContext.CheckPermission("DisableCentral"))
            //{
            //    ExtraWindow.Alert("Ошибка", "Для создания нового клуба необходимо подключиться к центральному серверу!");
            //    return;
            //}
            ClientContext.CreateDivision(() => Model.RefreshDivisions());
        }
    }
}