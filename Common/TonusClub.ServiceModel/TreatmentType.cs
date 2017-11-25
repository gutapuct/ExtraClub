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
    public partial class TreatmentType : INotifyPropertyChanged
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
        public virtual bool AllowsMultiple
        {
            get {return _allowsMultiple;}
            set 
    		{ 
    			if(_allowsMultiple != value)
    			{
    				_allowsMultiple = value; 
    				
    				
    
    				OnPropertyChanged("AllowsMultiple");
    			}
    		}
        }
        private bool _allowsMultiple;
    
    
    	[DataMember]
        public virtual int Duration
        {
            get {return _duration;}
            set 
    		{ 
    			if(_duration != value)
    			{
    				_duration = value; 
    				
    				
    
    				OnPropertyChanged("Duration");
    			}
    		}
        }
        private int _duration;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> TreatmentTypeGroupId
        {
            get { return _treatmentTypeGroupId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_treatmentTypeGroupId != value)
                    {
                        if (TreatmentTypeGroup != null && TreatmentTypeGroup.Id != value)
                        {
                            TreatmentTypeGroup = null;
                        }
                        _treatmentTypeGroupId = value;
        
        				OnPropertyChanged("TreatmentTypeGroupId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _treatmentTypeGroupId;
    
    
    	[DataMember]
        public virtual bool IsActive
        {
            get {return _isActive;}
            set 
    		{ 
    			if(_isActive != value)
    			{
    				_isActive = value; 
    				
    				
    
    				OnPropertyChanged("IsActive");
    			}
    		}
        }
        private bool _isActive;
    
    
    	[DataMember]
        public virtual string NameEn
        {
            get {return _nameEn;}
            set 
    		{ 
    			if(_nameEn != value)
    			{
    				_nameEn = value; 
    				
    				
    
    				OnPropertyChanged("NameEn");
    			}
    		}
        }
        private string _nameEn;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<Treatment> Treatments
        {
            get
            {
                if (_treatments == null)
                {
                    var newCollection = new FixupCollection<Treatment>();
                    newCollection.CollectionChanged += FixupTreatments;
                    _treatments = newCollection;
                }
                return _treatments;
            }
            set
            {
                if (!ReferenceEquals(_treatments, value))
                {
                    var previousValue = _treatments as FixupCollection<Treatment>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTreatments;
                    }
                    _treatments = value;
                    var newValue = value as FixupCollection<Treatment>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTreatments;
                    }
    				OnPropertyChanged("Treatments");
                }
            }
        }
        private ICollection<Treatment> _treatments;
    
        public virtual ICollection<TicketType> TicketTypes
        {
            get
            {
                if (_ticketTypes == null)
                {
                    var newCollection = new FixupCollection<TicketType>();
                    newCollection.CollectionChanged += FixupTicketTypes;
                    _ticketTypes = newCollection;
                }
                return _ticketTypes;
            }
            set
            {
                if (!ReferenceEquals(_ticketTypes, value))
                {
                    var previousValue = _ticketTypes as FixupCollection<TicketType>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTicketTypes;
                    }
                    _ticketTypes = value;
                    var newValue = value as FixupCollection<TicketType>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTicketTypes;
                    }
    				OnPropertyChanged("TicketTypes");
                }
            }
        }
        private ICollection<TicketType> _ticketTypes;
    
        public virtual ICollection<TreatmentType> TreatmentTypes1
        {
            get
            {
                if (_treatmentTypes1 == null)
                {
                    var newCollection = new FixupCollection<TreatmentType>();
                    newCollection.CollectionChanged += FixupTreatmentTypes1;
                    _treatmentTypes1 = newCollection;
                }
                return _treatmentTypes1;
            }
            set
            {
                if (!ReferenceEquals(_treatmentTypes1, value))
                {
                    var previousValue = _treatmentTypes1 as FixupCollection<TreatmentType>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTreatmentTypes1;
                    }
                    _treatmentTypes1 = value;
                    var newValue = value as FixupCollection<TreatmentType>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTreatmentTypes1;
                    }
    				OnPropertyChanged("TreatmentTypes1");
                }
            }
        }
        private ICollection<TreatmentType> _treatmentTypes1;
    
        public virtual ICollection<TreatmentType> TreatmentTypes
        {
            get
            {
                if (_treatmentTypes == null)
                {
                    var newCollection = new FixupCollection<TreatmentType>();
                    newCollection.CollectionChanged += FixupTreatmentTypes;
                    _treatmentTypes = newCollection;
                }
                return _treatmentTypes;
            }
            set
            {
                if (!ReferenceEquals(_treatmentTypes, value))
                {
                    var previousValue = _treatmentTypes as FixupCollection<TreatmentType>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTreatmentTypes;
                    }
                    _treatmentTypes = value;
                    var newValue = value as FixupCollection<TreatmentType>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTreatmentTypes;
                    }
    				OnPropertyChanged("TreatmentTypes");
                }
            }
        }
        private ICollection<TreatmentType> _treatmentTypes;
    
        public virtual ICollection<ContraIndication> ContraIndications
        {
            get
            {
                if (_contraIndications == null)
                {
                    var newCollection = new FixupCollection<ContraIndication>();
                    newCollection.CollectionChanged += FixupContraIndications;
                    _contraIndications = newCollection;
                }
                return _contraIndications;
            }
            set
            {
                if (!ReferenceEquals(_contraIndications, value))
                {
                    var previousValue = _contraIndications as FixupCollection<ContraIndication>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupContraIndications;
                    }
                    _contraIndications = value;
                    var newValue = value as FixupCollection<ContraIndication>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupContraIndications;
                    }
    				OnPropertyChanged("ContraIndications");
                }
            }
        }
        private ICollection<ContraIndication> _contraIndications;
    
        public virtual ICollection<TreatmentConfig> TreatmentConfigs
        {
            get
            {
                if (_treatmentConfigs == null)
                {
                    var newCollection = new FixupCollection<TreatmentConfig>();
                    newCollection.CollectionChanged += FixupTreatmentConfigs;
                    _treatmentConfigs = newCollection;
                }
                return _treatmentConfigs;
            }
            set
            {
                if (!ReferenceEquals(_treatmentConfigs, value))
                {
                    var previousValue = _treatmentConfigs as FixupCollection<TreatmentConfig>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTreatmentConfigs;
                    }
                    _treatmentConfigs = value;
                    var newValue = value as FixupCollection<TreatmentConfig>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTreatmentConfigs;
                    }
    				OnPropertyChanged("TreatmentConfigs");
                }
            }
        }
        private ICollection<TreatmentConfig> _treatmentConfigs;
    
        public virtual TreatmentTypeGroup TreatmentTypeGroup
        {
            get { return _treatmentTypeGroup; }
            set
            {
                if (!ReferenceEquals(_treatmentTypeGroup, value))
                {
                    var previousValue = _treatmentTypeGroup;
                    _treatmentTypeGroup = value;
                    FixupTreatmentTypeGroup(previousValue);
                }
            }
        }
        private TreatmentTypeGroup _treatmentTypeGroup;
    
        public virtual ICollection<AnketTreatment> AnketTreatments
        {
            get
            {
                if (_anketTreatments == null)
                {
                    var newCollection = new FixupCollection<AnketTreatment>();
                    newCollection.CollectionChanged += FixupAnketTreatments;
                    _anketTreatments = newCollection;
                }
                return _anketTreatments;
            }
            set
            {
                if (!ReferenceEquals(_anketTreatments, value))
                {
                    var previousValue = _anketTreatments as FixupCollection<AnketTreatment>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAnketTreatments;
                    }
                    _anketTreatments = value;
                    var newValue = value as FixupCollection<AnketTreatment>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAnketTreatments;
                    }
    				OnPropertyChanged("AnketTreatments");
                }
            }
        }
        private ICollection<AnketTreatment> _anketTreatments;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupTreatmentTypeGroup(TreatmentTypeGroup previousValue)
        {
            if (previousValue != null && previousValue.TreatmentTypes.Contains(this))
            {
                previousValue.TreatmentTypes.Remove(this);
            }
    
            if (TreatmentTypeGroup != null)
            {
                if (!TreatmentTypeGroup.TreatmentTypes.Contains(this))
                {
                    TreatmentTypeGroup.TreatmentTypes.Add(this);
                }
                if (TreatmentTypeGroupId != TreatmentTypeGroup.Id)
                {
                    TreatmentTypeGroupId = TreatmentTypeGroup.Id;
                }
            }
            else if (!_settingFK)
            {
                TreatmentTypeGroupId = null;
            }
        }
    
        private void FixupTreatments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Treatment item in e.NewItems)
                {
                    item.TreatmentType = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Treatment item in e.OldItems)
                {
                    if (ReferenceEquals(item.TreatmentType, this))
                    {
                        item.TreatmentType = null;
                    }
                }
            }
        }
    
        private void FixupTicketTypes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TicketType item in e.NewItems)
                {
                    if (!item.TreatmentTypes.Contains(this))
                    {
                        item.TreatmentTypes.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TicketType item in e.OldItems)
                {
                    if (item.TreatmentTypes.Contains(this))
                    {
                        item.TreatmentTypes.Remove(this);
                    }
                }
            }
        }
    
        private void FixupTreatmentTypes1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TreatmentType item in e.NewItems)
                {
                    if (!item.TreatmentTypes.Contains(this))
                    {
                        item.TreatmentTypes.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TreatmentType item in e.OldItems)
                {
                    if (item.TreatmentTypes.Contains(this))
                    {
                        item.TreatmentTypes.Remove(this);
                    }
                }
            }
        }
    
        private void FixupTreatmentTypes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TreatmentType item in e.NewItems)
                {
                    if (!item.TreatmentTypes1.Contains(this))
                    {
                        item.TreatmentTypes1.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TreatmentType item in e.OldItems)
                {
                    if (item.TreatmentTypes1.Contains(this))
                    {
                        item.TreatmentTypes1.Remove(this);
                    }
                }
            }
        }
    
        private void FixupContraIndications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ContraIndication item in e.NewItems)
                {
                    if (!item.TreatmentTypes.Contains(this))
                    {
                        item.TreatmentTypes.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ContraIndication item in e.OldItems)
                {
                    if (item.TreatmentTypes.Contains(this))
                    {
                        item.TreatmentTypes.Remove(this);
                    }
                }
            }
        }
    
        private void FixupTreatmentConfigs(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TreatmentConfig item in e.NewItems)
                {
                    item.TreatmentType = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TreatmentConfig item in e.OldItems)
                {
                    if (ReferenceEquals(item.TreatmentType, this))
                    {
                        item.TreatmentType = null;
                    }
                }
            }
        }
    
        private void FixupAnketTreatments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AnketTreatment item in e.NewItems)
                {
                    item.TreatmentType = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AnketTreatment item in e.OldItems)
                {
                    if (ReferenceEquals(item.TreatmentType, this))
                    {
                        item.TreatmentType = null;
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
