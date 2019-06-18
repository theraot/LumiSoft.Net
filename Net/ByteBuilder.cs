using System;
using System.Text;

namespace LumiSoft.Net
{
    /// <summary>
    /// Implements byte data builder.
    /// </summary>
    public class ByteBuilder
    {
        private readonly int _blockSize = 1024;
        private byte[] _buffer;
        private Encoding _charset;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ByteBuilder()
        {
            _buffer = new byte[_blockSize];
            _charset = Encoding.UTF8;
        }

        /// <summary>
        /// Gets or sets default charset encoding used for string related operations.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference value is set.</exception>
        public Encoding Charset
        {
            get => _charset;

            set => _charset = value ?? throw new ArgumentNullException("value");
        }

        /// <summary>
        /// Gets number of bytes in byte builder buffer.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Appends specified string value to the buffer. String is encoded with <see cref="Charset"/>.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public void Append(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Append(_charset.GetBytes(value));
        }

        /// <summary>
        /// Appends specified string value to the buffer.
        /// </summary>
        /// <param name="charset">Character encoding.</param>
        /// <param name="value">String value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>charset</b> or <b>value</b> is null reference.</exception>
        public void Append(Encoding charset, string value)
        {
            if (charset == null)
            {
                throw new ArgumentNullException("charset");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Append(charset.GetBytes(value));
        }

        /// <summary>
        /// Appends specified byte[] value to the buffer.
        /// </summary>
        /// <param name="value">Byte value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public void Append(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Append(value, 0, value.Length);
        }

        /// <summary>
        /// Appends specified byte[] value to the buffer.
        /// </summary>
        /// <param name="value">Byte value.</param>
        /// <param name="offset">Offset in the value.</param>
        /// <param name="count">Number of bytes to append.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public void Append(byte[] value, int offset, int count)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // Increase buffer if needed.
            while (_buffer.Length - Count < count)
            {
                var newBuffer = new byte[_buffer.Length + _blockSize];
                Array.Copy(_buffer, newBuffer, Count);
                _buffer = newBuffer;
            }

            Array.Copy(value, offset, _buffer, Count, count);
            Count += value.Length;
        }

        /// <summary>
        /// Returns this as byte[] data.
        /// </summary>
        /// <returns>Returns this as byte[] data.</returns>
        public byte[] ToByte()
        {
            var retVal = new byte[Count];
            Array.Copy(_buffer, retVal, Count);

            return retVal;
        }
    }
}
