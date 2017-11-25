using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Telerik.Windows.Controls;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    public partial class NewEditPackageWindow
    {
        public Package Package { get; set; }
        public ObservableCollection<PackageLine> PackageLines { get; set; }
        public ICollectionView PackageLinesView { get; private set; }
        public ObservableCollection<Good> Goods { get; set; }


        public NewEditPackageWindow(ClientContext context, Package package)
            : base(context)
        {
            InitializeComponent();
            Goods = new ObservableCollection<Good>(context.GetAllGoods());
            PackageLines = new ObservableCollection<PackageLine>();
            PackageLinesView = CollectionViewSource.GetDefaultView(PackageLines);

            PackageLinesGrid.RowValidating += new EventHandler<GridViewRowValidatingEventArgs>(PackageLinesGrid_RowValidating);

            if (package != null && package.Id != Guid.Empty)
            {
                Package = package;
                package.SerializedPackageLines.ForEach(i => PackageLines.Add(i));
            }
            else
            {
                Package = new Package
                {
                    Id = Guid.NewGuid(),
                    CompanyId = _context.CurrentCompany.CompanyId,
                    IsActive = true,
                    Name = "Новый пакет"
                };
            }

            DataContext = this;
        }

        void PackageLinesGrid_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var i = e.Row.Item as PackageLine;
            if (i.GoodId == Guid.Empty) e.ValidationResults.Add(
                new GridViewCellValidationResult { ErrorMessage = UIControls.Localization.Resources.GoodNeeded });
            if (i.Amount <= 0) e.ValidationResults.Add(
                new GridViewCellValidationResult { ErrorMessage = UIControls.Localization.Resources.IncorrectGoodAmount });
            e.IsValid = e.ValidationResults.Count == 0;
            if (!e.IsValid)
            {
                TonusWindow.Confirm(UIControls.Localization.Resources.InputError,
                    UIControls.Localization.Resources.Error + ":\n" + e.ValidationResults[0].ErrorMessage + "\n" + UIControls.Localization.Resources.CancelNewLine,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            e.Row.CancelEdit();
                            PackageLines.Remove(i);
                            PackageLinesView.Refresh();
                            e.IsValid = true;
                        }
                    });
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostPackage(Package, PackageLines);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
