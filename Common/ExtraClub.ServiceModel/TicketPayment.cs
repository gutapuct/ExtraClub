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
    public partial class TicketPayment : INotifyPropertyChanged
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
        public virtual System.Guid TicketId
        {
            get { return _ticketId; }
            set
            {
                if (_ticketId != value)
                {
                    if (Ticket != null && Ticket.Id != value)
                    {
                        Ticket = null;
                    }
                    _ticketId = value;
    
    				OnPropertyChanged("TicketId");
                }
            }
        }
        private System.Guid _ticketId;
    
    
    	[DataMember]
        public virtual System.DateTime PaymentDate
        {
            get {return _paymentDate;}
            set 
    		{ 
    			if(_paymentDate != value)
    			{
    				_paymentDate = value; 
    				
    				_paymentDate = DateTime.SpecifyKind(_paymentDate, DateTimeKind.Local);
    
    				OnPropertyChanged("PaymentDate");
    			}
    		}
        }
        private System.DateTime _paymentDate;
    
    
    	[DataMember]
        public virtual decimal Amount
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
        private decimal _amount;
    
    
    	[DataMember]
        public virtual System.Guid AuthorId
        {
            get { return _authorId; }
            set
            {
                if (_authorId != value)
                {
                    if (CreatedBy != null && CreatedBy.UserId != value)
                    {
                        CreatedBy = null;
                    }
                    _authorId = value;
    
    				OnPropertyChanged("AuthorId");
                }
            }
        }
        private System.Guid _authorId;
    
    
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
        public virtual Nullable<int> ReceiptNumber
        {
            get {return _receiptNumber;}
            set 
    		{ 
    			if(_receiptNumber != value)
    			{
    				_receiptNumber = value; 
    				
    				
    
    				OnPropertyChanged("ReceiptNumber");
    			}
    		}
        }
        private Nullable<int> _receiptNumber;
    
    
    	[DataMember]
        public virtual decimal RequestedAmount
        {
            get {return _requestedAmount;}
            set 
    		{ 
    			if(_requestedAmount != value)
    			{
    				_requestedAmount = value; 
    				
    				
    
    				OnPropertyChanged("RequestedAmount");
    			}
    		}
        }
        private decimal _requestedAmount;
    
    
    	[DataMember]
        public virtual bool IsRoboCompleted
        {
            get {return _isRoboCompleted;}
            set 
    		{ 
    			if(_isRoboCompleted != value)
    			{
    				_isRoboCompleted = value; 
    				
    				
    
    				OnPropertyChanged("IsRoboCompleted");
    			}
    		}
        }
        private bool _isRoboCompleted;
    
    
    	[DataMember]
        public virtual Nullable<int> TRoboNumber
        {
            get {return _tRoboNumber;}
            set 
    		{ 
    			if(_tRoboNumber != value)
    			{
    				_tRoboNumber = value; 
    				
    				
    
    				OnPropertyChanged("TRoboNumber");
    			}
    		}
        }
        private Nullable<int> _tRoboNumber;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> BarOrderId
        {
            get {return _barOrderId;}
            set 
    		{ 
    			if(_barOrderId != value)
    			{
    				_barOrderId = value; 
    				
    				
    
    				OnPropertyChanged("BarOrderId");
    			}
    		}
        }
        private Nullable<System.Guid> _barOrderId;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual Ticket Ticket
        {
            get { return _ticket; }
            set
            {
                if (!ReferenceEquals(_ticket, value))
                {
                    var previousValue = _ticket;
                    _ticket = value;
                    FixupTicket(previousValue);
                }
            }
        }
        private Ticket _ticket;
    
        public virtual User CreatedBy
        {
            get { return _createdBy; }
            set
            {
                if (!ReferenceEquals(_createdBy, value))
                {
                    var previousValue = _createdBy;
                    _createdBy = value;
                    FixupCreatedBy(previousValue);
                }
            }
        }
        private User _createdBy;

        #endregion

        #region Association Fixup
    
        private void FixupTicket(Ticket previousValue)
        {
            if (previousValue != null && previousValue.TicketPayments.Contains(this))
            {
                previousValue.TicketPayments.Remove(this);
            }
    
            if (Ticket != null)
            {
                if (!Ticket.TicketPayments.Contains(this))
                {
                    Ticket.TicketPayments.Add(this);
                }
                if (TicketId != Ticket.Id)
                {
                    TicketId = Ticket.Id;
                }
            }
        }
    
        private void FixupCreatedBy(User previousValue)
        {
            if (CreatedBy != null)
            {
                if (AuthorId != CreatedBy.UserId)
                {
                    AuthorId = CreatedBy.UserId;
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
