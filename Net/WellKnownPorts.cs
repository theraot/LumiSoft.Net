namespace LumiSoft.Net
{
    /// <summary>
    /// This class provides well known TCP/UDP service ports.
    /// </summary>
    public static class WellKnownPorts
    {
        /// <summary>
        /// DNS protocol.
        /// </summary>
        public static readonly int Dns = 53;

        /// <summary>
        /// FTP - control (command) port.
        /// </summary>
        public static readonly int FtpControl = 21;

        /// <summary>
        /// FTP over SSL protocol.
        /// </summary>
        public static readonly int FtpControlSsl = 990;

        /// <summary>
        /// FTP - data port.
        /// </summary>
        public static readonly int FtpData = 20;

        /// <summary>
        /// HTTP protocol.
        /// </summary>
        public static readonly int Http = 80;

        /// <summary>
        /// HTTPS protocol.
        /// </summary>
        public static readonly int Https = 443;

        /// <summary>
        /// IMAP4 protocol.
        /// </summary>
        public static readonly int Imap4 = 143;

        /// <summary>
        /// IMAP4 over SSL protocol.
        /// </summary>
        public static readonly int Imap4Ssl = 993;

        /// <summary>
        /// NNTP (Network News Transfer Protocol)  protocol.
        /// </summary>
        public static readonly int Nntp = 119;

        /// <summary>
        /// NTP (Network Time Protocol) protocol.
        /// </summary>
        public static readonly int Ntp = 123;

        /// <summary>
        /// POP3 protocol.
        /// </summary>
        public static readonly int Pop3 = 110;

        /// <summary>
        /// POP3 over SSL protocol.
        /// </summary>
        public static readonly int Pop3Ssl = 995;

        /// <summary>
        /// SMTP protocol.
        /// </summary>
        public static readonly int Smtp = 25;

        /// <summary>
        /// SMTP over SSL protocol.
        /// </summary>
        public static readonly int SmtpSsl = 465;
    }
}
