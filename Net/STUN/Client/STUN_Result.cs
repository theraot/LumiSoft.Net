using System.Net;

namespace LumiSoft.Net.STUN.Client
{
    public class STUN_Result
    {
        public STUN_Result(STUN_NetType netType, IPEndPoint publicEndPoint)
        {
            NetType = netType;
            PublicEndPoint = publicEndPoint;
        }

        public STUN_NetType NetType { get; }

        public IPEndPoint PublicEndPoint { get; }
    }
}