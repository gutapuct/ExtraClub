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
    public partial class CustomerCardType : INotifyPropertyChanged
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
        public virtual decimal Price
        {
            get {return _price;}
            set 
    		{ 
    			if(_price != value)
    			{
    				_price = value; 
    				
    				
    
    				OnPropertyChanged("Price");
    			}
    		}
        }
        private decimal _price;
    
    
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
        public virtual decimal Bonus
        {
            get {return _bonus;}
            set 
    		{ 
    			if(_bonus != value)
    			{
    				_bonus = value; 
    				
    				
    
    				OnPropertyChanged("Bonus");
    			}
    		}
        }
        private decimal _bonus;
    
    
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
        public virtual bool IsGuest
        {
            get {return _isGuest;}
            set 
    		{ 
    			if(_isGuest != value)
    			{
    				_isGuest = value; 
    				
    				
    
    				OnPropertyChanged("IsGuest");
    			}
    		}
        }
        private bool _isGuest;
    
    
    	[DataMember]
        public virtual bool IsVisit
        {
            get {return _isVisit;}
            set 
    		{ 
    			if(_isVisit != value)
    			{
    				_isVisit = value; 
    				
    				
    
    				OnPropertyChanged("IsVisit");
    			}
    		}
        }
        private bool _isVisit;
    
    
    	[DataMember]
        public virtual decimal LostPenalty
        {
            get {return _lostPenalty;}
            set 
    		{ 
    			if(_lostPenalty != value)
    			{
    				_lostPenalty = value; 
    				
    				
    
    				OnPropertyChanged("LostPenalty");
    			}
    		}
        }
        private decimal _lostPenalty;
    
    
    	[DataMember]
        public virtual decimal BonusPercent
        {
            get {return _bonusPercent;}
            set 
    		{ 
    			if(_bonusPercent != value)
    			{
    				_bonusPercent = value; 
    				
    				
    
    				OnPropertyChanged("BonusPercent");
    			}
    		}
        }
        private decimal _bonusPercent;
    
    
    	[DataMember]
        public virtual string Description
        {
            get {return _description;}
            set 
    		{ 
    			if(_description != value)
    			{
    				_description = value; 
    				
    				
    
    				OnPropertyChanged("Description");
    			}
    		}
        }
        private string _description;
    
    
    	[DataMember]
        public virtual double FreezePriceCoeff
        {
            get {return _freezePriceCoeff;}
            set 
    		{ 
    			if(_freezePriceCoeff != value)
    			{
    				_freezePriceCoeff = value; 
    				
    				
    
    				OnPropertyChanged("FreezePriceCoeff");
    			}
    		}
        }
        private double _freezePriceCoeff;
    
    
    	[DataMember]
        public virtual decimal ChildrenCost
        {
            get {return _childrenCost;}
            set 
    		{ 
    			if(_childrenCost != value)
    			{
    				_childrenCost = value; 
    				
    				
    
    				OnPropertyChanged("ChildrenCost");
    			}
    		}
        }
        private decimal _childrenCost;
    
    
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
    
    
    	[DataMember]
        public virtual decimal DiscountBar
        {
            get {return _discountBar;}
            set 
    		{ 
    			if(_discountBar != value)
    			{
    				_discountBar = value; 
    				
    				
    
    				OnPropertyChanged("DiscountBar");
    			}
    		}
        }
        private decimal _discountBar;
    
    
    	[DataMember]
        public virtual bool GiveBonusForCards
        {
            get {return _giveBonusForCards;}
            set 
    		{ 
    			if(_giveBonusForCards != value)
    			{
    				_giveBonusForCards = value; 
    				
    				
    
    				OnPropertyChanged("GiveBonusForCards");
    			}
    		}
        }
        private bool _giveBonusForCards;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<CustomerCard> CustomerCards
        {
            get
            {
                if (_customerCards == null)
                {
                    var newCollection = new FixupCollection<CustomerCard>();
                    newCollection.CollectionChanged += FixupCustomerCards;
                    _customerCards = newCollection;
                }
                return _customerCards;
            }
            set
            {
                if (!ReferenceEquals(_customerCards, value))
                {
                    var previousValue = _customerCards as FixupCollection<CustomerCard>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCustomerCards;
                    }
                    _customerCards = value;
                    var newValue = value as FixupCollection<CustomerCard>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCustomerCards;
                    }
    				OnPropertyChanged("CustomerCards");
                }
            }
        }
        private ICollection<CustomerCard> _customerCards;
    
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
    
        public virtual ICollection<Company> Companies
        {
            get
            {
                if (_companies == null)
                {
                    var newCollection = new FixupCollection<Company>();
                    newCollection.CollectionChanged += FixupCompanies;
                    _companies = newCollection;
                }
                return _companies;
            }
            set
            {
                if (!ReferenceEquals(_companies, value))
                {
                    var previousValue = _companies as FixupCollection<Company>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanies;
                    }
                    _companies = value;
                    var newValue = value as FixupCollection<Company>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanies;
                    }
    				OnPropertyChanged("Companies");
                }
            }
        }
        private ICollection<Company> _companies;
    
        public virtual ICollection<TicketType> TicketTypes
        {
            get
            {
                if (_ticketTypes == null)
                {
                    var newCollection = new FixupCollection<TicketType>();
                    newCollection.CollectionChanged += FixupTicketTypes;
                    _ticketTypes = newCollection;
                }
                return _ticketTypes;
            }
            set
            {
                if (!ReferenceEquals(_ticketTypes, value))
                {
                    var previousValue = _ticketTypes as FixupCollection<TicketType>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTicketTypes;
                    }
                    _ticketTypes = value;
                    var newValue = value as FixupCollection<TicketType>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTicketTypes;
                    }
    				OnPropertyChanged("TicketTypes");
                }
            }
        }
        private ICollection<TicketType> _ticketTypes;

        #endregion

        #region Association Fixup
    
        private void FixupCreatedBy(User previousValue)
        {
            if (CreatedBy != null)
            {
                if (AuthorId != CreatedBy.UserId)
                {
                    AuthorId = CreatedBy.UserId;
                }
            }
        }
    
        private void FixupCustomerCards(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CustomerCard item in e.NewItems)
                {
                    item.CustomerCardType = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CustomerCard item in e.OldItems)
                {
                    if (ReferenceEquals(item.CustomerCardType, this))
                    {
                        item.CustomerCardType = null;
                    }
                }
            }
        }
    
        private void FixupCompanies(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Company item in e.NewItems)
                {
                    if (!item.CustomerCardTypes.Contains(this))
                    {
                        item.CustomerCardTypes.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Company item in e.OldItems)
                {
                    if (item.CustomerCardTypes.Contains(this))
                    {
                        item.CustomerCardTypes.Remove(this);
                    }
                }
            }
        }
    
        private void FixupTicketTypes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TicketType item in e.NewItems)
                {
                    if (!item.CustomerCardTypes.Contains(this))
                    {
                        item.CustomerCardTypes.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TicketType item in e.OldItems)
                {
                    if (item.CustomerCardTypes.Contains(this))
                    {
                        item.CustomerCardTypes.Remove(this);
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
