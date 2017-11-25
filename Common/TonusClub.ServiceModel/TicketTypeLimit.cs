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
    public partial class TicketTypeLimit : INotifyPropertyChanged
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
        public virtual System.Guid TicketTypeId
        {
            get { return _ticketTypeId; }
            set
            {
                if (_ticketTypeId != value)
                {
                    if (TicketType != null && TicketType.Id != value)
                    {
                        TicketType = null;
                    }
                    _ticketTypeId = value;
    
    				OnPropertyChanged("TicketTypeId");
                }
            }
        }
        private System.Guid _ticketTypeId;
    
    
    	[DataMember]
        public virtual System.Guid TreatmentConfigId
        {
            get { return _treatmentConfigId; }
            set
            {
                if (_treatmentConfigId != value)
                {
                    if (TreatmentConfig != null && TreatmentConfig.Id != value)
                    {
                        TreatmentConfig = null;
                    }
                    _treatmentConfigId = value;
    
    				OnPropertyChanged("TreatmentConfigId");
                }
            }
        }
        private System.Guid _treatmentConfigId;
    
    
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
    
        public virtual TicketType TicketType
        {
            get { return _ticketType; }
            set
            {
                if (!ReferenceEquals(_ticketType, value))
                {
                    var previousValue = _ticketType;
                    _ticketType = value;
                    FixupTicketType(previousValue);
                }
            }
        }
        private TicketType _ticketType;
    
        public virtual TreatmentConfig TreatmentConfig
        {
            get { return _treatmentConfig; }
            set
            {
                if (!ReferenceEquals(_treatmentConfig, value))
                {
                    var previousValue = _treatmentConfig;
                    _treatmentConfig = value;
                    FixupTreatmentConfig(previousValue);
                }
            }
        }
        private TreatmentConfig _treatmentConfig;

        #endregion

        #region Association Fixup
    
        private void FixupTicketType(TicketType previousValue)
        {
            if (previousValue != null && previousValue.TicketTypeLimits.Contains(this))
            {
                previousValue.TicketTypeLimits.Remove(this);
            }
    
            if (TicketType != null)
            {
                if (!TicketType.TicketTypeLimits.Contains(this))
                {
                    TicketType.TicketTypeLimits.Add(this);
                }
                if (TicketTypeId != TicketType.Id)
                {
                    TicketTypeId = TicketType.Id;
                }
            }
        }
    
        private void FixupTreatmentConfig(TreatmentConfig previousValue)
        {
            if (previousValue != null && previousValue.TicketTypeLimits.Contains(this))
            {
                previousValue.TicketTypeLimits.Remove(this);
            }
    
            if (TreatmentConfig != null)
            {
                if (!TreatmentConfig.TicketTypeLimits.Contains(this))
                {
                    TreatmentConfig.TicketTypeLimits.Add(this);
                }
                if (TreatmentConfigId != TreatmentConfig.Id)
                {
                    TreatmentConfigId = TreatmentConfig.Id;
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