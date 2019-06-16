using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// Implements RTP media clock.
    /// </summary>
    public class RTP_Clock
    {
        private readonly DateTime m_CreateTime;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="baseValue">Clock base value from where clock starts.</param>
        /// <param name="rate">Clock rate in Hz.</param>
        public RTP_Clock(int baseValue,int rate)
        {
            if(rate < 1){
                throw new ArgumentException("Argument 'rate' value must be between 1 and 100 000.","rate");
            }

            BaseValue  = baseValue;
            Rate       = rate;
            m_CreateTime = DateTime.Now;
        }

        /// <summary>
        /// Convers milliseconds to RTP clock ticks. For example clock 8khz 20ms will be 160 RTP clock ticks.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <returns>Returns RTP clock ticks.</returns>
        public int MillisecondsToRtpTicks(int milliseconds)
        {
            return ((Rate * milliseconds) / 1000);
        }

        /// <summary>
        /// Gets clock base value from where clock started.
        /// </summary>
        public int BaseValue { get; }

        /// <summary>
        /// Gets current clock rate in Hz.
        /// </summary>
        public int Rate { get; } = 1;

        /// <summary>
        /// Gets current RTP timestamp.
        /// </summary>
        public uint RtpTimestamp
        {
            get{
                /*
                    m_Rate  -> 1000ms
                    elapsed -> x
                */

                long elapsed =  (long)((TimeSpan)(DateTime.Now - m_CreateTime)).TotalMilliseconds;
                
                return (uint)(BaseValue + ((Rate  * elapsed) / 1000));
            }
        }
    }
}
