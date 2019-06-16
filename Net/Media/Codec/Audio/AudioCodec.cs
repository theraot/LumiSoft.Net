﻿namespace LumiSoft.Net.Media.Codec.Audio
{
    /// <summary>
    /// This class is base calss for audio codecs.
    /// </summary>
    public abstract class AudioCodec : Codec
    {
        /// <summary>
        /// Gets uncompressed audio format info.
        /// </summary>
        public abstract AudioFormat AudioFormat
        {
            get;
        }

        /// <summary>
        /// Gets compressed audio format info.
        /// </summary>
        public abstract AudioFormat CompressedAudioFormat
        {
            get;
        }
    }
}
