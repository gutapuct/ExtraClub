using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.ClientDal.Wizards.NewClub
{
    public partial class DivisionSolariumsControl
    {
        NewDivisionWizard Model
        {
            get
            {
                return (NewDivisionWizard)DataContext;
            }
        }

        public DivisionSolariumsControl()
        {
            InitializeComponent();
        }

        private void NewSolariumButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditSolariumWindow>(() => Model.RefreshSolariums(), new ParameterOverride("context", Model.Context), new ParameterOverride("divId", Model.Division.Id));
        }

        private void EditSolariumButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Solariums.CurrentItem != null)
            {
                ProcessUserDialog<NewEditSolariumWindow>(() => Model.RefreshSolariums(), new ParameterOverride("context", Model.Context), new ParameterOverride("divId", Model.Division.Id), new ParameterOverride("solarium", Model.Solariums.CurrentItem));
            }
        }

        private void DeleteSolariumButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Solariums.CurrentItem == null) return;

            ExtraWindow.Confirm(
                "Удаление",
                "Удалить выделенный солярий?",
                wnd =>
                {
                    if ((wnd.DialogResult ?? false))
                    {
                        Model.Context.DeleteObject("Solariums", ((Solarium)Model.Solariums.CurrentItem).Id);
                        Model.RefreshSolariums();
                    }
                },
                "Да", "Нет");
        }

        private void SolariumsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditSolariumButton_Click(null, null);
            }
        }
    }
}
