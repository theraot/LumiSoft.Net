using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// This stream just junks all written data.
    /// </summary>
    public class JunkingStream : Stream
    {
        /// <summary>
        /// Gets a value indicating whether the stream supports reading. This property always returns false.
        /// </summary>
        public override bool CanRead => false;

        /// <summary>
        /// Gets a value indicating whether the stream supports seeking. This property always returns false.
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// Gets a value that indicates whether the stream supports writing.
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        /// Gets the length of the data available on the stream. This property always throws a NotSupportedException.
        /// </summary>
        public override long Length => throw new NotSupportedException();

        /// <summary>
        /// Gets or sets the current position in the stream. This property always throws a NotSupportedException.
        /// </summary>
        public override long Position
        {
            get => throw new NotSupportedException();

            set => throw new NotSupportedException();
        }
        /// <summary>
        /// Not used.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Reads data from the stream. This method always throws a NotSupportedException.
        /// </summary>
        /// <param name="buffer">This parameter is not used.</param>
        /// <param name="offset">This parameter is not used.</param>
        /// <param name="count">This parameter is not used.</param>
        /// <returns></returns>
        public override int Read([In, Out] byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the current position of the stream to the given value. This method always throws a NotSupportedException.
        /// </summary>
        /// <param name="offset">This parameter is not used.</param>
        /// <param name="origin">This parameter is not used.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream. This method always throws a NotSupportedException.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes data to the stream.
        /// </summary>
        /// <param name="buffer">An array of type Byte that contains the data to write to the stream.</param>
        /// <param name="offset">The location in buffer from which to start writing data.</param>
        /// <param name="count">The number of bytes to write to the stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
        }
    }
}
