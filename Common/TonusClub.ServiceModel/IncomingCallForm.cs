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
    public partial class IncomingCallForm : INotifyPropertyChanged
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
        public virtual string FormText
        {
            get {return _formText;}
            set 
    		{ 
    			if(_formText != value)
    			{
    				_formText = value; 
    				
    				
    
    				OnPropertyChanged("FormText");
    			}
    		}
        }
        private string _formText;
    
    
    	[DataMember]
        public virtual bool HasInputBox
        {
            get {return _hasInputBox;}
            set 
    		{ 
    			if(_hasInputBox != value)
    			{
    				_hasInputBox = value; 
    				
    				
    
    				OnPropertyChanged("HasInputBox");
    			}
    		}
        }
        private bool _hasInputBox;
    
    
    	[DataMember]
        public virtual bool IsStartForm
        {
            get {return _isStartForm;}
            set 
    		{ 
    			if(_isStartForm != value)
    			{
    				_isStartForm = value; 
    				
    				
    
    				OnPropertyChanged("IsStartForm");
    			}
    		}
        }
        private bool _isStartForm;
    
    
    	[DataMember]
        public virtual string Header
        {
            get {return _header;}
            set 
    		{ 
    			if(_header != value)
    			{
    				_header = value; 
    				
    				
    
    				OnPropertyChanged("Header");
    			}
    		}
        }
        private string _header;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<IncomingCallFormButton> IncomingCallFormButtons
        {
            get
            {
                if (_incomingCallFormButtons == null)
                {
                    var newCollection = new FixupCollection<IncomingCallFormButton>();
                    newCollection.CollectionChanged += FixupIncomingCallFormButtons;
                    _incomingCallFormButtons = newCollection;
                }
                return _incomingCallFormButtons;
            }
            set
            {
                if (!ReferenceEquals(_incomingCallFormButtons, value))
                {
                    var previousValue = _incomingCallFormButtons as FixupCollection<IncomingCallFormButton>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupIncomingCallFormButtons;
                    }
                    _incomingCallFormButtons = value;
                    var newValue = value as FixupCollection<IncomingCallFormButton>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupIncomingCallFormButtons;
                    }
    				OnPropertyChanged("IncomingCallFormButtons");
                }
            }
        }
        private ICollection<IncomingCallFormButton> _incomingCallFormButtons;

        #endregion

        #region Association Fixup
    
        private void FixupIncomingCallFormButtons(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IncomingCallFormButton item in e.NewItems)
                {
                    item.IncomingCallForm = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (IncomingCallFormButton item in e.OldItems)
                {
                    if (ReferenceEquals(item.IncomingCallForm, this))
                    {
                        item.IncomingCallForm = null;
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
