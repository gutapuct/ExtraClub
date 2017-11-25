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
    public partial class IncomingCallFormButton : INotifyPropertyChanged
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
        public virtual System.Guid IncomingCallFormId
        {
            get { return _incomingCallFormId; }
            set
            {
                if (_incomingCallFormId != value)
                {
                    if (IncomingCallForm != null && IncomingCallForm.Id != value)
                    {
                        IncomingCallForm = null;
                    }
                    _incomingCallFormId = value;
    
    				OnPropertyChanged("IncomingCallFormId");
                }
            }
        }
        private System.Guid _incomingCallFormId;
    
    
    	[DataMember]
        public virtual string ButtonText
        {
            get {return _buttonText;}
            set 
    		{ 
    			if(_buttonText != value)
    			{
    				_buttonText = value; 
    				
    				
    
    				OnPropertyChanged("ButtonText");
    			}
    		}
        }
        private string _buttonText;
    
    
    	[DataMember]
        public virtual int ButtonAction
        {
            get {return _buttonAction;}
            set 
    		{ 
    			if(_buttonAction != value)
    			{
    				_buttonAction = value; 
    				
    				
    
    				OnPropertyChanged("ButtonAction");
    			}
    		}
        }
        private int _buttonAction;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> Parameter
        {
            get {return _parameter;}
            set 
    		{ 
    			if(_parameter != value)
    			{
    				_parameter = value; 
    				
    				
    
    				OnPropertyChanged("Parameter");
    			}
    		}
        }
        private Nullable<System.Guid> _parameter;
    
    
    	[DataMember]
        public virtual bool IsFinal
        {
            get {return _isFinal;}
            set 
    		{ 
    			if(_isFinal != value)
    			{
    				_isFinal = value; 
    				
    				
    
    				OnPropertyChanged("IsFinal");
    			}
    		}
        }
        private bool _isFinal;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual IncomingCallForm IncomingCallForm
        {
            get { return _incomingCallForm; }
            set
            {
                if (!ReferenceEquals(_incomingCallForm, value))
                {
                    var previousValue = _incomingCallForm;
                    _incomingCallForm = value;
                    FixupIncomingCallForm(previousValue);
                }
            }
        }
        private IncomingCallForm _incomingCallForm;

        #endregion

        #region Association Fixup
    
        private void FixupIncomingCallForm(IncomingCallForm previousValue)
        {
            if (previousValue != null && previousValue.IncomingCallFormButtons.Contains(this))
            {
                previousValue.IncomingCallFormButtons.Remove(this);
            }
    
            if (IncomingCallForm != null)
            {
                if (!IncomingCallForm.IncomingCallFormButtons.Contains(this))
                {
                    IncomingCallForm.IncomingCallFormButtons.Add(this);
                }
                if (IncomingCallFormId != IncomingCallForm.Id)
                {
                    IncomingCallFormId = IncomingCallForm.Id;
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