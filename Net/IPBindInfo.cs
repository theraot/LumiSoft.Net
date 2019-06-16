using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace LumiSoft.Net
{
    /// <summary>
    /// Holds IP bind info.
    /// </summary>
    public class IPBindInfo
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="hostName">Host name.</param>
        /// <param name="protocol">Bind protocol.</param>
        /// <param name="ip">IP address to listen.</param>
        /// <param name="port">Port to listen.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null.</exception>
        public IPBindInfo(string hostName,BindInfoProtocol protocol,IPAddress ip,int port)
        {
            if(ip == null){
                throw new ArgumentNullException("ip");
            }

            HostName  = hostName;
            Protocol  = protocol;
            EndPoint = new IPEndPoint(ip,port);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="hostName">Host name.</param>
        /// <param name="ip">IP address to listen.</param>
        /// <param name="port">Port to listen.</param>
        /// <param name="sslMode">Specifies SSL mode.</param>
        /// <param name="sslCertificate">Certificate to use for SSL connections.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null.</exception>
        public IPBindInfo(string hostName,IPAddress ip,int port,SslMode sslMode,X509Certificate2 sslCertificate) : this(hostName,BindInfoProtocol.TCP,ip,port,sslMode,sslCertificate)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="hostName">Host name.</param>
        /// <param name="protocol">Bind protocol.</param>
        /// <param name="ip">IP address to listen.</param>
        /// <param name="port">Port to listen.</param>
        /// <param name="sslMode">Specifies SSL mode.</param>
        /// <param name="sslCertificate">Certificate to use for SSL connections.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IPBindInfo(string hostName,BindInfoProtocol protocol,IPAddress ip,int port,SslMode sslMode,X509Certificate2 sslCertificate)
        {
            if(ip == null){
                throw new ArgumentNullException("ip");
            }
            
            HostName     = hostName;
            Protocol     = protocol;
            EndPoint    = new IPEndPoint(ip,port);
            SslMode      = sslMode;
            SSL_Certificate = sslCertificate;
            if((sslMode == SslMode.SSL || sslMode == SslMode.TLS) && sslCertificate == null){
                throw new ArgumentException("SSL requested, but argument 'sslCertificate' is not provided.");
            }
        }


        #region override method Equals

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>Returns true if two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if(obj == null){
                return false;
            }
            if(!(obj is IPBindInfo)){
                return false;
            }

            IPBindInfo bInfo = (IPBindInfo)obj;
            if(bInfo.HostName != HostName){
                return false;
            }
            if(bInfo.Protocol != Protocol){
                return false;
            }
            if(!bInfo.EndPoint.Equals(EndPoint)){
                return false;
            }
            if(bInfo.SslMode != SslMode){
                return false;
            }
            if(!X509Certificate.Equals(bInfo.Certificate,SSL_Certificate)){
                return false;
            }

            return true;
        }

        #endregion

        #region override method GetHashCode

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns>Returns the hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets host name.
        /// </summary>
        public string HostName { get; } = "";

        /// <summary>
        /// Gets protocol.
        /// </summary>
        public BindInfoProtocol Protocol { get; } = BindInfoProtocol.TCP;

        /// <summary>
        /// Gets IP end point.
        /// </summary>
        public IPEndPoint EndPoint { get; }

        /// <summary>
        /// Gets IP address.
        /// </summary>
        public IPAddress IP
        {
            get{ return EndPoint.Address; }
        }

        /// <summary>
        /// Gets port.
        /// </summary>
        public int Port
        {
            get{ return EndPoint.Port; }
        }

        /// <summary>
        /// Gets SSL mode.
        /// </summary>
        public SslMode SslMode { get; } = SslMode.None;

        /// <summary>
        /// Gets SSL certificate.
        /// </summary>
        [Obsolete("Use property Certificate instead.")]
        public X509Certificate2 SSL_Certificate { get; }

        /// <summary>
        /// Gets SSL certificate.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get{ return SSL_Certificate; }
        }


        /// <summary>
        /// Gets or sets user data. This is used internally don't use it !!!.
        /// </summary>
        public object Tag { get; set; }

#endregion

    }
}
