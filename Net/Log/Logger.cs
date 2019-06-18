using System;
using System.Net;
using System.Security.Principal;

namespace LumiSoft.Net.Log
{
    /// <summary>
    /// General logging module.
    /// </summary>
    public class Logger : IDisposable
    {
        /// <summary>
        /// Is raised when new log entry is available.
        /// </summary>
        public event EventHandler<WriteLogEventArgs> WriteLog;

        /// <summary>
        /// Adds exception entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Authenticated user identity.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP endpoint.</param>
        /// <param name="remoteEp">Remote IP endpoint.</param>
        /// <param name="exception">Exception happened.</param>
        public void AddException(string id, GenericIdentity userIdentity, string text, IPEndPoint localEp, IPEndPoint remoteEp, Exception exception)
        {
            OnWriteLog(new LogEntry(LogEntryType.Exception, id, userIdentity, 0, text, localEp, remoteEp, exception));
        }

        /// <summary>
        /// Adds read log entry.
        /// </summary>
        /// <param name="size">Read data size in bytes.</param>
        /// <param name="text">Log text.</param>
        public void AddRead(long size, string text)
        {
            OnWriteLog(new LogEntry(LogEntryType.Read, "", size, text));
        }

        /// <summary>
        /// Adds read log entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Authenticated user identity.</param>
        /// <param name="size">Read data size in bytes.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP endpoint.</param>
        /// <param name="remoteEp">Remote IP endpoint.</param>
        public void AddRead(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEp, IPEndPoint remoteEp)
        {
            OnWriteLog(new LogEntry(LogEntryType.Read, id, userIdentity, size, text, localEp, remoteEp, (byte[])null));
        }

        /// <summary>
        /// Adds read log entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Authenticated user identity.</param>
        /// <param name="size">Read data size in bytes.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP endpoint.</param>
        /// <param name="remoteEp">Remote IP endpoint.</param>
        /// <param name="data">Log data.</param>
        public void AddRead(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEp, IPEndPoint remoteEp, byte[] data)
        {
            OnWriteLog(new LogEntry(LogEntryType.Read, id, userIdentity, size, text, localEp, remoteEp, data));
        }

        /// <summary>
        /// Adds text entry.
        /// </summary>
        /// <param name="text">Log text.</param>
        public void AddText(string text)
        {
            OnWriteLog(new LogEntry(LogEntryType.Text, "", 0, text));
        }

        /// <summary>
        /// Adds text entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="text">Log text.</param>
        public void AddText(string id, string text)
        {
            OnWriteLog(new LogEntry(LogEntryType.Text, id, 0, text));
        }

        /// <summary>
        /// Adds text entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Authenticated user identity.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP endpoint.</param>
        /// <param name="remoteEp">Remote IP endpoint.</param>
        public void AddText(string id, GenericIdentity userIdentity, string text, IPEndPoint localEp, IPEndPoint remoteEp)
        {
            OnWriteLog(new LogEntry(LogEntryType.Text, id, userIdentity, 0, text, localEp, remoteEp, (byte[])null));
        }

        /// <summary>
        /// Add write log entry.
        /// </summary>
        /// <param name="size">Written data size in bytes.</param>
        /// <param name="text">Log text.</param>
        public void AddWrite(long size, string text)
        {
            OnWriteLog(new LogEntry(LogEntryType.Write, "", size, text));
        }

        /// <summary>
        /// Add write log entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Authenticated user identity.</param>
        /// <param name="size">Written data size in bytes.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP endpoint.</param>
        /// <param name="remoteEp">Remote IP endpoint.</param>
        public void AddWrite(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEp, IPEndPoint remoteEp)
        {
            OnWriteLog(new LogEntry(LogEntryType.Write, id, userIdentity, size, text, localEp, remoteEp, (byte[])null));
        }

        /// <summary>
        /// Add write log entry.
        /// </summary>
        /// <param name="id">Log entry ID.</param>
        /// <param name="userIdentity">Authenticated user identity.</param>
        /// <param name="size">Written data size in bytes.</param>
        /// <param name="text">Log text.</param>
        /// <param name="localEp">Local IP endpoint.</param>
        /// <param name="remoteEp">Remote IP endpoint.</param>
        /// <param name="data">Log data.</param>
        public void AddWrite(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEp, IPEndPoint remoteEp, byte[] data)
        {
            OnWriteLog(new LogEntry(LogEntryType.Write, id, userIdentity, size, text, localEp, remoteEp, data));
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Raises WriteLog event.
        /// </summary>
        /// <param name="entry">Log entry.</param>
        private void OnWriteLog(LogEntry entry)
        {
            WriteLog?.Invoke(this, new WriteLogEventArgs(entry));
        }
    }
}
