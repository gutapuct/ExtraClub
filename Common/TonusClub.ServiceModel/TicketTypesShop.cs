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
    public partial class TicketTypesShop : INotifyPropertyChanged
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
        public virtual System.Guid CompanyId
        {
            get {return _companyId;}
            set 
    		{ 
    			if(_companyId != value)
    			{
    				_companyId = value; 
    				
    				
    
    				OnPropertyChanged("CompanyId");
    			}
    		}
        }
        private System.Guid _companyId;
    
    
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
        public virtual System.Guid TicketTypeId
        {
            get { return _ticketTypeId; }
            set
            {
                if (_ticketTypeId != value)
                {
                    if (TicketType != null && TicketType.Id != value)
                    {
                        TicketType = null;
                    }
                    _ticketTypeId = value;
    
    				OnPropertyChanged("TicketTypeId");
                }
            }
        }
        private System.Guid _ticketTypeId;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual TicketType TicketType
        {
            get { return _ticketType; }
            set
            {
                if (!ReferenceEquals(_ticketType, value))
                {
                    var previousValue = _ticketType;
                    _ticketType = value;
                    FixupTicketType(previousValue);
                }
            }
        }
        private TicketType _ticketType;

        #endregion

        #region Association Fixup
    
        private void FixupTicketType(TicketType previousValue)
        {
            if (previousValue != null && previousValue.TicketTypesShops.Contains(this))
            {
                previousValue.TicketTypesShops.Remove(this);
            }
    
            if (TicketType != null)
            {
                if (!TicketType.TicketTypesShops.Contains(this))
                {
                    TicketType.TicketTypesShops.Add(this);
                }
                if (TicketTypeId != TicketType.Id)
                {
                    TicketTypeId = TicketType.Id;
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
