using System;
using System.IO;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// This class represents auto switching memory/temp-file stream.
    /// </summary>
    public class MemoryStreamEx : Stream
    {
        private bool _isDisposed;
        private readonly int _maxMemSize;
        private Stream _stream;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="memSize">Maximum bytes store to memory, before switching over temporary file.</param>
        public MemoryStreamEx(int memSize)
        {
            _maxMemSize = memSize;

            _stream = new MemoryStream();
        }

        /// <summary>
        /// Destructor - Just in case user won't call dispose.
        /// </summary>
        ~MemoryStreamEx()
        {
            Dispose();
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

                return true;
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

                return _stream.Length;
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

                return _stream.Position;
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

                _stream.Position = value;
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
            _stream?.Close();
            _stream = null;

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

            _stream.Flush();
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null reference.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return _stream.Read(buffer, offset, count);
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

            return _stream.Seek(offset, origin);
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

            _stream.SetLength(value);
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
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null reference.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // We need switch to temporary file.
            if (_stream is MemoryStream && _stream.Position + count > _maxMemSize)
            {
                var fs = new FileStream(Path.GetTempPath() + "ls-" + Guid.NewGuid().ToString().Replace("-", "") + ".tmp", FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 32000, FileOptions.DeleteOnClose);

                _stream.Position = 0;
                NetUtils.StreamCopy(_stream, fs, 8000);
                _stream.Close();
                _stream = fs;
            }

            _stream.Write(buffer, offset, count);
        }
    }
}
