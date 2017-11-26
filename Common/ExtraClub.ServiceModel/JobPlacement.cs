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
    public partial class JobPlacement : INotifyPropertyChanged
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
        public virtual System.Guid EmployeeId
        {
            get { return _employeeId; }
            set
            {
                try
                {
                    _settingFK = true;
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
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _employeeId;
    
    
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
        public virtual System.Guid JobId
        {
            get { return _jobId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_jobId != value)
                    {
                        if (Job != null && Job.Id != value)
                        {
                            Job = null;
                        }
                        _jobId = value;
        
        				OnPropertyChanged("JobId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _jobId;
    
    
    	[DataMember]
        public virtual System.Guid CategoryId
        {
            get { return _categoryId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_categoryId != value)
                    {
                        if (EmployeeCategory != null && EmployeeCategory.Id != value)
                        {
                            EmployeeCategory = null;
                        }
                        _categoryId = value;
        
        				OnPropertyChanged("CategoryId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _categoryId;
    
    
    	[DataMember]
        public virtual System.DateTime ApplyDate
        {
            get {return _applyDate;}
            set 
    		{ 
    			if(_applyDate != value)
    			{
    				_applyDate = value; 
    				
    				_applyDate = DateTime.SpecifyKind(_applyDate, DateTimeKind.Local);
    
    				OnPropertyChanged("ApplyDate");
    			}
    		}
        }
        private System.DateTime _applyDate;
    
    
    	[DataMember]
        public virtual int Study
        {
            get {return _study;}
            set 
    		{ 
    			if(_study != value)
    			{
    				_study = value; 
    				
    				
    
    				OnPropertyChanged("Study");
    			}
    		}
        }
        private int _study;
    
    
    	[DataMember]
        public virtual int TestPeriod
        {
            get {return _testPeriod;}
            set 
    		{ 
    			if(_testPeriod != value)
    			{
    				_testPeriod = value; 
    				
    				
    
    				OnPropertyChanged("TestPeriod");
    			}
    		}
        }
        private int _testPeriod;
    
    
    	[DataMember]
        public virtual int Seniority
        {
            get {return _seniority;}
            set 
    		{ 
    			if(_seniority != value)
    			{
    				_seniority = value; 
    				
    				
    
    				OnPropertyChanged("Seniority");
    			}
    		}
        }
        private int _seniority;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> FireDate
        {
            get {return _fireDate;}
            set 
    		{ 
    			if(_fireDate != value)
    			{
    				_fireDate = value; 
    				
    				if (_fireDate.HasValue) _fireDate = DateTime.SpecifyKind(_fireDate.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("FireDate");
    			}
    		}
        }
        private Nullable<System.DateTime> _fireDate;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> FiredById
        {
            get { return _firedById; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_firedById != value)
                    {
                        if (FiredBy != null && FiredBy.UserId != value)
                        {
                            FiredBy = null;
                        }
                        _firedById = value;
        
        				OnPropertyChanged("FiredById");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _firedById;
    
    
    	[DataMember]
        public virtual bool IsAsset
        {
            get {return _isAsset;}
            set 
    		{ 
    			if(_isAsset != value)
    			{
    				_isAsset = value; 
    				
    				
    
    				OnPropertyChanged("IsAsset");
    			}
    		}
        }
        private bool _isAsset;
    
    
    	[DataMember]
        public virtual string FireCause
        {
            get {return _fireCause;}
            set 
    		{ 
    			if(_fireCause != value)
    			{
    				_fireCause = value; 
    				
    				
    
    				OnPropertyChanged("FireCause");
    			}
    		}
        }
        private string _fireCause;
    
    
    	[DataMember]
        public virtual System.Guid DocumentId
        {
            get { return _documentId; }
            set
            {
                try
                {
                    _settingFK = true;
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
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _documentId;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> FireDocumentId
        {
            get { return _fireDocumentId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_fireDocumentId != value)
                    {
                        if (EmployeeDocument1 != null && EmployeeDocument1.Id != value)
                        {
                            EmployeeDocument1 = null;
                        }
                        _fireDocumentId = value;
        
        				OnPropertyChanged("FireDocumentId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _fireDocumentId;
    
    

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
    
        public virtual EmployeeCategory EmployeeCategory
        {
            get { return _employeeCategory; }
            set
            {
                if (!ReferenceEquals(_employeeCategory, value))
                {
                    var previousValue = _employeeCategory;
                    _employeeCategory = value;
                    FixupEmployeeCategory(previousValue);
                }
            }
        }
        private EmployeeCategory _employeeCategory;
    
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
    
        public virtual Job Job
        {
            get { return _job; }
            set
            {
                if (!ReferenceEquals(_job, value))
                {
                    var previousValue = _job;
                    _job = value;
                    FixupJob(previousValue);
                }
            }
        }
        private Job _job;
    
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
    
        public virtual User FiredBy
        {
            get { return _firedBy; }
            set
            {
                if (!ReferenceEquals(_firedBy, value))
                {
                    var previousValue = _firedBy;
                    _firedBy = value;
                    FixupFiredBy(previousValue);
                }
            }
        }
        private User _firedBy;
    
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
    
        public virtual EmployeeDocument EmployeeDocument1
        {
            get { return _employeeDocument1; }
            set
            {
                if (!ReferenceEquals(_employeeDocument1, value))
                {
                    var previousValue = _employeeDocument1;
                    _employeeDocument1 = value;
                    FixupEmployeeDocument1(previousValue);
                }
            }
        }
        private EmployeeDocument _employeeDocument1;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements.Contains(this))
            {
                previousValue.JobPlacements.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.JobPlacements.Contains(this))
                {
                    Company.JobPlacements.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupEmployeeCategory(EmployeeCategory previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements.Contains(this))
            {
                previousValue.JobPlacements.Remove(this);
            }
    
            if (EmployeeCategory != null)
            {
                if (!EmployeeCategory.JobPlacements.Contains(this))
                {
                    EmployeeCategory.JobPlacements.Add(this);
                }
                if (CategoryId != EmployeeCategory.Id)
                {
                    CategoryId = EmployeeCategory.Id;
                }
            }
        }
    
        private void FixupEmployee(Employee previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements.Contains(this))
            {
                previousValue.JobPlacements.Remove(this);
            }
    
            if (Employee != null)
            {
                if (!Employee.JobPlacements.Contains(this))
                {
                    Employee.JobPlacements.Add(this);
                }
                if (EmployeeId != Employee.Id)
                {
                    EmployeeId = Employee.Id;
                }
            }
        }
    
        private void FixupJob(Job previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements.Contains(this))
            {
                previousValue.JobPlacements.Remove(this);
            }
    
            if (Job != null)
            {
                if (!Job.JobPlacements.Contains(this))
                {
                    Job.JobPlacements.Add(this);
                }
                if (JobId != Job.Id)
                {
                    JobId = Job.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements.Contains(this))
            {
                previousValue.JobPlacements.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.JobPlacements.Contains(this))
                {
                    CreatedBy.JobPlacements.Add(this);
                }
                if (AuthorId != CreatedBy.UserId)
                {
                    AuthorId = CreatedBy.UserId;
                }
            }
        }
    
        private void FixupFiredBy(User previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements1.Contains(this))
            {
                previousValue.JobPlacements1.Remove(this);
            }
    
            if (FiredBy != null)
            {
                if (!FiredBy.JobPlacements1.Contains(this))
                {
                    FiredBy.JobPlacements1.Add(this);
                }
                if (FiredById != FiredBy.UserId)
                {
                    FiredById = FiredBy.UserId;
                }
            }
            else if (!_settingFK)
            {
                FiredById = null;
            }
        }
    
        private void FixupEmployeeDocument(EmployeeDocument previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements.Contains(this))
            {
                previousValue.JobPlacements.Remove(this);
            }
    
            if (EmployeeDocument != null)
            {
                if (!EmployeeDocument.JobPlacements.Contains(this))
                {
                    EmployeeDocument.JobPlacements.Add(this);
                }
                if (DocumentId != EmployeeDocument.Id)
                {
                    DocumentId = EmployeeDocument.Id;
                }
            }
        }
    
        private void FixupEmployeeDocument1(EmployeeDocument previousValue)
        {
            if (previousValue != null && previousValue.JobPlacements1.Contains(this))
            {
                previousValue.JobPlacements1.Remove(this);
            }
    
            if (EmployeeDocument1 != null)
            {
                if (!EmployeeDocument1.JobPlacements1.Contains(this))
                {
                    EmployeeDocument1.JobPlacements1.Add(this);
                }
                if (FireDocumentId != EmployeeDocument1.Id)
                {
                    FireDocumentId = EmployeeDocument1.Id;
                }
            }
            else if (!_settingFK)
            {
                FireDocumentId = null;
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