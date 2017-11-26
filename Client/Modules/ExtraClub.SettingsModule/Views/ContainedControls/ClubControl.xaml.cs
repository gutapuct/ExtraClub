using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ExtraClub.ServiceModel;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    public partial class ClubControl
    {
        private readonly SettingsLargeViewModel _model;

        public ClubControl(SettingsLargeViewModel model)
        {
            DataContext = _model = model;
            InitializeComponent();

            SettingsManager.RegisterGridView(this, TreatmentsGrid);
            GotFocus += OrgCardTypesControl_GotFocus;
        }

        void OrgCardTypesControl_GotFocus(object sender, RoutedEventArgs e)
        {
            _model.UpdateFinished -= Model_UpdateFinished;
            _model.UpdateFinished += Model_UpdateFinished;
        }

        void Model_UpdateFinished(object sender, EventArgs e)
        {
            if (TreatmentTree != null)
            {
                TreatmentTree.ExpandAll();
            }
        }


        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (_model.CurrentTreatmentTreeItem != null && _model.CurrentTreatmentTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = _model.CurrentTreatmentTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(() => _model.RefreshCompanyFolders(), new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 3) });
        }


        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTreatmentTreeItem != null && _model.CurrentTreatmentTreeItem.Id != Guid.Empty && _model.CurrentTreatmentTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(() => _model.RefreshCompanyFolders(), new ResolverOverride[] { new ParameterOverride("folder", _model.CurrentTreatmentTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 3) });
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTreatmentTreeItem != null && _model.CurrentTreatmentTreeItem.Id != Guid.Empty && _model.CurrentTreatmentTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(_model.CurrentTreatmentTreeItem.Id);
                        _model.RefreshCompanyFolders();
                    }
                });
            }
        }

        private void NewTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentWindow>(() => _model.RefreshClubTreatments());
        }

        private void TreatmentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("ClubTreatsMgmt"))
            {
                EditTreatmentButton_Click(null, null);
            }
        }


        private void EditTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentWindow>(() => _model.RefreshClubTreatments(),
                    new ResolverOverride[] { new ParameterOverride("treatment", _model.TreatmentsView.CurrentItem) });
            }
        }

        private void DeleteTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentsView.CurrentItem == null) return;
            var tre = (Treatment)_model.TreatmentsView.CurrentItem;
            tre.IsActive = false;
            ClientContext.PostTreatment(tre);
            _model.RefreshClubTreatments();
        }

        private void TreatmentTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var radTreeView = e.Source as Telerik.Windows.Controls.RadTreeView;
            if (radTreeView != null)
                _model.CurrentTreatmentTreeItem = radTreeView.SelectedItem as CompanySettingsFolder;
        }

        private void ActivateTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentsView.CurrentItem == null) return;
            var tre = (Treatment)_model.TreatmentsView.CurrentItem;
            tre.IsActive = true;
            ClientContext.PostTreatment(tre);
            _model.RefreshClubTreatments();
        }

        private void TreatmentTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentsView.CurrentItem == null) return;
            var tre = (Treatment)_model.TreatmentsView.CurrentItem;

            ExtraWindow.Prompt("Метка тренажера", "Как отображать тренажер в часовых сетках?", tre.Tag, w =>
            {
                if (w.DialogResult ?? false)
                {
                    if (!String.IsNullOrWhiteSpace(w.TextResult))
                    {
                        tre.Tag = w.TextResult;
                        ClientContext.PostTreatment(tre);
                        _model.RefreshClubTreatments();
                    }
                }
            });
        }

        private void UpPriority_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentsView.CurrentItem != null)
            {
                var tr = _model.TreatmentsView.CurrentItem as Treatment;
                if (tr != null)
                {
                    ClientContext.MoveTreatment(tr.Id, true);
                    _model.RefreshClubTreatments();
                    _model.TreatmentsView.MoveCurrentTo(_model.Treatments.FirstOrDefault(i => i.Id == tr.Id));
                }
            }
        }

        private void DownPriority_Click(object sender, RoutedEventArgs e)
        {
            if (_model.TreatmentsView.CurrentItem != null)
            {
                var tr = _model.TreatmentsView.CurrentItem as Treatment;
                if (tr != null)
                {
                    ClientContext.MoveTreatment(tr.Id, false);
                    _model.RefreshClubTreatments();
                    _model.TreatmentsView.MoveCurrentTo(_model.Treatments.FirstOrDefault(i => i.Id == tr.Id));
                }
            }
        }
    }
}
