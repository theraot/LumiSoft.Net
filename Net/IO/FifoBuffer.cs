using System;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// Implements FIFO(first in - first out) buffer.
    /// </summary>
    public class FifoBuffer
    {
        private readonly byte[] _buffer;
        private readonly object _lock = new object();
        private int _readOffset;
        private int _writeOffset;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="maxSize">Maximum number of bytes can buffer in FIFO.</param>
        /// <exception cref="ArgumentException">Is raised when </exception>
        public FifoBuffer(int maxSize)
        {
            if (maxSize < 1)
            {
                throw new ArgumentException("Argument 'maxSize' value must be >= 1.");
            }

            _buffer = new byte[maxSize];
        }

        /// <summary>
        /// Gets number of bytes available in FIFO.
        /// </summary>
        public int Available => _writeOffset - _readOffset;

        /// <summary>
        /// Gets maximum number of bytes can buffer in FIFO.
        /// </summary>
        public int MaxSize => _buffer.Length;

        /// <summary>
        /// Clears buffer data.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _readOffset = 0;
                _writeOffset = 0;
            }
        }

        /// <summary>
        /// Reads up to specified count of bytes from the FIFO buffer.
        /// </summary>
        /// <param name="buffer">Buffer where to store data.</param>
        /// <param name="offset">Index in the buffer.</param>
        /// <param name="count">Maximum number of bytes to read.</param>
        /// <returns>Returns number of bytes read. Returns 0 if no data in the buffer.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when any of the arguments has out of allowed range.</exception>
        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
            }
            if (offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
            }

            lock (_lock)
            {
                var countToRead = Math.Min(count, _writeOffset - _readOffset);
                if (countToRead <= 0)
                {
                    return countToRead;
                }

                Array.Copy(_buffer, _readOffset, buffer, offset, countToRead);
                _readOffset += countToRead;

                return countToRead;
            }
        }

        /// <summary>
        /// Writes specified number of bytes to the FIFO buffer.
        /// </summary>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="offset">Index in the buffer.</param>
        /// <param name="count">Number of bytes to write.</param>
        /// <param name="ignoreBufferFull">If true, disables exception raising when FIFO full.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when any of the arguments has out of allowed range.</exception>
        /// <exception cref="DataSizeExceededException">Is raised when ignoreBufferFull = false and FIFO buffer has no room to store data.</exception>
        public void Write(byte[] buffer, int offset, int count, bool ignoreBufferFull)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
            }
            if (count < 0 || count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            lock (_lock)
            {
                var freeSpace = _buffer.Length - _writeOffset;

                // We don't have enough room to store data.
                if (freeSpace < count)
                {
                    TrimStart();

                    // Recalculate free space.
                    freeSpace = _buffer.Length - _writeOffset;

                    // After trim we can store data.
                    if (freeSpace >= count)
                    {
                        Array.Copy(buffer, offset, _buffer, _writeOffset, count);
                        _writeOffset += count;
                    }
                    // We have not enough space.
                    else
                    {
                        if (!ignoreBufferFull)
                        {
                            throw new DataSizeExceededException();
                        }
                    }
                }
                // Store data to buffer.
                else
                {
                    Array.Copy(buffer, offset, _buffer, _writeOffset, count);
                    _writeOffset += count;
                }
            }
        }

        /// <summary>
        /// Removes unused space from the buffer beginning.
        /// </summary>
        private void TrimStart()
        {
            if (_readOffset <= 0)
            {
                return;
            }

            var buffer = new byte[Available];
            Array.Copy(_buffer, _readOffset, buffer, 0, buffer.Length);
            Array.Copy(buffer, _buffer, buffer.Length);
            _readOffset = 0;
            _writeOffset = buffer.Length;
        }
    }
}
