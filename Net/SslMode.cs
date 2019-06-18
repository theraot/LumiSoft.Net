namespace LumiSoft.Net
{
    /// <summary>
    /// This enum holds SSL modes.
    /// </summary>
    public enum SslMode
    {
        /// <summary>
        /// No SSL is used.
        /// </summary>
        None,

        /// <summary>
        /// Connection is SSL.
        /// </summary>
        Ssl,

        /// <summary>
        /// Connection will be switched to SSL with start TLS.
        /// </summary>
        Tls
    }
}
