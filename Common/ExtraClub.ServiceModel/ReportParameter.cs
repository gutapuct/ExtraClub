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
    public partial class ReportParameter : INotifyPropertyChanged
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
        public virtual System.Guid ReportInfoId
        {
            get { return _reportInfoId; }
            set
            {
                if (_reportInfoId != value)
                {
                    if (ReportInfo != null && ReportInfo.Id != value)
                    {
                        ReportInfo = null;
                    }
                    _reportInfoId = value;
    
    				OnPropertyChanged("ReportInfoId");
                }
            }
        }
        private System.Guid _reportInfoId;
    
    
    	[DataMember]
        public virtual string Name
        {
            get {return _name;}
            set 
    		{ 
    			if(_name != value)
    			{
    				_name = value; 
    				
    				
    
    				OnPropertyChanged("Name");
    			}
    		}
        }
        private string _name;
    
    
    	[DataMember]
        public virtual string IntName
        {
            get {return _intName;}
            set 
    		{ 
    			if(_intName != value)
    			{
    				_intName = value; 
    				
    				
    
    				OnPropertyChanged("IntName");
    			}
    		}
        }
        private string _intName;
    
    
    	[DataMember]
        public virtual int Type
        {
            get {return _type;}
            set 
    		{ 
    			if(_type != value)
    			{
    				_type = value; 
    				
    				
    
    				OnPropertyChanged("Type");
    			}
    		}
        }
        private int _type;
    
    
    	[DataMember]
        public virtual int Order
        {
            get {return _order;}
            set 
    		{ 
    			if(_order != value)
    			{
    				_order = value; 
    				
    				
    
    				OnPropertyChanged("Order");
    			}
    		}
        }
        private int _order;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual ReportInfo ReportInfo
        {
            get { return _reportInfo; }
            set
            {
                if (!ReferenceEquals(_reportInfo, value))
                {
                    var previousValue = _reportInfo;
                    _reportInfo = value;
                    FixupReportInfo(previousValue);
                }
            }
        }
        private ReportInfo _reportInfo;

        #endregion

        #region Association Fixup
    
        private void FixupReportInfo(ReportInfo previousValue)
        {
            if (previousValue != null && previousValue.ReportParameters.Contains(this))
            {
                previousValue.ReportParameters.Remove(this);
            }
    
            if (ReportInfo != null)
            {
                if (!ReportInfo.ReportParameters.Contains(this))
                {
                    ReportInfo.ReportParameters.Add(this);
                }
                if (ReportInfoId != ReportInfo.Id)
                {
                    ReportInfoId = ReportInfo.Id;
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
