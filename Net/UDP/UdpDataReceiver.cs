using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LumiSoft.Net.UDP
{
    /// <summary>
    /// This class implements high performance UDP data receiver.
    /// </summary>
    /// <remarks>NOTE: High performance server applications should create multiple instances of this class per one socket.</remarks>
    public class UdpDataReceiver : IDisposable
    {
        private readonly int _bufferSize = 1400;
        private bool _isDisposed;
        private bool _isRunning;
        private byte[] _buffer;
        private UdpEPacketReceived _eventArgs;
        private Socket _socket;
        private SocketAsyncEventArgs _socketArgs;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="socket">UDP socket.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>socket</b> is null reference.</exception>
        public UdpDataReceiver(Socket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        /// <summary>
        /// Is raised when unhandled error happens.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error;

        /// <summary>
        /// Is raised when when new UDP packet is available.
        /// </summary>
        public event EventHandler<UdpEPacketReceived> PacketReceived;

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            _socket = null;
            _buffer = null;
            if (_socketArgs != null)
            {
                _socketArgs.Dispose();
                _socketArgs = null;
            }
            _eventArgs = null;

            PacketReceived = null;
            Error = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts receiving data.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is disposed and this method is accessed.</exception>
        public void Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;

            var isIoCompletionSupported = NetUtils.IsSocketAsyncSupported();

            _eventArgs = new UdpEPacketReceived();
            _buffer = new byte[_bufferSize];

            if (isIoCompletionSupported)
            {
                _socketArgs = new SocketAsyncEventArgs();
                _socketArgs.SetBuffer(_buffer, 0, _bufferSize);
                _socketArgs.RemoteEndPoint = new IPEndPoint(_socket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
                _socketArgs.Completed += delegate
                {
                    if (_isDisposed)
                    {
                        return;
                    }

                    try
                    {
                        if (_socketArgs.SocketError == SocketError.Success)
                        {
                            OnPacketReceived(_buffer, _socketArgs.BytesTransferred, (IPEndPoint)_socketArgs.RemoteEndPoint);
                        }
                        else
                        {
                            OnError(new Exception("Socket error '" + _socketArgs.SocketError + "'."));
                        }

                        IoCompletionReceive();
                    }
                    catch (Exception x)
                    {
                        OnError(x);
                    }
                };
            }

            // Move processing to thread pool.
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (_isDisposed)
                {
                    return;
                }

                try
                {
                    if (isIoCompletionSupported)
                    {
                        IoCompletionReceive();
                    }
                    else
                    {
                        EndPoint rtpRemoteEp = new IPEndPoint(_socket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
                        _socket.BeginReceiveFrom(
                            _buffer,
                            0,
                            _bufferSize,
                            SocketFlags.None,
                            ref rtpRemoteEp,
                            AsyncSocketReceive,
                            null
                        );
                    }
                }
                catch (Exception x)
                {
                    OnError(x);
                }
            });
        }

        /// <summary>
        /// Is called BeginReceiveFrom has completed.
        /// </summary>
        /// <param name="ar">The result of the asynchronous operation.</param>
        private void AsyncSocketReceive(IAsyncResult ar)
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                var count = _socket.EndReceiveFrom(ar, ref remoteEp);

                OnPacketReceived(_buffer, count, (IPEndPoint)remoteEp);
            }
            catch (Exception x)
            {
                OnError(x);
            }

            try
            {
                // Start receiving new packet.
                EndPoint rtpRemoteEp = new IPEndPoint(_socket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
                _socket.BeginReceiveFrom(
                    _buffer,
                    0,
                    _bufferSize,
                    SocketFlags.None,
                    ref rtpRemoteEp,
                    AsyncSocketReceive,
                    null
                );
            }
            catch (Exception x)
            {
                OnError(x);
            }
        }

        /// <summary>
        /// Receives synchronously(if packet(s) available now) or starts waiting UDP packet asynchronously if no packets at moment.
        /// </summary>
        private void IoCompletionReceive()
        {
            try
            {
                // Use active worker thread as long as ReceiveFromAsync completes synchronously.
                // (With this approach we don't have thread context switches while ReceiveFromAsync completes synchronously)
                while (!_isDisposed && !_socket.ReceiveFromAsync(_socketArgs))
                {
                    if (_socketArgs.SocketError == SocketError.Success)
                    {
                        try
                        {
                            OnPacketReceived(_buffer, _socketArgs.BytesTransferred, (IPEndPoint)_socketArgs.RemoteEndPoint);
                        }
                        catch (Exception x)
                        {
                            OnError(x);
                        }
                    }
                    else
                    {
                        OnError(new Exception("Socket error '" + _socketArgs.SocketError + "'."));
                    }

                    // Reset remote end point.
                    _socketArgs.RemoteEndPoint = new IPEndPoint(_socket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
                }
            }
            catch (Exception x)
            {
                OnError(x);
            }
        }

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="x">Exception happened.</param>
        private void OnError(Exception x)
        {
            if (_isDisposed)
            {
                return;
            }

            Error?.Invoke(this, new ExceptionEventArgs(x));
        }

        /// <summary>
        /// Raises <b>PacketReceived</b> event.
        /// </summary>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="count">Number of bytes stored in <b>buffer</b></param>
        /// <param name="remoteEp">Remote IP end point from where data was received.</param>
        private void OnPacketReceived(byte[] buffer, int count, IPEndPoint remoteEp)
        {
            if (PacketReceived == null)
            {
                return;
            }

            _eventArgs.Reuse(_socket, buffer, count, remoteEp);

            PacketReceived(this, _eventArgs);
        }
    }
}
