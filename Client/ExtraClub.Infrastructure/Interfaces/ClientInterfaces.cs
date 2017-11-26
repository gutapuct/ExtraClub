using System;

namespace ExtraClub.Infrastructure.Interfaces
{
    public interface ILargeSection
    {
        void SetState(object data);
    }

    public class ResizeEventArgs : EventArgs
    {
        public string ActiveRegionName { get; set; }
        public Guid Parameter { get; set; }

        public ResizeEventArgs() { }

        public ResizeEventArgs(string activeRegionName)
        {
            ActiveRegionName = activeRegionName;
        }
    }
}
