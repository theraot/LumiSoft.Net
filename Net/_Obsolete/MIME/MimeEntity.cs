using System;
using System.IO;
using System.Collections;
using System.Text;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.Mime
{
	/// <summary>
	/// Rfc 2822 Mime Entity.
	/// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MimeEntity
	{
        private readonly Hashtable             m_pHeaderFieldCache;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MimeEntity()
		{
			Header = new HeaderFieldCollection();
			ChildEntities = new MimeEntityCollection(this);
			m_pHeaderFieldCache = new Hashtable();            
		}

        /// <summary>
		/// Parses mime entity from stream.
		/// </summary>
		/// <param name="stream">Data stream from where to read data.</param>
		/// <param name="toBoundary">Entity data is readed to specified boundary.</param>
		/// <returns>Returns false if last entity. Returns true for mulipart entity, if there are more entities.</returns>
		internal bool Parse(SmartStream stream,string toBoundary)
		{
			// Clear header fields
			Header.Clear();
			m_pHeaderFieldCache.Clear();

			// Parse header
			Header.Parse(stream);

			// Parse entity and child entities if any (Conent-Type: multipart/xxx...)
            
			// Multipart entity
			if((this.ContentType & MediaType_enum.Multipart) != 0){
				// There must be be boundary ID (rfc 1341 7.2.1  The Content-Type field for multipart entities requires one parameter,
                // "boundary", which is used to specify the encapsulation boundary.)
				var boundaryID = this.ContentType_Boundary;
                if (boundaryID == null){
					// This is invalid message, just skip this mime entity
				}
				else{
					// There is one or more mime entities

                    // Find first boundary start position
                    var args = new SmartStream.ReadLineAsyncOP(new byte[8000],SizeExceededAction.JunkAndThrowException);
                    stream.ReadLine(args,false);
                    if(args.Error != null){
                        throw args.Error;
                    }
                    var lineString = args.LineUtf8;

                    while (lineString != null){
						if(lineString.StartsWith("--" + boundaryID)){
							break;
						}
						
                        stream.ReadLine(args,false);
                        if(args.Error != null){
                            throw args.Error;
                        }
                        lineString = args.LineUtf8;
					}
					// This is invalid entity, boundary start not found. Skip that entity.
					if(string.IsNullOrEmpty(lineString)){
						return false;
					}
					
					// Start parsing child entities of this entity
					while(true){					
						// Parse and add child entity
						var childEntity = new MimeEntity();
                        this.ChildEntities.Add(childEntity);
				
						// This is last entity, stop parsing
						if(childEntity.Parse(stream,boundaryID) == false){
							break;
						}
						// else{
						// There are more entities, parse them
					}
					
					// This entity is child of mulipart entity.
					// All this entity child entities are parsed,
					// we need to move stream position to next entity start.
					if(!string.IsNullOrEmpty(toBoundary)){
                        stream.ReadLine(args,false);
                        if(args.Error != null){
                            throw args.Error;
                        }
                        lineString = args.LineUtf8;

						while(lineString != null){
							if(lineString.StartsWith("--" + toBoundary)){
								break;
							}
							
							stream.ReadLine(args,false);
                            if(args.Error != null){
                                throw args.Error;
                            }
                            lineString = args.LineUtf8;
						}
						
						// Invalid boundary end, there can't be more entities 
						if(string.IsNullOrEmpty(lineString)){
							return false;
						}
					
						// See if last boundary or there is more. Last boundary ends with --
						if(lineString.EndsWith(toBoundary + "--")){
							return false; 
						}
						// else{
						// There are more entities					
						return true;
					}
				}
			}
			// Singlepart entity.
			else{
                // Boundary is specified, read data to specified boundary.
                if(!string.IsNullOrEmpty(toBoundary)){
                    var entityData = new MemoryStream();
                    var readLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000],SizeExceededAction.JunkAndThrowException);

                    // Read entity data while get boundary end tag --boundaryID-- or EOS.
                    while (true){                        
                        stream.ReadLine(readLineOP,false);
                        if(readLineOP.Error != null){
                            throw readLineOP.Error;
                        }
                        // End of stream reached. Normally we should get boundary end tag --boundaryID--, but some x mailers won't add it, so
                        // if we reach EOS, consider boundary closed.
                        if(readLineOP.BytesInBuffer == 0){
                            // Just return data what was readed.
                            DataEncoded = entityData.ToArray();
                            return false;
                        }
                        // We readed a line.

                        // We have boundary start/end tag or just "--" at the beginning of line.
                        if(readLineOP.LineBytesInBuffer >= 2 && readLineOP.Buffer[0] == '-' && readLineOP.Buffer[1] == '-'){
                            var lineString = readLineOP.LineUtf8;
                            // We have boundary end tag, no more boundaries.
                            if (lineString == "--" + toBoundary + "--"){
                                DataEncoded = entityData.ToArray();
                                return false;
                            }
                            // We have new boundary start.

                            if(lineString == "--" + toBoundary){
                                DataEncoded = entityData.ToArray();
                                return true;
                            }
                        }

                        // Write readed line.
                        entityData.Write(readLineOP.Buffer,0,readLineOP.BytesInBuffer);
                    }
                }
				// Boundary isn't specified, read data to the stream end. 

                var ms = new MemoryStream();
                stream.ReadAll(ms);
                DataEncoded = ms.ToArray();
            }
			
			return false;
		}

        /// <summary>
		/// Stores mime entity and it's child entities to specified stream.
		/// </summary>
		/// <param name="storeStream">Stream where to store mime entity.</param>
		public void ToStream(Stream storeStream)
		{			
			// Write headers
			var data = System.Text.Encoding.Default.GetBytes(FoldHeader(this.HeaderString));
            storeStream.Write(data,0,data.Length);

			// If multipart entity, write child entities.(multipart entity don't contain data, it contains nested entities )
			if((this.ContentType & MediaType_enum.Multipart) != 0){
				var boundary = this.ContentType_Boundary;
                foreach (MimeEntity entity in this.ChildEntities){
					// Write boundary start. Syntax: <CRLF>--BoundaryID<CRLF>
					data = System.Text.Encoding.Default.GetBytes("\r\n--" + boundary + "\r\n");
					storeStream.Write(data,0,data.Length);

					// Force child entity to store itself
                    entity.ToStream(storeStream);
				}

				// Write boundaries end Syntax: <CRLF>--BoundaryID--<CRLF>
				data = System.Text.Encoding.Default.GetBytes("\r\n--" + boundary + "--\r\n");
				storeStream.Write(data,0,data.Length);
			}
			// If singlepart (text,image,audio,video,message, ...), write entity data.
			else{				
				// Write blank line between headers and content
				storeStream.Write(new[]{(byte)'\r',(byte)'\n'},0,2);

				if(this.DataEncoded != null){
					storeStream.Write(this.DataEncoded,0,this.DataEncoded.Length);
				}				
			}
		}

        /// <summary>
		/// Saves this.Data property value to specified file.
		/// </summary>
		/// <param name="fileName">File name where to store data.</param>
		public void DataToFile(string fileName)
		{
			using(FileStream fs = File.Create(fileName)){
				DataToStream(fs);
			}
		}

        /// <summary>
		/// Saves this.Data property value to specified stream.
		/// </summary>
		/// <param name="stream">Stream where to store data.</param>
		public void DataToStream(Stream stream)
		{
			var data = this.Data;
            stream.Write(data,0,data.Length);
		}

        /// <summary>
		/// Loads MimeEntity.Data property from file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public void DataFromFile(string fileName)
		{
			using(FileStream fs = File.OpenRead(fileName)){
				DataFromStream(fs);
			}
		}

        /// <summary>
		/// Loads MimeEntity.Data property from specified stream. Note: reading starts from current position and stream isn't closed.
		/// </summary>
		/// <param name="stream">Data stream.</param>
		public void DataFromStream(Stream stream)
		{
			var data = new byte[stream.Length];
            stream.Read(data,0,(int)stream.Length);

			this.Data = data;
		}

        /// <summary>
		/// Encodes data with specified content transfer encoding.
		/// </summary>
		/// <param name="data">Data to encode.</param>
		/// <param name="encoding">Encoding with what to encode data.</param>
		private byte[] EncodeData(byte[] data,ContentTransferEncoding_enum encoding)
		{
			// Allow only known Content-Transfer-Encoding (ContentTransferEncoding_enum value),
            // otherwise we don't know how to encode data.
			if(encoding == ContentTransferEncoding_enum.NotSpecified){
				throw new Exception("Please specify Content-Transfer-Encoding first !");
			}
			if(encoding == ContentTransferEncoding_enum.Unknown){
				throw new Exception("Not supported Content-Transfer-Encoding. If it's your custom encoding, encode data yourself and set it with DataEncoded property !");
			}
				
			if(encoding == ContentTransferEncoding_enum.Base64){
				return Core.Base64Encode(data);
			}

            if(encoding == ContentTransferEncoding_enum.QuotedPrintable){
                return Core.QuotedPrintableEncode(data);
            }
            return data;
        }

        /// <summary>
		/// Folds header.
		/// </summary>
		/// <param name="header">Header string.</param>
		/// <returns></returns>
		private string FoldHeader(string header)
		{			
			/* Rfc 2822 2.2.3 Long Header Fields
				Each header field is logically a single line of characters comprising
				the field name, the colon, and the field body.  For convenience
				however, and to deal with the 998/78 character limitations per line,
				the field body portion of a header field can be split into a multiple
				line representation; this is called "folding".  The general rule is
				imply WSP characters), a CRLF may be inserted before any WSP.  For
				example, the header field:

					Subject: This is a test

					can be represented as:

							Subject: This
								is a test
			*/

			// Just fold header fields what contain <TAB>

			var retVal = new StringBuilder();

            header = header.Replace("\r\n","\n");
			var headerLines = header.Split('\n');
            foreach (string headerLine in headerLines){
				// Folding is needed
				if(headerLine.IndexOf('\t') > -1){
					retVal.Append(headerLine.Replace("\t","\r\n\t") + "\r\n");
				}
				// No folding needed, just write back header line
				else{
					retVal.Append(headerLine + "\r\n");
				}
			}
			// Split splits last line <CRLF> to element, but we don't need it 
			if(retVal.Length > 1){
				return retVal.ToString(0,retVal.Length - 2);
			}

            return retVal.ToString();
        }

        /// <summary>
		/// Gets message header.
		/// </summary>
		public HeaderFieldCollection Header { get; }

        /// <summary>
		/// Gets header as RFC 2822 message headers.
		/// </summary>
		public string HeaderString
		{			
			get{ return Header.ToHeaderString("utf-8"); }
		}

		/// <summary>
		/// Gets parent entity of this entity. If this entity is top level, then this property returns null.
		/// </summary>
		public MimeEntity ParentEntity { get; } = null;

        /// <summary>
		/// Gets child entities. This property is available only if ContentType = multipart/... .
		/// </summary>
		public MimeEntityCollection ChildEntities { get; }

        /// <summary>
		/// Gets or sets header field "<b>Mime-Version:</b>" value. Returns null if value isn't set.
		/// </summary>
		public string MimeVersion
		{
			get
            {
                if(Header.Contains("Mime-Version:")){
					return Header.GetFirst("Mime-Version:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("Mime-Version:")){
					Header.GetFirst("Mime-Version:").Value = value;
				}
				else{
					Header.Add("Mime-Version:",value);
				}
			}
		}

        /// <summary>
        /// Gets or sets header field "<b>Content-class:</b>" value. Returns null if value isn't set.<br/>
        /// Additional property to support messages of CalendarItem type which have iCal/vCal entries.
        /// </summary>
        public string ContentClass
        {
            get
            {
                if(Header.Contains("Content-Class:")){
                    return Header.GetFirst("Content-Class:").Value;
                }

                return null;
            }

            set{
                if(Header.Contains("Content-Class:")){
                    Header.GetFirst("Content-Class:").Value = value;
                }
                else{
                    Header.Add("Content-Class:", value);
                }
            }
        }

		/// <summary>
		/// Gets or sets header field "<b>Content-Type:</b>" value. This property specifies what entity data is.
		/// NOTE: ContentType can't be changed while there is data specified(Exception is thrown) in this mime entity, because it isn't
		/// possible todo data conversion between different types. For example text/xx has charset parameter and other types don't,
		/// changing loses it and text data becomes useless.
		/// </summary>
		public MediaType_enum ContentType
		{
			get
            {
                if(Header.Contains("Content-Type:")){  
					var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Type:")).Value;
                    return MimeUtils.ParseMediaType(contentType);
				}

                return MediaType_enum.NotSpecified;
            }

			set{
				if(this.DataEncoded != null){
					throw new Exception("ContentType can't be changed while there is data specified, set data to null before !");
				}
				if(value == MediaType_enum.Unknown){
					throw new Exception("MediaType_enum.Unkown isn't allowed to set !");
				}
				if(value == MediaType_enum.NotSpecified){
					throw new Exception("MediaType_enum.NotSpecified isn't allowed to set !");
				}
				
				var contentType = "";
                //--- Text/xxx --------------------------------//
                if (value == MediaType_enum.Text_plain){
					contentType = "text/plain; charset=\"utf-8\"";
				}
				else if(value == MediaType_enum.Text_html){
					contentType = "text/html; charset=\"utf-8\"";
				}
				else if(value == MediaType_enum.Text_xml){
					contentType = "text/xml; charset=\"utf-8\"";
				}
				else if(value == MediaType_enum.Text_rtf){
					contentType = "text/rtf; charset=\"utf-8\"";
				}
				else if(value == MediaType_enum.Text){
					contentType = "text; charset=\"utf-8\"";
				}
				//---------------------------------------------//

				//--- Image/xxx -------------------------------//
				else if(value == MediaType_enum.Image_gif){
					contentType = "image/gif";
				}
				else if(value == MediaType_enum.Image_tiff){
					contentType = "image/tiff";
				}
				else if(value == MediaType_enum.Image_jpeg){
					contentType = "image/jpeg";
				}
				else if(value == MediaType_enum.Image){
					contentType = "image";
				}
				//---------------------------------------------//

				//--- Audio/xxx -------------------------------//
				else if(value == MediaType_enum.Audio){
					contentType = "audio";
				}
				//---------------------------------------------//

				//--- Video/xxx -------------------------------//
				else if(value == MediaType_enum.Video){
					contentType = "video";
				}
				//---------------------------------------------//

				//--- Application/xxx -------------------------//
				else if(value == MediaType_enum.Application_octet_stream){
					contentType = "application/octet-stream";
				}
				else if(value == MediaType_enum.Application){
					contentType = "application";
				}
				//---------------------------------------------//

				//--- Multipart/xxx ---------------------------//
				else if(value == MediaType_enum.Multipart_mixed){
					contentType = "multipart/mixed;	boundary=\"part_" + Guid.NewGuid().ToString().Replace("-","_") + "\"";
				}
				else if(value == MediaType_enum.Multipart_alternative){
					contentType = "multipart/alternative;	boundary=\"part_" + Guid.NewGuid().ToString().Replace("-","_") + "\"";
				}
				else if(value == MediaType_enum.Multipart_parallel){
					contentType = "multipart/parallel;	boundary=\"part_" + Guid.NewGuid().ToString().Replace("-","_") + "\"";
				}
				else if(value == MediaType_enum.Multipart_related){
					contentType = "multipart/related;	boundary=\"part_" + Guid.NewGuid().ToString().Replace("-","_") + "\"";
				}
				else if(value == MediaType_enum.Multipart_signed){
					contentType = "multipart/signed;	boundary=\"part_" + Guid.NewGuid().ToString().Replace("-","_") + "\"";
				}
				else if(value == MediaType_enum.Multipart){
					contentType = "multipart;	boundary=\"part_" + Guid.NewGuid().ToString().Replace("-","_") + "\"";
				}
				//---------------------------------------------//

				//--- Message/xxx -----------------------------//
				else if(value == MediaType_enum.Message_rfc822){
					contentType = "message/rfc822";
				}
				else if(value == MediaType_enum.Message){
					contentType = "message";
				}
				//---------------------------------------------//

				else{
					throw new Exception("Invalid flags combination of MediaType_enum was specified !");
				}

				if(Header.Contains("Content-Type:")){
					Header.GetFirst("Content-Type:").Value = contentType;
				}
				else{
					Header.Add("Content-Type:",contentType);
				}
			}
		}

		
		/// <summary>
		/// Gets or sets header field "<b>Content-Type:</b>" value. Returns null if value isn't set. This property specifies what entity data is.
		/// This property is meant for advanced users, who needs other values what defined MediaType_enum provides.
		/// Example value: text/plain; charset="utf-8". 
		/// NOTE: ContentType can't be changed while there is data specified(Exception is thrown) in this mime entity, because it isn't
		/// possible todo data conversion between different types. For example text/xx has charset parameter and other types don't,
		/// changing loses it and text data becomes useless.
		/// </summary>
		public string ContentTypeString
		{
			get
            {
                if(Header.Contains("Content-Type:")){
					return Header.GetFirst("Content-Type:").Value;
				}

                return null;
            }

			set{ 
				if(this.DataEncoded != null){
					throw new Exception("ContentType can't be changed while there is data specified, set data to null before !");
				}
				if(Header.Contains("Content-Type:")){
					Header.GetFirst("Content-Type:").Value = value;
				}
				else{
					Header.Add("Content-Type:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Content-Transfer-Encoding:</b>" value. This property specifies how data is encoded/decoded.
		/// If you set this value, it's recommended that you use QuotedPrintable for text and Base64 for binary data.
		/// 7bit,_8bit,Binary are today obsolete (used for parsing). 
		/// </summary>
		public ContentTransferEncoding_enum ContentTransferEncoding
		{
			get
            {
                if(Header.Contains("Content-Transfer-Encoding:")){
					return MimeUtils.ParseContentTransferEncoding(Header.GetFirst("Content-Transfer-Encoding:").Value);
				}

                return ContentTransferEncoding_enum.NotSpecified;
            }

			set{
				if(value == ContentTransferEncoding_enum.Unknown){
					throw new Exception("ContentTransferEncoding_enum.Unknown isn't allowed to set !");
				}
				if(value == ContentTransferEncoding_enum.NotSpecified){
					throw new Exception("ContentTransferEncoding_enum.NotSpecified isn't allowed to set !");
				}

				var encoding = MimeUtils.ContentTransferEncodingToString(value);

                // There is entity data specified and encoding changed, we need to convert existing data
                if (this.DataEncoded != null){
					var oldEncoding = this.ContentTransferEncoding;
                    if (oldEncoding == ContentTransferEncoding_enum.Unknown || oldEncoding == ContentTransferEncoding_enum.NotSpecified){
						throw new Exception("Data can't be converted because old encoding '" + MimeUtils.ContentTransferEncodingToString(oldEncoding) + "' is unknown !");
					}

					this.DataEncoded = EncodeData(this.Data,value);
				}

				if(Header.Contains("Content-Transfer-Encoding:")){
					Header.GetFirst("Content-Transfer-Encoding:").Value = encoding;
				}
				else{
					Header.Add("Content-Transfer-Encoding:",encoding);
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Content-Disposition:</b>" value.
		/// </summary>
		public ContentDisposition_enum ContentDisposition
		{
			get
            {
                if(Header.Contains("Content-Disposition:")){
					return MimeUtils.ParseContentDisposition(Header.GetFirst("Content-Disposition:").Value);
				}

                return ContentDisposition_enum.NotSpecified;
            }

			set{
				if(value == ContentDisposition_enum.Unknown){
					throw new Exception("ContentDisposition_enum.Unknown isn't allowed to set !");
				}

				// Just remove Content-Disposition: header field if exists
				if(value == ContentDisposition_enum.NotSpecified){
					var disposition = Header.GetFirst("Content-Disposition:");
                    if (disposition != null){
						Header.Remove(disposition);
					}
				}
				else{
					var disposition = MimeUtils.ContentDispositionToString(value);
                    if (Header.Contains("Content-Disposition:")){
						Header.GetFirst("Content-Disposition:").Value = disposition;
					}
					else{
						Header.Add("Content-Disposition:",disposition);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Content-Description:</b>" value. Returns null if value isn't set.
		/// </summary>
		public string ContentDescription
		{
			get
            {
                if(Header.Contains("Content-Description:")){
					return Header.GetFirst("Content-Description:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("Content-Description:")){
					Header.GetFirst("Content-Description:").Value = value;
				}
				else{
					Header.Add("Content-Description:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Content-ID:</b>" value. Returns null if value isn't set.
		/// </summary>
		public string ContentID
		{
			get
            {
                if(Header.Contains("Content-ID:")){
					return Header.GetFirst("Content-ID:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("Content-ID:")){
					Header.GetFirst("Content-ID:").Value = value;
				}
				else{
					Header.Add("Content-ID:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets "<b>Content-Type:</b>" header field "<b>name</b>" parameter.
		/// Returns null if Content-Type: header field value isn't set or Content-Type: header field "<b>name</b>" parameter isn't set.
		/// <p/>
		/// Note: Content-Type must be application/xxx or exception is thrown.
		/// This property is obsolete today, it's replaced with <b>Content-Disposition: filename</b> parameter.
		/// If possible always set FileName property instead of it. 
		/// </summary>
		public string ContentType_Name
		{
			get
            {
                if(Header.Contains("Content-Type:")){
					var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Type:"));
                    if (contentType.Parameters.Contains("name")){
						return contentType.Parameters["name"];
					}

                    return null;
                }

                return null;
            }

			set{
				if(!Header.Contains("Content-Type:")){
					throw new Exception("Please specify Content-Type first !");
				}
				if((this.ContentType & MediaType_enum.Application) == 0){
					throw new Exception("Parameter name is available only for ContentType application/xxx !");
				}

				var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Type:"));
                if (contentType.Parameters.Contains("name")){
					contentType.Parameters["name"] = value;
				}
				else{
					contentType.Parameters.Add("name",value);
				}
			}
		}
        
		/// <summary>
		/// Gets or sets "<b>Content-Type:</b>" header field "<b>charset</b>" parameter.
		/// Returns null if Content-Type: header field value isn't set or Content-Type: header field "<b>charset</b>" parameter isn't set.
		/// If you don't know what charset to use then <b>utf-8</b> is recommended, most of times this is sufficient.
		/// Note: Content-Type must be text/xxx or exception is thrown.
		/// </summary>
		public string ContentType_CharSet
		{
			get
            {
                if(Header.Contains("Content-Type:")){
					var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Type:"));
                    if (contentType.Parameters.Contains("charset")){
						return contentType.Parameters["charset"];
					}

                    return null;
                }

                return null;
            }

			set{
				if(!Header.Contains("Content-Type:")){
					throw new Exception("Please specify Content-Type first !");
				}
				if((this.ContentType & MediaType_enum.Text) == 0){
					throw new Exception("Parameter boundary is available only for ContentType text/xxx !");
				}

				// There is data specified, we need to convert it because charset changed
				if(this.DataEncoded != null){
					var currentCharSet = this.ContentType_CharSet;
                    if (currentCharSet == null){
                        currentCharSet = "ascii";
                    }
					try{
						System.Text.Encoding.GetEncoding(currentCharSet);
					}
					catch{
						throw new Exception("Data can't be converted because current charset '" + currentCharSet + "' isn't supported !");
					}
					try{
						var encoding = System.Text.Encoding.GetEncoding(value);
                        this.Data = encoding.GetBytes(this.DataText);
					}
					catch{
						throw new Exception("Data can't be converted because new charset '" + value + "' isn't supported !");
					}
				}

				var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Type:"));
                if (contentType.Parameters.Contains("charset")){
					contentType.Parameters["charset"] = value;
				}
				else{
					contentType.Parameters.Add("charset",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets "<b>Content-Type:</b>" header field "<b>boundary</b>" parameter.
		/// Returns null if Content-Type: header field value isn't set or Content-Type: header field "<b>boundary</b>" parameter isn't set.
		/// Note: Content-Type must be multipart/xxx or exception is thrown.
		/// </summary>
		public string ContentType_Boundary
		{
			get
            {
                if(Header.Contains("Content-Type:")){
					var contentDisposition = new ParametizedHeaderField(Header.GetFirst("Content-Type:"));
                    if (contentDisposition.Parameters.Contains("boundary")){
						return contentDisposition.Parameters["boundary"];
					}

                    return null;
                }

                return null;
            }

			set{
				if(!Header.Contains("Content-Type:")){
					throw new Exception("Please specify Content-Type first !");
				}
				if((this.ContentType & MediaType_enum.Multipart) == 0){
					throw new Exception("Parameter boundary is available only for ContentType multipart/xxx !");
				}

				var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Type:"));
                if (contentType.Parameters.Contains("boundary")){
					contentType.Parameters["boundary"] = value;
				}
				else{
					contentType.Parameters.Add("boundary",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets "<b>Content-Disposition:</b>" header field "<b>filename</b>" parameter.
		/// Returns null if Content-Disposition: header field value isn't set or Content-Disposition: header field "<b>filename</b>" parameter isn't set.
		/// Note: Content-Disposition must be attachment or inline.
		/// </summary>
		public string ContentDisposition_FileName
		{
			get
            {
                if(Header.Contains("Content-Disposition:")){
					var contentDisposition = new ParametizedHeaderField(Header.GetFirst("Content-Disposition:"));
                    if (contentDisposition.Parameters.Contains("filename")){
						return MimeUtils.DecodeWords(contentDisposition.Parameters["filename"]);
					}

                    return null;
                }

                return null;
            }

			set{
				if(!Header.Contains("Content-Disposition:")){
					throw new Exception("Please specify Content-Disposition first !");
				}

				var contentType = new ParametizedHeaderField(Header.GetFirst("Content-Disposition:"));
                if (contentType.Parameters.Contains("filename")){
					contentType.Parameters["filename"] = MimeUtils.EncodeWord(value);
				}
				else{
					contentType.Parameters.Add("filename",MimeUtils.EncodeWord(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Date:</b>" value.
		/// </summary>
		public DateTime Date
		{
			get
            {
                if(Header.Contains("Date:")){
					try{
						return LumiSoft.Net.MIME.MIME_Utils.ParseRfc2822DateTime(Header.GetFirst("Date:").Value);
					}
					catch{
						return DateTime.MinValue;
					}
				}

                return DateTime.MinValue;
            }

			set{ 
				if(Header.Contains("Date:")){
					Header.GetFirst("Date:").Value = LumiSoft.Net.MIME.MIME_Utils.DateTimeToRfc2822(value);
				}
				else{
					Header.Add("Date:",MimeUtils.DateTimeToRfc2822(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Message-ID:</b>" value. Returns null if value isn't set.
		/// Syntax: '&lt;'id-left@id-right'&gt;'. Example: &lt;621bs724bfs8@jnfsjaas4263&gt;
		/// </summary>
		public string MessageID
		{
			get
            {
                if(Header.Contains("Message-ID:")){
					return Header.GetFirst("Message-ID:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("Message-ID:")){
					Header.GetFirst("Message-ID:").Value = value;
				}
				else{
					Header.Add("Message-ID:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>To:</b>" value. Returns null if value isn't set.
		/// </summary>
		public AddressList To
		{
			get
            {
                if(Header.Contains("To:")){
					// There is already cached version, return it
					if(m_pHeaderFieldCache.Contains("To:")){
						return (AddressList)m_pHeaderFieldCache["To:"];
					}
					// These isn't cached version, we need to create it

                    // Create and bound address-list to existing header field
                    var field = Header.GetFirst("To:");
                    var list = new AddressList();
                    list.Parse(field.EncodedValue);
                    list.BoundedHeaderField = field;

                    // Cache header field
                    m_pHeaderFieldCache["To:"] = list;

                    return list;
                }

                return null;
            }

			set{
				// Just remove header field
				if(value == null){
					Header.Remove(Header.GetFirst("To:"));
					return;
				}

				// Release old address collection
				if(m_pHeaderFieldCache["To:"] != null){
					((AddressList)m_pHeaderFieldCache["To:"]).BoundedHeaderField = null;
				}

				// Bound address-list to To: header field. If header field doesn't exist, add it.
				var to = Header.GetFirst("To:");
                if (to == null){
					to = new HeaderField("To:",value.ToAddressListString());
					Header.Add(to);
				}
                else{
                    to.Value = value.ToAddressListString();
                }
				value.BoundedHeaderField = to;

                m_pHeaderFieldCache["To:"] = value;
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Cc:</b>" value. Returns null if value isn't set.
		/// </summary>
		public AddressList Cc
		{
			get
            {
                if(Header.Contains("Cc:")){
					// There is already cached version, return it
					if(m_pHeaderFieldCache.Contains("Cc:")){
						return (AddressList)m_pHeaderFieldCache["Cc:"];
					}
					// These isn't cached version, we need to create it

                    // Create and bound address-list to existing header field
                    var field = Header.GetFirst("Cc:");
                    var list = new AddressList();
                    list.Parse(field.EncodedValue);
                    list.BoundedHeaderField = field;

                    // Cache header field
                    m_pHeaderFieldCache["Cc:"] = list;

                    return list;
                }

                return null;
            }

			set{
				// Just remove header field
				if(value == null){
					Header.Remove(Header.GetFirst("Cc:"));
					return;
				}

				// Release old address collection
				if(m_pHeaderFieldCache["Cc:"] != null){
					((AddressList)m_pHeaderFieldCache["Cc:"]).BoundedHeaderField = null;
				}

				// Bound address-list to To: header field. If header field doesn't exist, add it.
				var cc = Header.GetFirst("Cc:");
                if (cc == null){
					cc = new HeaderField("Cc:",value.ToAddressListString());
					Header.Add(cc);
				}
                else{
                    cc.Value = value.ToAddressListString();
                }
				value.BoundedHeaderField = cc;

                m_pHeaderFieldCache["Cc:"] = value;
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Bcc:</b>" value. Returns null if value isn't set.
		/// </summary>
		public AddressList Bcc
		{
			get
            {
                if(Header.Contains("Bcc:")){
					// There is already cached version, return it
					if(m_pHeaderFieldCache.Contains("Bcc:")){
						return (AddressList)m_pHeaderFieldCache["Bcc:"];
					}
					// These isn't cached version, we need to create it

                    // Create and bound address-list to existing header field
                    var field = Header.GetFirst("Bcc:");
                    var list = new AddressList();
                    list.Parse(field.EncodedValue);
                    list.BoundedHeaderField = field;

                    // Cache header field
                    m_pHeaderFieldCache["Bcc:"] = list;

                    return list;
                }

                return null;
            }

			set{
				// Just remove header field
				if(value == null){
					Header.Remove(Header.GetFirst("Bcc:"));
					return;
				}

				// Release old address collection
				if(m_pHeaderFieldCache["Bcc:"] != null){
					((AddressList)m_pHeaderFieldCache["Bcc:"]).BoundedHeaderField = null;
				}

				// Bound address-list to To: header field. If header field doesn't exist, add it.
				var bcc = Header.GetFirst("Bcc:");
                if (bcc == null){
					bcc = new HeaderField("Bcc:",value.ToAddressListString());
					Header.Add(bcc);
				}
                else{
                    bcc.Value = value.ToAddressListString();
                }
				value.BoundedHeaderField = bcc;

                m_pHeaderFieldCache["Bcc:"] = value;
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>From:</b>" value. Returns null if value isn't set.
		/// </summary>
		public AddressList From
		{
			get
            {
                if(Header.Contains("From:")){
					// There is already cached version, return it
					if(m_pHeaderFieldCache.Contains("From:")){
						return (AddressList)m_pHeaderFieldCache["From:"];
					}
					// These isn't cached version, we need to create it

                    // Create and bound address-list to existing header field
                    var field = Header.GetFirst("From:");
                    var list = new AddressList();
                    list.Parse(field.EncodedValue);                        
                    list.BoundedHeaderField = field;

                    // Cache header field
                    m_pHeaderFieldCache["From:"] = list;

                    return list;
                }

                return null;
            }

			set{
				// Just remove header field
				if(value == null && Header.Contains("From:")){
					Header.Remove(Header.GetFirst("From:"));
					return;
				}

				// Release old address collection
				if(m_pHeaderFieldCache["From:"] != null){
					((AddressList)m_pHeaderFieldCache["From:"]).BoundedHeaderField = null;
				}

				// Bound address-list to To: header field. If header field doesn't exist, add it.
				var from = Header.GetFirst("From:");
                if (from == null){
					from = new HeaderField("From:",value.ToAddressListString());
					Header.Add(from);
				}
                else{
                    from.Value = value.ToAddressListString();
                }
				value.BoundedHeaderField = from;

                m_pHeaderFieldCache["From:"] = value;
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Sender:</b>" value. Returns null if value isn't set.
		/// </summary>
		public MailboxAddress Sender
		{
			get
            {
                if(Header.Contains("Sender:")){
					return MailboxAddress.Parse(Header.GetFirst("Sender:").EncodedValue);
				}

                return null;
            }

			set{ 
				if(Header.Contains("Sender:")){
					Header.GetFirst("Sender:").Value = value.ToMailboxAddressString();
				}
				else{
					Header.Add("Sender:",value.ToMailboxAddressString());
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Reply-To:</b>" value. Returns null if value isn't set.
		/// </summary>
		public AddressList ReplyTo
		{
			get
            {
                if(Header.Contains("Reply-To:")){
					// There is already cached version, return it
					if(m_pHeaderFieldCache.Contains("Reply-To:")){
						return (AddressList)m_pHeaderFieldCache["Reply-To:"];
					}
					// These isn't cached version, we need to create it

                    // Create and bound address-list to existing header field
                    var field = Header.GetFirst("Reply-To:");
                    var list = new AddressList();
                    list.Parse(field.Value);
                    list.BoundedHeaderField = field;

                    // Cache header field
                    m_pHeaderFieldCache["Reply-To:"] = list;

                    return list;
                }

                return null;
            }

			set{
				// Just remove header field
				if(value == null && Header.Contains("Reply-To:")){
					Header.Remove(Header.GetFirst("Reply-To:"));
					return;
				}

				// Release old address collection
				if(m_pHeaderFieldCache["Reply-To:"] != null){
					((AddressList)m_pHeaderFieldCache["Reply-To:"]).BoundedHeaderField = null;
				}

				// Bound address-list to To: header field. If header field doesn't exist, add it.
				var replyTo = Header.GetFirst("Reply-To:");
                if (replyTo == null){
					replyTo = new HeaderField("Reply-To:",value.ToAddressListString());
					Header.Add(replyTo);
				}
                else{
                    replyTo.Value = value.ToAddressListString();
                }
				value.BoundedHeaderField = replyTo;

                m_pHeaderFieldCache["Reply-To:"] = value;
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>In-Reply-To:</b>" value. Returns null if value isn't set.
		/// </summary>
		public string InReplyTo
		{
			get
            {
                if(Header.Contains("In-Reply-To:")){
					return Header.GetFirst("In-Reply-To:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("In-Reply-To:")){
					Header.GetFirst("In-Reply-To:").Value = value;
				}
				else{
					Header.Add("In-Reply-To:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Disposition-Notification-To:</b>" value. Returns null if value isn't set.
		/// </summary>
		public string DSN
		{
			get
            {
                if(Header.Contains("Disposition-Notification-To:")){
					return Header.GetFirst("Disposition-Notification-To:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("Disposition-Notification-To:")){
					Header.GetFirst("Disposition-Notification-To:").Value = value;
				}
				else{
					Header.Add("Disposition-Notification-To:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets header field "<b>Subject:</b>" value. Returns null if value isn't set.
		/// </summary>
		public string Subject
		{
			get
            {
                if(Header.Contains("Subject:")){
					return Header.GetFirst("Subject:").Value;
				}

                return null;
            }

			set{ 
				if(Header.Contains("Subject:")){
					Header.GetFirst("Subject:").Value = value;
				}
				else{
					Header.Add("Subject:",value);
				}
			}
		}

		/// <summary>
		/// Gets or sets entity data. Data is encoded/decoded with "<b>Content-Transfer-Encoding:</b>" header field value.
		/// Note: This property can be set only if Content-Type: isn't multipart.
		/// </summary>
		public byte[] Data
		{
			get{ 
				// Decode Data
				var encoding = this.ContentTransferEncoding;
                if (encoding == ContentTransferEncoding_enum.Base64){
					return Core.Base64Decode(this.DataEncoded);				
				}

                if(encoding == ContentTransferEncoding_enum.QuotedPrintable){
                    return Core.QuotedPrintableDecode(this.DataEncoded);
                }
                return this.DataEncoded;
            }

			set{
				if(value == null){
					this.DataEncoded = null;
					return;
				}
				
				var encoding = this.ContentTransferEncoding;
                this.DataEncoded = EncodeData(value,encoding);
			}
		}

		/// <summary>
		/// Gets or sets entity text data. Data is encoded/decoded with "<b>Content-Transfer-Encoding:</b>" header field value with this.Charset charset.
		/// Note: This property is available only if ContentType is Text/xxx... or no content type specified, othwerwise Excpetion is thrown.
		/// </summary>
		public string DataText
		{			
			get{ 
				if((this.ContentType & MediaType_enum.Text) == 0 && (this.ContentType & MediaType_enum.NotSpecified) == 0){
					throw new Exception("This property is available only if ContentType is Text/xxx... !");
				}

				try{
					var charSet = this.ContentType_CharSet;
                    // Charset isn't specified, use system default
                    if (charSet == null){
						return System.Text.Encoding.Default.GetString(this.Data);
					}

                    return System.Text.Encoding.GetEncoding(charSet).GetString(this.Data);
                }
				// Not supported charset, use default
				catch{
					return System.Text.Encoding.Default.GetString(this.Data);
				}
			}

			set{
				if(value == null){
					this.DataEncoded = null;
					return;
				}

				// Check charset
				var charSet = this.ContentType_CharSet;
                if (charSet == null){
					throw new Exception("Please specify CharSet property first !");
				}
                               
                Encoding encoding = null;
				try{
                    encoding = Encoding.GetEncoding(charSet);					
				}
				catch{
					throw new Exception("Not supported charset '" + charSet + "' ! If you need to use this charset, then set data through Data or DataEncoded property.");
				}
                this.Data = encoding.GetBytes(value);
			}
		}

		/// <summary>
		/// Gets or sets entity encoded data. If you set this value, be sure that you encode this value as specified by Content-Transfer-Encoding: header field.
		/// Set this value only if you need custom Content-Transfer-Encoding: what current Mime class won't support, other wise set data through this.Data property. 
		/// Note: This property can be set only if Content-Type: isn't multipart.
		/// </summary>
		public byte[] DataEncoded { get; set; }
    }
}
