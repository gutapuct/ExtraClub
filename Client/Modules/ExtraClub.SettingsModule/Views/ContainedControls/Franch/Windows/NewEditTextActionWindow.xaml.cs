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
    /// Interaction logic for NewEditTextActionWindow.xaml
    /// </summary>
    public partial class NewEditTextActionWindow : WindowBase
    {
        public TextAction TextAction { get; set; }

        public List<CompanySettingsFolder> SettingsFolders { get; set; }

        public List<Division> Divisions { get; set; }

        public NewEditTextActionWindow(ClientContext context, TextAction action)
            : base(context)
        {
            SettingsFolders = context.GetCompanySettingsFolders(0);
            Divisions = context.GetDivisions();
            Owner = Application.Current.MainWindow;
            InitializeComponent();
#if BEAUTINIKA
            gclubs.Header = "Студии";
            tclubs.Text = "Если в данном разделе не выбрано ни одной студии, информер будет отображаться во всех студиях.";
#endif
            if (action == null || action.Id == Guid.Empty)
            {
                TextAction = new TextAction
                {
                    StartDate=DateTime.Today,
                    FinishDate=DateTime.Today.AddDays(7)
                };
            }
            else
            {
                TextAction = action;
                foreach (var d in action.SerializedActiveDivisionIds)
                {
                    var div = Divisions.SingleOrDefault(i => i.Id == d);
                    if (div != null)
                    {
                        div.Helper = true;
                    }
                }
            }

            this.DataContext = this;

        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextAction.Error)) return;
            var res = _context.PostTextAction(TextAction, Divisions.Where(i=>i.Helper).Select(i=>i.Id).ToArray());

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
