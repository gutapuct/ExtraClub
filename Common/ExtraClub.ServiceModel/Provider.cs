//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    
    [DataContract]
    [Serializable]
    public partial class Provider : INotifyPropertyChanged
    {
    	private bool _deleted;
    
    	[DataMember]
    	public bool Deleted {
    		get { return _deleted; }
    		set {
    			if (value != _deleted) Modified = true;
    			_deleted = value;
    		}
    	}
    
    	private bool _modified = false;
    	public bool Modified{
    		get
    		{
    			return _modified;
    		}
    		set
    		{
    			if (_modified != value)
    			{
    				_modified = value;
    				OnPropertyChanged("Modified");
    			}
    		}
    	}
    
    	[OnDeserialized()]
        private void OnDeserializedMethod(StreamingContext context)
        {
            _modified = false;
    		OnDeserialized();
        }
    
    	partial void OnDeserialized();
    
    	partial void OnPropertyChangedInternal(string propertyName);
    
        #region Primitive Properties
    	[DataMember]
        public virtual System.Guid Id
        {
            get {return _id;}
            set 
    		{ 
    			if(_id != value)
    			{
    				_id = value; 
    				
    				
    
    				OnPropertyChanged("Id");
    			}
    		}
        }
        private System.Guid _id;
    
    
    	[DataMember]
        public virtual string Name
        {
            get {return _name;}
            set 
    		{ 
    			if(_name != value)
    			{
    				_name = value; 
    				
    				
    
    				OnPropertyChanged("Name");
    			}
    		}
        }
        private string _name;
    
    
    	[DataMember]
        public virtual string INN
        {
            get {return _iNN;}
            set 
    		{ 
    			if(_iNN != value)
    			{
    				_iNN = value; 
    				
    				
    
    				OnPropertyChanged("INN");
    			}
    		}
        }
        private string _iNN;
    
    
    	[DataMember]
        public virtual string KPP
        {
            get {return _kPP;}
            set 
    		{ 
    			if(_kPP != value)
    			{
    				_kPP = value; 
    				
    				
    
    				OnPropertyChanged("KPP");
    			}
    		}
        }
        private string _kPP;
    
    
    	[DataMember]
        public virtual string FullName
        {
            get {return _fullName;}
            set 
    		{ 
    			if(_fullName != value)
    			{
    				_fullName = value; 
    				
    				
    
    				OnPropertyChanged("FullName");
    			}
    		}
        }
        private string _fullName;
    
    
    	[DataMember]
        public virtual string CorrAccount
        {
            get {return _corrAccount;}
            set 
    		{ 
    			if(_corrAccount != value)
    			{
    				_corrAccount = value; 
    				
    				
    
    				OnPropertyChanged("CorrAccount");
    			}
    		}
        }
        private string _corrAccount;
    
    
    	[DataMember]
        public virtual string SettlementAccount
        {
            get {return _settlementAccount;}
            set 
    		{ 
    			if(_settlementAccount != value)
    			{
    				_settlementAccount = value; 
    				
    				
    
    				OnPropertyChanged("SettlementAccount");
    			}
    		}
        }
        private string _settlementAccount;
    
    
    	[DataMember]
        public virtual string Bank
        {
            get {return _bank;}
            set 
    		{ 
    			if(_bank != value)
    			{
    				_bank = value; 
    				
    				
    
    				OnPropertyChanged("Bank");
    			}
    		}
        }
        private string _bank;
    
    
    	[DataMember]
        public virtual string BIK
        {
            get {return _bIK;}
            set 
    		{ 
    			if(_bIK != value)
    			{
    				_bIK = value; 
    				
    				
    
    				OnPropertyChanged("BIK");
    			}
    		}
        }
        private string _bIK;
    
    
    	[DataMember]
        public virtual string OKPO
        {
            get {return _oKPO;}
            set 
    		{ 
    			if(_oKPO != value)
    			{
    				_oKPO = value; 
    				
    				
    
    				OnPropertyChanged("OKPO");
    			}
    		}
        }
        private string _oKPO;
    
    
    	[DataMember]
        public virtual string OKONH
        {
            get {return _oKONH;}
            set 
    		{ 
    			if(_oKONH != value)
    			{
    				_oKONH = value; 
    				
    				
    
    				OnPropertyChanged("OKONH");
    			}
    		}
        }
        private string _oKONH;
    
    
    	[DataMember]
        public virtual string Director
        {
            get {return _director;}
            set 
    		{ 
    			if(_director != value)
    			{
    				_director = value; 
    				
    				
    
    				OnPropertyChanged("Director");
    			}
    		}
        }
        private string _director;
    
    
    	[DataMember]
        public virtual string LegalAddress
        {
            get {return _legalAddress;}
            set 
    		{ 
    			if(_legalAddress != value)
    			{
    				_legalAddress = value; 
    				
    				
    
    				OnPropertyChanged("LegalAddress");
    			}
    		}
        }
        private string _legalAddress;
    
    
    	[DataMember]
        public virtual string RealAddress
        {
            get {return _realAddress;}
            set 
    		{ 
    			if(_realAddress != value)
    			{
    				_realAddress = value; 
    				
    				
    
    				OnPropertyChanged("RealAddress");
    			}
    		}
        }
        private string _realAddress;
    
    
    	[DataMember]
        public virtual string Transport
        {
            get {return _transport;}
            set 
    		{ 
    			if(_transport != value)
    			{
    				_transport = value; 
    				
    				
    
    				OnPropertyChanged("Transport");
    			}
    		}
        }
        private string _transport;
    
    
    	[DataMember]
        public virtual string Accountant
        {
            get {return _accountant;}
            set 
    		{ 
    			if(_accountant != value)
    			{
    				_accountant = value; 
    				
    				
    
    				OnPropertyChanged("Accountant");
    			}
    		}
        }
        private string _accountant;
    
    
    	[DataMember]
        public virtual string Phone1
        {
            get {return _phone1;}
            set 
    		{ 
    			if(_phone1 != value)
    			{
    				_phone1 = value; 
    				
    				
    
    				OnPropertyChanged("Phone1");
    			}
    		}
        }
        private string _phone1;
    
    
    	[DataMember]
        public virtual string Phone2
        {
            get {return _phone2;}
            set 
    		{ 
    			if(_phone2 != value)
    			{
    				_phone2 = value; 
    				
    				
    
    				OnPropertyChanged("Phone2");
    			}
    		}
        }
        private string _phone2;
    
    
    	[DataMember]
        public virtual string Phone3
        {
            get {return _phone3;}
            set 
    		{ 
    			if(_phone3 != value)
    			{
    				_phone3 = value; 
    				
    				
    
    				OnPropertyChanged("Phone3");
    			}
    		}
        }
        private string _phone3;
    
    
    	[DataMember]
        public virtual string Fax
        {
            get {return _fax;}
            set 
    		{ 
    			if(_fax != value)
    			{
    				_fax = value; 
    				
    				
    
    				OnPropertyChanged("Fax");
    			}
    		}
        }
        private string _fax;
    
    
    	[DataMember]
        public virtual string Email
        {
            get {return _email;}
            set 
    		{ 
    			if(_email != value)
    			{
    				_email = value; 
    				
    				
    
    				OnPropertyChanged("Email");
    			}
    		}
        }
        private string _email;
    
    
    	[DataMember]
        public virtual string WebSite
        {
            get {return _webSite;}
            set 
    		{ 
    			if(_webSite != value)
    			{
    				_webSite = value; 
    				
    				
    
    				OnPropertyChanged("WebSite");
    			}
    		}
        }
        private string _webSite;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> OrganizationTypeId
        {
            get { return _organizationTypeId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_organizationTypeId != value)
                    {
                        if (OrganizationType != null && OrganizationType.Id != value)
                        {
                            OrganizationType = null;
                        }
                        _organizationTypeId = value;
        
        				OnPropertyChanged("OrganizationTypeId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _organizationTypeId;
    
    
    	[DataMember]
        public virtual string WorkTime
        {
            get {return _workTime;}
            set 
    		{ 
    			if(_workTime != value)
    			{
    				_workTime = value; 
    				
    				
    
    				OnPropertyChanged("WorkTime");
    			}
    		}
        }
        private string _workTime;
    
    
    	[DataMember]
        public virtual string Comments
        {
            get {return _comments;}
            set 
    		{ 
    			if(_comments != value)
    			{
    				_comments = value; 
    				
    				
    
    				OnPropertyChanged("Comments");
    			}
    		}
        }
        private string _comments;
    
    
    	[DataMember]
        public virtual System.Guid AuthorId
        {
            get { return _authorId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_authorId != value)
                    {
                        if (User != null && User.UserId != value)
                        {
                            User = null;
                        }
                        _authorId = value;
        
        				OnPropertyChanged("AuthorId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _authorId;
    
    
    	[DataMember]
        public virtual System.Guid CompanyId
        {
            get { return _companyId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_companyId != value)
                    {
                        if (Company != null && Company.CompanyId != value)
                        {
                            Company = null;
                        }
                        _companyId = value;
        
        				OnPropertyChanged("CompanyId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _companyId;
    
    
    	[DataMember]
        public virtual string ContactPerson
        {
            get {return _contactPerson;}
            set 
    		{ 
    			if(_contactPerson != value)
    			{
    				_contactPerson = value; 
    				
    				
    
    				OnPropertyChanged("ContactPerson");
    			}
    		}
        }
        private string _contactPerson;
    
    
    	[DataMember]
        public virtual string PostAddress
        {
            get {return _postAddress;}
            set 
    		{ 
    			if(_postAddress != value)
    			{
    				_postAddress = value; 
    				
    				
    
    				OnPropertyChanged("PostAddress");
    			}
    		}
        }
        private string _postAddress;
    
    
    	[DataMember]
        public virtual string Ogrn
        {
            get {return _ogrn;}
            set 
    		{ 
    			if(_ogrn != value)
    			{
    				_ogrn = value; 
    				
    				
    
    				OnPropertyChanged("Ogrn");
    			}
    		}
        }
        private string _ogrn;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> ProviderFolderId
        {
            get { return _providerFolderId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_providerFolderId != value)
                    {
                        if (ProviderFolder != null && ProviderFolder.Id != value)
                        {
                            ProviderFolder = null;
                        }
                        _providerFolderId = value;
        
        				OnPropertyChanged("ProviderFolderId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _providerFolderId;
    
    
    	[DataMember]
        public virtual bool IsVisible
        {
            get {return _isVisible;}
            set 
    		{ 
    			if(_isVisible != value)
    			{
    				_isVisible = value; 
    				
    				
    
    				OnPropertyChanged("IsVisible");
    			}
    		}
        }
        private bool _isVisible;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual Company Company
        {
            get { return _company; }
            set
            {
                if (!ReferenceEquals(_company, value))
                {
                    var previousValue = _company;
                    _company = value;
                    FixupCompany(previousValue);
                }
            }
        }
        private Company _company;
    
        public virtual OrganizationType OrganizationType
        {
            get { return _organizationType; }
            set
            {
                if (!ReferenceEquals(_organizationType, value))
                {
                    var previousValue = _organizationType;
                    _organizationType = value;
                    FixupOrganizationType(previousValue);
                }
            }
        }
        private OrganizationType _organizationType;
    
        public virtual User User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                }
            }
        }
        private User _user;
    
        public virtual ICollection<ProviderPayment> ProviderPayments
        {
            get
            {
                if (_providerPayments == null)
                {
                    var newCollection = new FixupCollection<ProviderPayment>();
                    newCollection.CollectionChanged += FixupProviderPayments;
                    _providerPayments = newCollection;
                }
                return _providerPayments;
            }
            set
            {
                if (!ReferenceEquals(_providerPayments, value))
                {
                    var previousValue = _providerPayments as FixupCollection<ProviderPayment>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProviderPayments;
                    }
                    _providerPayments = value;
                    var newValue = value as FixupCollection<ProviderPayment>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProviderPayments;
                    }
    				OnPropertyChanged("ProviderPayments");
                }
            }
        }
        private ICollection<ProviderPayment> _providerPayments;
    
        public virtual ICollection<Consignment> Consignments
        {
            get
            {
                if (_consignments == null)
                {
                    var newCollection = new FixupCollection<Consignment>();
                    newCollection.CollectionChanged += FixupConsignments;
                    _consignments = newCollection;
                }
                return _consignments;
            }
            set
            {
                if (!ReferenceEquals(_consignments, value))
                {
                    var previousValue = _consignments as FixupCollection<Consignment>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupConsignments;
                    }
                    _consignments = value;
                    var newValue = value as FixupCollection<Consignment>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupConsignments;
                    }
    				OnPropertyChanged("Consignments");
                }
            }
        }
        private ICollection<Consignment> _consignments;
    
        public virtual ProviderFolder ProviderFolder
        {
            get { return _providerFolder; }
            set
            {
                if (!ReferenceEquals(_providerFolder, value))
                {
                    var previousValue = _providerFolder;
                    _providerFolder = value;
                    FixupProviderFolder(previousValue);
                }
            }
        }
        private ProviderFolder _providerFolder;
    
        public virtual ICollection<BarOrder> BarOrders
        {
            get
            {
                if (_barOrders == null)
                {
                    var newCollection = new FixupCollection<BarOrder>();
                    newCollection.CollectionChanged += FixupBarOrders;
                    _barOrders = newCollection;
                }
                return _barOrders;
            }
            set
            {
                if (!ReferenceEquals(_barOrders, value))
                {
                    var previousValue = _barOrders as FixupCollection<BarOrder>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupBarOrders;
                    }
                    _barOrders = value;
                    var newValue = value as FixupCollection<BarOrder>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupBarOrders;
                    }
    				OnPropertyChanged("BarOrders");
                }
            }
        }
        private ICollection<BarOrder> _barOrders;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.Providers.Contains(this))
            {
                previousValue.Providers.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Providers.Contains(this))
                {
                    Company.Providers.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupOrganizationType(OrganizationType previousValue)
        {
            if (OrganizationType != null)
            {
                if (OrganizationTypeId != OrganizationType.Id)
                {
                    OrganizationTypeId = OrganizationType.Id;
                }
            }
            else if (!_settingFK)
            {
                OrganizationTypeId = null;
            }
        }
    
        private void FixupUser(User previousValue)
        {
            if (User != null)
            {
                if (AuthorId != User.UserId)
                {
                    AuthorId = User.UserId;
                }
            }
        }
    
        private void FixupProviderFolder(ProviderFolder previousValue)
        {
            if (previousValue != null && previousValue.Providers.Contains(this))
            {
                previousValue.Providers.Remove(this);
            }
    
            if (ProviderFolder != null)
            {
                if (!ProviderFolder.Providers.Contains(this))
                {
                    ProviderFolder.Providers.Add(this);
                }
                if (ProviderFolderId != ProviderFolder.Id)
                {
                    ProviderFolderId = ProviderFolder.Id;
                }
            }
            else if (!_settingFK)
            {
                ProviderFolderId = null;
            }
        }
    
        private void FixupProviderPayments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProviderPayment item in e.NewItems)
                {
                    item.Provider = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProviderPayment item in e.OldItems)
                {
                    if (ReferenceEquals(item.Provider, this))
                    {
                        item.Provider = null;
                    }
                }
            }
        }
    
        private void FixupConsignments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Consignment item in e.NewItems)
                {
                    item.Provider = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Consignment item in e.OldItems)
                {
                    if (ReferenceEquals(item.Provider, this))
                    {
                        item.Provider = null;
                    }
                }
            }
        }
    
        private void FixupBarOrders(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BarOrder item in e.NewItems)
                {
                    item.Provider = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (BarOrder item in e.OldItems)
                {
                    if (ReferenceEquals(item.Provider, this))
                    {
                        item.Provider = null;
                    }
                }
            }
        }

        #endregion

    
    	public event PropertyChangedEventHandler PropertyChanged;
    
    	protected void OnPropertyChanged(string propertyName)
        {
    		Modified = true;
    
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    		
    		OnPropertyChangedInternal(propertyName);
        }
    }
}