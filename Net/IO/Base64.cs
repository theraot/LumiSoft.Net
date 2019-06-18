using System;
using System.Text;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// This class implements base64 encoder/decoder.  Defined in RFC 4648.
    /// </summary>
    public static class Base64
    {
        private readonly static short[] _base64_Decode_Table = {
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,62,-1,-1,-1,63,52,53,
            54,55,56,57,58,59,60,61,-1,-1,
            -1,-1,-1,-1,-1, 0, 1, 2, 3, 4,
            5, 6, 7, 8, 9,10,11,12,13,14,
            15,16,17,18,19,20,21,22,23,24,
            25,-1,-1,-1,-1,-1,-1,26,27,28,
            29,30,31,32,33,34,35,36,37,38,
            39,40,41,42,43,44,45,46,47,48,
            49,50,51,-1,-1,-1,-1,-1
        };

        /// <summary>
        /// Decodes specified base64 string.
        /// </summary>
        /// <param name="value">Base64 string.</param>
        /// <param name="ignoreNonBase64Chars">If true all invalid base64 chars ignored. If false, FormatException is raised.</param>
        /// <returns>Returns decoded data.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="FormatException">Is raised when <b>value</b> contains invalid base64 data.</exception>
        public static byte[] Decode(string value, bool ignoreNonBase64Chars)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var encBuffer = Encoding.ASCII.GetBytes(value);
            var buffer = new byte[encBuffer.Length];

            var decodedCount = Decode(encBuffer, 0, encBuffer.Length, buffer, 0, ignoreNonBase64Chars);
            var result = new byte[decodedCount];
            Array.Copy(buffer, result, decodedCount);

            return result;
        }

        /// <summary>
        /// Decodes specified base64 data.
        /// </summary>
        /// <param name="data">Base64 encoded data buffer.</param>
        /// <param name="offset">Offset in the buffer.</param>
        /// <param name="count">Number of bytes available in the buffer.</param>
        /// <param name="ignoreNonBase64Chars">If true all invalid base64 chars ignored. If false, FormatException is raised.</param>
        /// <returns>Returns decoded data.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>data</b> is null reference.</exception>
        /// <exception cref="FormatException">Is raised when <b>value</b> contains invalid base64 data.</exception>
        public static byte[] Decode(byte[] data, int offset, int count, bool ignoreNonBase64Chars)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var buffer = new byte[data.Length];

            var decodedCount = Decode(data, offset, count, buffer, 0, ignoreNonBase64Chars);
            var result = new byte[decodedCount];
            Array.Copy(buffer, result, decodedCount);

            return result;
        }

        /// <summary>
        /// Decodes base64 encoded bytes.
        /// </summary>
        /// <param name="encBuffer">Base64 encoded data buffer.</param>
        /// <param name="encOffset">Offset in the encBuffer.</param>
        /// <param name="encCount">Number of bytes available in the encBuffer.</param>
        /// <param name="buffer">Buffer where to decode data.</param>
        /// <param name="offset">Offset int the buffer.</param>
        /// <param name="ignoreNonBase64Chars">If true all invalid base64 chars ignored. If false, FormatException is raised.</param>
        /// <returns>Returns number of bytes decoded.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>encBuffer</b> or <b>encBuffer</b> is null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when any of the arguments has out of valid range.</exception>
        /// <exception cref="FormatException">Is raised when <b>encBuffer</b> contains invalid base64 data.</exception>
        public static int Decode(byte[] encBuffer, int encOffset, int encCount, byte[] buffer, int offset, bool ignoreNonBase64Chars)
        {
            if (encBuffer == null)
            {
                throw new ArgumentNullException(nameof(encBuffer));
            }
            if (encOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(encOffset), "Argument 'encOffset' value must be >= 0.");
            }
            if (encCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(encCount), "Argument 'encCount' value must be >= 0.");
            }
            if (encOffset + encCount > encBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(encCount), "Argument 'count' is bigger than than argument 'encBuffer'.");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            /* RFC 4648.

                Base64 is processed from left to right by 4 6-bit byte block, 4 6-bit byte block
                are converted to 3 8-bit bytes.
                If base64 4 byte block doesn't have 3 8-bit bytes, missing bytes are marked with =.

                Value Encoding  Value Encoding  Value Encoding  Value Encoding
                    0 A            17 R            34 i            51 z
                    1 B            18 S            35 j            52 0
                    2 C            19 T            36 k            53 1
                    3 D            20 U            37 l            54 2
                    4 E            21 V            38 m            55 3
                    5 F            22 W            39 n            56 4
                    6 G            23 X            40 o            57 5
                    7 H            24 Y            41 p            58 6
                    8 I            25 Z            42 q            59 7
                    9 J            26 a            43 r            60 8
                    10 K           27 b            44 s            61 9
                    11 L           28 c            45 t            62 +
                    12 M           29 d            46 u            63 /
                    13 N           30 e            47 v
                    14 O           31 f            48 w         (pad) =
                    15 P           32 g            49 x
                    16 Q           33 h            50 y

                NOTE: 4 base64 6-bit bytes = 3 8-bit bytes
                    // |    6-bit    |    6-bit    |    6-bit    |    6-bit    |
                    // | 1 2 3 4 5 6 | 1 2 3 4 5 6 | 1 2 3 4 5 6 | 1 2 3 4 5 6 |
                    // |    8-bit         |    8-bit        |    8-bit         |
            */

            var decodeOffset = encOffset;
            var decodedOffset = 0;
            var base64Block = new byte[4];

            // Decode while we have data.
            while (decodeOffset - encOffset < encCount)
            {
                // Read 4-byte base64 block.
                var offsetInBlock = 0;
                while (offsetInBlock < 4)
                {
                    // Check that we won't exceed buffer data.
                    if (decodeOffset - encOffset >= encCount)
                    {
                        if (offsetInBlock == 0)
                        {
                            break;
                        }
                        // Incomplete 4-byte base64 data block.

                        throw new FormatException("Invalid incomplete base64 4-char block");
                    }

                    // Read byte.
                    short b = encBuffer[decodeOffset++];

                    // Pad char.
                    if (b == '=')
                    {
                        // Padding may appear only in last two chars of 4-char block.
                        if (offsetInBlock < 2)
                        {
                            throw new FormatException("Invalid base64 padding.");
                        }

                        // Skip next padding char.
                        if (offsetInBlock == 2)
                        {
                            decodeOffset++;
                        }

                        break;
                    }
                    // Non-base64 char.

                    if (b > 127 || _base64_Decode_Table[b] == -1)
                    {
                        if (!ignoreNonBase64Chars)
                        {
                            throw new FormatException("Invalid base64 char '" + b + "'.");
                        }
                        // Ignore that char.
                        //else{
                    }
                    // Base64 char.
                    else
                    {
                        base64Block[offsetInBlock++] = (byte)_base64_Decode_Table[b];
                    }
                }

                // Decode base64 block.
                if (offsetInBlock > 1)
                {
                    buffer[decodedOffset++] = (byte)((base64Block[0] << 2) | (base64Block[1] >> 4));
                }
                if (offsetInBlock > 2)
                {
                    buffer[decodedOffset++] = (byte)(((base64Block[1] & 0xF) << 4) | (base64Block[2] >> 2));
                }
                if (offsetInBlock > 3)
                {
                    buffer[decodedOffset++] = (byte)(((base64Block[2] & 0x3) << 6) | base64Block[3]);
                }
            }

            return decodedOffset;
        }
    }
}
