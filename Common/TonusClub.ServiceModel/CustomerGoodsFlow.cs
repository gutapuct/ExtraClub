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
    public partial class CustomerGoodsFlow : INotifyPropertyChanged
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
        public virtual System.Guid CustomerId
        {
            get { return _customerId; }
            set
            {
                if (_customerId != value)
                {
                    if (Customer != null && Customer.Id != value)
                    {
                        Customer = null;
                    }
                    _customerId = value;
    
    				OnPropertyChanged("CustomerId");
                }
            }
        }
        private System.Guid _customerId;
    
    
    	[DataMember]
        public virtual System.Guid GoodId
        {
            get { return _goodId; }
            set
            {
                if (_goodId != value)
                {
                    if (Good != null && Good.Id != value)
                    {
                        Good = null;
                    }
                    _goodId = value;
    
    				OnPropertyChanged("GoodId");
                }
            }
        }
        private System.Guid _goodId;
    
    
    	[DataMember]
        public virtual int Amount
        {
            get {return _amount;}
            set 
    		{ 
    			if(_amount != value)
    			{
    				_amount = value; 
    				
    				
    
    				OnPropertyChanged("Amount");
    			}
    		}
        }
        private int _amount;
    
    
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
        public virtual System.Guid CreatedById
        {
            get {return _createdById;}
            set 
    		{ 
    			if(_createdById != value)
    			{
    				_createdById = value; 
    				
    				
    
    				OnPropertyChanged("CreatedById");
    			}
    		}
        }
        private System.Guid _createdById;
    
    
    	[DataMember]
        public virtual System.Guid DivisionId
        {
            get {return _divisionId;}
            set 
    		{ 
    			if(_divisionId != value)
    			{
    				_divisionId = value; 
    				
    				
    
    				OnPropertyChanged("DivisionId");
    			}
    		}
        }
        private System.Guid _divisionId;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> StorehouseId
        {
            get {return _storehouseId;}
            set 
    		{ 
    			if(_storehouseId != value)
    			{
    				_storehouseId = value; 
    				
    				
    
    				OnPropertyChanged("StorehouseId");
    			}
    		}
        }
        private Nullable<System.Guid> _storehouseId;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual Customer Customer
        {
            get { return _customer; }
            set
            {
                if (!ReferenceEquals(_customer, value))
                {
                    var previousValue = _customer;
                    _customer = value;
                    FixupCustomer(previousValue);
                }
            }
        }
        private Customer _customer;
    
        public virtual Good Good
        {
            get { return _good; }
            set
            {
                if (!ReferenceEquals(_good, value))
                {
                    var previousValue = _good;
                    _good = value;
                    FixupGood(previousValue);
                }
            }
        }
        private Good _good;

        #endregion

        #region Association Fixup
    
        private void FixupCustomer(Customer previousValue)
        {
            if (previousValue != null && previousValue.CustomerGoodsFlows.Contains(this))
            {
                previousValue.CustomerGoodsFlows.Remove(this);
            }
    
            if (Customer != null)
            {
                if (!Customer.CustomerGoodsFlows.Contains(this))
                {
                    Customer.CustomerGoodsFlows.Add(this);
                }
                if (CustomerId != Customer.Id)
                {
                    CustomerId = Customer.Id;
                }
            }
        }
    
        private void FixupGood(Good previousValue)
        {
            if (previousValue != null && previousValue.CustomerGoodsFlows.Contains(this))
            {
                previousValue.CustomerGoodsFlows.Remove(this);
            }
    
            if (Good != null)
            {
                if (!Good.CustomerGoodsFlows.Contains(this))
                {
                    Good.CustomerGoodsFlows.Add(this);
                }
                if (GoodId != Good.Id)
                {
                    GoodId = Good.Id;
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