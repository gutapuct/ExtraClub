using System.Collections.Generic;
using ExtraClub.Infrastructure.Interfaces;
using Telerik.Windows.Controls;
using System.Windows.Controls;

namespace ExtraClub.UIControls.SettingsManager
{
    public class SettingsManager: ISettingsManager
    {
        //private Dictionary<object, List<Control>> _grouping = new Dictionary<object, List<Control>>();
        private Dictionary<Control, IStateContainer> _registered = new Dictionary<Control, IStateContainer>();

        public void RegisterGridView(object parent, RadGridView grid)
        {
            //if (!_grouping.ContainsKey(parent)) {
            //    _grouping.Add(parent, new List<Control>());
            //}
            //_grouping[parent].Add(grid);
            _registered.Add(grid, new RadGridViewSettings(grid));
        }


        public void RegisterWindow(ContentControl wnd)
        {
            _registered.Add(wnd, new WindowSettings(wnd));
        }

        //public void SaveState(object parent)
        //{
        //    //TODO: save in another thread. The problem is accessing rad data structures.
        //    //Thread t = new Thread(new ParameterizedThreadStart(DoSaveState));
        //    //t.Start(parent);
        //    DoSaveState(parent);
        //}

        //private void DoSaveState(object parent)
        //{
        //    foreach (var gw in _grouping[parent])
        //    {
        //        //_registered[gw].SaveState();
        //    }
        //}

        //public void LoadState(object parent)
        //{
        //    foreach (var gw in _grouping[parent])
        //    {
        //        try
        //        {
        //            //_registered[gw].LoadState();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine("SettingsManager.LoadState exception: " + e.Message);
        //        }
        //    }
        //}

        #region ISettingsManager Members


        //public void SaveAll()
        //{
        //    foreach (var key in _grouping.Keys)
        //    {
        //        foreach (var gw in _grouping[key])
        //        {
        //            //_registered[gw].SaveState();
        //        }
        //    }
        //}

        #endregion
    }
}
