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
    public partial class AnketTreatment : INotifyPropertyChanged
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
        public virtual System.Guid AnketId
        {
            get { return _anketId; }
            set
            {
                if (_anketId != value)
                {
                    if (Anket != null && Anket.Id != value)
                    {
                        Anket = null;
                    }
                    _anketId = value;
    
    				OnPropertyChanged("AnketId");
                }
            }
        }
        private System.Guid _anketId;
    
    
    	[DataMember]
        public virtual System.Guid TreatmentTypeId
        {
            get { return _treatmentTypeId; }
            set
            {
                if (_treatmentTypeId != value)
                {
                    if (TreatmentType != null && TreatmentType.Id != value)
                    {
                        TreatmentType = null;
                    }
                    _treatmentTypeId = value;
    
    				OnPropertyChanged("TreatmentTypeId");
                }
            }
        }
        private System.Guid _treatmentTypeId;
    
    
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
    
        public virtual Anket Anket
        {
            get { return _anket; }
            set
            {
                if (!ReferenceEquals(_anket, value))
                {
                    var previousValue = _anket;
                    _anket = value;
                    FixupAnket(previousValue);
                }
            }
        }
        private Anket _anket;
    
        public virtual TreatmentType TreatmentType
        {
            get { return _treatmentType; }
            set
            {
                if (!ReferenceEquals(_treatmentType, value))
                {
                    var previousValue = _treatmentType;
                    _treatmentType = value;
                    FixupTreatmentType(previousValue);
                }
            }
        }
        private TreatmentType _treatmentType;

        #endregion

        #region Association Fixup
    
        private void FixupAnket(Anket previousValue)
        {
            if (previousValue != null && previousValue.AnketTreatments.Contains(this))
            {
                previousValue.AnketTreatments.Remove(this);
            }
    
            if (Anket != null)
            {
                if (!Anket.AnketTreatments.Contains(this))
                {
                    Anket.AnketTreatments.Add(this);
                }
                if (AnketId != Anket.Id)
                {
                    AnketId = Anket.Id;
                }
            }
        }
    
        private void FixupTreatmentType(TreatmentType previousValue)
        {
            if (previousValue != null && previousValue.AnketTreatments.Contains(this))
            {
                previousValue.AnketTreatments.Remove(this);
            }
    
            if (TreatmentType != null)
            {
                if (!TreatmentType.AnketTreatments.Contains(this))
                {
                    TreatmentType.AnketTreatments.Add(this);
                }
                if (TreatmentTypeId != TreatmentType.Id)
                {
                    TreatmentTypeId = TreatmentType.Id;
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
