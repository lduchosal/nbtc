using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Version = Nbtc.Network.Payload.Version;
    
namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter 
    {
        /// <summary>
        /// https://en.bitcoin.it/wiki/Protocol_documentation
        /// 
        /// Message structure
        /// ```
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// | Field Size | Description | Data type | Comments                                        |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    4       | magic       | uint32_t  | Magic value indicating message origin network,  |
        /// |            |             |           | and used to seek to next message when stream    |
        /// |            |             |           | state is unknown                                |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |   12       | command     | char[12]  | ASCII string identifying the packet content,    |
        /// |            |             |           | NULL padded (non-NULL padding results in packet |
        /// |            |             |           | rejected)                                       |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    4       | length      | uint32_t  | Length of payload in number of bytes            |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    4       | checksum    | uint32_t  | First 4 bytes of sha256(sha256(payload))        |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    ?       | payload     | uchar[]   | The actual data                                 |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// ```
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Write(Network.Message message)
        {
            Write(message.Magic);
            var payload = message.Payload;
            
            using var mem = new MemoryStream();
            using var writer = new ProtocolWriter(mem);
            writer.WritePayload(payload);
            var bpayload = mem.ToArray();
            var checksum = Checksum(bpayload);
            
            Write(payload.Command);
            Write((UInt32)bpayload.Length); // len
            Write(checksum); // checksum
            Write(bpayload);
        }
        
        private UInt32 Checksum(byte[] bytes)
        {
            var sha = SHA256.Create();
            var doublesha = sha.ComputeHash(sha.ComputeHash(bytes));
            return BitConverter.ToUInt32(doublesha, 0);
        }

        public void WritePayload(IPayload payload) 
        {
            
            if (payload.Command == Command.Version)
            {
                Write(payload as Version);
            }
            else if (payload.Command == Command.Ping)
            {
                Write(payload as Ping);
            }
            else if (payload.Command == Command.Pong)
            {
                Write(payload as Pong);
            }
            else if (payload.Command == Command.Alert)
            {
                Write(payload as Alert);
            }
            else if (payload.Command == Command.Addr)
            {
                Write(payload as Addr);
            }
            else if (payload.Command == Command.Inv)
            {
                Write(payload as Inv);
            }
            else if (payload.Command == Command.GetAddr)
            {
                Write(payload as GetAddr);
            }
            else if (payload.Command == Command.GetHeaders)
            {
                Write(payload as GetHeaders);
            }
            else if (payload.Command == Command.VerAck)
            {
                Write(payload as VerAck);
            }
        }


        /// <summary>
        /// https://en.bitcoin.it/wiki/Protocol_documentation#getaddr
        /// 
        /// getaddr
        /// 
        /// The getaddr message sends a request to a node asking for information 
        /// about known active peers to help with finding potential nodes in the network. 
        /// The response to receiving this message is to transmit one or more addr messages 
        /// with one or more peers from a database of known active peers. 
        /// 
        /// The typical presumption is that a node is likely to be active if it has been sending 
        /// a message within the last three hours.
        /// 
        /// No additional data is transmitted with this message.
        /// 
        ///
        /// </summary>
        /// <param name="_getaddr"></param>
        public void Write(GetAddr getaddr)
        {
            // No payload data
        }

        public void Write(Network.NetworkId networkId)
        {
            Write((UInt32)networkId);
        }
        
        public void Write(Command command)
        {
            var lcommand = command.ToString().ToLower();
            var scommand = $"{lcommand}\0\0\0\0\0\0\0\0\0\0\0\0";
            var bytes = Encoding.ASCII.GetBytes(scommand);
            Write(bytes, 0, 12);
        }

    }
}