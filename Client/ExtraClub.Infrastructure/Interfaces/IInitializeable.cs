using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.Infrastructure.Interfaces
{
    public interface IInitializeable
    {
        void TestInitialize();
        void TryInitialize();
        bool Initialized { get; }
    }
}
