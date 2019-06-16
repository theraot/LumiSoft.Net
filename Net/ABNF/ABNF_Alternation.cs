﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.ABNF
{
    /// <summary>
    /// This class represent ABNF "alternation". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Alternation
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABNF_Alternation()
        {
            Items = new List<ABNF_Concatenation>();
        }

        /// <summary>
        /// Gets alternation items.
        /// </summary>
        public List<ABNF_Concatenation> Items { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Alternation Parse(System.IO.StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // alternation = concatenation *(*c-wsp "/" *c-wsp concatenation)

            var retVal = new ABNF_Alternation();

            while (true)
            {
                var item = ABNF_Concatenation.Parse(reader);
                if (item != null)
                {
                    retVal.Items.Add(item);
                }

                // We reached end of string.
                if (reader.Peek() == -1)
                {
                    break;
                }
                // We have next alternation item.

                if (reader.Peek() == '/')
                {
                    reader.Read();
                }
                // We have unexpected value, probably alternation ends.
                else
                {
                    break;
                }
            }

            return retVal;
        }
    }
}
