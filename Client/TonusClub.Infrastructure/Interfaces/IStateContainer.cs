using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.Infrastructure.Interfaces
{
    public interface IStateContainer
    {
        void SaveState();
        void LoadState();
    }
}
