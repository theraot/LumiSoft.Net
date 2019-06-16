namespace LumiSoft.Net.UPnP.NAT
{
    /// <summary>
    /// This class represents NAT port mapping entry.
    /// </summary>
    public class UPnP_NAT_Map
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="enabled">Specifies if NAT port map is enabled.</param>
        /// <param name="protocol">Port mapping protocol. Nomrally this value TCP or UDP.</param>
        /// <param name="remoteHost">Remote host IP address. NOTE: Some implementations may use wilcard(*,?) values.</param>
        /// <param name="externalPort">NAT external port number. NOTE: Some implementations may use wilcard(*,?) values.</param>
        /// <param name="internalHost">Internal host IP address.</param>
        /// <param name="internalPort">Internal host port number.</param>
        /// <param name="description">NAT port mapping description.</param>
        /// <param name="leaseDuration">Lease duration in in seconds. Value null means "never expires".</param>
        public UPnP_NAT_Map(bool enabled,string protocol,string remoteHost,string externalPort,string internalHost,int internalPort,string description,int leaseDuration)
        {
            Enabled       = enabled;
            Protocol      = protocol;
            RemoteHost    = remoteHost;
            ExternalPort  = externalPort;
            InternalHost  = internalHost;
            InternalPort  = internalPort;
            Description   = description;
            LeaseDuration = leaseDuration;
        }

        /// <summary>
        /// Gets if NAT port map is enabled.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// Gets port mapping protocol. Nomrally this value TCP or UDP.
        /// </summary>
        public string Protocol { get; } = "";

        /// <summary>
        /// Gets remote host IP address. NOTE: Some implementations may use wilcard(*,?) values.
        /// </summary>
        public string RemoteHost { get; } = "";

        /// <summary>
        /// Gets NAT external port number. NOTE: Some implementations may use wilcard(*,?) values.
        /// </summary>
        public string ExternalPort { get; } = "";

        /// <summary>
        /// Gets internal host IP address.
        /// </summary>
        public string InternalHost { get; } = "";

        /// <summary>
        /// Gets internal host port number.
        /// </summary>
        public int InternalPort { get; }

        /// <summary>
        /// Gets NAT port mapping description.
        /// </summary>
        public string Description { get; } = "";

        /// <summary>
        /// Gets lease duration in in seconds. Value null means "never expires".
        /// </summary>
        public int LeaseDuration { get; }
    }
}
