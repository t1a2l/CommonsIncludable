using System;

namespace Commons.Interfaces
{
    public interface ITimeable<T> where T : ITimeable<T>
    {
        int? HourOfDay { get; set; }
        event Action<T> OnEntryChanged;
    }
}
