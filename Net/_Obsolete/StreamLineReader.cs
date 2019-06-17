using System;
using System.IO;

namespace LumiSoft.Net
{
    /// <summary>
    /// Stream line reader.
    /// </summary>
    //[Obsolete("Use StreamHelper instead !")]
    public class StreamLineReader
    {
        private readonly byte[] _buffer = new byte[1024];
        private string _encoding = "";
        private readonly int _readBufferSize = 1024;
        private readonly Stream _strmSource;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="strmSource">Source stream from where to read data. Reading begins from stream current position.</param>
        public StreamLineReader(Stream strmSource)
        {
            _strmSource = strmSource;
        }

        /// <summary>
        /// Gets or sets if lines must be CRLF terminated or may be only LF terminated too.
        /// </summary>
        public bool CRLF_LinesOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets charset encoding to use for string based methods. Default("") encoding is system default encoding.
        /// </summary>
        public string Encoding
        {
            get => _encoding;

            set
            {
                // Check if encoding is valid
                System.Text.Encoding.GetEncoding(value);

                _encoding = value;
            }
        }

        /// <summary>
        /// Reads byte[] line from stream. NOTE: Returns null if end of stream reached.
        /// </summary>
        /// <returns>Return null if end of stream reached.</returns>
        public byte[] ReadLine()
        {
            // TODO: Allow to buffer source stream reads

            var buffer = _buffer;
            int posInBuffer = 0;

            int prevByte = _strmSource.ReadByte();
            int currByteInt = _strmSource.ReadByte();
            while (prevByte > -1)
            {
                // CRLF line found
                if ((prevByte == (byte)'\r' && (byte)currByteInt == (byte)'\n'))
                {
                    var retVal = new byte[posInBuffer];
                    Array.Copy(buffer, retVal, posInBuffer);
                    return retVal;
                }
                // LF line found and only LF lines allowed

                if (!CRLF_LinesOnly && currByteInt == '\n')
                {
                    var retVal = new byte[posInBuffer + 1];
                    Array.Copy(buffer, retVal, posInBuffer + 1);
                    retVal[posInBuffer] = (byte)prevByte;
                    return retVal;
                }

                // Buffer is full, add addition m_ReadBufferSize bytes
                if (posInBuffer == buffer.Length)
                {
                    var newBuffer = new byte[buffer.Length + _readBufferSize];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    buffer = newBuffer;
                }
                buffer[posInBuffer] = (byte)prevByte;
                posInBuffer++;
                prevByte = currByteInt;

                // Read next byte
                currByteInt = _strmSource.ReadByte();
            }

            // Line isn't terminated with <CRLF> and has some bytes left, return them.
            if (posInBuffer > 0)
            {
                var retVal = new byte[posInBuffer];
                Array.Copy(buffer, retVal, posInBuffer);
                return retVal;
            }

            return null;
        }

        /// <summary>
        /// Reads string line from stream. String is converted with specified Encoding property from byte[] line. NOTE: Returns null if end of stream reached.
        /// </summary>
        /// <returns></returns>
        public string ReadLineString()
        {
            var line = ReadLine();
            if (line != null)
            {
                if (_encoding == null || _encoding == "")
                {
                    return System.Text.Encoding.Default.GetString(line);
                }

                return System.Text.Encoding.GetEncoding(_encoding).GetString(line);
            }

            return null;
        }
    }
}
