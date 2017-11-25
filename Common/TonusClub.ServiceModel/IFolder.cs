using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TonusClub.ServiceModel
{
    public interface IFolder
    {
        Guid Id { get; set; }
        Guid? ParentFolderId { get; set; }
        string Name { get; set; }
        object SettingsFolders1 { get; }

        int CategoryId { get; set; }
    }
}
