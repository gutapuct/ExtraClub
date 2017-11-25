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
    public partial class Anthropometric : INotifyPropertyChanged
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
        public virtual string Height
        {
            get {return _height;}
            set 
    		{ 
    			if(_height != value)
    			{
    				_height = value; 
    				
    				
    
    				OnPropertyChanged("Height");
    			}
    		}
        }
        private string _height;
    
    
    	[DataMember]
        public virtual string Weight
        {
            get {return _weight;}
            set 
    		{ 
    			if(_weight != value)
    			{
    				_weight = value; 
    				
    				
    
    				OnPropertyChanged("Weight");
    			}
    		}
        }
        private string _weight;
    
    
    	[DataMember]
        public virtual string PSPulse
        {
            get {return _pSPulse;}
            set 
    		{ 
    			if(_pSPulse != value)
    			{
    				_pSPulse = value; 
    				
    				
    
    				OnPropertyChanged("PSPulse");
    			}
    		}
        }
        private string _pSPulse;
    
    
    	[DataMember]
        public virtual string ADUp
        {
            get {return _aDUp;}
            set 
    		{ 
    			if(_aDUp != value)
    			{
    				_aDUp = value; 
    				
    				
    
    				OnPropertyChanged("ADUp");
    			}
    		}
        }
        private string _aDUp;
    
    
    	[DataMember]
        public virtual string ADDown
        {
            get {return _aDDown;}
            set 
    		{ 
    			if(_aDDown != value)
    			{
    				_aDDown = value; 
    				
    				
    
    				OnPropertyChanged("ADDown");
    			}
    		}
        }
        private string _aDDown;
    
    
    	[DataMember]
        public virtual string Neck
        {
            get {return _neck;}
            set 
    		{ 
    			if(_neck != value)
    			{
    				_neck = value; 
    				
    				
    
    				OnPropertyChanged("Neck");
    			}
    		}
        }
        private string _neck;
    
    
    	[DataMember]
        public virtual string ChestIn
        {
            get {return _chestIn;}
            set 
    		{ 
    			if(_chestIn != value)
    			{
    				_chestIn = value; 
    				
    				
    
    				OnPropertyChanged("ChestIn");
    			}
    		}
        }
        private string _chestIn;
    
    
    	[DataMember]
        public virtual string ChestOut
        {
            get {return _chestOut;}
            set 
    		{ 
    			if(_chestOut != value)
    			{
    				_chestOut = value; 
    				
    				
    
    				OnPropertyChanged("ChestOut");
    			}
    		}
        }
        private string _chestOut;
    
    
    	[DataMember]
        public virtual string Shoulders
        {
            get {return _shoulders;}
            set 
    		{ 
    			if(_shoulders != value)
    			{
    				_shoulders = value; 
    				
    				
    
    				OnPropertyChanged("Shoulders");
    			}
    		}
        }
        private string _shoulders;
    
    
    	[DataMember]
        public virtual string RightRelax
        {
            get {return _rightRelax;}
            set 
    		{ 
    			if(_rightRelax != value)
    			{
    				_rightRelax = value; 
    				
    				
    
    				OnPropertyChanged("RightRelax");
    			}
    		}
        }
        private string _rightRelax;
    
    
    	[DataMember]
        public virtual string RightTense
        {
            get {return _rightTense;}
            set 
    		{ 
    			if(_rightTense != value)
    			{
    				_rightTense = value; 
    				
    				
    
    				OnPropertyChanged("RightTense");
    			}
    		}
        }
        private string _rightTense;
    
    
    	[DataMember]
        public virtual string LeftRelax
        {
            get {return _leftRelax;}
            set 
    		{ 
    			if(_leftRelax != value)
    			{
    				_leftRelax = value; 
    				
    				
    
    				OnPropertyChanged("LeftRelax");
    			}
    		}
        }
        private string _leftRelax;
    
    
    	[DataMember]
        public virtual string LeftTense
        {
            get {return _leftTense;}
            set 
    		{ 
    			if(_leftTense != value)
    			{
    				_leftTense = value; 
    				
    				
    
    				OnPropertyChanged("LeftTense");
    			}
    		}
        }
        private string _leftTense;
    
    
    	[DataMember]
        public virtual string ForearmRight
        {
            get {return _forearmRight;}
            set 
    		{ 
    			if(_forearmRight != value)
    			{
    				_forearmRight = value; 
    				
    				
    
    				OnPropertyChanged("ForearmRight");
    			}
    		}
        }
        private string _forearmRight;
    
    
    	[DataMember]
        public virtual string ForearmLeft
        {
            get {return _forearmLeft;}
            set 
    		{ 
    			if(_forearmLeft != value)
    			{
    				_forearmLeft = value; 
    				
    				
    
    				OnPropertyChanged("ForearmLeft");
    			}
    		}
        }
        private string _forearmLeft;
    
    
    	[DataMember]
        public virtual string Waist
        {
            get {return _waist;}
            set 
    		{ 
    			if(_waist != value)
    			{
    				_waist = value; 
    				
    				
    
    				OnPropertyChanged("Waist");
    			}
    		}
        }
        private string _waist;
    
    
    	[DataMember]
        public virtual string Stomach
        {
            get {return _stomach;}
            set 
    		{ 
    			if(_stomach != value)
    			{
    				_stomach = value; 
    				
    				
    
    				OnPropertyChanged("Stomach");
    			}
    		}
        }
        private string _stomach;
    
    
    	[DataMember]
        public virtual string Leg
        {
            get {return _leg;}
            set 
    		{ 
    			if(_leg != value)
    			{
    				_leg = value; 
    				
    				
    
    				OnPropertyChanged("Leg");
    			}
    		}
        }
        private string _leg;
    
    
    	[DataMember]
        public virtual string Buttocks
        {
            get {return _buttocks;}
            set 
    		{ 
    			if(_buttocks != value)
    			{
    				_buttocks = value; 
    				
    				
    
    				OnPropertyChanged("Buttocks");
    			}
    		}
        }
        private string _buttocks;
    
    
    	[DataMember]
        public virtual string LegRight
        {
            get {return _legRight;}
            set 
    		{ 
    			if(_legRight != value)
    			{
    				_legRight = value; 
    				
    				
    
    				OnPropertyChanged("LegRight");
    			}
    		}
        }
        private string _legRight;
    
    
    	[DataMember]
        public virtual string LegLeft
        {
            get {return _legLeft;}
            set 
    		{ 
    			if(_legLeft != value)
    			{
    				_legLeft = value; 
    				
    				
    
    				OnPropertyChanged("LegLeft");
    			}
    		}
        }
        private string _legLeft;
    
    
    	[DataMember]
        public virtual string ShinRight
        {
            get {return _shinRight;}
            set 
    		{ 
    			if(_shinRight != value)
    			{
    				_shinRight = value; 
    				
    				
    
    				OnPropertyChanged("ShinRight");
    			}
    		}
        }
        private string _shinRight;
    
    
    	[DataMember]
        public virtual string ShinLeft
        {
            get {return _shinLeft;}
            set 
    		{ 
    			if(_shinLeft != value)
    			{
    				_shinLeft = value; 
    				
    				
    
    				OnPropertyChanged("ShinLeft");
    			}
    		}
        }
        private string _shinLeft;
    
    
    	[DataMember]
        public virtual string Fat
        {
            get {return _fat;}
            set 
    		{ 
    			if(_fat != value)
    			{
    				_fat = value; 
    				
    				
    
    				OnPropertyChanged("Fat");
    			}
    		}
        }
        private string _fat;
    
    
    	[DataMember]
        public virtual string InternalMass
        {
            get {return _internalMass;}
            set 
    		{ 
    			if(_internalMass != value)
    			{
    				_internalMass = value; 
    				
    				
    
    				OnPropertyChanged("InternalMass");
    			}
    		}
        }
        private string _internalMass;
    
    
    	[DataMember]
        public virtual string MusculeMass
        {
            get {return _musculeMass;}
            set 
    		{ 
    			if(_musculeMass != value)
    			{
    				_musculeMass = value; 
    				
    				
    
    				OnPropertyChanged("MusculeMass");
    			}
    		}
        }
        private string _musculeMass;
    
    
    	[DataMember]
        public virtual string Water
        {
            get {return _water;}
            set 
    		{ 
    			if(_water != value)
    			{
    				_water = value; 
    				
    				
    
    				OnPropertyChanged("Water");
    			}
    		}
        }
        private string _water;
    
    
    	[DataMember]
        public virtual string BonesMass
        {
            get {return _bonesMass;}
            set 
    		{ 
    			if(_bonesMass != value)
    			{
    				_bonesMass = value; 
    				
    				
    
    				OnPropertyChanged("BonesMass");
    			}
    		}
        }
        private string _bonesMass;
    
    
    	[DataMember]
        public virtual string KkalBurn
        {
            get {return _kkalBurn;}
            set 
    		{ 
    			if(_kkalBurn != value)
    			{
    				_kkalBurn = value; 
    				
    				
    
    				OnPropertyChanged("KkalBurn");
    			}
    		}
        }
        private string _kkalBurn;
    
    
    	[DataMember]
        public virtual string MetaAge
        {
            get {return _metaAge;}
            set 
    		{ 
    			if(_metaAge != value)
    			{
    				_metaAge = value; 
    				
    				
    
    				OnPropertyChanged("MetaAge");
    			}
    		}
        }
        private string _metaAge;
    
    
    	[DataMember]
        public virtual string VascFat
        {
            get {return _vascFat;}
            set 
    		{ 
    			if(_vascFat != value)
    			{
    				_vascFat = value; 
    				
    				
    
    				OnPropertyChanged("VascFat");
    			}
    		}
        }
        private string _vascFat;
    
    
    	[DataMember]
        public virtual string MassIndex
        {
            get {return _massIndex;}
            set 
    		{ 
    			if(_massIndex != value)
    			{
    				_massIndex = value; 
    				
    				
    
    				OnPropertyChanged("MassIndex");
    			}
    		}
        }
        private string _massIndex;
    
    
    	[DataMember]
        public virtual string IdealWeight
        {
            get {return _idealWeight;}
            set 
    		{ 
    			if(_idealWeight != value)
    			{
    				_idealWeight = value; 
    				
    				
    
    				OnPropertyChanged("IdealWeight");
    			}
    		}
        }
        private string _idealWeight;
    
    
    	[DataMember]
        public virtual string FatStage
        {
            get {return _fatStage;}
            set 
    		{ 
    			if(_fatStage != value)
    			{
    				_fatStage = value; 
    				
    				
    
    				OnPropertyChanged("FatStage");
    			}
    		}
        }
        private string _fatStage;
    
    

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
            if (previousValue != null && previousValue.Anthropometrics.Contains(this))
            {
                previousValue.Anthropometrics.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Anthropometrics.Contains(this))
                {
                    Company.Anthropometrics.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupCustomer(Customer previousValue)
        {
            if (previousValue != null && previousValue.Anthropometrics.Contains(this))
            {
                previousValue.Anthropometrics.Remove(this);
            }
    
            if (Customer != null)
            {
                if (!Customer.Anthropometrics.Contains(this))
                {
                    Customer.Anthropometrics.Add(this);
                }
                if (CustomerId != Customer.Id)
                {
                    CustomerId = Customer.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.Anthropometrics.Contains(this))
            {
                previousValue.Anthropometrics.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.Anthropometrics.Contains(this))
                {
                    CreatedBy.Anthropometrics.Add(this);
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