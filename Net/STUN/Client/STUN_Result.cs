using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LumiSoft.Net.STUN.Client
{
    public class STUN_Result
    {
        private object udpBlocked;
        private object p;

        public STUN_Result(object udpBlocked, object p)
        {
            this.udpBlocked = udpBlocked;
            this.p = p;
        }

        public IPEndPoint PublicEndPoint { get; set; }
    }
}
