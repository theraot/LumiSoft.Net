using System;
using System.IO;
using System.Collections.Generic;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// This class combines multiple stream into one stream for reading.
    /// The most common usage for that stream is when you need to insert some data to the beginning of some stream.
    /// </summary>
    public class MultiStream : Stream
    {
        private bool _isDisposed;
        private Queue<Stream> _streams;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MultiStream()
        {
            _streams = new Queue<Stream>();
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

                return false;
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
        /// <exception cref="NotSupportedException">Is raised when one of the source streams won't support <b>Length</b> property.</exception>
        public override long Length
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                long length = 0;
                foreach (var stream in _streams.ToArray())
                {
                    length += stream.Length;
                }

                return length;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream. This method is not supported and always throws a NotSupportedException.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="NotSupportedException">Is raised when this property is accessed.</exception>
        public override long Position
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                throw new NotSupportedException();
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("SmartStream");
                }

                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Appends this stream to read queue.
        /// </summary>
        /// <param name="stream">Stream to add.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null.</exception>
        public void AppendStream(Stream stream)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            _streams.Enqueue(stream);
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
            _streams = null;

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

            while (true)
            {
                // We have read all streams data, no data left.
                if (_streams.Count == 0)
                {
                    return 0;
                }

                var readCount = _streams.Peek().Read(buffer, offset, count);
                // We have read all current stream data.
                if (readCount == 0)
                {
                    // Move to next stream .
                    _streams.Dequeue();

                    // Next while loop will process "read".
                }
                else
                {
                    return readCount;
                }
            }
        }

        /// <summary>
        /// Sets the position within the current stream. This method is not supported and always throws a NotSupportedException.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <b>origin</b> parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="NotSupportedException">Is raised when this method is accessed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SmartStream");
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the current stream. This method is not supported and always throws a NotSupportedException.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="Seek">Is raised when this method is accessed.</exception>
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
