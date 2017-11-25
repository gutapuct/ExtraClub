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

namespace TonusClub.ServiceModel
{
    
    [DataContract]
    [Serializable]
    public partial class Solarium : INotifyPropertyChanged
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
        public virtual System.Guid DivisionId
        {
            get { return _divisionId; }
            set
            {
                if (_divisionId != value)
                {
                    if (Division != null && Division.Id != value)
                    {
                        Division = null;
                    }
                    _divisionId = value;
    
    				OnPropertyChanged("DivisionId");
                }
            }
        }
        private System.Guid _divisionId;
    
    
    	[DataMember]
        public virtual System.Guid AuthorId
        {
            get { return _authorId; }
            set
            {
                if (_authorId != value)
                {
                    if (CreatedBy != null && CreatedBy.UserId != value)
                    {
                        CreatedBy = null;
                    }
                    _authorId = value;
    
    				OnPropertyChanged("AuthorId");
                }
            }
        }
        private System.Guid _authorId;
    
    
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
        public virtual bool IsActive
        {
            get {return _isActive;}
            set 
    		{ 
    			if(_isActive != value)
    			{
    				_isActive = value; 
    				
    				
    
    				OnPropertyChanged("IsActive");
    			}
    		}
        }
        private bool _isActive;
    
    
    	[DataMember]
        public virtual string Comment
        {
            get {return _comment;}
            set 
    		{ 
    			if(_comment != value)
    			{
    				_comment = value; 
    				
    				
    
    				OnPropertyChanged("Comment");
    			}
    		}
        }
        private string _comment;
    
    
    	[DataMember]
        public virtual string DogNumber
        {
            get {return _dogNumber;}
            set 
    		{ 
    			if(_dogNumber != value)
    			{
    				_dogNumber = value; 
    				
    				
    
    				OnPropertyChanged("DogNumber");
    			}
    		}
        }
        private string _dogNumber;
    
    
    	[DataMember]
        public virtual string SerialNumber
        {
            get {return _serialNumber;}
            set 
    		{ 
    			if(_serialNumber != value)
    			{
    				_serialNumber = value; 
    				
    				
    
    				OnPropertyChanged("SerialNumber");
    			}
    		}
        }
        private string _serialNumber;
    
    
    	[DataMember]
        public virtual string Delivery
        {
            get {return _delivery;}
            set 
    		{ 
    			if(_delivery != value)
    			{
    				_delivery = value; 
    				
    				
    
    				OnPropertyChanged("Delivery");
    			}
    		}
        }
        private string _delivery;
    
    
    	[DataMember]
        public virtual string GuaranteeExp
        {
            get {return _guaranteeExp;}
            set 
    		{ 
    			if(_guaranteeExp != value)
    			{
    				_guaranteeExp = value; 
    				
    				
    
    				OnPropertyChanged("GuaranteeExp");
    			}
    		}
        }
        private string _guaranteeExp;
    
    
    	[DataMember]
        public virtual string UseExp
        {
            get {return _useExp;}
            set 
    		{ 
    			if(_useExp != value)
    			{
    				_useExp = value; 
    				
    				
    
    				OnPropertyChanged("UseExp");
    			}
    		}
        }
        private string _useExp;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> LampsExpires
        {
            get {return _lampsExpires;}
            set 
    		{ 
    			if(_lampsExpires != value)
    			{
    				_lampsExpires = value; 
    				
    				if (_lampsExpires.HasValue) _lampsExpires = DateTime.SpecifyKind(_lampsExpires.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("LampsExpires");
    			}
    		}
        }
        private Nullable<System.DateTime> _lampsExpires;
    
    
    	[DataMember]
        public virtual int MaintenaceTime
        {
            get {return _maintenaceTime;}
            set 
    		{ 
    			if(_maintenaceTime != value)
    			{
    				_maintenaceTime = value; 
    				
    				
    
    				OnPropertyChanged("MaintenaceTime");
    			}
    		}
        }
        private int _maintenaceTime;
    
    
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
        public virtual int LapsResource
        {
            get {return _lapsResource;}
            set 
    		{ 
    			if(_lapsResource != value)
    			{
    				_lapsResource = value; 
    				
    				
    
    				OnPropertyChanged("LapsResource");
    			}
    		}
        }
        private int _lapsResource;
    
    
    	[DataMember]
        public virtual string Color
        {
            get {return _color;}
            set 
    		{ 
    			if(_color != value)
    			{
    				_color = value; 
    				
    				
    
    				OnPropertyChanged("Color");
    			}
    		}
        }
        private string _color;
    
    
    	[DataMember]
        public virtual string Size
        {
            get {return _size;}
            set 
    		{ 
    			if(_size != value)
    			{
    				_size = value; 
    				
    				
    
    				OnPropertyChanged("Size");
    			}
    		}
        }
        private string _size;
    
    
    	[DataMember]
        public virtual string Model
        {
            get {return _model;}
            set 
    		{ 
    			if(_model != value)
    			{
    				_model = value; 
    				
    				
    
    				OnPropertyChanged("Model");
    			}
    		}
        }
        private string _model;
    
    
    	[DataMember]
        public virtual decimal MinutePrice
        {
            get {return _minutePrice;}
            set 
    		{ 
    			if(_minutePrice != value)
    			{
    				_minutePrice = value; 
    				
    				
    
    				OnPropertyChanged("MinutePrice");
    			}
    		}
        }
        private decimal _minutePrice;
    
    
    	[DataMember]
        public virtual decimal TicketMinutePrice
        {
            get {return _ticketMinutePrice;}
            set 
    		{ 
    			if(_ticketMinutePrice != value)
    			{
    				_ticketMinutePrice = value; 
    				
    				
    
    				OnPropertyChanged("TicketMinutePrice");
    			}
    		}
        }
        private decimal _ticketMinutePrice;
    
    
    	[DataMember]
        public virtual string Code1C
        {
            get {return _code1C;}
            set 
    		{ 
    			if(_code1C != value)
    			{
    				_code1C = value; 
    				
    				
    
    				OnPropertyChanged("Code1C");
    			}
    		}
        }
        private string _code1C;
    
    

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
    
        public virtual Division Division
        {
            get { return _division; }
            set
            {
                if (!ReferenceEquals(_division, value))
                {
                    var previousValue = _division;
                    _division = value;
                    FixupDivision(previousValue);
                }
            }
        }
        private Division _division;
    
        public virtual User CreatedBy
        {
            get { return _createdBy; }
            set
            {
                if (!ReferenceEquals(_createdBy, value))
                {
                    var previousValue = _createdBy;
                    _createdBy = value;
                    FixupCreatedBy(previousValue);
                }
            }
        }
        private User _createdBy;
    
        public virtual ICollection<SolariumVisit> SolariumVisits
        {
            get
            {
                if (_solariumVisits == null)
                {
                    var newCollection = new FixupCollection<SolariumVisit>();
                    newCollection.CollectionChanged += FixupSolariumVisits;
                    _solariumVisits = newCollection;
                }
                return _solariumVisits;
            }
            set
            {
                if (!ReferenceEquals(_solariumVisits, value))
                {
                    var previousValue = _solariumVisits as FixupCollection<SolariumVisit>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSolariumVisits;
                    }
                    _solariumVisits = value;
                    var newValue = value as FixupCollection<SolariumVisit>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSolariumVisits;
                    }
    				OnPropertyChanged("SolariumVisits");
                }
            }
        }
        private ICollection<SolariumVisit> _solariumVisits;

        #endregion

        #region Association Fixup
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.Solariums.Contains(this))
            {
                previousValue.Solariums.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Solariums.Contains(this))
                {
                    Company.Solariums.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupDivision(Division previousValue)
        {
            if (previousValue != null && previousValue.Solariums.Contains(this))
            {
                previousValue.Solariums.Remove(this);
            }
    
            if (Division != null)
            {
                if (!Division.Solariums.Contains(this))
                {
                    Division.Solariums.Add(this);
                }
                if (DivisionId != Division.Id)
                {
                    DivisionId = Division.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.Solariums.Contains(this))
            {
                previousValue.Solariums.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.Solariums.Contains(this))
                {
                    CreatedBy.Solariums.Add(this);
                }
                if (AuthorId != CreatedBy.UserId)
                {
                    AuthorId = CreatedBy.UserId;
                }
            }
        }
    
        private void FixupSolariumVisits(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SolariumVisit item in e.NewItems)
                {
                    item.Solarium = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SolariumVisit item in e.OldItems)
                {
                    if (ReferenceEquals(item.Solarium, this))
                    {
                        item.Solarium = null;
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