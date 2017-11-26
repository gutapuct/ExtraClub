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
    public partial class SshFile : INotifyPropertyChanged
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
        public virtual string Path
        {
            get {return _path;}
            set 
    		{ 
    			if(_path != value)
    			{
    				_path = value; 
    				
    				
    
    				OnPropertyChanged("Path");
    			}
    		}
        }
        private string _path;
    
    
    	[DataMember]
        public virtual string Filename
        {
            get {return _filename;}
            set 
    		{ 
    			if(_filename != value)
    			{
    				_filename = value; 
    				
    				
    
    				OnPropertyChanged("Filename");
    			}
    		}
        }
        private string _filename;
    
    
    	[DataMember]
        public virtual long Length
        {
            get {return _length;}
            set 
    		{ 
    			if(_length != value)
    			{
    				_length = value; 
    				
    				
    
    				OnPropertyChanged("Length");
    			}
    		}
        }
        private long _length;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> AvailableTill
        {
            get {return _availableTill;}
            set 
    		{ 
    			if(_availableTill != value)
    			{
    				_availableTill = value; 
    				
    				if (_availableTill.HasValue) _availableTill = DateTime.SpecifyKind(_availableTill.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("AvailableTill");
    			}
    		}
        }
        private Nullable<System.DateTime> _availableTill;
    
    
    	[DataMember]
        public virtual System.DateTime ModifiedDate
        {
            get {return _modifiedDate;}
            set 
    		{ 
    			if(_modifiedDate != value)
    			{
    				_modifiedDate = value; 
    				
    				_modifiedDate = DateTime.SpecifyKind(_modifiedDate, DateTimeKind.Local);
    
    				OnPropertyChanged("ModifiedDate");
    			}
    		}
        }
        private System.DateTime _modifiedDate;
    
    

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