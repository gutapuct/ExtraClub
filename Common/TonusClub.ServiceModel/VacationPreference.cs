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
    public partial class VacationPreference : INotifyPropertyChanged
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
        public virtual System.Guid EmployeeId
        {
            get { return _employeeId; }
            set
            {
                if (_employeeId != value)
                {
                    if (Employee != null && Employee.Id != value)
                    {
                        Employee = null;
                    }
                    _employeeId = value;
    
    				OnPropertyChanged("EmployeeId");
                }
            }
        }
        private System.Guid _employeeId;
    
    
    	[DataMember]
        public virtual System.DateTime StartDate
        {
            get {return _startDate;}
            set 
    		{ 
    			if(_startDate != value)
    			{
    				_startDate = value; 
    				
    				_startDate = DateTime.SpecifyKind(_startDate, DateTimeKind.Local);
    
    				OnPropertyChanged("StartDate");
    			}
    		}
        }
        private System.DateTime _startDate;
    
    
    	[DataMember]
        public virtual System.DateTime EndDate
        {
            get {return _endDate;}
            set 
    		{ 
    			if(_endDate != value)
    			{
    				_endDate = value; 
    				
    				_endDate = DateTime.SpecifyKind(_endDate, DateTimeKind.Local);
    
    				OnPropertyChanged("EndDate");
    			}
    		}
        }
        private System.DateTime _endDate;
    
    
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
        public virtual short PrefType
        {
            get {return _prefType;}
            set 
    		{ 
    			if(_prefType != value)
    			{
    				_prefType = value; 
    				
    				
    
    				OnPropertyChanged("PrefType");
    			}
    		}
        }
        private short _prefType;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual Employee Employee
        {
            get { return _employee; }
            set
            {
                if (!ReferenceEquals(_employee, value))
                {
                    var previousValue = _employee;
                    _employee = value;
                    FixupEmployee(previousValue);
                }
            }
        }
        private Employee _employee;

        #endregion

        #region Association Fixup
    
        private void FixupEmployee(Employee previousValue)
        {
            if (previousValue != null && previousValue.VacationPreferences.Contains(this))
            {
                previousValue.VacationPreferences.Remove(this);
            }
    
            if (Employee != null)
            {
                if (!Employee.VacationPreferences.Contains(this))
                {
                    Employee.VacationPreferences.Add(this);
                }
                if (EmployeeId != Employee.Id)
                {
                    EmployeeId = Employee.Id;
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
