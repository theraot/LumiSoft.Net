using System;

namespace LumiSoft.Net.ABNF
{
    /// <summary>
    /// This class represent ABNF "group". Defined in RFC 5234 4.
    /// </summary>
    public class ABFN_Group : ABNF_Element
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABFN_Group()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABFN_Group Parse(System.IO.StringReader reader)
        {
            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            // group = "(" *c-wsp alternation *c-wsp ")"

            if(reader.Peek() != '('){
                throw new ParseException("Invalid ABNF 'group' value '" + reader.ReadToEnd() + "'.");
            }

            // Eat "(".
            reader.Read();

            // TODO: *c-wsp

            ABFN_Group retVal = new ABFN_Group();

            // We reached end of stream, no closing ")".
            if(reader.Peek() == -1){
                throw new ParseException("Invalid ABNF 'group' value '" + reader.ReadToEnd() + "'.");
            }
         
            retVal.Alternation = ABNF_Alternation.Parse(reader);

            // We don't have closing ")".
            if(reader.Peek() != ')'){
                throw new ParseException("Invalid ABNF 'group' value '" + reader.ReadToEnd() + "'."); 
            }
            else{
                reader.Read();
            }

            return retVal;
        }


        /// <summary>
        /// Gets option alternation elements.
        /// </summary>
        public ABNF_Alternation Alternation { get; private set; }
    }
}
