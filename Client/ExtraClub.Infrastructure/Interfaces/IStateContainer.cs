using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.Infrastructure.Interfaces
{
    public interface IStateContainer
    {
        void SaveState();
        void LoadState();
    }
}
