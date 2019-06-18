namespace LumiSoft.Net.STUN.Message
{
    // ReSharper disable once InconsistentNaming
    internal class STUN_t_ChangeRequest
    {
        public STUN_t_ChangeRequest(bool changeIP, bool changePort)
        {
            ChangeIP = changeIP;
            ChangePort = changePort;
        }

        public bool ChangeIP { get; }

        public bool ChangePort { get; }
    }
}