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
    public partial class Role : INotifyPropertyChanged
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
        public virtual System.Guid RoleId
        {
            get {return _roleId;}
            set 
    		{ 
    			if(_roleId != value)
    			{
    				_roleId = value; 
    				
    				
    
    				OnPropertyChanged("RoleId");
    			}
    		}
        }
        private System.Guid _roleId;
    
    
    	[DataMember]
        public virtual string RoleName
        {
            get {return _roleName;}
            set 
    		{ 
    			if(_roleName != value)
    			{
    				_roleName = value; 
    				
    				
    
    				OnPropertyChanged("RoleName");
    			}
    		}
        }
        private string _roleName;
    
    
    	[DataMember]
        public virtual System.DateTime CreatedOn
        {
            get {return _createdOn;}
            set 
    		{ 
    			if(_createdOn != value)
    			{
    				_createdOn = value; 
    				
    				_createdOn = DateTime.SpecifyKind(_createdOn, DateTimeKind.Local);
    
    				OnPropertyChanged("CreatedOn");
    			}
    		}
        }
        private System.DateTime _createdOn;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> ModifiedOn
        {
            get {return _modifiedOn;}
            set 
    		{ 
    			if(_modifiedOn != value)
    			{
    				_modifiedOn = value; 
    				
    				if (_modifiedOn.HasValue) _modifiedOn = DateTime.SpecifyKind(_modifiedOn.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("ModifiedOn");
    			}
    		}
        }
        private Nullable<System.DateTime> _modifiedOn;
    
    
    	[DataMember]
        public virtual System.Guid CompanyId
        {
            get { return _companyId; }
            set
            {
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
        }
        private System.Guid _companyId;
    
    
    	[DataMember]
        public virtual string CardDiscs
        {
            get {return _cardDiscs;}
            set 
    		{ 
    			if(_cardDiscs != value)
    			{
    				_cardDiscs = value; 
    				
    				
    
    				OnPropertyChanged("CardDiscs");
    			}
    		}
        }
        private string _cardDiscs;
    
    
    	[DataMember]
        public virtual string TicketDiscs
        {
            get {return _ticketDiscs;}
            set 
    		{ 
    			if(_ticketDiscs != value)
    			{
    				_ticketDiscs = value; 
    				
    				
    
    				OnPropertyChanged("TicketDiscs");
    			}
    		}
        }
        private string _ticketDiscs;
    
    
    	[DataMember]
        public virtual bool IsReadonly
        {
            get {return _isReadonly;}
            set 
    		{ 
    			if(_isReadonly != value)
    			{
    				_isReadonly = value; 
    				
    				
    
    				OnPropertyChanged("IsReadonly");
    			}
    		}
        }
        private bool _isReadonly;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> SettingsFolderId
        {
            get {return _settingsFolderId;}
            set 
    		{ 
    			if(_settingsFolderId != value)
    			{
    				_settingsFolderId = value; 
    				
    				
    
    				OnPropertyChanged("SettingsFolderId");
    			}
    		}
        }
        private Nullable<System.Guid> _settingsFolderId;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> CreatedBy
        {
            get {return _createdBy;}
            set 
    		{ 
    			if(_createdBy != value)
    			{
    				_createdBy = value; 
    				
    				
    
    				OnPropertyChanged("CreatedBy");
    			}
    		}
        }
        private Nullable<System.Guid> _createdBy;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> ModifiedBy
        {
            get {return _modifiedBy;}
            set 
    		{ 
    			if(_modifiedBy != value)
    			{
    				_modifiedBy = value; 
    				
    				
    
    				OnPropertyChanged("ModifiedBy");
    			}
    		}
        }
        private Nullable<System.Guid> _modifiedBy;
    
    
    	[DataMember]
        public virtual bool IsFixed
        {
            get {return _isFixed;}
            set 
    		{ 
    			if(_isFixed != value)
    			{
    				_isFixed = value; 
    				
    				
    
    				OnPropertyChanged("IsFixed");
    			}
    		}
        }
        private bool _isFixed;
    
    
    	[DataMember]
        public virtual string TicketRubDiscs
        {
            get {return _ticketRubDiscs;}
            set 
    		{ 
    			if(_ticketRubDiscs != value)
    			{
    				_ticketRubDiscs = value; 
    				
    				
    
    				OnPropertyChanged("TicketRubDiscs");
    			}
    		}
        }
        private string _ticketRubDiscs;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<Permission> Permissions
        {
            get
            {
                if (_permissions == null)
                {
                    var newCollection = new FixupCollection<Permission>();
                    newCollection.CollectionChanged += FixupPermissions;
                    _permissions = newCollection;
                }
                return _permissions;
            }
            set
            {
                if (!ReferenceEquals(_permissions, value))
                {
                    var previousValue = _permissions as FixupCollection<Permission>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPermissions;
                    }
                    _permissions = value;
                    var newValue = value as FixupCollection<Permission>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPermissions;
                    }
    				OnPropertyChanged("Permissions");
                }
            }
        }
        private ICollection<Permission> _permissions;
    
        public virtual ICollection<User> Users
        {
            get
            {
                if (_users == null)
                {
                    var newCollection = new FixupCollection<User>();
                    newCollection.CollectionChanged += FixupUsers;
                    _users = newCollection;
                }
                return _users;
            }
            set
            {
                if (!ReferenceEquals(_users, value))
                {
                    var previousValue = _users as FixupCollection<User>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUsers;
                    }
                    _users = value;
                    var newValue = value as FixupCollection<User>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUsers;
                    }
    				OnPropertyChanged("Users");
                }
            }
        }
        private ICollection<User> _users;
    
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
    
        public virtual ICollection<CustomReport> CustomReports
        {
            get
            {
                if (_customReports == null)
                {
                    var newCollection = new FixupCollection<CustomReport>();
                    newCollection.CollectionChanged += FixupCustomReports;
                    _customReports = newCollection;
                }
                return _customReports;
            }
            set
            {
                if (!ReferenceEquals(_customReports, value))
                {
                    var previousValue = _customReports as FixupCollection<CustomReport>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCustomReports;
                    }
                    _customReports = value;
                    var newValue = value as FixupCollection<CustomReport>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCustomReports;
                    }
    				OnPropertyChanged("CustomReports");
                }
            }
        }
        private ICollection<CustomReport> _customReports;

        #endregion

        #region Association Fixup
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.Roles.Contains(this))
            {
                previousValue.Roles.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Roles.Contains(this))
                {
                    Company.Roles.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupPermissions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Permission item in e.NewItems)
                {
                    if (!item.Roles.Contains(this))
                    {
                        item.Roles.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Permission item in e.OldItems)
                {
                    if (item.Roles.Contains(this))
                    {
                        item.Roles.Remove(this);
                    }
                }
            }
        }
    
        private void FixupUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (User item in e.NewItems)
                {
                    if (!item.Roles.Contains(this))
                    {
                        item.Roles.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (User item in e.OldItems)
                {
                    if (item.Roles.Contains(this))
                    {
                        item.Roles.Remove(this);
                    }
                }
            }
        }
    
        private void FixupCustomReports(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CustomReport item in e.NewItems)
                {
                    if (!item.Roles.Contains(this))
                    {
                        item.Roles.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CustomReport item in e.OldItems)
                {
                    if (item.Roles.Contains(this))
                    {
                        item.Roles.Remove(this);
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
