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
    public partial class GoodPrice : INotifyPropertyChanged
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
        public virtual decimal CommonPrice
        {
            get {return _commonPrice;}
            set 
    		{ 
    			if(_commonPrice != value)
    			{
    				_commonPrice = value; 
    				
    				
    
    				OnPropertyChanged("CommonPrice");
    			}
    		}
        }
        private decimal _commonPrice;
    
    
    	[DataMember]
        public virtual Nullable<decimal> EmployeePrice
        {
            get {return _employeePrice;}
            set 
    		{ 
    			if(_employeePrice != value)
    			{
    				_employeePrice = value; 
    				
    				
    
    				OnPropertyChanged("EmployeePrice");
    			}
    		}
        }
        private Nullable<decimal> _employeePrice;
    
    
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
        public virtual bool InPricelist
        {
            get {return _inPricelist;}
            set 
    		{ 
    			if(_inPricelist != value)
    			{
    				_inPricelist = value; 
    				
    				
    
    				OnPropertyChanged("InPricelist");
    			}
    		}
        }
        private bool _inPricelist;
    
    
    	[DataMember]
        public virtual Nullable<decimal> BonusPrice
        {
            get {return _bonusPrice;}
            set 
    		{ 
    			if(_bonusPrice != value)
    			{
    				_bonusPrice = value; 
    				
    				
    
    				OnPropertyChanged("BonusPrice");
    			}
    		}
        }
        private Nullable<decimal> _bonusPrice;
    
    
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
        public virtual Nullable<decimal> RentPrice
        {
            get {return _rentPrice;}
            set 
    		{ 
    			if(_rentPrice != value)
    			{
    				_rentPrice = value; 
    				
    				
    
    				OnPropertyChanged("RentPrice");
    			}
    		}
        }
        private Nullable<decimal> _rentPrice;
    
    
    	[DataMember]
        public virtual Nullable<decimal> RentFine
        {
            get {return _rentFine;}
            set 
    		{ 
    			if(_rentFine != value)
    			{
    				_rentFine = value; 
    				
    				
    
    				OnPropertyChanged("RentFine");
    			}
    		}
        }
        private Nullable<decimal> _rentFine;
    
    

        #endregion

        #region Navigation Properties
    
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

        #endregion

        #region Association Fixup
    
        private void FixupDivision(Division previousValue)
        {
            if (previousValue != null && previousValue.GoodPrices.Contains(this))
            {
                previousValue.GoodPrices.Remove(this);
            }
    
            if (Division != null)
            {
                if (!Division.GoodPrices.Contains(this))
                {
                    Division.GoodPrices.Add(this);
                }
                if (DivisionId != Division.Id)
                {
                    DivisionId = Division.Id;
                }
            }
        }
    
        private void FixupGood(Good previousValue)
        {
            if (previousValue != null && previousValue.GoodPrices.Contains(this))
            {
                previousValue.GoodPrices.Remove(this);
            }
    
            if (Good != null)
            {
                if (!Good.GoodPrices.Contains(this))
                {
                    Good.GoodPrices.Add(this);
                }
                if (GoodId != Good.Id)
                {
                    GoodId = Good.Id;
                }
            }
        }
    
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
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.GoodPrices.Contains(this))
            {
                previousValue.GoodPrices.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.GoodPrices.Contains(this))
                {
                    Company.GoodPrices.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
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
