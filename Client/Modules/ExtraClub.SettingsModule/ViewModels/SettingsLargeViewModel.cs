using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.ServiceModel;
using System.Windows.Data;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using System.Windows;
using ExtraClub.UIControls.Windows;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.SettingsModule.ViewModels
{
    public class SettingsLargeViewModel : ViewModelBase
    {
        public static Guid DeletedFolderId = Guid.Parse("7BF68FAE-C4C1-418D-A134-375BD45E2AB9");

        public ICollectionView TreatmentsView { get; set; }
        public ICollectionView TreatmentSeqRestView { get; set; }
        public ICollectionView TreatmentIntRestView { get; set; }
        public ICollectionView TreatmentAmRestView { get; set; }

        public List<Treatment> Treatments = new List<Treatment>();
        private List<TreatmentSeqRest> TreatmentSeqRests = new List<TreatmentSeqRest>();
        private List<TreatmentSeqRest> TreatmentIntRests = new List<TreatmentSeqRest>();
        private List<TreatmentSeqRest> TreatmentAmRests = new List<TreatmentSeqRest>();

        private bool _OnlyActiveusers = true;
        public bool OnlyActiveUsers
        {
            get
            {
                return _OnlyActiveusers;
            }
            set
            {
                _OnlyActiveusers = value;
                UsersView.Refresh();
                OnPropertyChanged("OnlyActiveUsers");
            }
        }
#if !BEAUTINIKA

        private bool _IsClubTargetsOnly = true;
        public bool IsClubTargetsOnly
        {
            get
            {
                return _IsClubTargetsOnly;
            }
            set
            {
                _IsClubTargetsOnly = value;

                var clubTreatmentConfigs = OrgTreatmentConfigs.Where(i => !i.SettingsFolderId.HasValue || _TreatmentConfigsFoldersView.Any(j => j.AccessingCompanies.Any(k => k.CompanyId == ClientContext.CurrentCompany.CompanyId)))
                   .Where(i => i.IsActive)
                   .Where(i => Treatments.Where(j => j.IsActive).Any(j => j.TreatmentTypeId == i.TreatmentTypeId))
                   .Select(i => i.Id);

                //var clubTreatmentConfigs = Treatments.Where(i => i.IsActive).Select(i => FrTreatmentConfigs.Where(j => j.TreatmentTypeId == i.TreatmentTypeId).Select(j => j.Id).FirstOrDefault()).Distinct().ToArray();
                TargetDetails.Clear();
                TargetDetails.AddRange(Targets.Select(i => new Tuple<string, string, bool>(i.Name,
                    String.Join("\nили ", TargetConfigs.Where(j => j.TargetTypeId == i.Id).Where(j => !value || j.TreatmentConfigIds.Split(',').Select(k => Guid.Parse(k)).All(k => clubTreatmentConfigs.Contains(k)))
                        .Select(j => String.Join(", ", j.TreatmentConfigIds.Split(',').Select(k => Guid.Parse(k)).Select(k => OrgTreatmentConfigs.Where(l => l.Id == k)
                        .Select(l => l.Name).FirstOrDefault() ?? k.ToString()).ToArray())).ToArray()), true))
                    .OrderBy(j => j.Item1));
                TargetDetailsView.Refresh();
                
                
                OnPropertyChanged("IsClubTargetsOnly");
            }
        }
#endif
        public ICollectionView OrgCardTypesView { get; set; }
        public ICollectionView OrgTicketTypesView { get; set; }
        public ICollectionView ContrasView { get; set; }
        public ICollectionView OrgTreatmentTypesView { get; set; }
        public ICollectionView OrgTreatmentConfigsView { get; set; }
        public ICollectionView FrTreatmentConfigsView { get; set; }
        public ICollectionView TreatmentProgramsView { get; set; }
        public ICollectionView ActionsView { get; set; }
        public ICollectionView ParallelRulesView { get; set; }
        public ICollectionView TemplatesView { get; set; }
        public ICollectionView SolariumsView { get; set; }
        public ICollectionView SolariumWarningsView { get; set; }
        public ICollectionView FrCardTypesView { get; set; }
        public ICollectionView FrTicketTypesView { get; set; }
        public ICollectionView FrInstalmentsView { get; set; }
        public ICollectionView StorehousesView { get; set; }
        public ICollectionView SpendingTypesView { get; set; }
        public ICollectionView IncomeTypesView { get; set; }
        public ICollectionView CorporatesView { get; set; }
        public ICollectionView FilesView { get; set; }
        public ICollectionView CallScrenarioView { get; set; }
        public ICollectionView RolesView { get; set; }
        public ICollectionView UsersView { get; set; }
        public ICollectionView OrgInstalmentsView { get; set; }
        public ICollectionView CardTypeFoldersView { get; set; }
        public ICollectionView TicketTypeFoldersView { get; set; }
        public ICollectionView FrInformFoldersView { get; set; }
        public ICollectionView FrInstalmentFoldersView { get; set; }
        public ICollectionView ProgramsFoldersView { get; set; }
        public ICollectionView CorpFoldersView { get; set; }
        public ICollectionView TreatmentFoldersView { get; set; }
        public ICollectionView SolFoldersView { get; set; }
        public ICollectionView StoreFoldersView { get; set; }
        public ICollectionView RoleFoldersView { get; set; }
        public ICollectionView FrCardTypeFoldersView { get; set; }
        public ICollectionView FrTicketTypeFoldersView { get; set; }
        public ICollectionView InstalmentFoldersView { get; set; }
        public ICollectionView TreatmentConfigsFoldersView { get; set; }
        public ICollectionView OrgCompanies { get; set; }
        public ICollectionView Divisions { get; set; }
        public ICollectionView AdvertGroupsView { get; set; }
        public ICollectionView AdvertTypesView { get; set; }
        public ICollectionView NewsView { get; set; }
        public ICollectionView FrCustomerStatusesView { get; set; }
        public ICollectionView CumulativesView { get; set; }
        public ICollectionView BarDiscountsView { get; set; }
        public ICollectionView PackagesView { get; set; }
        public ICollectionView TreatmentsAvailableView { get; set; }
        public ICollectionView TargetsView { get; set; }
        public ICollectionView TargetDetailsView { get; set; }

#if BEAUTINIKA
        public ICollectionView FrRoomsView { get; set; }
#endif
        public List<CustomerTargetType> Targets = new List<CustomerTargetType>();
#if !BEAUTINIKA
        public List<TargetTypeSet> TargetConfigs = new List<TargetTypeSet>();
        public ICollectionView TargetConfigsView { get; set; }
#endif
        private List<Tuple<bool, string>> _TreatmentsAvailable = new List<Tuple<bool, string>>();
        public List<Tuple<string, string, bool>> TargetDetails = new List<Tuple<string, string, bool>>();
        private List<Package> _Packages = new List<Package>();
        private List<Company> _OrgCompanies = new List<Company>();
        private List<CustomerCardType> OrgCardTypes = new List<CustomerCardType>();
        private List<TicketType> OrgTicketTypes = new List<TicketType>();
        private List<ContraIndication> Contras = new List<ContraIndication>();
        private List<TreatmentType> OrgTreatmentTypes = new List<TreatmentType>();
        private List<TreatmentConfig> OrgTreatmentConfigs = new List<TreatmentConfig>();
        private List<TreatmentConfig> FrTreatmentConfigs = new List<TreatmentConfig>();

        private List<TreatmentProgram> TreatmentPrograms = new List<TreatmentProgram>();
        private List<TextAction> Actions = new List<TextAction>();
        private List<TreatmentsParalleling> ParallelRules = new List<TreatmentsParalleling>();
        private List<ReportTemplate> Templates = new List<ReportTemplate>();
        private List<Solarium> Solariums = new List<Solarium>();
        private List<Pair> SolariumWarnings = new List<Pair>();
        private List<CustomerCardType> FrCardTypes = new List<CustomerCardType>();
        private List<TicketType> FrTicketTypes = new List<TicketType>();
        private List<Instalment> _FrInstalmentsView = new List<Instalment>();
        private List<Storehouse> Storehouses = new List<Storehouse>();
        private List<SpendingType> SpendingTypes = new List<SpendingType>();
        private List<IncomeType> IncomeTypes = new List<IncomeType>();
        public List<Corporate> Corporates = new List<Corporate>();
        private List<File> Files = new List<File>();
        private List<IncomingCallForm> CallScrenario = new List<IncomingCallForm>();
        private List<Role> _RolesView = new List<Role>();
        internal List<User> _UsersView = new List<User>();
        internal List<Instalment> _OrgInstalmentsView = new List<Instalment>();
        private List<SettingsFolder> _CardTypeFoldersView = new List<SettingsFolder>();
        private List<SettingsFolder> _TicketTypeFoldersView = new List<SettingsFolder>();
        private List<CompanySettingsFolder> _FrInformFoldersView = new List<CompanySettingsFolder>();
        private List<CompanySettingsFolder> _ProgramsFoldersView = new List<CompanySettingsFolder>();
        private List<CompanySettingsFolder> _CorpFoldersView = new List<CompanySettingsFolder>();
        private List<CompanySettingsFolder> _SolFoldersView = new List<CompanySettingsFolder>();
        private List<CompanySettingsFolder> _StoreFoldersView = new List<CompanySettingsFolder>();
        private List<CompanySettingsFolder> _RoleFoldersView = new List<CompanySettingsFolder>();
        private List<SettingsFolder> _InstalmentFoldersView = new List<SettingsFolder>();
        private List<SettingsFolder> _TreatmentConfigsFoldersView = new List<SettingsFolder>();
        private List<News> News = new List<News>();
        private List<CumulativeDiscount> _CumulativesView = new List<CumulativeDiscount>();
        private List<BarDiscount> _BarDiscountsView = new List<BarDiscount>();

        private List<CompanySettingsFolder> _TreatmentFoldersView = new List<CompanySettingsFolder>();
        private List<SettingsFolder> _FrCardTypeFoldersView = new List<SettingsFolder>();
        private List<SettingsFolder> _FrTicketTypeFoldersView = new List<SettingsFolder>();
        private List<SettingsFolder> _FrInstalmentFoldersView = new List<SettingsFolder>();
        private List<Division> _Divisions = new List<Division>();
        private List<AdvertGroup> _AdvertGroups = new List<AdvertGroup>();
        private List<AdvertType> _AdvertTypes = new List<AdvertType>();
        private List<CustomerStatus> FrCustomerStatuses = new List<CustomerStatus>();
#if BEAUTINIKA
        private List<Room> FrRooms = new List<Room>();
#endif

        private AdvertGroup _CurrentAdvertGroup;
        public AdvertGroup CurrentAdvertGroup
        {
            get
            {
                return _CurrentAdvertGroup;
            }
            set
            {
                _CurrentAdvertGroup = value;
                AdvertTypesView.Refresh();
                OnPropertyChanged("CurrentAdvertGroup");
            }
        }

        private Company _company;
        public Company Company
        {
            get
            {
                return _company;
            }
            set
            {
                _company = value;
                OnPropertyChanged("Company");
            }
        }

        private bool _ShowInactiveTreatments = true;
        public bool ShowInactiveTreatments
        {
            get
            {
                return _ShowInactiveTreatments;
            }
            set
            {
                _ShowInactiveTreatments = value;
                OnPropertyChanged("ShowInactiveTreatments");
                OrgTreatmentConfigsView.Refresh();
                OrgTreatmentTypesView.Refresh();
            }
        }

        private Division _Division;
        public Division Division
        {
            get
            {
                return _Division;
            }
            set
            {
                _Division = value;
                OnPropertyChanged("Division");
            }
        }


        private bool _SolariumWarningsModified;
        public bool SolariumWarningsModified
        {
            get
            {
                return _SolariumWarningsModified;
            }
            set
            {
                _SolariumWarningsModified = value;
                OnPropertyChanged("SolariumWarningsModified");
            }
        }


        private SettingsFolder _CurrentCardTypeTreeItem;
        public SettingsFolder CurrentCardTypeTreeItem
        {
            get
            {
                return _CurrentCardTypeTreeItem;
            }
            set
            {
                _CurrentCardTypeTreeItem = value;
                OrgCardTypesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((CustomerCardType)item).SettingsFolderId;
                    if(!((CustomerCardType)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private SettingsFolder _CurrentTicketTypeTreeItem;
        public SettingsFolder CurrentTicketTypeTreeItem
        {
            get
            {
                return _CurrentTicketTypeTreeItem;
            }
            set
            {
                _CurrentTicketTypeTreeItem = value;
                OrgTicketTypesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((TicketType)item).SettingsFolderId;
                    if(!((TicketType)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }


        private SettingsFolder _CurrentTreatmentConfigTreeItem;
        public SettingsFolder CurrentTreatmentConfigTreeItem
        {
            get
            {
                return _CurrentTreatmentConfigTreeItem;
            }
            set
            {
                _CurrentTreatmentConfigTreeItem = value;
                OrgTreatmentConfigsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((TreatmentConfig)item).SettingsFolderId;
                    if(!((TreatmentConfig)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }


        private SettingsFolder _FrCurrentCardTypeTreeItem;
        public SettingsFolder FrCurrentCardTypeTreeItem
        {
            get
            {
                return _FrCurrentCardTypeTreeItem;
            }
            set
            {
                _FrCurrentCardTypeTreeItem = value;
                FrCardTypesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((CustomerCardType)item).SettingsFolderId;
                    if(!((CustomerCardType)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private SettingsFolder _FrCurrentTicketTypeTreeItem;
        public SettingsFolder FrCurrentTicketTypeTreeItem
        {
            get
            {
                return _FrCurrentTicketTypeTreeItem;
            }
            set
            {
                _FrCurrentTicketTypeTreeItem = value;
                FrTicketTypesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((TicketType)item).SettingsFolderId;
                    if(!((TicketType)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private SettingsFolder _FrCurrentInstalmentTreeItem;
        public SettingsFolder FrCurrentInstalmentTreeItem
        {
            get
            {
                return _FrCurrentInstalmentTreeItem;
            }
            set
            {
                _FrCurrentInstalmentTreeItem = value;
                FrInstalmentsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Instalment)item).SettingsFolderId;
                    if(!((Instalment)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentFrInformTreeItem;
        public CompanySettingsFolder CurrentFrInformTreeItem
        {
            get
            {
                return _CurrentFrInformTreeItem;
            }
            set
            {
                _CurrentFrInformTreeItem = value;
                ActionsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((TextAction)item).SettingsFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentProgramTreeItem;
        public CompanySettingsFolder CurrentProgramTreeItem
        {
            get
            {
                return _CurrentProgramTreeItem;
            }
            set
            {
                _CurrentProgramTreeItem = value;
                TreatmentProgramsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((TreatmentProgram)item).SettingsFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentCorpTreeItem;
        public CompanySettingsFolder CurrentCorpTreeItem
        {
            get
            {
                return _CurrentCorpTreeItem;
            }
            set
            {
                _CurrentCorpTreeItem = value;
                CorporatesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Corporate)item).SettingsFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentTreatmentTreeItem;
        public CompanySettingsFolder CurrentTreatmentTreeItem
        {
            get
            {
                return _CurrentTreatmentTreeItem;
            }
            set
            {
                _CurrentTreatmentTreeItem = value;
                TreatmentsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Treatment)item).SettingsFolderId;
                    if(!((Treatment)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentSolTreeItem;
        public CompanySettingsFolder CurrentSolTreeItem
        {
            get
            {
                return _CurrentSolTreeItem;
            }
            set
            {
                _CurrentSolTreeItem = value;
                SolariumsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Solarium)item).SettingsFolderId;
                    if(!((Solarium)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentStoreTreeItem;
        public CompanySettingsFolder CurrentStoreTreeItem
        {
            get
            {
                return _CurrentStoreTreeItem;
            }
            set
            {
                _CurrentStoreTreeItem = value;
                StorehousesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Storehouse)item).SettingsFolderId;
                    if(!((Storehouse)item).IsActive) id = DeletedFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private CompanySettingsFolder _CurrentRoleTreeItem;
        public CompanySettingsFolder CurrentRoleTreeItem
        {
            get
            {
                return _CurrentRoleTreeItem;
            }
            set
            {
                _CurrentRoleTreeItem = value;
                RolesView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Role)item).SettingsFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private SettingsFolder _CurrentOrgInstalmentTreeItem;
        public SettingsFolder CurrentOrgInstalmentTreeItem
        {
            get
            {
                return _CurrentOrgInstalmentTreeItem;
            }
            set
            {
                _CurrentOrgInstalmentTreeItem = value;
                OrgInstalmentsView.Filter = delegate(object item)
                {
                    if(item == null) return false;
                    var id = ((Instalment)item).SettingsFolderId;
                    if(value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        public bool IsMaximumRole
        {
            get
            {
                RefreshRoles();
                return _RolesView.All(x => x.RoleName != "Максимальная");;
            }
        }

        public List<TreatmentContraView> ContrasTreatmentList { get; set; }

        public SettingsLargeViewModel(IUnityContainer container)
            : base()
        {
            TreatmentsView = CollectionViewSource.GetDefaultView(Treatments);
            TreatmentSeqRestView = CollectionViewSource.GetDefaultView(TreatmentSeqRests);
            TreatmentIntRestView = CollectionViewSource.GetDefaultView(TreatmentIntRests);
            TreatmentAmRestView = CollectionViewSource.GetDefaultView(TreatmentAmRests);

            OrgCardTypesView = CollectionViewSource.GetDefaultView(OrgCardTypes);
            OrgCardTypesView.Filter = delegate(object item)
            {
                if(item == null) return false;
                var id = ((CustomerCardType)item).SettingsFolderId;
                if(!((CustomerCardType)item).IsActive) id = DeletedFolderId;
                return !id.HasValue;
            };

            OrgCompanies = CollectionViewSource.GetDefaultView(_OrgCompanies);

            OrgTicketTypesView = CollectionViewSource.GetDefaultView(OrgTicketTypes);
            ContrasView = CollectionViewSource.GetDefaultView(Contras);
            OrgTreatmentTypesView = CollectionViewSource.GetDefaultView(OrgTreatmentTypes);
            OrgTreatmentTypesView.Filter = delegate(object item)
            {
                if(item == null) return false;
                return ((TreatmentType)item).IsActive || ShowInactiveTreatments;
            };
            OrgTreatmentConfigsView = CollectionViewSource.GetDefaultView(OrgTreatmentConfigs);
            OrgTreatmentConfigsView.Filter = delegate(object item)
            {
                if(item == null) return false;
                return ((TreatmentConfig)item).IsActive || ShowInactiveTreatments;
            };



            FrTreatmentConfigsView = CollectionViewSource.GetDefaultView(FrTreatmentConfigs);

            TreatmentProgramsView = CollectionViewSource.GetDefaultView(TreatmentPrograms);
            ActionsView = CollectionViewSource.GetDefaultView(Actions);

            ParallelRulesView = CollectionViewSource.GetDefaultView(ParallelRules);
            TemplatesView = CollectionViewSource.GetDefaultView(Templates);

            SolariumsView = CollectionViewSource.GetDefaultView(Solariums);
            SolariumWarningsView = CollectionViewSource.GetDefaultView(SolariumWarnings);

            FrCardTypesView = CollectionViewSource.GetDefaultView(FrCardTypes);
            FrTicketTypesView = CollectionViewSource.GetDefaultView(FrTicketTypes);

            StorehousesView = CollectionViewSource.GetDefaultView(Storehouses);

            SpendingTypesView = CollectionViewSource.GetDefaultView(SpendingTypes);
            IncomeTypesView = CollectionViewSource.GetDefaultView(IncomeTypes);

            CorporatesView = CollectionViewSource.GetDefaultView(Corporates);

            FilesView = CollectionViewSource.GetDefaultView(Files);
            CallScrenarioView = CollectionViewSource.GetDefaultView(CallScrenario);

            RolesView = CollectionViewSource.GetDefaultView(_RolesView);
            UsersView = CollectionViewSource.GetDefaultView(_UsersView);

            FrInstalmentsView = CollectionViewSource.GetDefaultView(_FrInstalmentsView);
#if BEAUTINIKA
            FrRoomsView = CollectionViewSource.GetDefaultView(FrRooms);
#endif
            NewsView = CollectionViewSource.GetDefaultView(News);

            UsersView.Filter = delegate(object item)
            {
                if(!OnlyActiveUsers) return true;
                if(item == null) return false;
                return ((User)item).IsActive;
            };

            OrgInstalmentsView = CollectionViewSource.GetDefaultView(_OrgInstalmentsView);

            CardTypeFoldersView = CollectionViewSource.GetDefaultView(_CardTypeFoldersView);
            TicketTypeFoldersView = CollectionViewSource.GetDefaultView(_TicketTypeFoldersView);

            FrInformFoldersView = CollectionViewSource.GetDefaultView(_FrInformFoldersView);
            ProgramsFoldersView = CollectionViewSource.GetDefaultView(_ProgramsFoldersView);
            RoleFoldersView = CollectionViewSource.GetDefaultView(_RoleFoldersView);

            CorpFoldersView = CollectionViewSource.GetDefaultView(_CorpFoldersView);

            TreatmentFoldersView = CollectionViewSource.GetDefaultView(_TreatmentFoldersView);
            SolFoldersView = CollectionViewSource.GetDefaultView(_SolFoldersView);
            StoreFoldersView = CollectionViewSource.GetDefaultView(_StoreFoldersView);
            InstalmentFoldersView = CollectionViewSource.GetDefaultView(_InstalmentFoldersView);
            TreatmentConfigsFoldersView = CollectionViewSource.GetDefaultView(_TreatmentConfigsFoldersView);

            FrCardTypeFoldersView = CollectionViewSource.GetDefaultView(_FrCardTypeFoldersView);
            FrTicketTypeFoldersView = CollectionViewSource.GetDefaultView(_FrTicketTypeFoldersView);
            FrInstalmentFoldersView = CollectionViewSource.GetDefaultView(_FrInstalmentFoldersView);

            Divisions = CollectionViewSource.GetDefaultView(_Divisions);

            AdvertGroupsView = CollectionViewSource.GetDefaultView(_AdvertGroups);
            AdvertTypesView = CollectionViewSource.GetDefaultView(_AdvertTypes);

            FrCustomerStatuses = new List<CustomerStatus>();
            FrCustomerStatusesView = CollectionViewSource.GetDefaultView(FrCustomerStatuses);

            CumulativesView = CollectionViewSource.GetDefaultView(_CumulativesView);
            BarDiscountsView = CollectionViewSource.GetDefaultView(_BarDiscountsView);
            PackagesView = CollectionViewSource.GetDefaultView(_Packages);
            TreatmentsAvailableView = CollectionViewSource.GetDefaultView(_TreatmentsAvailable);
            TargetDetailsView = CollectionViewSource.GetDefaultView(TargetDetails);
            TargetsView = CollectionViewSource.GetDefaultView(Targets);
#if !BEAUTINIKA
            TargetConfigsView = CollectionViewSource.GetDefaultView(TargetConfigs);
#endif

            AdvertTypesView.Filter = delegate(object item)
            {
                if(CurrentAdvertGroup == null) return false;
                var type = item as AdvertType;
                return _CurrentAdvertGroup.Id == type.AdvertGroupId;
            };

            OnPropertyChanged("UnityContainer");
        }

        protected override void RefreshDataInternal()
        {

            Company = Clone<Company>(ClientContext.CurrentCompany);
            Division = Clone<Division>(ClientContext.CurrentDivision);

            Treatments.Clear();
            Treatments.AddRange(ClientContext.GetAllTreatments());


            var seqs = ClientContext.GetAllTreatmentSeqRests();
            TreatmentSeqRests.Clear();
            TreatmentSeqRests.AddRange(seqs.Where(ts => !ts.Interval.HasValue && !ts.Amount.HasValue));
            TreatmentIntRests.Clear();
            TreatmentIntRests.AddRange(seqs.Where(ts => ts.Interval.HasValue));
            TreatmentAmRests.Clear();
            TreatmentAmRests.AddRange(seqs.Where(ts => ts.Amount.HasValue));
            OrgCardTypes.Clear();
            OrgCardTypes.AddRange(ClientContext.GetAllCustomerCardTypes());
            OrgTicketTypes.Clear();
            OrgTicketTypes.AddRange(ClientContext.GetAllTicketTypes());
            Contras.Clear();
            Contras.AddRange(ClientContext.GetAllContras());

            ContrasTreatmentList = Treatments.OrderBy(i => i.NameWithTag).Where(i => i.IsActive).Select(i => new TreatmentContraView
            {
                Name = i.NameWithTag,
                Contras = Contras.Where(c => c.SerializedTreatmentTypes.Any(j => j.Id == i.TreatmentTypeId)).Select(j => j.Name).OrderBy(j => j).ToList()
            }).ToList();

            OrgTreatmentConfigs.Clear();
            OrgTreatmentConfigs.AddRange(ClientContext.GetAllTreatmentConfigsAdmin());

            FrTreatmentConfigs.Clear();
            FrTreatmentConfigs.AddRange(ClientContext.GetAllTreatmentConfigs());

            TreatmentPrograms.Clear();
            TreatmentPrograms.AddRange(ClientContext.GetTreatmentPrograms());

            OrgTreatmentTypes.Clear();
            OrgTreatmentTypes.AddRange(ClientContext.GetAllTreatmentTypes());

            Actions.Clear();
            Actions.AddRange(ClientContext.GetAllActions());

            Templates.Clear();
            Templates.AddRange(ClientContext.GetAllTemplates());

            FrCardTypes.Clear();
            FrCardTypes.AddRange(ClientContext.GetCustomerCardTypes(false));

            FrTicketTypes.Clear();
            FrTicketTypes.AddRange(ClientContext.GetTicketTypes(false));

            _FrInstalmentsView.Clear();
            _FrInstalmentsView.AddRange(ClientContext.GetCompanyInstalments(false));

            SpendingTypes.Clear();
            SpendingTypes.AddRange(ClientContext.GetDivisionSpendingTypes());

            ParallelRules.Clear();
            UpdateParallelRules();

            Solariums.Clear();
            Solariums.AddRange(ClientContext.GetDivisionSolariums(false));

            SolariumWarnings.Clear();
            ClientContext.GetSolariumWarnings().ToList().ForEach(i => SolariumWarnings.Add(new Pair { Key = i.Key, Value = i.Value }));

            Storehouses.Clear();
            Storehouses.AddRange(ClientContext.GetStorehouses());

            Corporates.Clear();
            Corporates.AddRange(ClientContext.GetCorporates());

            Files.Clear();
            Files.AddRange(ClientContext.GetDivisionFiles());

            CallScrenario.Clear();
            CallScrenario.AddRange(ClientContext.GetCallScrenarioForms());

            _RolesView.Clear();
            _RolesView.AddRange(ClientContext.GetRoles());

            _UsersView.Clear();
            _UsersView.AddRange(ClientContext.GetUsers());

            _OrgInstalmentsView.Clear();
            _OrgInstalmentsView.AddRange(ClientContext.GetAllInstalments());

            RefreshAsync<Company>(_OrgCompanies, OrgCompanies, () => ClientContext.GetCompanies(), false);

            RefreshAsync<IncomeType>(IncomeTypes, IncomeTypesView, () => ClientContext.GetDivisionIncomeTypes(), false);

            _CardTypeFoldersView.Clear();
            _CardTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(0, false), "Типы карт"));

            _TicketTypeFoldersView.Clear();
            _TicketTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(1, false), "Типы абонементов"));

            _FrCardTypeFoldersView.Clear();
            _FrCardTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(0, true), "Типы карт"));

            _FrTicketTypeFoldersView.Clear();
            _FrTicketTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(1, true), "Типы абонементов"));

            _FrInstalmentFoldersView.Clear();
            _FrInstalmentFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(2, true), "Рассрочки", false));


            _InstalmentFoldersView.Clear();
            _InstalmentFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(2, false), "Рассрочки", false));

            _TreatmentConfigsFoldersView.Clear();
            _TreatmentConfigsFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(3, false), "Услуги", false));


            _FrInformFoldersView.Clear();
            _FrInformFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(0), "Информеры", false));

            _RoleFoldersView.Clear();
            _RoleFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(6), "Роли", false));

            _ProgramsFoldersView.Clear();
            _ProgramsFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(1), "Программы занятий", false));

            _CorpFoldersView.Clear();
            _CorpFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(2), "Корпоративные договора", false));

            _TreatmentFoldersView.Clear();
            _TreatmentFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(3), "Оборудование", true));

            _SolFoldersView.Clear();
            _SolFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(4), "Солярии", true));

            _StoreFoldersView.Clear();
            _StoreFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(5), "Склады", true));

            _Divisions.Clear();
            _Divisions.AddRange(ClientContext.GetDivisions());

            ClientContext.UpdateCompany();
            Company = Clone<Company>(ClientContext.CurrentCompany);
            Division = Clone<Division>(ClientContext.CurrentDivision);

            _AdvertGroups.Clear();
            _AdvertGroups.AddRange(ClientContext.GetAdvertGroups());

            _AdvertTypes.Clear();
            _AdvertTypes.AddRange(ClientContext.GetAdvertTypes());

            News.Clear();
            News.AddRange(ClientContext.GetNews());

            _CumulativesView.Clear();
            _CumulativesView.AddRange(ClientContext.GetCumulativeDiscounts());

            _BarDiscountsView.Clear();
            _BarDiscountsView.AddRange(ClientContext.GetBarDiscounts());

            _Packages.Clear();
            _Packages.AddRange(ClientContext.GetPackages());

            _TreatmentsAvailable.Clear();

            _TreatmentsAvailable.AddRange(OrgTreatmentConfigs.Where(i => !i.SettingsFolderId.HasValue || _TreatmentConfigsFoldersView.Any(j => j.AccessingCompanies.Any(k => k.CompanyId == ClientContext.CurrentCompany.CompanyId)))
                .Where(i => i.IsActive)
                .Select(i => new Tuple<bool, string>(Treatments.Where(j => j.IsActive).Any(j => j.TreatmentTypeId == i.TreatmentTypeId), i.Name
#if !BEAUTINIKA
 + ", ограничения по возрасту для смарт-тренировок: " + i.DisableAges
#endif
)));

            Targets.Clear();
#if !BEAUTINIKA

            Targets.AddRange(ClientContext.GetTargetTypes());

            TargetConfigs.Clear();
            TargetConfigs.AddRange(ClientContext.GetTargetConfigs());

            var clubTreatmentConfigs = OrgTreatmentConfigs.Where(i => !i.SettingsFolderId.HasValue || _TreatmentConfigsFoldersView.Any(j => j.AccessingCompanies.Any(k => k.CompanyId == ClientContext.CurrentCompany.CompanyId)))
               .Where(i => i.IsActive)
               .Where(i => Treatments.Where(j => j.IsActive).Any(j => j.TreatmentTypeId == i.TreatmentTypeId))
               .Select(i => i.Id);

            //var clubTreatmentConfigs = Treatments.Where(i => i.IsActive).Select(i => FrTreatmentConfigs.Where(j => j.TreatmentTypeId == i.TreatmentTypeId).Select(j => j.Id).FirstOrDefault()).Distinct().ToArray();
            TargetDetails.Clear();
            TargetDetails.AddRange(Targets.Select(i => new Tuple<string, string, bool>(i.Name,
                String.Join("\nили ", TargetConfigs.Where(j => j.TargetTypeId == i.Id).Where(j => !_IsClubTargetsOnly || j.TreatmentConfigIds.Split(',').Select(k => Guid.Parse(k)).All(k => clubTreatmentConfigs.Contains(k)))
                    .Select(j => String.Join(", ", j.TreatmentConfigIds.Split(',').Select(k => Guid.Parse(k)).Select(k => OrgTreatmentConfigs.Where(l => l.Id == k)
                    .Select(l => l.Name).FirstOrDefault() ?? k.ToString()).ToArray())).ToArray()), true))
                .OrderBy(j => j.Item1));
#endif

            FrCustomerStatuses.Clear();
            FrCustomerStatuses.AddRange(ClientContext.GetAllStatuses().Select(i => new CustomerStatus { Id = i.Key, Name = i.Value, CompanyId = ClientContext.CurrentCompany.CompanyId, IsAvail = true }));

#if BEAUTINIKA
            FrRooms.Clear();
            FrRooms.AddRange(ClientContext.GetRooms());
#endif
        }

        public void RefreshCompanies()
        {
            RefreshAsync<Company>(_OrgCompanies, OrgCompanies, () => ClientContext.GetCompanies(), true);
        }

        private IEnumerable<T> ConstructSettingsFolders<T>(List<T> list, string rootName, bool generateBin = true)
            where T : IFolder, new()
        {
            var res = new List<T>();
            res.Add(new T { Id = Guid.Empty, Name = rootName });
            list.Where(i => !i.ParentFolderId.HasValue).ToList().ForEach(i =>
            {
                ((ICollection<T>)res[0].SettingsFolders1).Add(i);
                list.Remove(i);
            });
            int cnt = 0;
            while(list.Count != cnt)
            {
                cnt = list.Count;
                foreach(var i in list.ToList())
                {
                    var host = SearchList(res, i.ParentFolderId.Value);
                    if(host != null)
                    {
                        ((ICollection<T>)host.SettingsFolders1).Add(i);
                        list.Remove(i);
                    }
                }
            }
            if(generateBin)
            {
                res.Add(new T { Id = DeletedFolderId, Name = "Удаленные" });
            }
            return res;
        }

        public T SearchList<T>(List<T> src, Guid targetId)
            where T : IFolder, new()
        {
            foreach(var i in src)
            {
                if(i.Id == targetId)
                {
                    return i;
                }
                var res = SearchList(((ICollection<T>)i.SettingsFolders1).ToList(), targetId);
                if(res != null) return res;
            }
            return default(T);
        }

        private void UpdateParallelRules()
        {
            ParallelRules.AddRange(ClientContext.GetParallelingRules());
        }

        public void RefreshDivisionIncomeTypes()
        {
            RefreshAsync<IncomeType>(IncomeTypes, IncomeTypesView, () => ClientContext.GetDivisionIncomeTypes());
        }
#if BEAUTINIKA
        public void RefreshRooms()
        {
            RefreshAsync<Room>(FrRooms, FrRoomsView, () => ClientContext.GetRooms());
        }
#endif

        protected override void RefreshFinished()
        {
            base.RefreshFinished();

            OnPropertyChanged("ContrasTreatmentList");
            TreatmentsView.Refresh();
            TreatmentSeqRestView.Refresh();
            TreatmentIntRestView.Refresh();
            TreatmentAmRestView.Refresh();

            OrgCompanies.Refresh();

            OrgCardTypesView.Refresh();
            OrgTicketTypesView.Refresh();
            ContrasView.Refresh();
            OrgTreatmentConfigsView.Refresh();
            FrTreatmentConfigsView.Refresh();
            TreatmentProgramsView.Refresh();
            OrgTreatmentTypesView.Refresh();
            ActionsView.Refresh();
            ParallelRulesView.Refresh();
            TemplatesView.Refresh();
            SolariumsView.Refresh();
            SolariumWarningsView.Refresh();
            TreatmentConfigsFoldersView.Refresh();

            FrCardTypesView.Refresh();
            FrTicketTypesView.Refresh();
            FrInstalmentsView.Refresh();

            SpendingTypesView.Refresh();

            StorehousesView.Refresh();

            CorporatesView.Refresh();
            FilesView.Refresh();
            CallScrenarioView.Refresh();

            RolesView.Refresh();
            UsersView.Refresh();

            OrgInstalmentsView.Refresh();

            IncomeTypesView.Refresh();

            CardTypeFoldersView.Refresh();
            TicketTypeFoldersView.Refresh();

            FrInformFoldersView.Refresh();
            ProgramsFoldersView.Refresh();
            CorpFoldersView.Refresh();
            RoleFoldersView.Refresh();

            InstalmentFoldersView.Refresh();

            TreatmentFoldersView.Refresh();

            SolFoldersView.Refresh();
            StoreFoldersView.Refresh();

            FrTicketTypeFoldersView.Refresh();
            FrCardTypeFoldersView.Refresh();
            FrInstalmentFoldersView.Refresh();

            Divisions.Refresh();
            AdvertGroupsView.Refresh();
            if(_AdvertGroups.Count > 0) CurrentAdvertGroup = _AdvertGroups[0];
            AdvertTypesView.Refresh();

            NewsView.Refresh();
            CumulativesView.Refresh();
            BarDiscountsView.Refresh();
            PackagesView.Refresh();
            TreatmentsAvailableView.Refresh();
            TargetDetailsView.Refresh();
#if !BEAUTINIKA
            TargetConfigsView.Refresh();
#endif
            SolariumWarningsModified = false;
            OnUpdateFinished();

            FrCustomerStatusesView.Refresh();
            TargetsView.Refresh();

#if BEAUTINIKA
            FrRoomsView.Refresh();
#endif
        }

        public void RefreshUsers()
        {
            _UsersView.Clear();
            _UsersView.AddRange(ClientContext.GetUsers());
            UsersView.Refresh();
        }

        public void RefreshTargets()
        {
            Targets.Clear();
#if !BEAUTINIKA
            Targets.AddRange(ClientContext.GetTargetTypes());
#endif
            TargetsView.Refresh();
        }

#if !BEAUTINIKA
        public void RefreshTargetConfigs()
        {
            TargetConfigs.Clear();
            TargetConfigs.AddRange(ClientContext.GetTargetConfigs());
            TargetConfigsView.Refresh();
        }
#endif

        public void RefreshCustomerStatuses()
        {
            FrCustomerStatuses.Clear();
            FrCustomerStatuses.AddRange(ClientContext.GetAllStatuses().Select(i => new CustomerStatus { Id = i.Key, Name = i.Value, CompanyId = ClientContext.CurrentCompany.CompanyId, IsAvail = true }));
            FrCustomerStatusesView.Refresh();
        }

        public void RefreshCumulativeDiscounts()
        {
            _CumulativesView.Clear();
            _CumulativesView.AddRange(ClientContext.GetCumulativeDiscounts());
            CumulativesView.Refresh();
        }

        public void RefreshBarDiscounts()
        {
            _BarDiscountsView.Clear();
            _BarDiscountsView.AddRange(ClientContext.GetBarDiscounts());
            BarDiscountsView.Refresh();
        }

        public void RefreshPackages()
        {
            _Packages.Clear();
            _Packages.AddRange(ClientContext.GetPackages());
            PackagesView.Refresh();
        }

        public void RefreshInstalments()
        {
            _OrgInstalmentsView.Clear();
            _OrgInstalmentsView.AddRange(ClientContext.GetAllInstalments());
            OrgInstalmentsView.Refresh();
        }

        public void RefreshDivisions()
        {
            _Divisions.Clear();
            _Divisions.AddRange(ClientContext.GetDivisions());
            Divisions.Refresh();
        }

        public void RefreshRoles()
        {
            _RolesView.Clear();
            _RolesView.AddRange(ClientContext.GetRoles());
            RolesView.Refresh();
        }

        public void RefreshCallScrenario()
        {
            CallScrenario.Clear();
            CallScrenario.AddRange(ClientContext.GetCallScrenarioForms());
            CallScrenarioView.Refresh();
        }

        public void RefreshFiles()
        {
            Files.Clear();
            Files.AddRange(ClientContext.GetDivisionFiles());
            FilesView.Refresh();
        }

        internal void RefreshCorporates()
        {
            Corporates.Clear();
            Corporates.AddRange(ClientContext.GetCorporates());
            CorporatesView.Refresh();
        }

        internal void RefreshStorehouses()
        {
            Storehouses.Clear();
            Storehouses.AddRange(ClientContext.GetStorehouses());
            StorehousesView.Refresh();
        }

        internal void RefreshOrgTreatmentTypes()
        {
            OrgTreatmentTypes.Clear();
            OrgTreatmentTypes.AddRange(ClientContext.GetAllTreatmentTypes());
            OrgTreatmentTypesView.Refresh();
        }

        internal void RefreshSpendingTypes()
        {
            SpendingTypes.Clear();
            SpendingTypes.AddRange(ClientContext.GetDivisionSpendingTypes());
            SpendingTypesView.Refresh();
        }

        internal void RefreshSolariums()
        {
            Solariums.Clear();
            Solariums.AddRange(ClientContext.GetDivisionSolariums(false));
            SolariumsView.Refresh();
        }

        internal void RefreshParallelRules()
        {
            ParallelRules.Clear();
            UpdateParallelRules();
            ParallelRulesView.Refresh();
        }

        internal void RefreshTreatmentPrograms()
        {
            TreatmentPrograms.Clear();
            TreatmentPrograms.AddRange(ClientContext.GetTreatmentPrograms());
            TreatmentProgramsView.Refresh();
        }

        internal void RefreshOrgTreatmentConfigs()
        {
            OrgTreatmentConfigs.Clear();
            OrgTreatmentConfigs.AddRange(ClientContext.GetAllTreatmentConfigsAdmin());
            OrgTreatmentConfigsView.Refresh();
        }

        internal void RefreshFrTreatmentConfigs()
        {
            FrTreatmentConfigs.Clear();
            FrTreatmentConfigs.AddRange(ClientContext.GetAllTreatmentConfigs());
            FrTreatmentConfigsView.Refresh();
        }

        internal void RefreshOrgCardTypes()
        {
            OrgCardTypes.Clear();
            OrgCardTypes.AddRange(ClientContext.GetAllCustomerCardTypes());
            OrgCardTypesView.Refresh();
        }

        internal void RefreshOrgTicketTypes()
        {
            OrgTicketTypes.Clear();
            OrgTicketTypes.AddRange(ClientContext.GetAllTicketTypes());
            OrgTicketTypesView.Refresh();
        }

        internal void RefreshTemplates()
        {
            Templates.Clear();
            Templates.AddRange(ClientContext.GetAllTemplates());
            TemplatesView.Refresh();
        }

        internal void RefreshActions()
        {
            Actions.Clear();
            Actions.AddRange(ClientContext.GetAllActions());
            ActionsView.Refresh();
        }

        internal void RefreshAdvertGroups()
        {
            _AdvertGroups.Clear();
            _AdvertGroups.AddRange(ClientContext.GetAdvertGroups());
            AdvertGroupsView.Refresh();
            if(_AdvertGroups.Count > 0) CurrentAdvertGroup = _AdvertGroups[0];
            AdvertTypesView.Refresh();
        }

        internal void RefreshAdvertTypes()
        {
            _AdvertTypes.Clear();
            _AdvertTypes.AddRange(ClientContext.GetAdvertTypes());
            AdvertTypesView.Refresh();
        }

        internal void RefreshContras()
        {
            Guid? id = null;
            if(SelectedContra != null) id = SelectedContra.Id;
            Contras.Clear();
            Contras.AddRange(ClientContext.GetAllContras());
            ContrasView.Refresh();
            if(id.HasValue)
            {
                SelectedContra = Contras.FirstOrDefault(i => i.Id == id.Value);
            }
        }

        public CustomerCardType SelectedOrgCardType { get; set; }
        public TicketType SelectedOrgTicketType { get; set; }


        public ContraIndication SelectedContra { get; set; }

        internal void CommitCompany()
        {
            if(!Company.Modified) return;
            ClientContext.PostCompany(Company);
            ClientContext.UpdateCompany();
            Company = Clone<Company>(ClientContext.CurrentCompany);
        }

        internal void CommitDivision()
        {
            if(!Division.Modified) return;
            ClientContext.PostDivision(Division);
            ClientContext.UpdateDivision();
            Division = Clone<Division>(ClientContext.CurrentDivision);
        }

        internal void PostSolariumWarnings()
        {
            for(int i = 0; i < SolariumWarnings.Count; i++)
            {
                for(int j = i + 1; j < SolariumWarnings.Count; j++)
                {
                    if(SolariumWarnings[i].Key == SolariumWarnings[j].Key)
                    {
                        ExtraWindow.Alert(new DialogParameters
                        {
                            Header = "Ошибка",
                            Content = "Столбец \"Длительность\" не должен содержать\nповторяющихся значений.",
                            OkButtonContent = "ОК",
                            Owner = Application.Current.MainWindow
                        });
                        return;
                    }
                }
            }
            var res = new List<KeyValuePair<int, string>>();
            SolariumWarnings.ForEach(i => res.Add(new KeyValuePair<int, string>(i.Key, i.Value)));
            ClientContext.PostSolariumWarnings(res);
            RefreshSolariumWarnings();
        }

        public void RefreshSolariumWarnings()
        {
            SolariumWarnings.Clear();
            ClientContext.GetSolariumWarnings().ToList().ForEach(i => SolariumWarnings.Add(new Pair { Key = i.Key, Value = i.Value }));
            SolariumWarningsView.Refresh();
        }

        private class Pair
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }

        internal void RefreshFolders()
        {
            _CardTypeFoldersView.Clear();
            _CardTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(0, false), "Типы карт"));
            CardTypeFoldersView.Refresh();
            _TicketTypeFoldersView.Clear();
            _TicketTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(1, false), "Типы абонементов"));
            TicketTypeFoldersView.Refresh();
            _FrCardTypeFoldersView.Clear();
            _FrCardTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(0, true), "Типы карт"));
            _FrTicketTypeFoldersView.Clear();
            _FrTicketTypeFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(1, true), "Типы абонементов"));
            _FrInstalmentFoldersView.Clear();
            _FrInstalmentFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(2, true), "Рассрочки", false));
            _InstalmentFoldersView.Clear();
            _InstalmentFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(2, false), "Рассрочки", false));
            _TreatmentConfigsFoldersView.Clear();
            _TreatmentConfigsFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetSettingsFolders(3, false), "Услуги", false));

            InstalmentFoldersView.Refresh();

            RefreshOrgCardTypes();
            RefreshOrgTicketTypes();
            OnUpdateFinished();
            FrTicketTypeFoldersView.Refresh();
            FrCardTypeFoldersView.Refresh();
            TreatmentConfigsFoldersView.Refresh();
        }

        internal void RefreshCompanyFolders()
        {
            _FrInformFoldersView.Clear();
            _FrInformFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(0), "Информеры", false));
            FrInformFoldersView.Refresh();

            _ProgramsFoldersView.Clear();
            _ProgramsFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(1), "Программы занятий", false));
            ProgramsFoldersView.Refresh();

            _CorpFoldersView.Clear();
            _CorpFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(2), "Корпоративные договора", false));
            CorpFoldersView.Refresh();

            _TreatmentFoldersView.Clear();
            _TreatmentFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(3), "Оборудование"));
            TreatmentFoldersView.Refresh();


            _SolFoldersView.Clear();
            _SolFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(4), "Солярии"));
            SolFoldersView.Refresh();

            _StoreFoldersView.Clear();
            _StoreFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(5), "Склады", true));
            StoreFoldersView.Refresh();


            _RoleFoldersView.Clear();
            _RoleFoldersView.AddRange(ConstructSettingsFolders(ClientContext.GetCompanySettingsFolders(6), "Роли", false));
            RoleFoldersView.Refresh();

            RefreshActions();
            RefreshTreatmentPrograms();
            TreatmentsView.Refresh();
            RefreshStorehouses();
            RefreshSolariums();
            RefreshRoles();
            RefreshInstalments();
            OnUpdateFinished();

        }

        internal void RefreshNews()
        {
            News.Clear();
            News.AddRange(ClientContext.GetNews());
            NewsView.Refresh();
        }

        internal void RefreshClubTreatments()
        {
            Treatments.Clear();
            Treatments.AddRange(ClientContext.GetAllTreatments());
            TreatmentsView.Refresh();
        }
    }

    public class TreatmentContraView
    {
        public string Name { get; set; }
        public List<string> Contras { get; set; }
    }

}
