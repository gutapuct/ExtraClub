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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.UIControls.Windows;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using Microsoft.Practices.Unity;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for SpendingTypesControl.xaml
    /// </summary>
    public partial class SpendingTypesControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public SpendingTypesControl()
        {
            InitializeComponent();
        }

        private void SpendingTypesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("ClubSpendingTypesMgmt"))
            {
                EditTypeButton_Click(null, null);
            }
        }

        private void NewTypeButton_Click(object sender1, RoutedEventArgs e1)
        {
            ExtraWindow.Prompt("Новая категория затрат",
                 "Название категории:",
                 "",
                e =>
                {
                    if (e.DialogResult ?? false)
                    {
                        ClientContext.PostSpendingType(Guid.Empty, e.TextResult.Trim());
                        Model.RefreshSpendingTypes();
                    }
                });
        }

        private void EditTypeButton_Click(object sender1, RoutedEventArgs e1)
        {
            if (Model.SpendingTypesView.CurrentItem != null)
            {
                var st = Model.SpendingTypesView.CurrentItem as SpendingType;
                ExtraWindow.Prompt("Редактирование категории затрат",
                    "Название категории:",
                    "",
                    e =>
                    {
                        if (e.DialogResult ?? false)
                        {
                            ClientContext.PostSpendingType(st.Id, e.TextResult.Trim());
                            Model.RefreshSpendingTypes();
                        }
                    });
            }
        }

        private void RemoveTypeButton_Click(object sender1, RoutedEventArgs e1)
        {
            if (Model.SpendingTypesView.CurrentItem != null)
            {
                var st = Model.SpendingTypesView.CurrentItem as SpendingType;
                ExtraWindow.Confirm("Удаление категории",
                    "Удалить выделенную категорию?",
                    e =>
                    {
                        if (e.DialogResult ?? false)
                        {
                            ClientContext.PostSpendingTypeRemove(st.Id);
                            Model.RefreshSpendingTypes();
                        }
                    });
            }
        }

    }
}
