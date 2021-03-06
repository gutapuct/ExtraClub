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
    public partial class PackageLine : INotifyPropertyChanged
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
        public virtual System.Guid PackageId
        {
            get { return _packageId; }
            set
            {
                if (_packageId != value)
                {
                    if (Package != null && Package.Id != value)
                    {
                        Package = null;
                    }
                    _packageId = value;
    
    				OnPropertyChanged("PackageId");
                }
            }
        }
        private System.Guid _packageId;
    
    
    	[DataMember]
        public virtual System.Guid GoodId
        {
            get {return _goodId;}
            set 
    		{ 
    			if(_goodId != value)
    			{
    				_goodId = value; 
    				
    				
    
    				OnPropertyChanged("GoodId");
    			}
    		}
        }
        private System.Guid _goodId;
    
    
    	[DataMember]
        public virtual int Amount
        {
            get {return _amount;}
            set 
    		{ 
    			if(_amount != value)
    			{
    				_amount = value; 
    				
    				
    
    				OnPropertyChanged("Amount");
    			}
    		}
        }
        private int _amount;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual Package Package
        {
            get { return _package; }
            set
            {
                if (!ReferenceEquals(_package, value))
                {
                    var previousValue = _package;
                    _package = value;
                    FixupPackage(previousValue);
                }
            }
        }
        private Package _package;

        #endregion

        #region Association Fixup
    
        private void FixupPackage(Package previousValue)
        {
            if (previousValue != null && previousValue.PackageLines.Contains(this))
            {
                previousValue.PackageLines.Remove(this);
            }
    
            if (Package != null)
            {
                if (!Package.PackageLines.Contains(this))
                {
                    Package.PackageLines.Add(this);
                }
                if (PackageId != Package.Id)
                {
                    PackageId = Package.Id;
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
