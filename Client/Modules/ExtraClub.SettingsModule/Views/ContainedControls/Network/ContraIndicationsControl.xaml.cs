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
using ExtraClub.UIControls;
using ExtraClub.SettingsModule.ViewModels;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ContraIndicationsControl.xaml
    /// </summary>
    public partial class ContraIndicationsControl : ModuleViewBase
    {
        public ContraIndicationsControl()
        {
            InitializeComponent();
        }

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        private void ContrasGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null && ClientContext.CheckPermission("NetContrasMgmt"))
                {
                    EditContraButton_Click(null, null);
                }
            }
        }

        private void NewContraButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditContraWindow>(() => Model.RefreshContras());
        }

        private void EditContraButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedContra != null)
            {
                ProcessUserDialog<NewEditContraWindow>(w => Model.RefreshContras(), new ResolverOverride[] { new ParameterOverride("contraInd", Model.SelectedContra) });
            }
        }

        private void DeleteContraButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedContra == null) return;
            ExtraWindow.Confirm("Удаление",
                 "Удалить противопоказание \"" + Model.SelectedContra.Name + "\"?",
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.PostContraIndicationTreatmentTypes(Model.SelectedContra.Id, new List<Guid>());
                        ClientContext.DeleteContraIndication(Model.SelectedContra.Id);
                        Model.RefreshContras();
                    }
                });
        }

    }
}
