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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for DictionariesControl.xaml
    /// </summary>
    public partial class DictionariesControl : ModuleViewBase
    {
        public DictionariesControl()
        {
            InitializeComponent();
        }

        public void Init(IDictionaryManager dictionaryManager, ClientContext clientContext)
        {
            DictControl.Init(dictionaryManager, clientContext);
            //DictControl.RegisterDictionary("CustomerStatuses");
            //DictControl.RegisterDictionary("CustomerTargetTypes");
            DictControl.RegisterDictionary("TreatmentTypeGroups");
        }
    }
}
