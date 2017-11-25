using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using TonusClub.ServiceModel;

namespace TonusClub.Infrastructure.Interfaces
{
    public interface IDictionaryManager
    {
        ICollectionView GetViewSource(string TableName);
        DictionaryInfo GetDictionaryInfoBySetName(string entitySetName);

        void RefreshDictionary(DictionaryInfo dictInfo);

        bool HasChanges(DictionaryInfo di);

        string GetValue(string Dictionary, Guid? Key);

        bool ContainsElement(DictionaryInfo dictionary, string elementName);

        void AddNewElement(DictionaryInfo dictionary, string elementName);

        void RenameElement(DictionaryInfo dictionary, Guid elementGuid, string newName);

        string RemoveElement(DictionaryInfo dictionary, Guid elementGuid);
    }
}
