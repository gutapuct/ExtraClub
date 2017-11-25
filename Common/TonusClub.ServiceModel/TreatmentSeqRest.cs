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
    public partial class TreatmentSeqRest : INotifyPropertyChanged
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
        public virtual System.Guid TreatmentType1Id
        {
            get { return _treatmentType1Id; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_treatmentType1Id != value)
                    {
                        if (Treatment1Type != null && Treatment1Type.Id != value)
                        {
                            Treatment1Type = null;
                        }
                        _treatmentType1Id = value;
        
        				OnPropertyChanged("TreatmentType1Id");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _treatmentType1Id;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> TreatmentType2Id
        {
            get { return _treatmentType2Id; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_treatmentType2Id != value)
                    {
                        if (Treatment2Type != null && Treatment2Type.Id != value)
                        {
                            Treatment2Type = null;
                        }
                        _treatmentType2Id = value;
        
        				OnPropertyChanged("TreatmentType2Id");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _treatmentType2Id;
    
    
    	[DataMember]
        public virtual Nullable<int> Interval
        {
            get {return _interval;}
            set 
    		{ 
    			if(_interval != value)
    			{
    				_interval = value; 
    				
    				
    
    				OnPropertyChanged("Interval");
    			}
    		}
        }
        private Nullable<int> _interval;
    
    
    	[DataMember]
        public virtual Nullable<int> Amount
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
        private Nullable<int> _amount;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual TreatmentType Treatment1Type
        {
            get { return _treatment1Type; }
            set
            {
                if (!ReferenceEquals(_treatment1Type, value))
                {
                    var previousValue = _treatment1Type;
                    _treatment1Type = value;
                    FixupTreatment1Type(previousValue);
                }
            }
        }
        private TreatmentType _treatment1Type;
    
        public virtual TreatmentType Treatment2Type
        {
            get { return _treatment2Type; }
            set
            {
                if (!ReferenceEquals(_treatment2Type, value))
                {
                    var previousValue = _treatment2Type;
                    _treatment2Type = value;
                    FixupTreatment2Type(previousValue);
                }
            }
        }
        private TreatmentType _treatment2Type;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupTreatment1Type(TreatmentType previousValue)
        {
            if (Treatment1Type != null)
            {
                if (TreatmentType1Id != Treatment1Type.Id)
                {
                    TreatmentType1Id = Treatment1Type.Id;
                }
            }
        }
    
        private void FixupTreatment2Type(TreatmentType previousValue)
        {
            if (Treatment2Type != null)
            {
                if (TreatmentType2Id != Treatment2Type.Id)
                {
                    TreatmentType2Id = Treatment2Type.Id;
                }
            }
            else if (!_settingFK)
            {
                TreatmentType2Id = null;
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
