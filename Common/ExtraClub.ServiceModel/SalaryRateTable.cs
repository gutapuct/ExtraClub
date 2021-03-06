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
    public partial class SalaryRateTable : INotifyPropertyChanged
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
        public virtual System.Guid SalarySchemeCoefficientId
        {
            get { return _salarySchemeCoefficientId; }
            set
            {
                if (_salarySchemeCoefficientId != value)
                {
                    if (SalarySchemeCoefficient != null && SalarySchemeCoefficient.Id != value)
                    {
                        SalarySchemeCoefficient = null;
                    }
                    _salarySchemeCoefficientId = value;
    
    				OnPropertyChanged("SalarySchemeCoefficientId");
                }
            }
        }
        private System.Guid _salarySchemeCoefficientId;
    
    
    	[DataMember]
        public virtual Nullable<decimal> FromValue
        {
            get {return _fromValue;}
            set 
    		{ 
    			if(_fromValue != value)
    			{
    				_fromValue = value; 
    				
    				
    
    				OnPropertyChanged("FromValue");
    			}
    		}
        }
        private Nullable<decimal> _fromValue;
    
    
    	[DataMember]
        public virtual Nullable<decimal> ToValue
        {
            get {return _toValue;}
            set 
    		{ 
    			if(_toValue != value)
    			{
    				_toValue = value; 
    				
    				
    
    				OnPropertyChanged("ToValue");
    			}
    		}
        }
        private Nullable<decimal> _toValue;
    
    
    	[DataMember]
        public virtual decimal Result
        {
            get {return _result;}
            set 
    		{ 
    			if(_result != value)
    			{
    				_result = value; 
    				
    				
    
    				OnPropertyChanged("Result");
    			}
    		}
        }
        private decimal _result;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual SalarySchemeCoefficient SalarySchemeCoefficient
        {
            get { return _salarySchemeCoefficient; }
            set
            {
                if (!ReferenceEquals(_salarySchemeCoefficient, value))
                {
                    var previousValue = _salarySchemeCoefficient;
                    _salarySchemeCoefficient = value;
                    FixupSalarySchemeCoefficient(previousValue);
                }
            }
        }
        private SalarySchemeCoefficient _salarySchemeCoefficient;

        #endregion

        #region Association Fixup
    
        private void FixupSalarySchemeCoefficient(SalarySchemeCoefficient previousValue)
        {
            if (previousValue != null && previousValue.SalaryRateTables.Contains(this))
            {
                previousValue.SalaryRateTables.Remove(this);
            }
    
            if (SalarySchemeCoefficient != null)
            {
                if (!SalarySchemeCoefficient.SalaryRateTables.Contains(this))
                {
                    SalarySchemeCoefficient.SalaryRateTables.Add(this);
                }
                if (SalarySchemeCoefficientId != SalarySchemeCoefficient.Id)
                {
                    SalarySchemeCoefficientId = SalarySchemeCoefficient.Id;
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
