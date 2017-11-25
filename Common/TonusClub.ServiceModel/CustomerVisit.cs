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
    public partial class CustomerVisit : INotifyPropertyChanged
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
        public virtual System.DateTime InTime
        {
            get {return _inTime;}
            set 
    		{ 
    			if(_inTime != value)
    			{
    				_inTime = value; 
    				
    				_inTime = DateTime.SpecifyKind(_inTime, DateTimeKind.Local);
    
    				OnPropertyChanged("InTime");
    			}
    		}
        }
        private System.DateTime _inTime;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> OutTime
        {
            get {return _outTime;}
            set 
    		{ 
    			if(_outTime != value)
    			{
    				_outTime = value; 
    				
    				if (_outTime.HasValue) _outTime = DateTime.SpecifyKind(_outTime.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("OutTime");
    			}
    		}
        }
        private Nullable<System.DateTime> _outTime;
    
    
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
        public virtual string Receipt
        {
            get {return _receipt;}
            set 
    		{ 
    			if(_receipt != value)
    			{
    				_receipt = value; 
    				
    				
    
    				OnPropertyChanged("Receipt");
    			}
    		}
        }
        private string _receipt;
    
    

        #endregion

        #region Navigation Properties
    
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
    
        private void FixupCustomer(Customer previousValue)
        {
            if (previousValue != null && previousValue.CustomerVisits.Contains(this))
            {
                previousValue.CustomerVisits.Remove(this);
            }
    
            if (Customer != null)
            {
                if (!Customer.CustomerVisits.Contains(this))
                {
                    Customer.CustomerVisits.Add(this);
                }
                if (CustomerId != Customer.Id)
                {
                    CustomerId = Customer.Id;
                }
            }
        }
    
        private void FixupDivision(Division previousValue)
        {
            if (previousValue != null && previousValue.CustomerVisits.Contains(this))
            {
                previousValue.CustomerVisits.Remove(this);
            }
    
            if (Division != null)
            {
                if (!Division.CustomerVisits.Contains(this))
                {
                    Division.CustomerVisits.Add(this);
                }
                if (DivisionId != Division.Id)
                {
                    DivisionId = Division.Id;
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