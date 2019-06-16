﻿namespace LumiSoft.Net
{
    /// <summary>
    /// This is base class for asynchronous operation.
    /// </summary>
    public abstract class AsyncOP
    {

        /// <summary>
        /// Gets if asynchronous operation has completed.
        /// </summary>
        public abstract bool IsCompleted
        {
            get;
        }

        /// <summary>
        /// Gets if operation completed synchronously.
        /// </summary>
        public abstract bool IsCompletedSynchronously
        {
            get;
        }
        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public abstract bool IsDisposed
        {
            get;
        }
    }
}
