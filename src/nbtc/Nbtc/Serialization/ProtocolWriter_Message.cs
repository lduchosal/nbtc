using System;
using System.IO;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

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
        public void Write<T> (Message<T>  message) where T : IPayload
        {
            Write(message.Magic);
            WritePayload(message.Payload);
        }

        public void WritePayload<T>(T payload) where T : IPayload
        {
            Write(payload.Command);
            
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


        public void Write(GetHeaders payload)
        {
            throw new NotImplementedException();
        }

        public void Write(GetAddr payload)
        {
            throw new NotImplementedException();
        }

        public void Write(Magic magic)
        {
            Write((UInt64)magic);
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