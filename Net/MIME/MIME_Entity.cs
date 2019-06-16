using System;
using System.IO;
using System.Text;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// Represents a MIME entity. Defined in RFC 2045 2.4.
    /// </summary>
    public class MIME_Entity : IDisposable
    {
        private MIME_b m_pBody;
        private readonly MIME_b_Provider m_pBodyProvider;
        private MIME_h_Collection m_pHeader;
        private MIME_Entity m_pParent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MIME_Entity()
        {
            m_pHeader = new MIME_h_Collection(new MIME_h_Provider());
            m_pBodyProvider = new MIME_b_Provider();
        }

        /// <summary>
        /// Gets or sets MIME entity body.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public MIME_b Body
        {
            get { return m_pBody; }

            set
            {
                m_pBody = value ?? throw new ArgumentNullException("Body");
                m_pBody.SetParent(this, true);
            }
        }

        /// <summary>
        /// Gets or sets message alternative content. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Information about the media features of alternative content formats available for the current message.</remarks>
        public string ContentAlternative
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Alternative");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Alternative");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Alternative");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Alternative", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets base to be used for resolving relative URIs within this content part. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Base to be used for resolving relative URIs within this content part. See also Content-Location.</remarks>
        public string ContentBase
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Base");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Base");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Base");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Base", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets description of message body part. Value null means that header field does not exist. Defined in RFC 2045 8.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Description of a particular body part of a message; for example, a caption for an image body part.</remarks>
        public string ContentDescription
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Description");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Description");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Description");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Description", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets content disposition. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Indicates whether a MIME body part is to be shown inline or is an attachment; can also indicate a
        /// suggested filename for use when saving an attachment to a file.</remarks>
        /// <exception cref="ParseException">Is raised when header field parsing errors.</exception>
        public MIME_h_ContentDisposition ContentDisposition
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Disposition");
                if (h != null)
                {
                    if (!(h is MIME_h_ContentDisposition))
                    {
                        throw new ParseException("Header field 'ContentDisposition' parsing failed.");
                    }

                    return (MIME_h_ContentDisposition)h;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Disposition");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Disposition");
                    if (h == null)
                    {
                        m_pHeader.Add(value);
                    }
                    else
                    {
                        m_pHeader.ReplaceFirst(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets time duration of content. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Time duration of body part content, in seconds (e.g., for audio message).</remarks>
        public string ContentDuration
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Duration");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Duration");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Duration");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Duration", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets content features of a MIME body part. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>The 'Content-features:' header can be used to annotate a MIME body part with a media feature expression,
        /// to indicate features of the body part content. See also RFC 2533, RFC 2506, and RFC 2045.</remarks>
        public string Contentfeatures
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-features");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-features");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-features");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-features", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets content body part ID. Value null means that header field does not exist. Defined in RFC 2045 7.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Specifies a Unique ID for one MIME body part of the content of a message.</remarks>
        public string ContentID
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-ID");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-ID");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-ID");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-ID", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets language of message content. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Can include a code for the natural language used in a message; e.g., 'en' for English.
        /// Can also contain a list of languages for a message containing more than one language.</remarks>
        public string ContentLanguage
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Language");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Language");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Language");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Language", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets URI for retrieving a body part. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>URI using which the content of this body-part part was retrieved,
        /// might be retrievable, or which otherwise gives a globally unique identification of the content.</remarks>
        public string ContentLocation
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Location");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Location");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Location");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Location", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets content MD5 checksum. Value null means that header field does not exist.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Checksum of content to ensure that it has not been modified.</remarks>
        public string ContentMD5
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-MD5");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-MD5");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-MD5");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-MD5", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets content transfer encoding. Value null means that header field does not exist.
        /// RFC defined values are in <see cref="MIME_TransferEncodings">MIME_TransferEncodings</see>. Defined in RFC 2045 6.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>Coding method used in a MIME message body part.</remarks>
        public string ContentTransferEncoding
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Transfer-Encoding");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value.Trim();
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Transfer-Encoding");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Transfer-Encoding");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("Content-Transfer-Encoding", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets MIME content type. Value null means that header field does not exist. Defined in RFC 2045 5.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ParseException">Is raised when header field parsing errors.</exception>
        public MIME_h_ContentType ContentType
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("Content-Type");
                if (h != null)
                {
                    if (!(h is MIME_h_ContentType))
                    {
                        throw new ParseException("Header field 'ContentType' parsing failed.");
                    }

                    return (MIME_h_ContentType)h;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("Content-Type");
                }
                else
                {
                    var h = m_pHeader.GetFirst("Content-Type");
                    if (h == null)
                    {
                        m_pHeader.Add(value);
                    }
                    else
                    {
                        m_pHeader.ReplaceFirst(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets MIME entity header field collection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public MIME_h_Collection Header
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pHeader;
            }
        }

        // Permanent headerds list: http://www.rfc-editor.org/rfc/rfc4021.txt

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets if this entity is modified since it has loaded.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public bool IsModified
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pHeader.IsModified || m_pBody.IsModified;
            }
        }

        /// <summary>
        /// Gets or sets MIME version number. Value null means that header field does not exist. Normally this value is 1.0. Defined in RFC 2045 section 4.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <remarks>An indicator that this message is formatted according to the MIME
        /// standard, and an indication of which version of MIME is used.</remarks>
        public string MimeVersion
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var h = m_pHeader.GetFirst("MIME-Version");
                if (h != null)
                {
                    return ((MIME_h_Unstructured)h).Value;
                }

                return null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value == null)
                {
                    m_pHeader.RemoveAll("MIME-Version");
                }
                else
                {
                    var h = m_pHeader.GetFirst("MIME-Version");
                    if (h == null)
                    {
                        h = new MIME_h_Unstructured("MIME-Version", value);
                        m_pHeader.Add(h);
                    }
                    else
                    {
                        ((MIME_h_Unstructured)h).Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parent entity of this entity, returns null if this is the root entity.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public MIME_Entity Parent
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pParent;
            }
        }

        /// <summary>
        /// Cleans up any resources being used. This method is thread-safe.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (IsDisposed)
                {
                    return;
                }
                IsDisposed = true;

                m_pHeader = null;
                m_pParent = null;
            }
        }

        /// <summary>
        /// Returns MIME entity as byte[].
        /// </summary>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <returns>Returns MIME entity as byte[].</returns>
        public byte[] ToByte(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
        {
            return ToByte(headerWordEncoder, headerParmetersCharset, false);
        }

        /// <summary>
        /// Returns MIME entity as byte[].
        /// </summary>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <param name="headerReencode">If true always specified encoding is used for header. If false and header field value not modified,
        /// original encoding is kept.</param>
        /// <returns>Returns MIME entity as byte[].</returns>
        public byte[] ToByte(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ToStream(ms, headerWordEncoder, headerParmetersCharset, headerReencode);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Stores MIME entity to the specified file.
        /// </summary>
        /// <param name="file">File name with path where to store MIME entity.</param>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void ToFile(string file, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
        {
            ToFile(file, headerWordEncoder, headerParmetersCharset, false);
        }

        /// <summary>
        /// Stores MIME entity to the specified file.
        /// </summary>
        /// <param name="file">File name with path where to store MIME entity.</param>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <param name="headerReencode">If true always specified encoding is used for header. If false and header field value not modified,
        /// original encoding is kept.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void ToFile(string file, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (file == "")
            {
                throw new ArgumentException("Argument 'file' value must be specified.");
            }

            using (FileStream fs = File.Create(file))
            {
                ToStream(fs, headerWordEncoder, headerParmetersCharset, headerReencode);
            }
        }

        /// <summary>
        /// Store MIME enity to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store MIME entity. Storing starts form stream current position.</param>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null.</exception>
        public void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
        {
            ToStream(stream, headerWordEncoder, headerParmetersCharset, false);
        }

        /// <summary>
        /// Store MIME enity to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store MIME entity. Storing starts form stream current position.</param>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <param name="headerReencode">If true always specified encoding is used for header. If false and header field value not modified,
        /// original encoding is kept.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null.</exception>
        public void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            m_pHeader.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
            stream.Write(new byte[] { (int)'\r', (int)'\n' }, 0, 2);
            m_pBody.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
        }

        /// <summary>
        /// Returns MIME entity as string.
        /// </summary>
        /// <returns>Returns MIME entity as string.</returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns MIME entity as string.
        /// </summary>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <returns>Returns MIME entity as string.</returns>
        public string ToString(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
        {
            return ToString(headerWordEncoder, headerParmetersCharset, false);
        }

        /// <summary>
        /// Returns MIME entity as string.
        /// </summary>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <param name="headerReencode">If true always specified encoding is used for header. If false and header field value not modified,
        /// original encoding is kept.</param>
        /// <returns>Returns MIME entity as string.</returns>
        public string ToString(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ToStream(ms, headerWordEncoder, headerParmetersCharset, headerReencode);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Sets MIME entity parent entity.
        /// </summary>
        /// <param name="parent">Parent entity.</param>
        internal void SetParent(MIME_Entity parent)
        {
            m_pParent = parent;
        }

        /// <summary>
        /// Parses MIME entiry from the specified stream.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="headerEncoding">Header reading encoding. If not sure UTF-8 is recommended.</param>
        /// <param name="defaultContentType">Default content type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b>,<b>headerEncoding</b> or <b>defaultContentType</b> is null reference.</exception>
        internal protected void Parse(SmartStream stream, Encoding headerEncoding, MIME_h_ContentType defaultContentType)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (headerEncoding == null)
            {
                throw new ArgumentNullException("headerEncoding");
            }
            if (defaultContentType == null)
            {
                throw new ArgumentNullException("defaultContentType");
            }

            m_pHeader.Parse(stream, headerEncoding);

            m_pBody = m_pBodyProvider.Parse(this, stream, defaultContentType);
            m_pBody.SetParent(this, false);
        }
    }
}
