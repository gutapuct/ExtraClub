using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.ClientDal.Wizards.NewClub
{
    public partial class StorehousesControl
    {
        NewDivisionWizard Model
        {
            get
            {
                return (NewDivisionWizard)DataContext;
            }
        }

        public StorehousesControl()
        {
            InitializeComponent();
        }

        private void NewStorehouseButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditStorehouseWindow>(
                ()=>Model.RefreshStorehouses(),
                 new ParameterOverride("context", Model.Context),
                    new ParameterOverride("divId", Model.Division.Id));
        }

        private void EditStorehouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Storehouses.CurrentItem != null)
            {
                ProcessUserDialog<NewEditStorehouseWindow>(
                    () => Model.RefreshStorehouses(),
                    new ParameterOverride("context", Model.Context),
                    new ParameterOverride("divId", Model.Division.Id),
                    new ParameterOverride("storehouse", ViewModelBase.Clone<Storehouse>(Model.Storehouses.CurrentItem)));
            }
        }

        private void DeleteStorehouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Storehouses.CurrentItem == null) return;
            ExtraWindow.Confirm("Удаление", "Удалить выделенный склад?",
                wnd=>
                {
                    if ((wnd.DialogResult ?? false))
                    {
                        Model.Context.DeleteObject("Storehouses", ((Storehouse)Model.Storehouses.CurrentItem).Id);
                        Model.RefreshStorehouses();
                    }
                });
        }

        private void StorehousesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditStorehouseButton_Click(null, null);
            }
        }
    }
}
