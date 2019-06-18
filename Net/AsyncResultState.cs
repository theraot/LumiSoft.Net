using System;
using System.Threading;

namespace LumiSoft.Net
{
    /// <summary>
    /// (For internal use only). This class provides holder for IAsyncResult interface and extends it's features.
    /// </summary>
    internal class AsyncResultState : IAsyncResult
    {
        private readonly AsyncCallback _callback;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="asyncObject">Caller's async object.</param>
        /// <param name="asyncDelegate">Delegate which is called asynchronously.</param>
        /// <param name="callback">Callback to call when the connect operation is complete.</param>
        /// <param name="state">User data.</param>
        public AsyncResultState(object asyncObject, Delegate asyncDelegate, AsyncCallback callback, object state)
        {
            AsyncObject = asyncObject;
            AsyncDelegate = asyncDelegate;
            _callback = callback;
            AsyncState = state;
        }

        /// <summary>
        /// Gets delegate which is called asynchronously.
        /// </summary>
        public Delegate AsyncDelegate { get; }

        /// <summary>
        /// Gets or sets caller's async object.
        /// </summary>
        public object AsyncObject { get; }

        /// <summary>
        /// Gets source asynchronous result what we wrap.
        /// </summary>
        public IAsyncResult AsyncResult { get; private set; }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        public object AsyncState { get; }

        /// <summary>
        /// Gets a WaitHandle that is used to wait for an asynchronous operation to complete.
        /// </summary>
        public WaitHandle AsyncWaitHandle => AsyncResult.AsyncWaitHandle;

        /// <summary>
        /// Gets an indication of whether the asynchronous operation completed synchronously.
        /// </summary>
        public bool CompletedSynchronously => AsyncResult.CompletedSynchronously;

        /// <summary>
        /// Gets an indication whether the asynchronous operation has completed.
        /// </summary>
        public bool IsCompleted => AsyncResult.IsCompleted;

        /// <summary>
        /// Gets if the user called the End*() method.
        /// </summary>
        public bool IsEndCalled { get; set; }

        /// <summary>
        /// This method is called by AsyncDelegate when asynchronous operation completes.
        /// </summary>
        public void CompletedCallback()
        {
            _callback?.Invoke(this);
        }

        /// <summary>
        /// Sets AsyncResult value.
        /// </summary>
        /// <param name="asyncResult">Asynchronous result to wrap.</param>
        public void SetAsyncResult(IAsyncResult asyncResult)
        {
            AsyncResult = asyncResult ?? throw new ArgumentNullException(nameof(asyncResult));
        }
    }
}