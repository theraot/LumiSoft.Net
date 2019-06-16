using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace LumiSoft.Net.ICMP
{
	/// <summary>
	/// ICMP type.
	/// </summary>
	public enum ICMP_Type
	{
		/// <summary>
		/// Echo rely.
		/// </summary>
		EchoReply = 0,

		/// <summary>
		/// Time to live exceeded reply.
		/// </summary>
		TimeExceeded = 11,

		/// <summary>
		/// Echo.
		/// </summary>
		Echo = 8,
    }

    /// <summary>
	/// Echo reply message.
	/// </summary>
	public class EchoMessage
	{
        private readonly int       m_TTL;

        /// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="ip">IP address what sent echo message.</param>
		/// <param name="ttl">Time to live in milli seconds.</param>
		/// <param name="time">Time what elapsed before getting echo response.</param>
		internal EchoMessage(IPAddress ip,int ttl,int time)
		{
			IPAddress  = ip;
			m_TTL  = ttl;
			ReplyTime = time;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        [Obsolete("Will be removed !")]
		public string ToStringEx()
		{
			return "TTL=" + m_TTL + "\tTime=" + ReplyTime + "ms" + "\tIP=" + IPAddress;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="messages"></param>
		/// <returns></returns>
        [Obsolete("Will be removed !")]
		public static string ToStringEx(EchoMessage[] messages)
		{
			var retVal = "";

            foreach (EchoMessage m in messages){
				retVal += m.ToStringEx() + "\r\n";
			}

			return retVal;
        }

        /// <summary>
        /// Gets IP address what sent echo message.
        /// </summary>
        public IPAddress IPAddress { get; }
        /*
        /// <summary>
        /// Gets time to live in milli seconds.
        /// </summary>
        public int TTL
        {
            get{ return m_TTL; }
        }*/

        /// <summary>
        /// Gets time in milliseconds what toke to get reply.
        /// </summary>
        public int ReplyTime { get; }
    }

    /// <summary>
	/// Icmp utils.
	/// </summary>
	public class Icmp
	{
        /// <summary>
		/// Traces specified ip.
		/// </summary>
		/// <param name="destIP">Destination IP address.</param>
		/// <returns></returns>
		public static EchoMessage[] Trace(string destIP)
		{
            return Trace(IPAddress.Parse(destIP),2000);
        }

		/// <summary>
		/// Traces specified ip.
		/// </summary>
		/// <param name="ip">IP address to tracce.</param>
		/// <param name="timeout">Send recieve timeout in milli seconds.</param>
		/// <returns></returns>
		public static EchoMessage[] Trace(IPAddress ip,int timeout)
		{
			var retVal = new List<EchoMessage>();

            //Create Raw ICMP Socket 
            var s = new Socket(AddressFamily.InterNetwork,SocketType.Raw,ProtocolType.Icmp);

            var ipdest = new IPEndPoint(ip,80);
            var endpoint = (EndPoint)(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0],80));

            ushort id         = (ushort)DateTime.Now.Millisecond;
			var sendPacket = CreatePacket(id);

            int continuesNoReply = 0;
			//send requests with increasing number of TTL
			for(int ittl=1;ittl<=30; ittl++){
				var buffer = new byte[1024];

                try
                {
					//Socket options to set TTL and Timeouts 
					s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.IpTimeToLive      ,ittl);
					s.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendTimeout   ,timeout); 
					s.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReceiveTimeout,timeout); 

					//Get current time
					var startTime = DateTime.Now;

                    //Send Request
                    s.SendTo(sendPacket,sendPacket.Length,SocketFlags.None,ipdest);
				
					//Receive				
					s.ReceiveFrom(buffer,buffer.Length,SocketFlags.None,ref endpoint);

					//Calculate time required
					var ts = DateTime.Now - startTime;
                    retVal.Add(new EchoMessage(((IPEndPoint)endpoint).Address,ittl,ts.Milliseconds));

					// Endpoint reached
					if(buffer[20] == (byte)ICMP_Type.EchoReply){
						break;
					}
					
					// Un wanted reply
					if(buffer[20] != (byte)ICMP_Type.TimeExceeded){
						throw new Exception("UnKnown error !");
					}

					continuesNoReply = 0;
				}
				catch{
					//ToDo: Handle recive/send timeouts
					continuesNoReply++;
				}

				// If there is 3 continues no reply, consider that destination host won't accept ping.
				if(continuesNoReply >= 3){
					break;
				}
			}

			return retVal.ToArray();
		}

        /// <summary>
        /// Pings specified destination host.
        /// </summary>
        /// <param name="ip">IP address to ping.</param>
        /// <param name="timeout">Send recieve timeout in milli seconds.</param>
        /// <returns></returns>
		public static EchoMessage Ping(IPAddress ip,int timeout)
		{
            //Create Raw ICMP Socket 
			var s = new Socket(AddressFamily.InterNetwork,SocketType.Raw,ProtocolType.Icmp);

            var ipdest = new IPEndPoint(ip,80);
            var endpoint = (EndPoint)(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0],80));

            ushort id         = (ushort)DateTime.Now.Millisecond;
			var sendPacket = CreatePacket(id);

            //Socket options to set TTL and Timeouts 
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.IpTimeToLive      ,30);
			s.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendTimeout   ,timeout); 
			s.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReceiveTimeout,timeout); 

			//Get current time
			var startTime = DateTime.Now;

            //Send Request
            s.SendTo(sendPacket,sendPacket.Length,SocketFlags.None,ipdest);
				
			//Receive
			var buffer = new byte[1024];
            s.ReceiveFrom(buffer,buffer.Length,SocketFlags.None,ref endpoint);
            			
			// Endpoint reached
			if(buffer[20] == (byte)ICMP_Type.EchoReply){				
			}					
			// Un wanted reply
			else if(buffer[20] != (byte)ICMP_Type.TimeExceeded){
				throw new Exception("UnKnown error !");
			}

            //Calculate time elapsed
			var ts = DateTime.Now - startTime;
            return new EchoMessage(((IPEndPoint)endpoint).Address,0,ts.Milliseconds);
        }

        private static byte[] CreatePacket(ushort id)
		{
			/*Rfc 792  Echo or Echo Reply Message
			  0               8              16              24
			 +---------------+---------------+---------------+---------------+
			 |     Type      |     Code      |           Checksum            |
			 +---------------+---------------+---------------+---------------+
			 |           ID Number           |            Sequence Number    |
			 +---------------+---------------+---------------+---------------+
			 |     Data...        
			 +---------------+---------------+---------------+---------------+
			*/

			var packet = new byte[8 + 2];
            packet[0] = (byte)ICMP_Type.Echo; // Type
			packet[1] = 0;  // Code
			packet[2] = 0;  // Checksum
			packet[3] = 0;  // Checksum
			packet[4] = 0;  // ID
			packet[5] = 0;  // ID
			packet[6] = 0;  // Sequence
			packet[7] = 0;  // Sequence

			// Set id
			Array.Copy(BitConverter.GetBytes(id), 0, packet, 4, 2);

			// Fill data 2 byte data
			for(int i=0;i<2;i++){
				packet[i + 8] = (byte)'x'; // Data
			}
			
			// calculate checksum
			int checkSum = 0;
			for(int i= 0;i<packet.Length;i+= 2){ 
				checkSum += Convert.ToInt32(BitConverter.ToUInt16(packet,i));
			}

			// The checksum is the 16-bit ones's complement of the one's
			// complement sum of the ICMP message starting with the ICMP Type.
			checkSum  = (checkSum & 0xffff);
			Array.Copy(BitConverter.GetBytes((ushort)~checkSum),0,packet,2,2);

			return packet;
		}
    }
}
