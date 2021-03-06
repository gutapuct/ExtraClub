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
    public partial class Spending : INotifyPropertyChanged
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
        public virtual Nullable<System.Guid> DivisionId
        {
            get { return _divisionId; }
            set
            {
                try
                {
                    _settingFK = true;
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
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _divisionId;
    
    
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
                try
                {
                    _settingFK = true;
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
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _authorId;
    
    
    	[DataMember]
        public virtual int Number
        {
            get {return _number;}
            set 
    		{ 
    			if(_number != value)
    			{
    				_number = value; 
    				
    				
    
    				OnPropertyChanged("Number");
    			}
    		}
        }
        private int _number;
    
    
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
        public virtual System.Guid SpendingTypeId
        {
            get { return _spendingTypeId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_spendingTypeId != value)
                    {
                        if (SpendingType != null && SpendingType.Id != value)
                        {
                            SpendingType = null;
                        }
                        _spendingTypeId = value;
        
        				OnPropertyChanged("SpendingTypeId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _spendingTypeId;
    
    
    	[DataMember]
        public virtual decimal Amount
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
        private decimal _amount;
    
    
    	[DataMember]
        public virtual string PaymentType
        {
            get {return _paymentType;}
            set 
    		{ 
    			if(_paymentType != value)
    			{
    				_paymentType = value; 
    				
    				
    
    				OnPropertyChanged("PaymentType");
    			}
    		}
        }
        private string _paymentType;
    
    
    	[DataMember]
        public virtual bool IsInvestment
        {
            get {return _isInvestment;}
            set 
    		{ 
    			if(_isInvestment != value)
    			{
    				_isInvestment = value; 
    				
    				
    
    				OnPropertyChanged("IsInvestment");
    			}
    		}
        }
        private bool _isInvestment;
    
    
    	[DataMember]
        public virtual bool IsFinAction
        {
            get {return _isFinAction;}
            set 
    		{ 
    			if(_isFinAction != value)
    			{
    				_isFinAction = value; 
    				
    				
    
    				OnPropertyChanged("IsFinAction");
    			}
    		}
        }
        private bool _isFinAction;
    
    

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
    
        public virtual SpendingType SpendingType
        {
            get { return _spendingType; }
            set
            {
                if (!ReferenceEquals(_spendingType, value))
                {
                    var previousValue = _spendingType;
                    _spendingType = value;
                    FixupSpendingType(previousValue);
                }
            }
        }
        private SpendingType _spendingType;
    
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
    
        private bool _settingFK = false;
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.Spendings.Contains(this))
            {
                previousValue.Spendings.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Spendings.Contains(this))
                {
                    Company.Spendings.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupDivision(Division previousValue)
        {
            if (previousValue != null && previousValue.Spendings.Contains(this))
            {
                previousValue.Spendings.Remove(this);
            }
    
            if (Division != null)
            {
                if (!Division.Spendings.Contains(this))
                {
                    Division.Spendings.Add(this);
                }
                if (DivisionId != Division.Id)
                {
                    DivisionId = Division.Id;
                }
            }
            else if (!_settingFK)
            {
                DivisionId = null;
            }
        }
    
        private void FixupSpendingType(SpendingType previousValue)
        {
            if (previousValue != null && previousValue.Spendings.Contains(this))
            {
                previousValue.Spendings.Remove(this);
            }
    
            if (SpendingType != null)
            {
                if (!SpendingType.Spendings.Contains(this))
                {
                    SpendingType.Spendings.Add(this);
                }
                if (SpendingTypeId != SpendingType.Id)
                {
                    SpendingTypeId = SpendingType.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.Spendings.Contains(this))
            {
                previousValue.Spendings.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.Spendings.Contains(this))
                {
                    CreatedBy.Spendings.Add(this);
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
