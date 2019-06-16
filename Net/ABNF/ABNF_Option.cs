﻿using System;

namespace LumiSoft.Net.ABNF
{
    /// <summary>
    /// This class represent ABNF "option". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Option : ABNF_Element
    {
        /// <summary>
        /// Gets option alternation elements.
        /// </summary>
        public ABNF_Alternation Alternation { get; private set; }
        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Option Parse(System.IO.StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // option = "[" *c-wsp alternation *c-wsp "]"

            if (reader.Peek() != '[')
            {
                throw new ParseException("Invalid ABNF 'option' value '" + reader.ReadToEnd() + "'.");
            }

            // Eat "[".
            reader.Read();

            // TODO: *c-wsp

            var retVal = new ABNF_Option();

            // We reached end of stream, no closing "]".
            if (reader.Peek() == -1)
            {
                throw new ParseException("Invalid ABNF 'option' value '" + reader.ReadToEnd() + "'.");
            }

            retVal.Alternation = ABNF_Alternation.Parse(reader);

            // We don't have closing ")".
            if (reader.Peek() != ']')
            {
                throw new ParseException("Invalid ABNF 'option' value '" + reader.ReadToEnd() + "'.");
            }

            reader.Read();

            return retVal;
        }
    }
}
