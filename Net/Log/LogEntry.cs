using System;
using System.Net;
using System.Security.Principal;

namespace LumiSoft.Net.Log
{
    /// <summary>
    /// Implements log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">Log entry type.</param>
        /// <param name="id">Log entry ID.</param>
        /// <param name="size">Specified how much data was read or written.</param>
        /// <param name="text">Description text.</param>
        public LogEntry(LogEntryType type, string id, long size, string text)
        {
            EntryType = type;
            Id = id;
            Size = size;
            Text = text;

            Time = DateTime.Now;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">Log entry type.</param>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Log entry owner user or null if none.</param>
        /// <param name="size">Log entry read/write size in bytes.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP end point.</param>
        /// <param name="remoteEp">Remote IP end point.</param>
        /// <param name="data">Log data.</param>
        public LogEntry(LogEntryType type, string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEp, IPEndPoint remoteEp, byte[] data)
        {
            EntryType = type;
            Id = id;
            UserIdentity = userIdentity;
            Size = size;
            Text = text;
            LocalEndPoint = localEp;
            RemoteEndPoint = remoteEp;
            Data = data;

            Time = DateTime.Now;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">Log entry type.</param>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Log entry owner user or null if none.</param>
        /// <param name="size">Log entry read/write size in bytes.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP end point.</param>
        /// <param name="remoteEp">Remote IP end point.</param>
        /// <param name="exception">Exception happened. Can be null.</param>
        public LogEntry(LogEntryType type, string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEp, IPEndPoint remoteEp, Exception exception)
        {
            EntryType = type;
            Id = id;
            UserIdentity = userIdentity;
            Size = size;
            Text = text;
            LocalEndPoint = localEp;
            RemoteEndPoint = remoteEp;
            Exception = exception;

            Time = DateTime.Now;
        }

        /// <summary>
        /// Gets log data. Value null means no log data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Gets log entry type.
        /// </summary>
        public LogEntryType EntryType { get; }

        /// <summary>
        /// Gets exception happened. This property is available only if LogEntryType.Exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets log entry ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets local IP end point. Value null means no local end point.
        /// </summary>
        public IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets remote IP end point. Value null means no remote end point.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets how much data was read or written, depends on <b>LogEntryType</b>.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Gets describing text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets time when log entry was created.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Gets log entry related user identity.
        /// </summary>
        public GenericIdentity UserIdentity { get; }
    }
}
