using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.Infrastructure.Interfaces
{
    public interface IInitializeable
    {
        void TestInitialize();
        void TryInitialize();
        bool Initialized { get; }
    }
}
