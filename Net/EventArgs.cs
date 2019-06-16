using System;

namespace LumiSoft.Net
{
    /// <summary>
    /// This class universal event arguments for transporting single value.
    /// </summary>
    /// <typeparam name="T">Event data.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Event data.</param>
        public EventArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets event data.
        /// </summary>
        public T Value { get; }
    }
}
