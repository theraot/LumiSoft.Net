namespace LumiSoft.Net
{
    /// <summary>
    /// This class represent 2-point <b>long</b> value range.
    /// </summary>
    public class RangeLong
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Start/End value.</param>
        public RangeLong(long value)
        {
            Start = value;
            End = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="start">Range start value.</param>
        /// <param name="end">Range end value.</param>
        public RangeLong(long start, long end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets range end.
        /// </summary>
        public long End { get; }

        /// <summary>
        /// Gets range start.
        /// </summary>
        public long Start { get; }

        /// <summary>
        /// Gets if the specified value is within range.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Returns true if specified value is within range, otherwise false.</returns>
        public bool Contains(long value)
        {
            return value >= Start && value <= End;
        }
    }
}
