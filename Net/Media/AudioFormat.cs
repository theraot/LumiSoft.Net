namespace LumiSoft.Net.Media
{
    /// <summary>
    /// This class holds audio information for input or output audio devices.
    /// </summary>
    public class AudioFormat
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="samplesPerSecond">The number of samples per second that are provided by the audio format.</param>
        /// <param name="bitsPerSample">The number of bits that are used to store the audio information for a single sample of an audio format.</param>
        /// <param name="channels">The number of channels that are provided by the audio format.</param>
        public AudioFormat(int samplesPerSecond,int bitsPerSample,int channels)
        {
            SamplesPerSecond = samplesPerSecond;
            BitsPerSample    = bitsPerSample;
            Channels         = channels;
        }


        #region ovveride method Equals

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>Returns true if two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if(obj == null){
                return false;
            }
            if(!(obj is AudioFormat)){
                return false;
            }

            AudioFormat format = (AudioFormat)obj;
            if(format.SamplesPerSecond != this.SamplesPerSecond){
                return false;
            }
            if(format.BitsPerSample != this.BitsPerSample){
                return false;
            }
            if(format.Channels != this.Channels){
                return false;
            }

            return true;
        }

        #endregion

        #region override method GetHashCode

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns>Returns the hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets the number of samples per second that are provided by the audio format.
        /// </summary>
        public int SamplesPerSecond { get; }

        /// <summary>
        /// Gets the number of bits that are used to store the audio information for a single sample of an audio format.
        /// </summary>
        public int BitsPerSample { get; }

        /// <summary>
        /// Gets the number of channels that are provided by the audio format.
        /// </summary>
        public int Channels { get; }

#endregion
    }
}
