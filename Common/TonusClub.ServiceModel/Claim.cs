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
    public partial class Claim : INotifyPropertyChanged
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
        public virtual System.Guid CreatedBy
        {
            get {return _createdBy;}
            set 
    		{ 
    			if(_createdBy != value)
    			{
    				_createdBy = value; 
    				
    				
    
    				OnPropertyChanged("CreatedBy");
    			}
    		}
        }
        private System.Guid _createdBy;
    
    
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
        public virtual string StatusDescription
        {
            get {return _statusDescription;}
            set 
    		{ 
    			if(_statusDescription != value)
    			{
    				_statusDescription = value; 
    				
    				
    
    				OnPropertyChanged("StatusDescription");
    			}
    		}
        }
        private string _statusDescription;
    
    
    	[DataMember]
        public virtual Nullable<int> FtmId
        {
            get {return _ftmId;}
            set 
    		{ 
    			if(_ftmId != value)
    			{
    				_ftmId = value; 
    				
    				
    
    				OnPropertyChanged("FtmId");
    			}
    		}
        }
        private Nullable<int> _ftmId;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> FinishDate
        {
            get {return _finishDate;}
            set 
    		{ 
    			if(_finishDate != value)
    			{
    				_finishDate = value; 
    				
    				if (_finishDate.HasValue) _finishDate = DateTime.SpecifyKind(_finishDate.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("FinishDate");
    			}
    		}
        }
        private Nullable<System.DateTime> _finishDate;
    
    
    	[DataMember]
        public virtual string FinishedByName
        {
            get {return _finishedByName;}
            set 
    		{ 
    			if(_finishedByName != value)
    			{
    				_finishedByName = value; 
    				
    				
    
    				OnPropertyChanged("FinishedByName");
    			}
    		}
        }
        private string _finishedByName;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> FinishedByFtmId
        {
            get {return _finishedByFtmId;}
            set 
    		{ 
    			if(_finishedByFtmId != value)
    			{
    				_finishedByFtmId = value; 
    				
    				
    
    				OnPropertyChanged("FinishedByFtmId");
    			}
    		}
        }
        private Nullable<System.Guid> _finishedByFtmId;
    
    
    	[DataMember]
        public virtual string FinishDescription
        {
            get {return _finishDescription;}
            set 
    		{ 
    			if(_finishDescription != value)
    			{
    				_finishDescription = value; 
    				
    				
    
    				OnPropertyChanged("FinishDescription");
    			}
    		}
        }
        private string _finishDescription;
    
    
    	[DataMember]
        public virtual string ContactInfo
        {
            get {return _contactInfo;}
            set 
    		{ 
    			if(_contactInfo != value)
    			{
    				_contactInfo = value; 
    				
    				
    
    				OnPropertyChanged("ContactInfo");
    			}
    		}
        }
        private string _contactInfo;
    
    
    	[DataMember]
        public virtual string Subject
        {
            get {return _subject;}
            set 
    		{ 
    			if(_subject != value)
    			{
    				_subject = value; 
    				
    				
    
    				OnPropertyChanged("Subject");
    			}
    		}
        }
        private string _subject;
    
    
    	[DataMember]
        public virtual string Message
        {
            get {return _message;}
            set 
    		{ 
    			if(_message != value)
    			{
    				_message = value; 
    				
    				
    
    				OnPropertyChanged("Message");
    			}
    		}
        }
        private string _message;
    
    
    	[DataMember]
        public virtual string Eq_BuyDate
        {
            get {return _eq_BuyDate;}
            set 
    		{ 
    			if(_eq_BuyDate != value)
    			{
    				_eq_BuyDate = value; 
    				
    				
    
    				OnPropertyChanged("Eq_BuyDate");
    			}
    		}
        }
        private string _eq_BuyDate;
    
    
    	[DataMember]
        public virtual string Eq_Serial
        {
            get {return _eq_Serial;}
            set 
    		{ 
    			if(_eq_Serial != value)
    			{
    				_eq_Serial = value; 
    				
    				
    
    				OnPropertyChanged("Eq_Serial");
    			}
    		}
        }
        private string _eq_Serial;
    
    
    	[DataMember]
        public virtual string Eq_Guaranty
        {
            get {return _eq_Guaranty;}
            set 
    		{ 
    			if(_eq_Guaranty != value)
    			{
    				_eq_Guaranty = value; 
    				
    				
    
    				OnPropertyChanged("Eq_Guaranty");
    			}
    		}
        }
        private string _eq_Guaranty;
    
    
    	[DataMember]
        public virtual string PrefFinishDate
        {
            get {return _prefFinishDate;}
            set 
    		{ 
    			if(_prefFinishDate != value)
    			{
    				_prefFinishDate = value; 
    				
    				
    
    				OnPropertyChanged("PrefFinishDate");
    			}
    		}
        }
        private string _prefFinishDate;
    
    
    	[DataMember]
        public virtual int ClaimTypeId
        {
            get {return _claimTypeId;}
            set 
    		{ 
    			if(_claimTypeId != value)
    			{
    				_claimTypeId = value; 
    				
    				
    
    				OnPropertyChanged("ClaimTypeId");
    			}
    		}
        }
        private int _claimTypeId;
    
    
    	[DataMember]
        public virtual string ContactEmail
        {
            get {return _contactEmail;}
            set 
    		{ 
    			if(_contactEmail != value)
    			{
    				_contactEmail = value; 
    				
    				
    
    				OnPropertyChanged("ContactEmail");
    			}
    		}
        }
        private string _contactEmail;

        [DataMember]
        public virtual int? ActualScore
        {
            get { return _actualScore; }
            set
            {
                if (_actualScore != value)
                {
                    _actualScore = value;

                    OnPropertyChanged("ActualScore");
                }
            }
        }
        private int? _actualScore;

        [DataMember]
        public virtual string ContactPhone
        {
            get {return _contactPhone;}
            set 
    		{ 
    			if(_contactPhone != value)
    			{
    				_contactPhone = value; 
    				
    				
    
    				OnPropertyChanged("ContactPhone");
    			}
    		}
        }
        private string _contactPhone;
    
    
    	[DataMember]
        public virtual int StatusId
        {
            get {return _statusId;}
            set 
    		{ 
    			if(_statusId != value)
    			{
    				_statusId = value; 
    				
    				
    
    				OnPropertyChanged("StatusId");
    			}
    		}
        }
        private int _statusId;
    
    
    	[DataMember]
        public virtual Nullable<System.DateTime> SubmitDate
        {
            get {return _submitDate;}
            set 
    		{ 
    			if(_submitDate != value)
    			{
    				_submitDate = value; 
    				
    				if (_submitDate.HasValue) _submitDate = DateTime.SpecifyKind(_submitDate.Value, DateTimeKind.Local);
    
    				OnPropertyChanged("SubmitDate");
    			}
    		}
        }
        private Nullable<System.DateTime> _submitDate;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> SubmitUser
        {
            get {return _submitUser;}
            set 
    		{ 
    			if(_submitUser != value)
    			{
    				_submitUser = value; 
    				
    				
    
    				OnPropertyChanged("SubmitUser");
    			}
    		}
        }
        private Nullable<System.Guid> _submitUser;
    
    
    	[DataMember]
        public virtual string Circulation
        {
            get {return _circulation;}
            set 
    		{ 
    			if(_circulation != value)
    			{
    				_circulation = value; 
    				
    				
    
    				OnPropertyChanged("Circulation");
    			}
    		}
        }
        private string _circulation;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> Eq_TreatmentId
        {
            get {return _eq_TreatmentId;}
            set 
    		{ 
    			if(_eq_TreatmentId != value)
    			{
    				_eq_TreatmentId = value; 
    				
    				
    
    				OnPropertyChanged("Eq_TreatmentId");
    			}
    		}
        }
        private Nullable<System.Guid> _eq_TreatmentId;
    
    
    	[DataMember]
        public virtual string Eq_TechContact
        {
            get {return _eq_TechContact;}
            set 
    		{ 
    			if(_eq_TechContact != value)
    			{
    				_eq_TechContact = value; 
    				
    				
    
    				OnPropertyChanged("Eq_TechContact");
    			}
    		}
        }
        private string _eq_TechContact;
    
    
    	[DataMember]
        public virtual string Eq_SerialGutwell
        {
            get {return _eq_SerialGutwell;}
            set 
    		{ 
    			if(_eq_SerialGutwell != value)
    			{
    				_eq_SerialGutwell = value; 
    				
    				
    
    				OnPropertyChanged("Eq_SerialGutwell");
    			}
    		}
        }
        private string _eq_SerialGutwell;
    
    
    	[DataMember]
        public virtual string Eq_Model
        {
            get {return _eq_Model;}
            set 
    		{ 
    			if(_eq_Model != value)
    			{
    				_eq_Model = value; 
    				
    				
    
    				OnPropertyChanged("Eq_Model");
    			}
    		}
        }
        private string _eq_Model;
    
    
    	[DataMember]
        public virtual string Eq_ClubAddr
        {
            get {return _eq_ClubAddr;}
            set 
    		{ 
    			if(_eq_ClubAddr != value)
    			{
    				_eq_ClubAddr = value; 
    				
    				
    
    				OnPropertyChanged("Eq_ClubAddr");
    			}
    		}
        }
        private string _eq_ClubAddr;
    
    
    	[DataMember]
        public virtual string Eq_PostAddr
        {
            get {return _eq_PostAddr;}
            set 
    		{ 
    			if(_eq_PostAddr != value)
    			{
    				_eq_PostAddr = value; 
    				
    				
    
    				OnPropertyChanged("Eq_PostAddr");
    			}
    		}
        }
        private string _eq_PostAddr;
    
    

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
