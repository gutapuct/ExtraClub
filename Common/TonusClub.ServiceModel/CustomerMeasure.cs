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
    public partial class CustomerMeasure : INotifyPropertyChanged
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
        public virtual System.DateTime Date
        {
            get {return _date;}
            set 
    		{ 
    			if(_date != value)
    			{
    				_date = value; 
    				
    				_date = DateTime.SpecifyKind(_date, DateTimeKind.Local);
    
    				OnPropertyChanged("Date");
    			}
    		}
        }
        private System.DateTime _date;
    
    
    	[DataMember]
        public virtual string LoadType
        {
            get {return _loadType;}
            set 
    		{ 
    			if(_loadType != value)
    			{
    				_loadType = value; 
    				
    				
    
    				OnPropertyChanged("LoadType");
    			}
    		}
        }
        private string _loadType;
    
    
    	[DataMember]
        public virtual string AD0Up
        {
            get {return _aD0Up;}
            set 
    		{ 
    			if(_aD0Up != value)
    			{
    				_aD0Up = value; 
    				
    				
    
    				OnPropertyChanged("AD0Up");
    			}
    		}
        }
        private string _aD0Up;
    
    
    	[DataMember]
        public virtual string AD0Down
        {
            get {return _aD0Down;}
            set 
    		{ 
    			if(_aD0Down != value)
    			{
    				_aD0Down = value; 
    				
    				
    
    				OnPropertyChanged("AD0Down");
    			}
    		}
        }
        private string _aD0Down;
    
    
    	[DataMember]
        public virtual string PS0
        {
            get {return _pS0;}
            set 
    		{ 
    			if(_pS0 != value)
    			{
    				_pS0 = value; 
    				
    				
    
    				OnPropertyChanged("PS0");
    			}
    		}
        }
        private string _pS0;
    
    
    	[DataMember]
        public virtual string AD1Up
        {
            get {return _aD1Up;}
            set 
    		{ 
    			if(_aD1Up != value)
    			{
    				_aD1Up = value; 
    				
    				
    
    				OnPropertyChanged("AD1Up");
    			}
    		}
        }
        private string _aD1Up;
    
    
    	[DataMember]
        public virtual string AD1Down
        {
            get {return _aD1Down;}
            set 
    		{ 
    			if(_aD1Down != value)
    			{
    				_aD1Down = value; 
    				
    				
    
    				OnPropertyChanged("AD1Down");
    			}
    		}
        }
        private string _aD1Down;
    
    
    	[DataMember]
        public virtual string PS1
        {
            get {return _pS1;}
            set 
    		{ 
    			if(_pS1 != value)
    			{
    				_pS1 = value; 
    				
    				
    
    				OnPropertyChanged("PS1");
    			}
    		}
        }
        private string _pS1;
    
    
    	[DataMember]
        public virtual string AD2Up
        {
            get {return _aD2Up;}
            set 
    		{ 
    			if(_aD2Up != value)
    			{
    				_aD2Up = value; 
    				
    				
    
    				OnPropertyChanged("AD2Up");
    			}
    		}
        }
        private string _aD2Up;
    
    
    	[DataMember]
        public virtual string AD2Down
        {
            get {return _aD2Down;}
            set 
    		{ 
    			if(_aD2Down != value)
    			{
    				_aD2Down = value; 
    				
    				
    
    				OnPropertyChanged("AD2Down");
    			}
    		}
        }
        private string _aD2Down;
    
    
    	[DataMember]
        public virtual string PS2
        {
            get {return _pS2;}
            set 
    		{ 
    			if(_pS2 != value)
    			{
    				_pS2 = value; 
    				
    				
    
    				OnPropertyChanged("PS2");
    			}
    		}
        }
        private string _pS2;
    
    
    	[DataMember]
        public virtual string AD3Up
        {
            get {return _aD3Up;}
            set 
    		{ 
    			if(_aD3Up != value)
    			{
    				_aD3Up = value; 
    				
    				
    
    				OnPropertyChanged("AD3Up");
    			}
    		}
        }
        private string _aD3Up;
    
    
    	[DataMember]
        public virtual string AD3Down
        {
            get {return _aD3Down;}
            set 
    		{ 
    			if(_aD3Down != value)
    			{
    				_aD3Down = value; 
    				
    				
    
    				OnPropertyChanged("AD3Down");
    			}
    		}
        }
        private string _aD3Down;
    
    
    	[DataMember]
        public virtual string PS3
        {
            get {return _pS3;}
            set 
    		{ 
    			if(_pS3 != value)
    			{
    				_pS3 = value; 
    				
    				
    
    				OnPropertyChanged("PS3");
    			}
    		}
        }
        private string _pS3;
    
    
    	[DataMember]
        public virtual string Conclusion
        {
            get {return _conclusion;}
            set 
    		{ 
    			if(_conclusion != value)
    			{
    				_conclusion = value; 
    				
    				
    
    				OnPropertyChanged("Conclusion");
    			}
    		}
        }
        private string _conclusion;
    
    

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

        #endregion

        #region Association Fixup
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.CustomerMeasures.Contains(this))
            {
                previousValue.CustomerMeasures.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.CustomerMeasures.Contains(this))
                {
                    Company.CustomerMeasures.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupCustomer(Customer previousValue)
        {
            if (previousValue != null && previousValue.CustomerMeasures.Contains(this))
            {
                previousValue.CustomerMeasures.Remove(this);
            }
    
            if (Customer != null)
            {
                if (!Customer.CustomerMeasures.Contains(this))
                {
                    Customer.CustomerMeasures.Add(this);
                }
                if (CustomerId != Customer.Id)
                {
                    CustomerId = Customer.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.CustomerMeasures.Contains(this))
            {
                previousValue.CustomerMeasures.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.CustomerMeasures.Contains(this))
                {
                    CreatedBy.CustomerMeasures.Add(this);
                }
                if (AuthorId != CreatedBy.UserId)
                {
                    AuthorId = CreatedBy.UserId;
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
