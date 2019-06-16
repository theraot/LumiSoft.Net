using System;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class holds MAIL FROM: command value.
    /// </summary>
    public class SMTP_MailFrom
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailbox">Mailbox value.</param>
        /// <param name="size">SIZE parameter value.</param>
        /// <param name="body">BODY parameter value.</param>
        /// <param name="ret">DSN RET parameter value.</param>
        /// <param name="envid">DSN ENVID parameter value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>mailbox</b> is null reference.</exception>
        public SMTP_MailFrom(string mailbox,int size,string body,SMTP_DSN_Ret ret,string envid)
        {
            if(mailbox == null){
                throw new ArgumentNullException("mailbox");
            }

            Mailbox = mailbox;
            Size    = size;
            Body    = body;
            RET     = ret;
            ENVID   = envid;
        }


        /// <summary>
        /// Gets SMTP "mailbox" value. Actually this is just email address.
        /// This value can be "" if "null reverse-path".
        /// </summary>
        public string Mailbox { get; } = "";

        /// <summary>
        /// Gets MAIL FROM: SIZE parameter value. Value -1 means not specified.
        /// Defined in RFC 1870.
        /// </summary>
        public int Size { get; } = -1;

        /// <summary>
        /// Gets MAIL FROM: BODY parameter value. Value null means not specified.
        /// Defined in RFC 1652.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets DSN RET parameter value. Value null means not specified.
        /// RET specifies whether message or headers should be included in any failed DSN issued for message transmission.
        /// Defined in RFC 1891.
        /// </summary>
        public SMTP_DSN_Ret RET { get; } = SMTP_DSN_Ret.NotSpecified;

        /// <summary>
        /// Gets DSN ENVID parameter value. Value null means not specified.
        /// Defined in RFC 1891.
        /// </summary>
        public string ENVID { get; }
    }
}
