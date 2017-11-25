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
    public partial class EmployeeTrip : INotifyPropertyChanged
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
        public virtual System.DateTime BeginDate
        {
            get {return _beginDate;}
            set 
    		{ 
    			if(_beginDate != value)
    			{
    				_beginDate = value; 
    				
    				_beginDate = DateTime.SpecifyKind(_beginDate, DateTimeKind.Local);
    
    				OnPropertyChanged("BeginDate");
    			}
    		}
        }
        private System.DateTime _beginDate;
    
    
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
        public virtual string Destination
        {
            get {return _destination;}
            set 
    		{ 
    			if(_destination != value)
    			{
    				_destination = value; 
    				
    				
    
    				OnPropertyChanged("Destination");
    			}
    		}
        }
        private string _destination;
    
    
    	[DataMember]
        public virtual string Base
        {
            get {return _base;}
            set 
    		{ 
    			if(_base != value)
    			{
    				_base = value; 
    				
    				
    
    				OnPropertyChanged("Base");
    			}
    		}
        }
        private string _base;
    
    
    	[DataMember]
        public virtual string Target
        {
            get {return _target;}
            set 
    		{ 
    			if(_target != value)
    			{
    				_target = value; 
    				
    				
    
    				OnPropertyChanged("Target");
    			}
    		}
        }
        private string _target;
    
    
    	[DataMember]
        public virtual System.Guid DocumentId
        {
            get { return _documentId; }
            set
            {
                if (_documentId != value)
                {
                    if (EmployeeDocument != null && EmployeeDocument.Id != value)
                    {
                        EmployeeDocument = null;
                    }
                    _documentId = value;
    
    				OnPropertyChanged("DocumentId");
                }
            }
        }
        private System.Guid _documentId;
    
    

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
    
        public virtual EmployeeDocument EmployeeDocument
        {
            get { return _employeeDocument; }
            set
            {
                if (!ReferenceEquals(_employeeDocument, value))
                {
                    var previousValue = _employeeDocument;
                    _employeeDocument = value;
                    FixupEmployeeDocument(previousValue);
                }
            }
        }
        private EmployeeDocument _employeeDocument;

        #endregion

        #region Association Fixup
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.EmployeeTrips.Contains(this))
            {
                previousValue.EmployeeTrips.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.EmployeeTrips.Contains(this))
                {
                    Company.EmployeeTrips.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupEmployee(Employee previousValue)
        {
            if (previousValue != null && previousValue.EmployeeTrips.Contains(this))
            {
                previousValue.EmployeeTrips.Remove(this);
            }
    
            if (Employee != null)
            {
                if (!Employee.EmployeeTrips.Contains(this))
                {
                    Employee.EmployeeTrips.Add(this);
                }
                if (EmployeeId != Employee.Id)
                {
                    EmployeeId = Employee.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.EmployeeTrips.Contains(this))
            {
                previousValue.EmployeeTrips.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.EmployeeTrips.Contains(this))
                {
                    CreatedBy.EmployeeTrips.Add(this);
                }
                if (AuthorId != CreatedBy.UserId)
                {
                    AuthorId = CreatedBy.UserId;
                }
            }
        }
    
        private void FixupEmployeeDocument(EmployeeDocument previousValue)
        {
            if (previousValue != null && previousValue.EmployeeTrips.Contains(this))
            {
                previousValue.EmployeeTrips.Remove(this);
            }
    
            if (EmployeeDocument != null)
            {
                if (!EmployeeDocument.EmployeeTrips.Contains(this))
                {
                    EmployeeDocument.EmployeeTrips.Add(this);
                }
                if (DocumentId != EmployeeDocument.Id)
                {
                    DocumentId = EmployeeDocument.Id;
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
