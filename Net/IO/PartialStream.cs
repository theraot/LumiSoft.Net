using System;
using System.IO;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// Implements read-only stream what operates on specified range of source stream
    /// </summary>
    public class PartialStream : Stream
    {
        private bool _isDisposed;
        private readonly long _length;
        private long _osition;
        private readonly Stream _stream;
        private readonly long _start;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="start">Zero based start position in source stream.</param>
        /// <param name="length">Length of stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public PartialStream(Stream stream, long start, long length)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (!stream.CanSeek)
            {
                throw new ArgumentException("Argument 'stream' does not support seeking.");
            }
            if (start < 0)
            {
                throw new ArgumentException("Argument 'start' value must be >= 0.");
            }
            if (start + length > stream.Length)
            {
                throw new ArgumentException("Argument 'length' value will exceed source stream length.");
            }

            _stream = stream;
            _start = start;
            _length = length;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override bool CanRead
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override bool CanSeek
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override bool CanWrite
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="Seek">Is raised when this property is accessed.</exception>
        public override long Length
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                return _length;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override long Position
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                return _osition;
            }

            set
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }
                if (value < 0 || value > Length)
                {
                    throw new ArgumentException("Property 'Position' value must be >= 0 and <= this.Length.");
                }

                _osition = value;
            }
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public new void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            base.Dispose();
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public override void Flush()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }

            if (_stream.Position != _start + _osition)
            {
                _stream.Position = _start + _osition;
            }
            var readedCount = _stream.Read(buffer, offset, Math.Min(count, (int)(Length - _osition)));
            _osition += readedCount;

            return readedCount;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <b>origin</b> parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                    _osition = 0;
                    break;
                case SeekOrigin.Current:
                    break;
                case SeekOrigin.End:
                    _osition = _length;
                    break;
                default:
                    break;
            }

            return _osition;
        }

        /// <summary>
        /// Sets the length of the current stream. This method is not supported and always throws a NotSupportedException.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="NotSupportedException">Is raised when this method is accessed.</exception>
        public override void SetLength(long value)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// This method is not supported and always throws a NotSupportedException.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="NotSupportedException">Is raised when this method is accessed.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }

            throw new NotSupportedException();
        }
    }
}
