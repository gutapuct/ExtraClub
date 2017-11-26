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
using TonusClub.UIControls;
using Microsoft.Practices.Unity;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Network;

namespace TonusClub.SettingsModule.Views.ContainedControls
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
