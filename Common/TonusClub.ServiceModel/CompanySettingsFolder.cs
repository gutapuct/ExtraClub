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
    public partial class CompanySettingsFolder : INotifyPropertyChanged
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
        public virtual Nullable<System.Guid> DivisionId
        {
            get {return _divisionId;}
            set 
    		{ 
    			if(_divisionId != value)
    			{
    				_divisionId = value; 
    				
    				
    
    				OnPropertyChanged("DivisionId");
    			}
    		}
        }
        private Nullable<System.Guid> _divisionId;
    
    
    	[DataMember]
        public virtual Nullable<System.Guid> ParentFolderId
        {
            get { return _parentFolderId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_parentFolderId != value)
                    {
                        if (CompanySettingsFolder1 != null && CompanySettingsFolder1.Id != value)
                        {
                            CompanySettingsFolder1 = null;
                        }
                        _parentFolderId = value;
        
        				OnPropertyChanged("ParentFolderId");
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _parentFolderId;
    
    
    	[DataMember]
        public virtual int CategoryId
        {
            get {return _categoryId;}
            set 
    		{ 
    			if(_categoryId != value)
    			{
    				_categoryId = value; 
    				
    				
    
    				OnPropertyChanged("CategoryId");
    			}
    		}
        }
        private int _categoryId;
    
    

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<CompanySettingsFolder> CompanySettingsFolders1
        {
            get
            {
                if (_companySettingsFolders1 == null)
                {
                    var newCollection = new FixupCollection<CompanySettingsFolder>();
                    newCollection.CollectionChanged += FixupCompanySettingsFolders1;
                    _companySettingsFolders1 = newCollection;
                }
                return _companySettingsFolders1;
            }
            set
            {
                if (!ReferenceEquals(_companySettingsFolders1, value))
                {
                    var previousValue = _companySettingsFolders1 as FixupCollection<CompanySettingsFolder>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanySettingsFolders1;
                    }
                    _companySettingsFolders1 = value;
                    var newValue = value as FixupCollection<CompanySettingsFolder>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanySettingsFolders1;
                    }
    				OnPropertyChanged("CompanySettingsFolders1");
                }
            }
        }
        private ICollection<CompanySettingsFolder> _companySettingsFolders1;
    
        public virtual CompanySettingsFolder CompanySettingsFolder1
        {
            get { return _companySettingsFolder1; }
            set
            {
                if (!ReferenceEquals(_companySettingsFolder1, value))
                {
                    var previousValue = _companySettingsFolder1;
                    _companySettingsFolder1 = value;
                    FixupCompanySettingsFolder1(previousValue);
                }
            }
        }
        private CompanySettingsFolder _companySettingsFolder1;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCompanySettingsFolder1(CompanySettingsFolder previousValue)
        {
            if (previousValue != null && previousValue.CompanySettingsFolders1.Contains(this))
            {
                previousValue.CompanySettingsFolders1.Remove(this);
            }
    
            if (CompanySettingsFolder1 != null)
            {
                if (!CompanySettingsFolder1.CompanySettingsFolders1.Contains(this))
                {
                    CompanySettingsFolder1.CompanySettingsFolders1.Add(this);
                }
                if (ParentFolderId != CompanySettingsFolder1.Id)
                {
                    ParentFolderId = CompanySettingsFolder1.Id;
                }
            }
            else if (!_settingFK)
            {
                ParentFolderId = null;
            }
        }
    
        private void FixupCompanySettingsFolders1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanySettingsFolder item in e.NewItems)
                {
                    item.CompanySettingsFolder1 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanySettingsFolder item in e.OldItems)
                {
                    if (ReferenceEquals(item.CompanySettingsFolder1, this))
                    {
                        item.CompanySettingsFolder1 = null;
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
