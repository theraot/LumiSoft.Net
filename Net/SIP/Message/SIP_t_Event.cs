using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "Event" value. Defined in RFC 3265.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3265 Syntax:
    ///     Event       = event-type *( SEMI event-param )
    ///     event-param = generic-param / ( "id" EQUAL token )
    /// </code>
    /// </remarks>
    public class SIP_t_Event : SIP_t_ValueWithParams
    {
        private string m_EventType = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP 'Event' value.</param>
        public SIP_t_Event(string value)
        {
            Parse(value);
        }

        /// <summary>
        /// Parses "Event" from specified value.
        /// </summary>
        /// <param name="value">SIP "Event" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>value</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "Event" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                Event       = event-type *( SEMI event-param )
                event-param = generic-param / ( "id" EQUAL token )
            */

            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            // event-type
            var word = reader.ReadWord();
            m_EventType = word ?? throw new SIP_ParseException("SIP Event 'event-type' value is missing !");

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "Event" value.
        /// </summary>
        /// <returns>Returns "Event" value.</returns>
        public override string ToStringValue()
        {
            /*
                Event       = event-type *( SEMI event-param )
                event-param = generic-param / ( "id" EQUAL token )
            */

            var retVal = new StringBuilder();

            // event-type
            retVal.Append(m_EventType);

            // Add parameters
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }

        /// <summary>
        /// Gets or sets event type.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null vallue is passed.</exception>
        /// <exception cref="ArgumentException">Is raised when emptu string passed.</exception>
        public string EventType
        {
            get{ return m_EventType; }

            set{
                if(value == null){
                    throw new ArgumentNullException("EventType");
                }
                if(value == ""){
                    throw new ArgumentException("Property EventType value can't be '' !");
                }

                m_EventType = value;
            }
        }

        /// <summary>
        /// Gets or sets 'id' parameter value. Value null means not specified.
        /// </summary>
        public string ID
        {
            get{ 
                var parameter = Parameters["id"];
                if (parameter != null){
                    return parameter.Value;
                }

                return null;
            }

            set{                
                if(string.IsNullOrEmpty(value)){
                    Parameters.Remove("id");
                }
                else{
                    Parameters.Set("id",value);
                }
            }
        }
    }
}
