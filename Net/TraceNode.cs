#pragma warning disable ET002 // Namespace does not match file path or default namespace

using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Net
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class TraceNode
    {
        internal TraceNode(IPAddress address, IPStatus status, int? ttl)
        {
            Address = address;
            Status = status;
            Ttl = ttl;
        }

        public int? Ttl { get; }

        public IPAddress Address { get; }

        public IPStatus Status { get; }
    }
}