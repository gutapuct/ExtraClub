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
    public partial class EmployeeDocument : INotifyPropertyChanged
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
        public virtual int DocType
        {
            get {return _docType;}
            set 
    		{ 
    			if(_docType != value)
    			{
    				_docType = value; 
    				
    				
    
    				OnPropertyChanged("DocType");
    			}
    		}
        }
        private int _docType;
    
    
    	[DataMember]
        public virtual System.Guid ReferenceId
        {
            get {return _referenceId;}
            set 
    		{ 
    			if(_referenceId != value)
    			{
    				_referenceId = value; 
    				
    				
    
    				OnPropertyChanged("ReferenceId");
    			}
    		}
        }
        private System.Guid _referenceId;
    
    
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
    
        public virtual ICollection<EmployeeTrip> EmployeeTrips
        {
            get
            {
                if (_employeeTrips == null)
                {
                    var newCollection = new FixupCollection<EmployeeTrip>();
                    newCollection.CollectionChanged += FixupEmployeeTrips;
                    _employeeTrips = newCollection;
                }
                return _employeeTrips;
            }
            set
            {
                if (!ReferenceEquals(_employeeTrips, value))
                {
                    var previousValue = _employeeTrips as FixupCollection<EmployeeTrip>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEmployeeTrips;
                    }
                    _employeeTrips = value;
                    var newValue = value as FixupCollection<EmployeeTrip>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEmployeeTrips;
                    }
    				OnPropertyChanged("EmployeeTrips");
                }
            }
        }
        private ICollection<EmployeeTrip> _employeeTrips;
    
        public virtual ICollection<EmployeeVacation> EmployeeVacations
        {
            get
            {
                if (_employeeVacations == null)
                {
                    var newCollection = new FixupCollection<EmployeeVacation>();
                    newCollection.CollectionChanged += FixupEmployeeVacations;
                    _employeeVacations = newCollection;
                }
                return _employeeVacations;
            }
            set
            {
                if (!ReferenceEquals(_employeeVacations, value))
                {
                    var previousValue = _employeeVacations as FixupCollection<EmployeeVacation>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEmployeeVacations;
                    }
                    _employeeVacations = value;
                    var newValue = value as FixupCollection<EmployeeVacation>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEmployeeVacations;
                    }
    				OnPropertyChanged("EmployeeVacations");
                }
            }
        }
        private ICollection<EmployeeVacation> _employeeVacations;
    
        public virtual ICollection<JobPlacement> JobPlacements
        {
            get
            {
                if (_jobPlacements == null)
                {
                    var newCollection = new FixupCollection<JobPlacement>();
                    newCollection.CollectionChanged += FixupJobPlacements;
                    _jobPlacements = newCollection;
                }
                return _jobPlacements;
            }
            set
            {
                if (!ReferenceEquals(_jobPlacements, value))
                {
                    var previousValue = _jobPlacements as FixupCollection<JobPlacement>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupJobPlacements;
                    }
                    _jobPlacements = value;
                    var newValue = value as FixupCollection<JobPlacement>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupJobPlacements;
                    }
    				OnPropertyChanged("JobPlacements");
                }
            }
        }
        private ICollection<JobPlacement> _jobPlacements;
    
        public virtual ICollection<JobPlacement> JobPlacements1
        {
            get
            {
                if (_jobPlacements1 == null)
                {
                    var newCollection = new FixupCollection<JobPlacement>();
                    newCollection.CollectionChanged += FixupJobPlacements1;
                    _jobPlacements1 = newCollection;
                }
                return _jobPlacements1;
            }
            set
            {
                if (!ReferenceEquals(_jobPlacements1, value))
                {
                    var previousValue = _jobPlacements1 as FixupCollection<JobPlacement>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupJobPlacements1;
                    }
                    _jobPlacements1 = value;
                    var newValue = value as FixupCollection<JobPlacement>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupJobPlacements1;
                    }
    				OnPropertyChanged("JobPlacements1");
                }
            }
        }
        private ICollection<JobPlacement> _jobPlacements1;

        #endregion

        #region Association Fixup
    
        private void FixupCompany(Company previousValue)
        {
            if (previousValue != null && previousValue.EmployeeDocuments.Contains(this))
            {
                previousValue.EmployeeDocuments.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.EmployeeDocuments.Contains(this))
                {
                    Company.EmployeeDocuments.Add(this);
                }
                if (CompanyId != Company.CompanyId)
                {
                    CompanyId = Company.CompanyId;
                }
            }
        }
    
        private void FixupEmployeeTrips(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EmployeeTrip item in e.NewItems)
                {
                    item.EmployeeDocument = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EmployeeTrip item in e.OldItems)
                {
                    if (ReferenceEquals(item.EmployeeDocument, this))
                    {
                        item.EmployeeDocument = null;
                    }
                }
            }
        }
    
        private void FixupEmployeeVacations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EmployeeVacation item in e.NewItems)
                {
                    item.EmployeeDocument = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EmployeeVacation item in e.OldItems)
                {
                    if (ReferenceEquals(item.EmployeeDocument, this))
                    {
                        item.EmployeeDocument = null;
                    }
                }
            }
        }
    
        private void FixupJobPlacements(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (JobPlacement item in e.NewItems)
                {
                    item.EmployeeDocument = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (JobPlacement item in e.OldItems)
                {
                    if (ReferenceEquals(item.EmployeeDocument, this))
                    {
                        item.EmployeeDocument = null;
                    }
                }
            }
        }
    
        private void FixupJobPlacements1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (JobPlacement item in e.NewItems)
                {
                    item.EmployeeDocument1 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (JobPlacement item in e.OldItems)
                {
                    if (ReferenceEquals(item.EmployeeDocument1, this))
                    {
                        item.EmployeeDocument1 = null;
                    }
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
