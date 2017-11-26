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
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ParallelActions.xaml
    /// </summary>
    public partial class ParallelActions : ModuleViewBase
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public ParallelActions()
        {
            InitializeComponent();
        }

        private void NewRuleButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditParallelRuleWindow>(() => Model.RefreshParallelRules());
        }

        private void EditRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ParallelRulesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditParallelRuleWindow>(_ => Model.RefreshParallelRules(),
                    new ParameterOverride("rule", Model.ParallelRulesView.CurrentItem));
            }
        }

        private void DeleteRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ParallelRulesView.CurrentItem == null) return;
            ExtraWindow.Confirm(
                "Удаление",
                "Удалить выделенное правило?",
                wnd =>
                {
                    if ((wnd.DialogResult ?? false))
                    {
                        ClientContext.DeleteParallelRule((TreatmentsParalleling)Model.ParallelRulesView.CurrentItem);
                        Model.RefreshParallelRules();
                    }
                },
                "Да",
                "Нет"
            );
        }

        private void ParallelRulesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("PlanningMgmgt"))
            {
                EditRuleButton_Click(null, null);
            }
        }

    }
}
