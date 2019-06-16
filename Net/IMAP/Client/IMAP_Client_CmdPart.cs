using System;

namespace LumiSoft.Net.IMAP.Client
{
    /// <summary>
    /// This class represent IMAP command part.
    /// </summary>
    /// <remarks>
    /// Complete command consits of multiple parts.
    /// </remarks>
    internal class IMAP_Client_CmdPart
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">Command part type.</param>
        /// <param name="data">Command data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>data</b> is null reference.</exception>
        public IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type type,string data)
        {
            if(data == null){
                throw new ArgumentNullException("data");
            }

            Type  = type;
            Value = data;
        }


        #region Properties implementation

        /// <summary>
        /// Gets command part ype.
        /// </summary>
        public IMAP_Client_CmdPart_Type Type { get; } = IMAP_Client_CmdPart_Type.Constant;

        /// <summary>
        /// Gets command part string value.
        /// </summary>
        public string Value { get; }

#endregion
    }
}
