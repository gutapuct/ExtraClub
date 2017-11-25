using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Windows;

namespace TonusClub.Infrastructure.Interfaces
{
    public interface ISettingsManager
    {
        void RegisterGridView(object parent, RadGridView grid);

        //void SaveState(object parent);
        //void LoadState(object parent);

        //void SaveAll();

        void RegisterWindow(ContentControl window);
    }
}
