using System;
using System.IO;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP message. This is base class for SIP_Request and SIP_Response. Defined in RFC 3261.
    /// </summary>
    public abstract class SIP_Message
    {
        /// <summary>
        /// Default constuctor.
        /// </summary>
        public SIP_Message()
        {
            Header = new SIP_HeaderFieldCollection();
        }

        /// <summary>
        /// Gets or sets what features end point supports.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_AcceptRange> Accept
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_AcceptRange>(this, "Accept:"); }
        }

        /// <summary>
        /// Gets or sets Accept-Contact header value. Defined in RFC 3841.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_ACValue> AcceptContact
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_ACValue>(this, "Accept-Contact:"); }
        }

        /// <summary>
        /// Gets encodings what end point supports. Example: Accept-Encoding: gzip.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_Encoding> AcceptEncoding
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_Encoding>(this, "Accept-Encoding:"); }
        }

        /// <summary>
        /// Gets preferred languages for reason phrases, session descriptions, or
        /// status responses carried as message bodies in the response. If no Accept-Language
        /// header field is present, the server SHOULD assume all languages are acceptable to the client.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_Language> AcceptLanguage
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_Language>(this, "Accept-Language:"); }
        }

        /// <summary>
        /// Gets Accept-Resource-Priority headers. Defined in RFC 4412.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_RValue> AcceptResourcePriority
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_RValue>(this, "Accept-Resource-Priority:"); }
        }

        /// <summary>
        /// Gets AlertInfo values collection. When present in an INVITE request, the Alert-Info header
        /// field specifies an alternative ring tone to the UAS. When present in a 180 (Ringing) response,
        /// the Alert-Info header field specifies an alternative ringback tone to the UAC.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_AlertParam> AlertInfo
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_AlertParam>(this, "Alert-Info:"); }
        }

        /// <summary>
        /// Gets methods collection which is supported by the UA which generated the message.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_Method> Allow
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_Method>(this, "Allow:"); }
        }

        /// <summary>
        /// Gets Allow-Events header which indicates the event packages supported by the client. Defined in rfc 3265.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_EventType> AllowEvents
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_EventType>(this, "Allow-Events:"); }
        }

        /// <summary>
        /// Gets the Authentication-Info header fields which provides for mutual authentication
        /// with HTTP Digest.
        /// </summary>
        public SIP_SVGroupHFCollection<SIP_t_AuthenticationInfo> AuthenticationInfo
        {
            get { return new SIP_SVGroupHFCollection<SIP_t_AuthenticationInfo>(this, "Authentication-Info:"); }
        }

        /// <summary>
        /// Gets the Authorization header fields which contains authentication credentials of a UA.
        /// </summary>
        public SIP_SVGroupHFCollection<SIP_t_Credentials> Authorization
        {
            get { return new SIP_SVGroupHFCollection<SIP_t_Credentials>(this, "Authorization:"); }
        }

        /// <summary>
        /// Gets or sets the Call-ID header field which uniquely identifies a particular invitation or all
        /// registrations of a particular client.
        /// Value null means not specified.
        /// </summary>
        public string CallID
        {
            get
            {
                var h = Header.GetFirst("Call-ID:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Call-ID:");
                }
                else
                {
                    Header.Set("Call-ID:", value);
                }
            }
        }

        /// <summary>
        /// Gets the Call-Info header field which provides additional information about the
        /// caller or callee, depending on whether it is found in a request or response.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_Info> CallInfo
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_Info>(this, "Call-Info:"); }
        }

        /// <summary>
        /// Gets contact header fields. The Contact header field provides a SIP or SIPS URI that can be used
        /// to contact that specific instance of the UA for subsequent requests.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_ContactParam> Contact
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_ContactParam>(this, "Contact:"); }
        }

        /// <summary>
        /// Gets or sets the Content-Disposition header field which describes how the message body
        /// or, for multipart messages, a message body part is to be interpreted by the UAC or UAS.
        /// Value null means not specified.
        /// </summary>
        public SIP_t_ContentDisposition ContentDisposition
        {
            get
            {
                var h = Header.GetFirst("Content-Disposition:");
                return ((SIP_SingleValueHF<SIP_t_ContentDisposition>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Content-Disposition:");
                }
                else
                {
                    Header.Set("Content-Disposition:", value.ToStringValue());
                }
            }
        }

        /// <summary>
        /// Gets the Content-Encodings which is used as a modifier to the "media-type". When present,
        /// its value indicates what additional content codings have been applied to the entity-body,
        /// and thus what decoding mechanisms MUST be applied in order to obtain the media-type referenced
        /// by the Content-Type header field.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_ContentCoding> ContentEncoding
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_ContentCoding>(this, "Content-Encoding:"); }
        }

        /// <summary>
        /// Gets content languages.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_LanguageTag> ContentLanguage
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_LanguageTag>(this, "Content-Language:"); }
        }

        /// <summary>
        /// Gets SIP request content data size in bytes.
        /// </summary>
        public int ContentLength
        {
            get
            {
                if (Data == null)
                {
                    return 0;
                }

                return Data.Length;
            }
        }

        /// <summary>
        /// Gets or sets the Content-Type header field which indicates the media type of the
        /// message-body sent to the recipient.
        /// Value null means not specified.
        /// </summary>
        public string ContentType
        {
            get
            {
                var h = Header.GetFirst("Content-Type:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Content-Type:");
                }
                else
                {
                    Header.Set("Content-Type:", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets command sequence number and the request method.
        /// Value null means not specified.
        /// </summary>
        public SIP_t_CSeq CSeq
        {
            get
            {
                var h = Header.GetFirst("CSeq:");
                return ((SIP_SingleValueHF<SIP_t_CSeq>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("CSeq:");
                }
                else
                {
                    Header.Set("CSeq:", value.ToStringValue());
                }
            }
        }

        /// <summary>
        /// Gets or sets content data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets date and time. Value DateTime.MinValue means that value not specified.
        /// </summary>
        public DateTime Date
        {
            get
            {
                var h = Header.GetFirst("Date:");
                if (h != null)
                {
                    return DateTime.ParseExact(h.Value, "r", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }

                return DateTime.MinValue;
            }

            set
            {
                if (value == DateTime.MinValue)
                {
                    Header.RemoveFirst("Date:");
                }
                else
                {
                    Header.Set("Date:", value.ToString("r"));
                }
            }
        }

        /// <summary>
        /// Gets the Error-Info header field which provides a pointer to additional
        /// information about the error status response.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_ErrorUri> ErrorInfo
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_ErrorUri>(this, "Error-Info:"); }
        }

        /// <summary>
        /// Gets or sets Event header. Defined in RFC 3265.
        /// </summary>
        public SIP_t_Event Event
        {
            get
            {
                var h = Header.GetFirst("Event:");
                return ((SIP_SingleValueHF<SIP_t_Event>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Event:");
                }
                else
                {
                    Header.Set("Event:", value.ToStringValue());
                }
            }
        }

        /// <summary>
        /// Gets or sets relative time after which the message (or content) expires.
        /// Value -1 means that value not specified.
        /// </summary>
        public int Expires
        {
            get
            {
                var h = Header.GetFirst("Expires:");
                if (h != null)
                {
                    return Convert.ToInt32(h.Value);
                }

                return -1;
            }

            set
            {
                if (value < 0)
                {
                    Header.RemoveFirst("Expires:");
                }
                else
                {
                    Header.Set("Expires:", value.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets initiator of the request.
        /// Value null means not specified.
        /// </summary>
        public SIP_t_From From
        {
            get
            {
                var h = Header.GetFirst("From:");
                return ((SIP_SingleValueHF<SIP_t_From>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("From:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_From>("From:", value));
                }
            }
        }

        /// <summary>
        /// Gets direct access to header.
        /// </summary>
        public SIP_HeaderFieldCollection Header { get; }

        /// <summary>
        /// Gets History-Info headers. Defined in RFC 4244.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_HiEntry> HistoryInfo
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_HiEntry>(this, "History-Info:"); }
        }

        /// <summary>
        /// Identity header value. Value null means not specified. Defined in RFC 4474.
        /// </summary>
        public string Identity
        {
            get
            {
                var h = Header.GetFirst("Identity:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Identity:");
                }
                else
                {
                    Header.Set("Identity:", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets Identity-Info header value. Value null means not specified.
        /// Defined in RFC 4474.
        /// </summary>
        public SIP_t_IdentityInfo IdentityInfo
        {
            get
            {
                var h = Header.GetFirst("Identity-Info:");
                return ((SIP_SingleValueHF<SIP_t_IdentityInfo>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Identity-Info:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_IdentityInfo>("Identity-Info:", value));
                }
            }
        }

        /// <summary>
        /// Gets the In-Reply-To header fields which enumerates the Call-IDs that this call
        /// references or returns.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_CallID> InReplyTo
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_CallID>(this, "In-Reply-To:"); }
        }

        /// <summary>
        /// Gets or sets Join header which indicates that a new dialog (created by the INVITE in which
        /// the Join header field in contained) should be joined with a dialog identified by the header
        /// field, and any associated dialogs or conferences. Defined in 3911. Value null means not specified.
        /// </summary>
        public SIP_t_Join Join
        {
            get
            {
                var h = Header.GetFirst("Join:");
                return ((SIP_SingleValueHF<SIP_t_Join>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Join:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_Join>("Join:", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets limit the number of proxies or gateways that can forward the request
        /// to the next downstream server.
        /// Value -1 means that value not specified.
        /// </summary>
        public int MaxForwards
        {
            get
            {
                var h = Header.GetFirst("Max-Forwards:");
                if (h != null)
                {
                    return Convert.ToInt32(h.Value);
                }

                return -1;
            }

            set
            {
                if (value < 0)
                {
                    Header.RemoveFirst("Max-Forwards:");
                }
                else
                {
                    Header.Set("Max-Forwards:", value.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets mime version. Currently 1.0 is only defined value.
        /// Value null means not specified.
        /// </summary>
        public string MimeVersion
        {
            get
            {
                var h = Header.GetFirst("Mime-Version:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Mime-Version:");
                }
                else
                {
                    Header.Set("Mime-Version:", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets minimum refresh interval supported for soft-state elements managed by that server.
        /// Value -1 means that value not specified.
        /// </summary>
        public int MinExpires
        {
            get
            {
                var h = Header.GetFirst("Min-Expires:");
                if (h != null)
                {
                    return Convert.ToInt32(h.Value);
                }

                return -1;
            }

            set
            {
                if (value < 0)
                {
                    Header.RemoveFirst("Min-Expires:");
                }
                else
                {
                    Header.Set("Min-Expires:", value.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets Min-SE header which indicates the minimum value for the session interval,
        /// in units of delta-seconds. Defined in 4028. Value null means not specified.
        /// </summary>
        public SIP_t_MinSE MinSE
        {
            get
            {
                var h = Header.GetFirst("Min-SE:");
                return ((SIP_SingleValueHF<SIP_t_MinSE>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Min-SE:");
                }
                else
                {
                    Header.Set("Min-SE:", value.ToStringValue());
                }
            }
        }

        /// <summary>
        /// Gets or sets organization name which the SIP element issuing the request or response belongs.
        /// Value null means not specified.
        /// </summary>
        public string Organization
        {
            get
            {
                var h = Header.GetFirst("Organization:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Organization:");
                }
                else
                {
                    Header.Set("Organization:", value);
                }
            }
        }

        /// <summary>
        /// Gets an Path header. It is used in conjunction with SIP REGISTER requests and with 200
        /// class messages in response to REGISTER (REGISTER responses). Defined in rfc 3327.
        /// </summary>
        public SIP_SVGroupHFCollection<SIP_t_AddressParam> Path
        {
            get { return new SIP_SVGroupHFCollection<SIP_t_AddressParam>(this, "Path:"); }
        }

        /// <summary>
        /// Gest or sets priority that the SIP request should have to the receiving human or its agent.
        /// Value null means not specified.
        /// </summary>
        public string Priority
        {
            get
            {
                var h = Header.GetFirst("Priority:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Priority:");
                }
                else
                {
                    Header.Set("Priority:", value);
                }
            }
        }

        // Privacy                       [RFC3323]

        /// <summary>
        /// Gets an proxy authentication challenge.
        /// </summary>
        public SIP_SVGroupHFCollection<SIP_t_Challenge> ProxyAuthenticate
        {
            get { return new SIP_SVGroupHFCollection<SIP_t_Challenge>(this, "Proxy-Authenticate:"); }
        }

        /// <summary>
        /// Gest credentials containing the authentication information of the user agent
        /// for the proxy and/or realm of the resource being requested.
        /// </summary>
        public SIP_SVGroupHFCollection<SIP_t_Credentials> ProxyAuthorization
        {
            get { return new SIP_SVGroupHFCollection<SIP_t_Credentials>(this, "Proxy-Authorization:"); }
        }

        /// <summary>
        /// Gets proxy-sensitive features that must be supported by the proxy.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_OptionTag> ProxyRequire
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Proxy-Require:"); }
        }

        /// <summary>
        /// Gets or sets RAck header. Defined in 3262. Value null means not specified.
        /// </summary>
        public SIP_t_RAck RAck
        {
            get
            {
                var h = Header.GetFirst("RAck:");
                return ((SIP_SingleValueHF<SIP_t_RAck>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("RAck:");
                }
                else
                {
                    Header.Set("RAck:", value.ToStringValue());
                }
            }
        }

        /// <summary>
        /// Gets the Reason header. Defined in rfc 3326.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_ReasonValue> Reason
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_ReasonValue>(this, "Reason:"); }
        }

        /// <summary>
        /// Gets the Record-Route header fields what is inserted by proxies in a request to
        /// force future requests in the dialog to be routed through the proxy.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_AddressParam> RecordRoute
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Record-Route:"); }
        }

        /// <summary>
        /// Gets or sets Referred-By header. Defined in rfc 3892. Value null means not specified.
        /// </summary>
        public SIP_t_ReferredBy ReferredBy
        {
            get
            {
                var h = Header.GetFirst("Referred-By:");
                return ((SIP_SingleValueHF<SIP_t_ReferredBy>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Referred-By:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_ReferredBy>("Referred-By:", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets Refer-Sub header. Defined in rfc 4488. Value null means not specified.
        /// </summary>
        public SIP_t_ReferSub ReferSub
        {
            get
            {
                var h = Header.GetFirst("Refer-Sub:");
                return ((SIP_SingleValueHF<SIP_t_ReferSub>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Refer-Sub:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_ReferSub>("Refer-Sub:", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets Refer-To header. Defined in rfc 3515. Value null means not specified.
        /// </summary>
        public SIP_t_AddressParam ReferTo
        {
            get
            {
                var h = Header.GetFirst("Refer-To:");
                return ((SIP_SingleValueHF<SIP_t_AddressParam>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Refer-To:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_AddressParam>("Refer-To:", value));
                }
            }
        }

        /// <summary>
        /// Gets Reject-Contact headers. Defined in RFC 3841.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_RCValue> RejectContact
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_RCValue>(this, "Reject-Contact:"); }
        }

        /// <summary>
        /// Gets or sets Replaces header. Defined in rfc 3891. Value null means not specified.
        /// </summary>
        public SIP_t_Replaces Replaces
        {
            get
            {
                var h = Header.GetFirst("Replaces:");
                return ((SIP_SingleValueHF<SIP_t_Replaces>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Replaces:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_Replaces>("Replaces:", value));
                }
            }
        }

        /// <summary>
        /// Gets logical return URI that may be different from the From header field.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_AddressParam> ReplyTo
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Reply-To:"); }
        }

        /// <summary>
        /// Gets or sets Request-Disposition header. The Request-Disposition header field specifies caller preferences for
        /// how a server should process a request. Defined in rfc 3841.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_Directive> RequestDisposition
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_Directive>(this, "Request-Disposition:"); }
        }

        /// <summary>
        /// Gets options that the UAC expects the UAS to support in order to process the request.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_OptionTag> Require
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Require:"); }
        }

        /// <summary>
        /// Gets Resource-Priority headers. Defined in RFC 4412.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_RValue> ResourcePriority
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_RValue>(this, "Resource-Priority:"); }
        }

        /// <summary>
        /// Gets or sets how many seconds the service is expected to be unavailable to the requesting client.
        /// Value null means that value not specified.
        /// </summary>
        public SIP_t_RetryAfter RetryAfter
        {
            get
            {
                var h = Header.GetFirst("Retry-After:");
                return ((SIP_SingleValueHF<SIP_t_RetryAfter>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Retry-After:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_RetryAfter>("Retry-After:", value));
                }
            }
        }

        /// <summary>
        /// Gets force routing for a request through the listed set of proxies.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_AddressParam> Route
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Route:"); }
        }

        /// <summary>
        /// Gets or sets RSeq header. Value -1 means that value not specified. Defined in rfc 3262.
        /// </summary>
        public int RSeq
        {
            get
            {
                var h = Header.GetFirst("RSeq:");
                if (h != null)
                {
                    return Convert.ToInt32(h.Value);
                }

                return -1;
            }

            set
            {
                if (value < 0)
                {
                    Header.RemoveFirst("RSeq:");
                }
                else
                {
                    Header.Set("RSeq:", value.ToString());
                }
            }
        }

        /// <summary>
        /// Gets Security-Client headers. Defined in RFC 3329.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_SecMechanism> SecurityClient
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_SecMechanism>(this, "Security-Client:"); }
        }

        /// <summary>
        /// Gets Security-Server headers. Defined in RFC 3329.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_SecMechanism> SecurityServer
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_SecMechanism>(this, "Security-Server:"); }
        }

        /// <summary>
        /// Gets Security-Verify headers. Defined in RFC 3329.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_SecMechanism> SecurityVerify
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_SecMechanism>(this, "Security-Verify:"); }
        }

        /// <summary>
        /// Gets or sets the software used by the UAS to handle the request.
        /// Value null means not specified.
        /// </summary>
        public string Server
        {
            get
            {
                var h = Header.GetFirst("Server:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Server:");
                }
                else
                {
                    Header.Set("Server:", value);
                }
            }
        }

        /// <summary>
        /// Gets the Service-Route header. Defined in rfc 3608.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_AddressParam> ServiceRoute
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Service-Route:"); }
        }

        /// <summary>
        /// Gets or sets Session-Expires expires header. Value null means that value not specified.
        /// Defined in rfc 4028.
        /// </summary>
        public SIP_t_SessionExpires SessionExpires
        {
            get
            {
                var h = Header.GetFirst("Session-Expires:");
                return ((SIP_SingleValueHF<SIP_t_SessionExpires>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Session-Expires:");
                }
                else
                {
                    Header.Set("Session-Expires:", value.ToStringValue());
                }
            }
        }

        /// <summary>
        /// Gets or sets SIP-ETag header value. Value null means not specified. Defined in RFC 3903.
        /// </summary>
        public string SIPETag
        {
            get
            {
                var h = Header.GetFirst("SIP-ETag:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("SIP-ETag:");
                }
                else
                {
                    Header.Set("SIP-ETag:", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets SIP-ETag header value. Value null means not specified. Defined in RFC 3903.
        /// </summary>
        public string SIPIfMatch
        {
            get
            {
                var h = Header.GetFirst("SIP-If-Match:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("SIP-If-Match:");
                }
                else
                {
                    Header.Set("SIP-If-Match:", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets call subject text.
        /// Value null means not specified.
        /// </summary>
        public string Subject
        {
            get
            {
                var h = Header.GetFirst("Subject:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Subject:");
                }
                else
                {
                    Header.Set("Subject:", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets Subscription-State header value. Value null means that value not specified.
        /// Defined in RFC 3265.
        /// </summary>
        public SIP_t_SubscriptionState SubscriptionState
        {
            get
            {
                var h = Header.GetFirst("Subscription-State:");
                return ((SIP_SingleValueHF<SIP_t_SubscriptionState>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Subscription-State:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_SubscriptionState>("Subscription-State:", value));
                }
            }
        }

        /// <summary>
        /// Gets extensions supported by the UAC or UAS. Known values are defined in SIP_OptionTags class.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_OptionTag> Supported
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Supported:"); }
        }

        /// <summary>
        /// Gets or sets Target-Dialog header value. Value null means that value not specified.
        /// Defined in RFC 4538.
        /// </summary>
        public SIP_t_TargetDialog TargetDialog
        {
            get
            {
                var h = Header.GetFirst("Target-Dialog:");
                return ((SIP_SingleValueHF<SIP_t_TargetDialog>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Target-Dialog:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_TargetDialog>("Target-Dialog:", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets when the UAC sent the request to the UAS.
        /// Value null means that value not specified.
        /// </summary>
        public SIP_t_Timestamp Timestamp
        {
            get
            {
                var h = Header.GetFirst("Timestamp:");
                return ((SIP_SingleValueHF<SIP_t_Timestamp>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("Timestamp:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_Timestamp>("Timestamp:", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets logical recipient of the request.
        /// Value null means not specified.
        /// </summary>
        public SIP_t_To To
        {
            get
            {
                var h = Header.GetFirst("To:");
                return ((SIP_SingleValueHF<SIP_t_To>) h)?.ValueX;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("To:");
                }
                else
                {
                    Header.Add(new SIP_SingleValueHF<SIP_t_To>("To:", value));
                }
            }
        }

        /// <summary>
        /// Gets features not supported by the UAS.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_OptionTag> Unsupported
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Unsupported:"); }
        }

        /// <summary>
        /// Gets or sets information about the UAC originating the request.
        /// Value null means not specified.
        /// </summary>
        public string UserAgent
        {
            get
            {
                var h = Header.GetFirst("User-Agent:");
                return h?.Value;
            }

            set
            {
                if (value == null)
                {
                    Header.RemoveFirst("User-Agent:");
                }
                else
                {
                    Header.Set("User-Agent:", value);
                }
            }
        }

        /// <summary>
        /// Gets Via header fields.The Via header field indicates the transport used for the transaction
        /// and identifies the location where the response is to be sent.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_ViaParm> Via
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_ViaParm>(this, "Via:"); }
        }

        /// <summary>
        /// Gets additional information about the status of a response.
        /// </summary>
        public SIP_MVGroupHFCollection<SIP_t_WarningValue> Warning
        {
            get { return new SIP_MVGroupHFCollection<SIP_t_WarningValue>(this, "Warning:"); }
        }

        /// <summary>
        /// Gets or authentication challenge.
        /// </summary>
        public SIP_SVGroupHFCollection<SIP_t_Challenge> WWWAuthenticate
        {
            get { return new SIP_SVGroupHFCollection<SIP_t_Challenge>(this, "WWW-Authenticate:"); }
        }

        /// <summary>
        /// Parses SIP message from specified byte array.
        /// </summary>
        /// <param name="data">SIP message data.</param>
        protected void InternalParse(byte[] data)
        {
            InternalParse(new MemoryStream(data));
        }

        /// <summary>
        /// Parses SIP message from specified stream.
        /// </summary>
        /// <param name="stream">SIP message stream.</param>
        protected void InternalParse(Stream stream)
        {
            /* SIP message syntax:
                header-line<CRFL>
                ....
                <CRFL>
                data size of Content-Length header field.
            */

            // Parse header
            Header.Parse(stream);

            // Parse data
            int contentLength = 0;
            try
            {
                contentLength = Convert.ToInt32(Header.GetFirst("Content-Length:").Value);
            }
            catch
            {
            }
            if (contentLength > 0)
            {
                var data = new byte[contentLength];
                stream.Read(data, 0, data.Length);
                Data = data;
            }
        }

        /// <summary>
        /// Stores SIP_Message to specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store SIP_Message.</param>
        protected void InternalToStream(Stream stream)
        {
            // Ensure that we add right Contnet-Length.
            Header.RemoveAll("Content-Length:");
            if (Data != null)
            {
                Header.Add("Content-Length:", Convert.ToString(Data.Length));
            }
            else
            {
                Header.Add("Content-Length:", Convert.ToString(0));
            }

            // Store header
            var header = Encoding.UTF8.GetBytes(Header.ToHeaderString());
            stream.Write(header, 0, header.Length);

            // Store data
            if (Data != null && Data.Length > 0)
            {
                stream.Write(Data, 0, Data.Length);
            }
        }
    }
}
