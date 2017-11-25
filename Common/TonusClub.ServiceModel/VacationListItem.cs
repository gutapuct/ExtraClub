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
    public partial class VacationListItem : INotifyPropertyChanged
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
        public virtual System.Guid VacationListId
        {
            get { return _vacationListId; }
            set
            {
                if (_vacationListId != value)
                {
                    if (VacationList != null && VacationList.Id != value)
                    {
                        VacationList = null;
                    }
                    _vacationListId = value;
    
    				OnPropertyChanged("VacationListId");
                }
            }
        }
        private System.Guid _vacationListId;
    
    
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
        public virtual System.DateTime FinishDate
        {
            get {return _finishDate;}
            set 
    		{ 
    			if(_finishDate != value)
    			{
    				_finishDate = value; 
    				
    				_finishDate = DateTime.SpecifyKind(_finishDate, DateTimeKind.Local);
    
    				OnPropertyChanged("FinishDate");
    			}
    		}
        }
        private System.DateTime _finishDate;
    
    

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
    
        public virtual VacationList VacationList
        {
            get { return _vacationList; }
            set
            {
                if (!ReferenceEquals(_vacationList, value))
                {
                    var previousValue = _vacationList;
                    _vacationList = value;
                    FixupVacationList(previousValue);
                }
            }
        }
        private VacationList _vacationList;

        #endregion

        #region Association Fixup
    
        private void FixupEmployee(Employee previousValue)
        {
            if (previousValue != null && previousValue.VacationListItems.Contains(this))
            {
                previousValue.VacationListItems.Remove(this);
            }
    
            if (Employee != null)
            {
                if (!Employee.VacationListItems.Contains(this))
                {
                    Employee.VacationListItems.Add(this);
                }
                if (EmployeeId != Employee.Id)
                {
                    EmployeeId = Employee.Id;
                }
            }
        }
    
        private void FixupVacationList(VacationList previousValue)
        {
            if (previousValue != null && previousValue.VacationListItems.Contains(this))
            {
                previousValue.VacationListItems.Remove(this);
            }
    
            if (VacationList != null)
            {
                if (!VacationList.VacationListItems.Contains(this))
                {
                    VacationList.VacationListItems.Add(this);
                }
                if (VacationListId != VacationList.Id)
                {
                    VacationListId = VacationList.Id;
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
