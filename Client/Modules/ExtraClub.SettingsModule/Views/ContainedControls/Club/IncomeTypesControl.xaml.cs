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
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.UIControls.Windows;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Club
{
    public partial class IncomeTypesControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public IncomeTypesControl()
        {
            InitializeComponent();
        }

        private void IncomeTypesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("ClubIncomeTypesMgmt"))
            {
                EditTypeButton_Click(null, null);
            }
        }

        private void NewTypeButton_Click(object sender1, RoutedEventArgs e1)
        {
            ExtraWindow.Prompt(
                "Новая категория доходов",
                "Название категории:",
                "",
                e =>
                {
                    if ((e.DialogResult ?? false) && e.TextResult != null)
                    {
                        ClientContext.PostIncomeType(Guid.Empty, e.TextResult.Trim());
                        Model.RefreshDivisionIncomeTypes();
                    }
                });
        }

        private void EditTypeButton_Click(object sender1, RoutedEventArgs e1)
        {
            if (Model.IncomeTypesView.CurrentItem != null)
            {
                var st = Model.IncomeTypesView.CurrentItem as IncomeType;
                ExtraWindow.Prompt(
                     "Редактирование категории доходов",
                     "Название категории:",
                     "",
                    e =>
                    {
                        if (e.DialogResult ?? false)
                        {
                            ClientContext.PostIncomeType(st.Id, e.TextResult.Trim());
                            Model.RefreshDivisionIncomeTypes();
                        }
                    });
            }
        }


        private void RemoveTypeButton_Click(object sender1, RoutedEventArgs e1)
        {
            if (Model.IncomeTypesView.CurrentItem != null)
            {
                var st = Model.IncomeTypesView.CurrentItem as IncomeType;
                ExtraWindow.Confirm("Удаление категории",
                    "Удалить выделенную категорию?",
                    e =>
                    {
                        if (e.DialogResult ?? false)
                        {
                            ClientContext.PostIncomeTypeRemove(st.Id);
                            Model.RefreshDivisionIncomeTypes();
                        }
                    });
            }
        }


    }
}
