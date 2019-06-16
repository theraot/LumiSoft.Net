﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.ABNF
{
    /// <summary>
    /// This class represent ABNF "concatenation". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Concatenation
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABNF_Concatenation()
        {
            Items = new List<ABNF_Repetition>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Concatenation Parse(System.IO.StringReader reader)
        {
            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            // concatenation  =  repetition *(1*c-wsp repetition)
            // repetition     =  [repeat] element

            ABNF_Concatenation retVal = new ABNF_Concatenation();
            
            while(true){
                ABNF_Repetition item = ABNF_Repetition.Parse(reader);
                if(item != null){
                    retVal.Items.Add(item);
                }
                // We reached end of string.
                else if(reader.Peek() == -1){
                    break;
                }
                // We have next concatenation item.
                else if(reader.Peek() == ' '){
                    reader.Read();
                }
                // We have unexpected value, probably concatenation ends.
                else{
                    break;
                }
            }

            return retVal;
        }


        /// <summary>
        /// Gets concatenation items.
        /// </summary>
        public List<ABNF_Repetition> Items { get; }
    }
}
