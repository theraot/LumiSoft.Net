using System;
using System.IO;
using System.Text;
using System.Net;

using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
    /// <summary>
    /// SIP server request. Related RFC 3261.
    /// </summary>
    public class SIP_Request : SIP_Message
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="method">SIP request method.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>method</b> is null reference.</exception>
        public SIP_Request(string method)
        {
            if(method == null){
                throw new ArgumentNullException("method");
            }

            RequestLine = new SIP_RequestLine(method,new AbsoluteUri());
        }

        /// <summary>
        /// Clones this request.
        /// </summary>
        /// <returns>Returns new cloned request.</returns>
        public SIP_Request Copy()
        {
            SIP_Request retVal = SIP_Request.Parse(this.ToByteData());
            retVal.Flow           = Flow;
            retVal.LocalEndPoint  = LocalEndPoint;
            retVal.RemoteEndPoint = RemoteEndPoint;

            return retVal;
        }

        /// <summary>
        /// Checks if SIP request has all required values as request line,header fields and their values.
        /// Throws Exception if not valid SIP request.
        /// </summary>
        public void Validate()
        {
            // Request SIP version
            // Via: + branch prameter
            // To:
            // From:
            // CallID:
            // CSeq
            // Max-Forwards RFC 3261 8.1.1.

            if(!this.RequestLine.Version.ToUpper().StartsWith("SIP/2.0")){
                throw new SIP_ParseException("Not supported SIP version '" + this.RequestLine.Version + "' !");
            }

            if(this.Via.GetTopMostValue() == null){
                throw new SIP_ParseException("Via: header field is missing !");
            }
            if(this.Via.GetTopMostValue().Branch == null){
                throw new SIP_ParseException("Via: header field branch parameter is missing !");
            }

            if(this.To == null){
                throw new SIP_ParseException("To: header field is missing !");
            }

            if(this.From == null){
                throw new SIP_ParseException("From: header field is missing !");
            }

            if(this.CallID == null){
                throw new SIP_ParseException("CallID: header field is missing !");
            }

            if(this.CSeq == null){
                throw new SIP_ParseException("CSeq: header field is missing !");
            }

            if(this.MaxForwards == -1){
                // We can fix it by setting it to default value 70.
                this.MaxForwards = 70;
            }

            /* RFC 3261 12.1.2
                When a UAC sends a request that can establish a dialog (such as an INVITE) it MUST 
                provide a SIP or SIPS URI with global scope (i.e., the same SIP URI can be used in 
                messages outside this dialog) in the Contact header field of the request. If the 
                request has a Request-URI or a topmost Route header field value with a SIPS URI, the
                Contact header field MUST contain a SIPS URI.
            */
            if(SIP_Utils.MethodCanEstablishDialog(this.RequestLine.Method)){
                if(this.Contact.GetAllValues().Length == 0){
                    throw new SIP_ParseException("Contact: header field is missing, method that can establish a dialog MUST provide a SIP or SIPS URI !");
                }
                if(this.Contact.GetAllValues().Length > 1){
                    throw new SIP_ParseException("There may be only 1 Contact: header for the method that can establish a dialog !");
                }
                if(!this.Contact.GetTopMostValue().Address.IsSipOrSipsUri){
                    throw new SIP_ParseException("Method that can establish a dialog MUST have SIP or SIPS uri in Contact: header !");
                }
            }

            // TODO: Invite must have From:/To: tag

            // TODO: Check that request-Method equals CSeq method

            // TODO: PRACK must have RAck and RSeq header fields.

            // TODO: get in transport made request, so check if sips and sip set as needed.
        }

        /// <summary>
        /// Parses SIP_Request from byte array.
        /// </summary>
        /// <param name="data">Valid SIP request data.</param>
        /// <returns>Returns parsed SIP_Request obeject.</returns>
        /// <exception cref="ArgumentNullException">Raised when <b>data</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public static SIP_Request Parse(byte[] data)
        {
            if(data == null){
                throw new ArgumentNullException("data");
            }

            return Parse(new MemoryStream(data));
        }

        /// <summary>
        /// Parses SIP_Request from stream.
        /// </summary>
        /// <param name="stream">Stream what contains valid SIP request.</param>
        /// <returns>Returns parsed SIP_Request obeject.</returns>
        /// <exception cref="ArgumentNullException">Raised when <b>stream</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public static SIP_Request Parse(Stream stream)
        {
            /* Syntax:
                SIP-Method SIP-URI SIP-Version
                SIP-Message                          
            */

            if(stream == null){
                throw new ArgumentNullException("stream");
            }
                        
            // Parse Response-line
            StreamLineReader r = new StreamLineReader(stream);
            r.Encoding = "utf-8";
            string[] method_uri_version = r.ReadLineString().Split(' ');
            if(method_uri_version.Length != 3){
                throw new Exception("Invalid SIP request data ! Method line doesn't contain: SIP-Method SIP-URI SIP-Version.");
            }
            SIP_Request retVal         = new SIP_Request(method_uri_version[0]);
            retVal.RequestLine.Uri     = AbsoluteUri.Parse(method_uri_version[1]);
            retVal.RequestLine.Version = method_uri_version[2];

            // Parse SIP message
            retVal.InternalParse(stream);

            return retVal;
        }

        /// <summary>
        /// Stores SIP_Request to specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store.</param>
        public void ToStream(Stream stream)
        {
            // Add request-line
            byte[] responseLine = Encoding.UTF8.GetBytes(RequestLine.ToString());
            stream.Write(responseLine,0,responseLine.Length);

            // Add SIP-message
            this.InternalToStream(stream);
        }

        /// <summary>
        /// Converts this request to raw srver request data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteData()
        {
            MemoryStream retVal = new MemoryStream();
            ToStream(retVal);

            return retVal.ToArray();
        }

        /// <summary>
        /// Returns request as string.
        /// </summary>
        /// <returns>Returns request as string.</returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(ToByteData());
        }

        /// <summary>
        /// Gets request-line.
        /// </summary>
        public SIP_RequestLine RequestLine { get; }

        /// <summary>
        /// Gets or sets flow what received or sent this request. Returns null if this request isn't sent or received.
        /// </summary>
        internal SIP_Flow Flow { get; set; }

        /// <summary>
        /// Gets or sets local end point what sent/received this request. Returns null if this request isn't sent or received.
        /// </summary>
        internal IPEndPoint LocalEndPoint { get; set; }

        /// <summary>
        /// Gets or sets remote end point what sent/received this request. Returns null if this request isn't sent or received.
        /// </summary>
        internal IPEndPoint RemoteEndPoint { get; set; }
    }    
}
