using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net.UDP
{
    /// <summary>
    /// This class implements generic UDP server.
    /// </summary>
    public class UdpServer : IDisposable
    {
        private readonly long _bytesReceived = 0;
        private long _bytesSent;
        private bool _isRunning;
        private int _mtu = 1400;
        private readonly long _acketsReceived = 0;
        private long _acketsSent;
        private IPEndPoint[] _bindings;
        private List<UdpDataReceiver> _dataReceivers;
        private CircleCollection<Socket> _sendSocketsIPv4;
        private CircleCollection<Socket> _sendSocketsIPv6;
        private List<Socket> _sockets;
        private readonly int _receiversPerSocket = 10;
        private DateTime _startTime;

        /// <summary>
        /// This event is raised when unexpected error happens.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// This event is raised when new UDP packet received.
        /// </summary>
        public event EventHandler<UdpEPacketReceived> PacketReceived;

        /// <summary>
        /// Gets or sets IP end point where UDP server is bound.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public IPEndPoint[] Bindings
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }

                return _bindings;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                // See if changed. Also if server running we must restart it.
                var changed = false;
                if (_bindings == null)
                {
                    changed = true;
                }
                else if (_bindings.Length != value.Length)
                {
                    changed = true;
                }
                else
                {
                    for (var i = 0; i < _bindings.Length; i++)
                    {
                        if (_bindings[i].Equals(value[i]))
                        {
                            continue;
                        }

                        changed = true;
                        break;
                    }
                }

                if (!changed)
                {
                    return;
                }

                _bindings = value;
                Restart();
            }
        }

        /// <summary>
        /// Gets how many bytes this UDP server has received since start.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this property is accessed.</exception>
        public long BytesReceived
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (!_isRunning)
                {
                    throw new InvalidOperationException("UDP server is not running.");
                }

                return _bytesReceived;
            }
        }

        /// <summary>
        /// Gets how many bytes this UDP server has sent since start.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this property is accessed.</exception>
        public long BytesSent
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (!_isRunning)
                {
                    throw new InvalidOperationException("UDP server is not running.");
                }

                return _bytesSent;
            }
        }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets if UDP server is running.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsRunning
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }

                return _isRunning;
            }
        }

        /// <summary>
        /// Gets or sets maximum network transmission unit.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when server is running and this property value is tried to set.</exception>
        public int Mtu
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }

                return _mtu;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (_isRunning)
                {
                    throw new InvalidOperationException("MTU value can be changed only if UDP server is not running.");
                }

                _mtu = value;
            }
        }

        /// <summary>
        /// Gets how many UDP packets this UDP server has received since start.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this property is accessed.</exception>
        public long PacketsReceived
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (!_isRunning)
                {
                    throw new InvalidOperationException("UDP server is not running.");
                }

                return _acketsReceived;
            }
        }

        /// <summary>
        /// Gets how many UDP packets this UDP server has sent since start.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this property is accessed.</exception>
        public long PacketsSent
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (!_isRunning)
                {
                    throw new InvalidOperationException("UDP server is not running.");
                }

                return _acketsSent;
            }
        }

        /// <summary>
        /// Gets time when server was started.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this property is accessed.</exception>
        public DateTime StartTime
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UdpServer));
                }
                if (!_isRunning)
                {
                    throw new InvalidOperationException("UDP server is not running.");
                }

                return _startTime;
            }
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = false;
            Stop();
            // Release all events.
            Error = null;
            PacketReceived = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets suitable local IP end point for the specified remote endpoint.
        /// If there are multiple sending local end points, they will be load-balanced with round-robin.
        /// </summary>
        /// <param name="remoteEp">Remote end point.</param>
        /// <returns>Returns local IP end point.</returns>
        /// <exception cref="ArgumentNullException">Is raised when argument <b>remoteEP</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when argument <b>remoteEP</b> has invalid value.</exception>
        /// <exception cref="InvalidOperationException">Is raised when no suitable IPv4 or IPv6 socket for <b>remoteEP</b>.</exception>
        public IPEndPoint GetLocalEndPoint(IPEndPoint remoteEp)
        {
            if (remoteEp == null)
            {
                throw new ArgumentNullException(nameof(remoteEp));
            }

            switch (remoteEp.AddressFamily)
            {
                case AddressFamily.InterNetwork when _sendSocketsIPv4.Count == 0:
                    // We don't have any IPv4 local end point.
                    throw new InvalidOperationException("There is no suitable IPv4 local end point in this.Bindings.");
                case AddressFamily.InterNetwork:
                    return (IPEndPoint)_sendSocketsIPv4.Next().LocalEndPoint;
                case AddressFamily.InterNetworkV6 when _sendSocketsIPv6.Count == 0:
                    // We don't have any IPv6 local end point.
                    throw new InvalidOperationException("There is no suitable IPv6 local end point in this.Bindings.");
                case AddressFamily.InterNetworkV6:
                    return (IPEndPoint)_sendSocketsIPv6.Next().LocalEndPoint;
                default:
                    throw new ArgumentException("Argument 'remoteEP' has unknown AddressFamily.");
            }
        }

        /// <summary>
        /// Restarts running server. If server is not running, this methods has no effect.
        /// </summary>
        public void Restart()
        {
            if (!_isRunning)
            {
                return;
            }

            Stop();
            Start();
        }

        /// <summary>
        /// Sends specified UDP packet to the specified remote end point.
        /// </summary>
        /// <param name="packet">UDP packet to send.</param>
        /// <param name="count">Number of bytes to send.</param>
        /// <param name="remoteEp">Remote end point.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when any of the arguments is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void SendPacket(byte[] packet, int count, IPEndPoint remoteEp)
        {
            SendPacket(packet, count, remoteEp, out _);
        }

        /// <summary>
        /// Sends specified UDP packet to the specified remote end point.
        /// </summary>
        /// <param name="packet">UDP packet to send.</param>
        /// <param name="count">Number of bytes to send.</param>
        /// <param name="remoteEp">Remote end point.</param>
        /// <param name="localEp">Returns local IP end point which was used to send UDP packet.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when any of the arguments is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void SendPacket(byte[] packet, int count, IPEndPoint remoteEp, out IPEndPoint localEp)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(UdpServer));
            }
            if (!_isRunning)
            {
                throw new InvalidOperationException("UDP server is not running.");
            }
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }
            if (remoteEp == null)
            {
                throw new ArgumentNullException(nameof(remoteEp));
            }

            localEp = null;
            SendPacket(null, packet, count, remoteEp, out localEp);
        }

        /// <summary>
        /// Sends specified UDP packet to the specified remote end point.
        /// </summary>
        /// <param name="localEp">Local end point to use for sending.</param>
        /// <param name="packet">UDP packet to send.</param>
        /// <param name="count">Number of bytes to send.</param>
        /// <param name="remoteEp">Remote end point.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when any of the arguments is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void SendPacket(IPEndPoint localEp, byte[] packet, int count, IPEndPoint remoteEp)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(UdpServer));
            }
            if (!_isRunning)
            {
                throw new InvalidOperationException("UDP server is not running.");
            }
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }
            if (localEp == null)
            {
                throw new ArgumentNullException(nameof(localEp));
            }
            if (remoteEp == null)
            {
                throw new ArgumentNullException(nameof(remoteEp));
            }
            if (localEp.AddressFamily != remoteEp.AddressFamily)
            {
                throw new ArgumentException("Argumnet localEP and remoteEP AddressFamily won't match.");
            }

            // Search specified local end point socket.
            Socket socket = null;
            switch (localEp.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                {
                    foreach (var s in _sendSocketsIPv4.ToArray())
                    {
                        if (!localEp.Equals((IPEndPoint) s.LocalEndPoint))
                            {
                                continue;
                            }

                            socket = s;
                        break;
                    }

                    break;
                }

                case AddressFamily.InterNetworkV6:
                {
                    foreach (var s in _sendSocketsIPv6.ToArray())
                    {
                        if (!localEp.Equals((IPEndPoint) s.LocalEndPoint))
                            {
                                continue;
                            }

                            socket = s;
                        break;
                    }

                    break;
                }

                default:
                    throw new ArgumentException("Argument 'localEP' has unknown AddressFamily.");
            }

            // We don't have specified local end point.
            if (socket == null)
            {
                throw new ArgumentException("Specified local end point '" + localEp + "' doesn't exist.");
            }

            SendPacket(socket, packet, count, remoteEp, out _);
        }

        /// <summary>
        /// Starts UDP server.
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;

            _startTime = DateTime.Now;
            _dataReceivers = new List<UdpDataReceiver>();

            // Run only if we have some listening point.
            if (_bindings == null)
            {
                return;
            }
            // We must replace IPAddress.Any to all available IPs, otherwise it's impossible to send
            // reply back to UDP packet sender on same local EP where packet received. This is very
            // important when clients are behind NAT.
            var listeningEPs = new List<IPEndPoint>();
            foreach (var ep in _bindings)
            {
                if (ep.Address.Equals(IPAddress.Any))
                {
                    // Add localhost.
                    var epLocalhost = new IPEndPoint(IPAddress.Loopback, ep.Port);
                    if (!listeningEPs.Contains(epLocalhost))
                    {
                        listeningEPs.Add(epLocalhost);
                    }
                    // Add all host IPs.
                    foreach (var ip in Dns.GetHostAddresses(""))
                    {
                        var epNew = new IPEndPoint(ip, ep.Port);
                        if (!listeningEPs.Contains(epNew))
                        {
                            listeningEPs.Add(epNew);
                        }
                    }
                }
                else
                {
                    if (!listeningEPs.Contains(ep))
                    {
                        listeningEPs.Add(ep);
                    }
                }
            }

            // Create sockets.
            _sockets = new List<Socket>();
            foreach (var ep in listeningEPs)
            {
                try
                {
                    var socket = NetUtils.CreateSocket(ep, ProtocolType.Udp);
                    _sockets.Add(socket);

                    // Create UDP data receivers.
                    for (var i = 0; i < _receiversPerSocket; i++)
                    {
                        var receiver = new UdpDataReceiver(socket);
                        receiver.PacketReceived += delegate (object s, UdpEPacketReceived e)
                        {
                            try
                            {
                                ProcessUdpPacket(e);
                            }
                            catch (Exception x)
                            {
                                OnError(x);
                            }
                        };
                        receiver.Error += delegate (object s, ExceptionEventArgs e)
                        {
                            OnError(e.Exception);
                        };
                        _dataReceivers.Add(receiver);
                        receiver.Start();
                    }
                }
                catch (Exception x)
                {
                    OnError(x);
                }
            }

            // Create round-robin send sockets. NOTE: We must skip localhost, it can't be used
            // for sending out of server.
            _sendSocketsIPv4 = new CircleCollection<Socket>();
            _sendSocketsIPv6 = new CircleCollection<Socket>();
            foreach (var socket in _sockets)
            {
                if (((IPEndPoint)socket.LocalEndPoint).AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!((IPEndPoint)socket.LocalEndPoint).Address.Equals(IPAddress.Loopback))
                    {
                        _sendSocketsIPv4.Add(socket);
                    }
                }
                else if (((IPEndPoint)socket.LocalEndPoint).AddressFamily == AddressFamily.InterNetworkV6)
                {
                    _sendSocketsIPv6.Add(socket);
                }
            }
        }

        /// <summary>
        /// Stops UDP server.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }
            _isRunning = false;

            foreach (var receiver in _dataReceivers)
            {
                receiver.Dispose();
            }
            _dataReceivers = null;
            foreach (var socket in _sockets)
            {
                socket.Close();
            }
            _sockets = null;
            _sendSocketsIPv4 = null;
            _sendSocketsIPv6 = null;
        }

        /// <summary>
        /// Sends specified UDP packet to the specified remote end point.
        /// </summary>
        /// <param name="socket">UDP socket to use for data sending.</param>
        /// <param name="packet">UDP packet to send.</param>
        /// <param name="count">Number of bytes to send.</param>
        /// <param name="remoteEp">Remote end point.</param>
        /// <param name="localEp">Returns local IP end point which was used to send UDP packet.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when UDP server is not running and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when any of the arguments is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        internal void SendPacket(Socket socket, byte[] packet, int count, IPEndPoint remoteEp, out IPEndPoint localEp)
        {
            // Round-Robin all local end points, if no end point specified.
            if (socket == null)
            {
                // Get right IP address family socket which matches remote end point.
                if (remoteEp.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (_sendSocketsIPv4.Count == 0)
                    {
                        throw new ArgumentException("There is no suitable IPv4 local end point in this.Bindings.");
                    }

                    socket = _sendSocketsIPv4.Next();
                }
                else if (remoteEp.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    if (_sendSocketsIPv6.Count == 0)
                    {
                        throw new ArgumentException("There is no suitable IPv6 local end point in this.Bindings.");
                    }

                    socket = _sendSocketsIPv6.Next();
                }
                else
                {
                    throw new ArgumentException("Invalid remote end point address family.");
                }
            }

            // Send packet.
            socket.SendTo(packet, 0, count, SocketFlags.None, remoteEp);

            localEp = (IPEndPoint)socket.LocalEndPoint;

            _bytesSent += count;
            _acketsSent++;
        }

        /// <summary>
        /// Raises Error event.
        /// </summary>
        /// <param name="x">Exception occured.</param>
        private void OnError(Exception x)
        {
            Error?.Invoke(this, new ErrorEventArgs(x, new System.Diagnostics.StackTrace()));
        }

        /// <summary>
        /// Raises PacketReceived event.
        /// </summary>
        /// <param name="e">Event data.</param>
        private void OnUdpPacketReceived(UdpEPacketReceived e)
        {
            PacketReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Processes specified incoming UDP packet.
        /// </summary>
        /// <param name="e">Packet event data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>e</b> is null reference.</exception>
        private void ProcessUdpPacket(UdpEPacketReceived e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            OnUdpPacketReceived(e);
        }
    }
}
