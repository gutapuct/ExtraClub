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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TonusClub.EmployeesModule.ViewModels;
using TonusClub.EmployeesModule.Views.ContainedControls.Vacancies.Windows;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
//using Telerik.Windows.Controls;
using TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Vacancies
{
    /// <summary>
    /// Interaction logic for EmployeeCategories.xaml
    /// </summary>
    public partial class EmployeeCategories
    {
        private EmployeesLargeViewModel Model
        {
            get
            {
                return this.DataContext as EmployeesLargeViewModel;
            }
        }

        public EmployeeCategories()
        {
            InitializeComponent();
        }

        private void CategoriesViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (!ClientContext.CheckPermission("CategoriesManagementBtns")) return;
                EditButton_Click(null, null);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (Model.CategoriesView.CurrentItem == null) return;
            var cat = (EmployeeCategory)Model.CategoriesView.CurrentItem;
            var cnt = ClientContext.GetActiveEmployeesCountForCategoryId(cat.Id);
            if (cnt > 0)
            {
                ProcessUserDialog<CategoryWindow>(() =>
                {
                    cnt = ClientContext.GetActiveEmployeesCountForCategoryId(cat.Id);
                    if (cnt == 0)
                    {
                        ClientContext.HideEmployeeCategoryById(cat.Id);
                        Model.RefreshCategories();
                    }
                }, new ParameterOverride("catId", cat.Id));

            }
            else
            {
                TonusWindow.Confirm("Удаление",
                    "Удалить категорию \"" + ((EmployeeCategory)Model.CategoriesView.CurrentItem).Name + "\"?",
                    wnd =>
                    {
                        if ((wnd.DialogResult ?? false))
                        {
                            ClientContext.HideEmployeeCategoryById(cat.Id);
                            Model.RefreshCategories();
                        }
                    });
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CategoriesView.CurrentItem != null)
            {
                var cnt = ClientContext.GetActiveEmployeesCountForCategoryId(((EmployeeCategory)Model.CategoriesView.CurrentItem).Id);
                if (cnt > 0)
                {
                    TonusWindow.Confirm("Редактирование категории",
                        "В данной категории работает " + cnt + " сотрудников. Изменить характеристики категории с сегодняшнего числа?",
                        wnd =>
                        {
                            if (wnd.DialogResult ?? false)
                            {
                                ProcessUserDialog<NewEditCategoryWindow>(() => Model.RefreshCategories(),
                                    new ParameterOverride("category", ViewModelBase.Clone<EmployeeCategory>(Model.CategoriesView.CurrentItem)));
                            }
                        });
                }
                else
                {
                    ProcessUserDialog<NewEditCategoryWindow>(() => Model.RefreshCategories(),
                        new ParameterOverride("category", ViewModelBase.Clone<EmployeeCategory>(Model.CategoriesView.CurrentItem)));
                }
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCategoryWindow>(() => Model.RefreshCategories());
        }
    }
}
