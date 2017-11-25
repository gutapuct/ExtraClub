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
    public partial class EmployeeWorkGraph : INotifyPropertyChanged
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
        public virtual System.DateTime Begin
        {
            get {return _begin;}
            set 
    		{ 
    			if(_begin != value)
    			{
    				_begin = value; 
    				
    				_begin = DateTime.SpecifyKind(_begin, DateTimeKind.Local);
    
    				OnPropertyChanged("Begin");
    			}
    		}
        }
        private System.DateTime _begin;
    
    
    	[DataMember]
        public virtual System.DateTime End
        {
            get {return _end;}
            set 
    		{ 
    			if(_end != value)
    			{
    				_end = value; 
    				
    				_end = DateTime.SpecifyKind(_end, DateTimeKind.Local);
    
    				OnPropertyChanged("End");
    			}
    		}
        }
        private System.DateTime _end;
    
    
    	[DataMember]
        public virtual byte[] SerializedData
        {
            get {return _serializedData;}
            set 
    		{ 
    			if(_serializedData != value)
    			{
    				_serializedData = value; 
    				
    				
    
    				OnPropertyChanged("SerializedData");
    			}
    		}
        }
        private byte[] _serializedData;
    
    

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
    
        private void FixupDivision(Division previousValue)
        {
            if (previousValue != null && previousValue.EmployeeWorkGraphs.Contains(this))
            {
                previousValue.EmployeeWorkGraphs.Remove(this);
            }
    
            if (Division != null)
            {
                if (!Division.EmployeeWorkGraphs.Contains(this))
                {
                    Division.EmployeeWorkGraphs.Add(this);
                }
                if (DivisionId != Division.Id)
                {
                    DivisionId = Division.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (previousValue != null && previousValue.EmployeeWorkGraphs.Contains(this))
            {
                previousValue.EmployeeWorkGraphs.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.EmployeeWorkGraphs.Contains(this))
                {
                    CreatedBy.EmployeeWorkGraphs.Add(this);
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
