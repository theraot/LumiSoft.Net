using System;

namespace LumiSoft.Net
{
    /// <summary>
    /// This class holds UDP or TCP port range.
    /// </summary>
    public class PortRange
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="start">Start port.</param>
        /// <param name="end">End port.</param>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when any of the aruments value is out of range.</exception>
        public PortRange(int start,int end)
        {
            if(start < 1 || start > 0xFFFF){
                throw new ArgumentOutOfRangeException("Argument 'start' value must be > 0 and << 65 535.");
            }
            if(end < 1 || end > 0xFFFF){
                throw new ArgumentOutOfRangeException("Argument 'end' value must be > 0 and << 65 535.");
            }
            if(start > end){
                throw new ArgumentOutOfRangeException("Argumnet 'start' value must be >= argument 'end' value.");
            }

            Start = start;
            End   = end;
        }


        /// <summary>
        /// Gets start port.
        /// </summary>
        public int Start { get; } = 1000;

        /// <summary>
        /// Gets end port.
        /// </summary>
        public int End { get; } = 1100;
    }
}
