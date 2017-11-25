using System;

namespace TonusClub.UIControls.Interfaces
{
    public class ColumnInfo<T>
    {
        public string Header { get; set; }
        public Func<T, object> GetMethod { get; set; }

        public ColumnInfo(string header, Func<T, object> getMethod)
        {
            Header = header;
            GetMethod = getMethod;
        }
    }
}
