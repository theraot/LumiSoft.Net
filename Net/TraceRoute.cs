#pragma warning disable ET002 // Namespace does not match file path or default namespace

using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Net
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class TraceRoute
    {
        public static void Trace(IPAddress destination, Func<IPAddress, TraceNode, bool> next, Action completed)
        {
            const int bufferSize = 32;
            const int timeoutMilliseconds = 1000;

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (completed == null)
            {
                completed = () => { };
            }
            var buffer = new byte[bufferSize];
            var options = new PingOptions(1, true);
            var pings = new []{new Ping()};
            pings[0].PingCompleted += OnPingCompleted;
            pings[0].SendAsync(destination, timeoutMilliseconds, buffer, options, null);

            void OnPingCompleted(object sender, PingCompletedEventArgs e)
            {
                var reply = e.Reply;
                var address = reply.Address;
                var status = reply.Status;
                var done = reply != null
                           && (
                               status == IPStatus.TimedOut
                               || !next.Invoke(destination, new TraceNode(address, status, reply.Options?.Ttl))
                               || status == IPStatus.Success
                               || address.Equals(destination)
                           );
                if (done)
                {
                    pings[0].Dispose();
                    pings[0] = null;
                    completed();
                    completed = null;
                }
                else
                {
                    options.Ttl++;
                    pings[0].SendAsync(destination, timeoutMilliseconds, buffer, options, null);
                }
            }
        }
    }
}