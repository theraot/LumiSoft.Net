﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP QUOTA response. Defined in RFC 2087 5.1.
    /// </summary>
    public class IMAP_r_u_Quota : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="quotaRootName">Qouta root name.</param>
        /// <param name="entries">Resource limit entries.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>quotaRootName</b> or <b>entries</b> is null reference.</exception>
        public IMAP_r_u_Quota(string quotaRootName,IMAP_Quota_Entry[] entries)
        {
            QuotaRootName = quotaRootName ?? throw new ArgumentNullException("quotaRootName");
            Entries      = entries ?? throw new ArgumentNullException("entries");
        }

        /// <summary>
        /// Parses QUOTA response from quota-response string.
        /// </summary>
        /// <param name="response">QUOTA response string.</param>
        /// <returns>Returns parsed QUOTA response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        public static IMAP_r_u_Quota Parse(string response)
        {
            if(response == null){
                throw new ArgumentNullException("response");
            }

            /* RFC 2087 5.1. QUOTA Response.
                Data:       quota root name
                            list of resource names, usages, and limits

                This response occurs as a result of a GETQUOTA or GETQUOTAROOT
                command. The first string is the name of the quota root for which
                this quota applies.

                The name is followed by a S-expression format list of the resource
                usage and limits of the quota root.  The list contains zero or
                more triplets.  Each triplet conatins a resource name, the current
                usage of the resource, and the resource limit.

                Resources not named in the list are not limited in the quota root.
                Thus, an empty list means there are no administrative resource
                limits in the quota root.

                Example:    S: * QUOTA "" (STORAGE 10 512)
            */

            var r = new StringReader(response);
            // Eat "*"
            r.ReadWord();
            // Eat "QUOTA"
            r.ReadWord();

            var                 name    = r.ReadWord();
            var               items   = r.ReadParenthesized().Split(' ');
            var entries = new List<IMAP_Quota_Entry>();
            for (int i=0;i<items.Length;i+=3){
                entries.Add(new IMAP_Quota_Entry(items[i],Convert.ToInt64(items[i + 1]),Convert.ToInt64(items[i + 2])));
            }

            return new IMAP_r_u_Quota(name,entries.ToArray());
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            // Example:    S: * QUOTA "" (STORAGE 10 512)

            var retVal = new StringBuilder();
            retVal.Append("* QUOTA \"" + QuotaRootName + "\" (");
            for(int i=0;i<Entries.Length;i++){
                if(i > 0){
                    retVal.Append(" ");
                }
                retVal.Append(Entries[i].ResourceName + " " + Entries[i].CurrentUsage + " " + Entries[i].MaxUsage);
            }
            retVal.Append(")\r\n");

            return retVal.ToString();
        }

        /// <summary>
        /// Gets quota root name.
        /// </summary>
        public string QuotaRootName { get; } = "";

        /// <summary>
        /// Gets resource limit entries.
        /// </summary>
        public IMAP_Quota_Entry[] Entries { get; }
    }
}
