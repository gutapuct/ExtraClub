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
using Microsoft.Practices.Unity;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.SettingsModule.Views.ContainedControls.Network;
using ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    public partial class ReportTemplates : ModuleViewBase
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public ReportTemplates()
        {
            InitializeComponent();
        }

        private void EditTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.TemplatesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTemplateWindow>(() => Model.RefreshTemplates(), new ParameterOverride("template", Model.TemplatesView.CurrentItem));
            }
        }


        private void TemplatesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("NetTemplatesMgmt"))
            {
                EditTemplateButton_Click(null, null);
            }
        }
    }
}
