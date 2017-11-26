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
using System.Windows.Shapes;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.ServiceModel;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.SalarySchemes.Windows
{
    /// <summary>
    /// Interaction logic for NewEditSchemeWindow.xaml
    /// </summary>
    public partial class NewEditSchemeWindow
    {
        public ICollectionView CoefficientsView { get; set; }
        public SalaryScheme SalaryScheme { get; set; }

        IUnityContainer _container;

        public NewEditSchemeWindow(ClientContext context, SalaryScheme scheme, IUnityContainer container)
            : base(context)
        {
            _container = container;
            if (scheme == null || scheme.Id == Guid.Empty)
            {
                SalaryScheme = new SalaryScheme
                {
                    CompanyId = context.CurrentCompany.CompanyId,
                    DivisionId = context.CurrentDivision.Id,
                    IsOvertimePaid = true,
                    Name = "Новая схема",
                    SerializedSalarySchemeCoefficients = new List<SalarySchemeCoefficient>(),
                    Late1Minutes = 10,
                    Late1Fine = 300,
                    Late2Minutes = 30,
                    Late2Fine = 500
                };
            }
            else
            {
                SalaryScheme = scheme;
            }

            CoefficientsView = CollectionViewSource.GetDefaultView(SalaryScheme.SerializedSalarySchemeCoefficients);
            InitializeComponent();
            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostSalaryScheme(SalaryScheme);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ModuleViewBase.IsRowClicked(e))
            {
                var coeff = ViewModelBase.Clone<SalarySchemeCoefficient>(CoefficientsView.CurrentItem);
                //foreach (var i in ((SalarySchemeCoefficient)CoefficientsView.CurrentItem).SalaryRateTables)
                //{
                //    coeff.SalaryRateTables.Add(ViewModelBase.Clone<SalaryRateTable>(i));
                //}
                var dlg = _container.Resolve<NewEditSubschemeWindow>(new ResolverOverride[] { 
                    new ParameterOverride("coeff", coeff) });
                if (dlg.ShowDialog() ?? false == true)
                {
                    SalaryScheme.SerializedSalarySchemeCoefficients.Remove((SalarySchemeCoefficient)CoefficientsView.CurrentItem);
                    SalaryScheme.SerializedSalarySchemeCoefficients.Add(dlg.CoeffResult);
                    CoefficientsView.Refresh();
                }
            }
        }

        private void AddSubScheme(object sender, RoutedEventArgs e)
        {
            var dlg = _container.Resolve<NewEditSubschemeWindow>();
            dlg.Closed = new Action(() =>
            {
                SalaryScheme.SerializedSalarySchemeCoefficients.Add(dlg.CoeffResult);
                CoefficientsView.Refresh();
            });
            dlg.ShowDialog();
        }

        private void RemoveSubScheme(object sender, RoutedEventArgs e)
        {
            if (CoefficientsView.CurrentItem == null) return;
            SalaryScheme.SerializedSalarySchemeCoefficients.Remove((SalarySchemeCoefficient)CoefficientsView.CurrentItem);
            CoefficientsView.Refresh();
        }
    }
}
