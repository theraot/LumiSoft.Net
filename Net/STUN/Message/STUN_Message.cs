using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LumiSoft.Net.STUN.Message
{
    internal class STUN_Message
    {
        public STUN_Message(STUN_MessageType type)
            : this()
        {
            Type = type;
        }

        public STUN_Message(byte[] data)
            : this()
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            /* RFC 5389 6.
                All STUN messages MUST start with a 20-byte header followed by zero
                or more Attributes.  The STUN header contains a STUN message type,
                magic cookie, transaction ID, and message length.

                 0                   1                   2                   3
                 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 |0 0|     STUN Message Type     |         Message Length        |
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 |                         Magic Cookie                          |
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 |                                                               |
                 |                     Transaction ID (96 bits)                  |
                 |                                                               |
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

               The message length is the count, in bytes, of the size of the
               message, not including the 20 byte header.
            */
            if (data.Length < 20)
            {
                throw new ArgumentException("Invalid STUN message value !");
            }

            var offset = 0;

            //--- message header --------------------------------------------------

            // STUN Message Type
            switch ((data[offset++] << 8) | data[offset++])
            {
                case (int) STUN_MessageType.BindingErrorResponse:
                    Type = STUN_MessageType.BindingErrorResponse;
                    break;
                case (int) STUN_MessageType.BindingRequest:
                    Type = STUN_MessageType.BindingRequest;
                    break;
                case (int) STUN_MessageType.BindingResponse:
                    Type = STUN_MessageType.BindingResponse;
                    break;
                case (int) STUN_MessageType.SharedSecretErrorResponse:
                    Type = STUN_MessageType.SharedSecretErrorResponse;
                    break;
                case (int) STUN_MessageType.SharedSecretRequest:
                    Type = STUN_MessageType.SharedSecretRequest;
                    break;
                case (int) STUN_MessageType.SharedSecretResponse:
                    Type = STUN_MessageType.SharedSecretResponse;
                    break;
                default:
                    throw new ArgumentException("Invalid STUN message type value !");
            }

            // Message Length
            var messageLength = (data[offset++] << 8) | data[offset++];

            // Magic Cookie
            MagicCookie = (data[offset++] << 24) | (data[offset++] << 16) | (data[offset++] << 8) | data[offset++];

            // Transaction ID
            TransactionId = new byte[12];
            Array.Copy(data, offset, TransactionId, 0, 12);
            offset += 12;

            //--- Message attributes ---------------------------------------------
            while (offset - 20 < messageLength)
            {
                ParseAttribute(data, ref offset);
            }
        }

        private STUN_Message()
        {
            TransactionId = new byte[12];
            new Random().NextBytes(TransactionId);
        }

        public IPEndPoint ChangedAddress { get; set; }

        public STUN_t_ChangeRequest ChangeRequest { get; set; }

        public STUN_t_ErrorCode ErrorCode { get; set; }

        public int MagicCookie { get; }

        public IPEndPoint MappedAddress { get; set; }

        public string Password { get; set; }

        public IPEndPoint ReflectedFrom { get; set; }

        public IPEndPoint ResponseAddress { get; set; }

        public string ServerName { get; set; }

        public IPEndPoint SourceAddress { get; set; }

        public byte[] TransactionId { get; }

        public STUN_MessageType Type { get; }

        public string UserName { get; set; }

        public byte[] ToByteData()
        {
            /* RFC 5389 6.
                All STUN messages MUST start with a 20-byte header followed by zero
                or more Attributes.  The STUN header contains a STUN message type,
                magic cookie, transaction ID, and message length.

                 0                   1                   2                   3
                 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 |0 0|     STUN Message Type     |         Message Length        |
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 |                         Magic Cookie                          |
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 |                                                               |
                 |                     Transaction ID (96 bits)                  |
                 |                                                               |
                 +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

               The message length is the count, in bytes, of the size of the
               message, not including the 20 byte header.
            */

            // We allocate 512 for header, that should be more than enough.
            var msg = new byte[512];

            var offset = 0;

            //--- message header -------------------------------------

            // STUN Message Type (2 bytes)
            msg[offset++] = (byte) (((int) Type >> 8) & 0x3F);
            msg[offset++] = (byte) ((int) Type & 0xFF);

            // Message Length (2 bytes) will be assigned at last.
            msg[offset++] = 0;
            msg[offset++] = 0;

            // Magic Cookie
            msg[offset++] = (byte) ((MagicCookie >> 24) & 0xFF);
            msg[offset++] = (byte) ((MagicCookie >> 16) & 0xFF);
            msg[offset++] = (byte) ((MagicCookie >> 8) & 0xFF);
            msg[offset++] = (byte) ((MagicCookie >> 0) & 0xFF);

            // Transaction ID (16 bytes)
            Array.Copy(TransactionId, 0, msg, offset, 12);
            offset += 12;

            //--- Message attributes ------------------------------------

            /* RFC 3489 11.2.
                After the header are 0 or more attributes.  Each attribute is TLV
                encoded, with a 16 bit type, 16 bit length, and variable value:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |         Type                  |            Length             |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |                             Value                             ....
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            if (MappedAddress != null)
            {
                StoreEndPoint(AttributeType.MappedAddress, MappedAddress, msg, ref offset);
            }
            else if (ResponseAddress != null)
            {
                StoreEndPoint(AttributeType.ResponseAddress, ResponseAddress, msg, ref offset);
            }
            else if (ChangeRequest != null)
            {
                /*
                    The CHANGE-REQUEST attribute is used by the client to request that
                    the server use a different address and/or port when sending the
                    response.  The attribute is 32 bits long, although only two bits (A
                    and B) are used:

                     0                   1                   2                   3
                     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 A B 0|
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                    The meaning of the flags is:

                    A: This is the "change IP" flag.  If true, it requests the server
                       to send the Binding Response with a different IP address than the
                       one the Binding Request was received on.

                    B: This is the "change port" flag.  If true, it requests the
                       server to send the Binding Response with a different port than the
                       one the Binding Request was received on.
                */

                // Attribute header
                msg[offset++] = (int) AttributeType.ChangeRequest >> 8;
                msg[offset++] = (int) AttributeType.ChangeRequest & 0xFF;
                msg[offset++] = 0;
                msg[offset++] = 4;

                msg[offset++] = 0;
                msg[offset++] = 0;
                msg[offset++] = 0;
                msg[offset++] = (byte) ((Convert.ToInt32(ChangeRequest.ChangeIP) << 2) | (Convert.ToInt32(ChangeRequest.ChangePort) << 1));
            }
            else if (SourceAddress != null)
            {
                StoreEndPoint(AttributeType.SourceAddress, SourceAddress, msg, ref offset);
            }
            else if (ChangedAddress != null)
            {
                StoreEndPoint(AttributeType.ChangedAddress, ChangedAddress, msg, ref offset);
            }
            else if (UserName != null)
            {
                var userBytes = Encoding.ASCII.GetBytes(UserName);

                // Attribute header
                msg[offset++] = (int) AttributeType.Username >> 8;
                msg[offset++] = (int) AttributeType.Username & 0xFF;
                msg[offset++] = (byte) (userBytes.Length >> 8);
                msg[offset++] = (byte) (userBytes.Length & 0xFF);

                Array.Copy(userBytes, 0, msg, offset, userBytes.Length);
                offset += userBytes.Length;
            }
            else if (Password != null)
            {
                var userBytes = Encoding.ASCII.GetBytes(UserName ?? string.Empty);

                // Attribute header
                msg[offset++] = (int) AttributeType.Password >> 8;
                msg[offset++] = (int) AttributeType.Password & 0xFF;
                msg[offset++] = (byte) (userBytes.Length >> 8);
                msg[offset++] = (byte) (userBytes.Length & 0xFF);

                Array.Copy(userBytes, 0, msg, offset, userBytes.Length);
                offset += userBytes.Length;
            }
            else if (ErrorCode != null)
            {
                /* 3489 11.2.9.
                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |                   0                     |Class|     Number    |
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |      Reason Phrase (variable)                                ..
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                */

                var reasonBytes = Encoding.ASCII.GetBytes(ErrorCode.ReasonText);

                // Header
                msg[offset++] = 0;
                msg[offset++] = (int) AttributeType.ErrorCode;
                msg[offset++] = 0;
                msg[offset++] = (byte) (4 + reasonBytes.Length);

                // Empty
                msg[offset++] = 0;
                msg[offset++] = 0;
                // Class
                msg[offset++] = (byte) (ErrorCode.Code / 100);
                // Number
                msg[offset++] = (byte) (ErrorCode.Code % 100);
                // ReasonPhrase
                Array.Copy(reasonBytes, msg, reasonBytes.Length);
                offset += reasonBytes.Length;
            }
            else if (ReflectedFrom != null)
            {
                StoreEndPoint(AttributeType.ReflectedFrom, ReflectedFrom, msg, ref offset);
            }

            // Update Message Length. NOTE: 20 bytes header not included.
            msg[2] = (byte) ((offset - 20) >> 8);
            msg[3] = (byte) ((offset - 20) & 0xFF);

            // Make result with actual size.
            var result = new byte[offset];
            Array.Copy(msg, result, result.Length);

            return result;
        }

        private static IPEndPoint ParseEndPoint(byte[] data, ref int offset)
        {
            /*
                It consists of an eight bit address family, and a sixteen bit
                port, followed by a fixed length value representing the IP address.

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |x x x x x x x x|    Family     |           Port                |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |                             Address                           |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            // Skip family
            offset++;
            offset++;

            // Port
            var port = (data[offset++] << 8) | data[offset++];

            // Address
            var ip = new byte[4];
            ip[0] = data[offset++];
            ip[1] = data[offset++];
            ip[2] = data[offset++];
            ip[3] = data[offset++];

            return new IPEndPoint(new IPAddress(ip), port);
        }

        private void ParseAttribute(byte[] data, ref int offset)
        {
            /* RFC 3489 11.2.
                Each attribute is TLV encoded, with a 16 bit type, 16 bit length, and variable value:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |         Type                  |            Length             |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |                             Value                             ....
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            // Type
            var type = (AttributeType) ((data[offset++] << 8) | data[offset++]);

            // Length
            var length = (data[offset++] << 8) | data[offset++];

            // MAPPED-ADDRESS
            switch (type)
            {
                case AttributeType.MappedAddress:
                    MappedAddress = ParseEndPoint(data, ref offset);
                    break;
                // RESPONSE-ADDRESS
                case AttributeType.ResponseAddress:
                    ResponseAddress = ParseEndPoint(data, ref offset);
                    break;
                // CHANGE-REQUEST
                case AttributeType.ChangeRequest:
                    /*
                    The CHANGE-REQUEST attribute is used by the client to request that
                    the server use a different address and/or port when sending the
                    response.  The attribute is 32 bits long, although only two bits (A
                    and B) are used:

                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 A B 0|
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                    The meaning of the flags is:

                    A: This is the "change IP" flag.  If true, it requests the server
                    to send the Binding Response with a different IP address than the
                    one the Binding Request was received on.

                    B: This is the "change port" flag.  If true, it requests the
                    server to send the Binding Response with a different port than the
                    one the Binding Request was received on.
                    */

                    // Skip 3 bytes
                    offset += 3;

                    ChangeRequest = new STUN_t_ChangeRequest((data[offset] & 4) != 0, (data[offset] & 2) != 0);
                    offset++;
                    break;
                // SOURCE-ADDRESS
                case AttributeType.SourceAddress:
                    SourceAddress = ParseEndPoint(data, ref offset);
                    break;
                // CHANGED-ADDRESS
                case AttributeType.ChangedAddress:
                    ChangedAddress = ParseEndPoint(data, ref offset);
                    break;
                // USERNAME
                case AttributeType.Username:
                    UserName = Encoding.Default.GetString(data, offset, length);
                    offset += length;
                    break;
                // PASSWORD
                case AttributeType.Password:
                    Password = Encoding.Default.GetString(data, offset, length);
                    offset += length;
                    break;
                // MESSAGE-INTEGRITY
                case AttributeType.MessageIntegrity:
                    offset += length;
                    break;
                // ERROR-CODE
                case AttributeType.ErrorCode:
                    /* 3489 11.2.9.
                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |                   0                     |Class|     Number    |
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |      Reason Phrase (variable)                                ..
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    */

                    var errorCode = ((data[offset + 2] & 0x7) * 100) + (data[offset + 3] & 0xFF);

                    ErrorCode = new STUN_t_ErrorCode(errorCode, Encoding.Default.GetString(data, offset + 4, length - 4));
                    offset += length;
                    break;
                // UNKNOWN-ATTRIBUTES
                case AttributeType.UnknownAttribute:
                    offset += length;
                    break;
                // REFLECTED-FROM
                case AttributeType.ReflectedFrom:
                    ReflectedFrom = ParseEndPoint(data, ref offset);
                    break;
                // XorMappedAddress
                // XorOnly
                // ServerName
                case AttributeType.ServerName:
                    ServerName = Encoding.Default.GetString(data, offset, length);
                    offset += length;
                    break;
                // Unknown
                default:
                    offset += length;
                    break;
            }
        }

        private static void StoreEndPoint(AttributeType type, IPEndPoint endPoint, IList<byte> message, ref int offset)
        {
            /*
                It consists of an eight bit address family, and a sixteen bit
                port, followed by a fixed length value representing the IP address.

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |x x x x x x x x|    Family     |           Port                |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |                             Address                           |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            // Header
            message[offset++] = (byte) ((int) type >> 8);
            message[offset++] = (byte) ((int) type & 0xFF);
            message[offset++] = 0;
            message[offset++] = 8;

            // Unused
            message[offset++] = 0;
            // Family
            message[offset++] = 1; //IPv4
            // Port
            message[offset++] = (byte) (endPoint.Port >> 8);
            message[offset++] = (byte) (endPoint.Port & 0xFF);
            // Address
            var ipBytes = endPoint.Address.GetAddressBytes();
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[1];
            message[offset++] = ipBytes[2];
            message[offset++] = ipBytes[3];
        }
    }
}