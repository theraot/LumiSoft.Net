namespace LumiSoft.Net.Media
{
    /// <summary>
    /// This class represents audio oputput device(speakers,head-phones, ....).
    /// </summary>
    public class AudioOutDevice
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="index">Device index in devices.</param>
        /// <param name="name">Device name.</param>
        /// <param name="channels">Number of audio channels.</param>
        internal AudioOutDevice(int index,string name,int channels)
        {
            Index    = index;
            Name     = name;
            Channels = channels;
        }

        /// <summary>
        /// Gets device name.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets number of output channels(mono,stereo,...) supported.
        /// </summary>
        public int Channels { get; } = 1;

        /// <summary>
        /// Gets device index in devices.
        /// </summary>
        internal int Index { get; }
    }
}
