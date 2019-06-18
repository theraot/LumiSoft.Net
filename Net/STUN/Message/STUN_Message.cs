using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LumiSoft.Net.STUN.Message
{
    class STUN_Message
    {
        public IPEndPoint SourceAddress { get; internal set; }
        public STUN_MessageType Type { get; set; }
        public STUN_t_ChangeRequest ChangeRequest { get; set; }
        public object MappedAddress { get; set; }
        public IPEndPoint ChangedAddress { get; set; }
    }
}
