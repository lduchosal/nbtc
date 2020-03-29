using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class ProtocolReader 
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
        /// <returns></returns>
        public Message<T> ReadMessage<T>() where T : IPayload
        {
            var magic = ReadMagic();
            var command = ReadCommand();
            var payload = ReadPayload<T>(command);
            
            return new Message<T>
            {
                Magic = magic,
                Payload = (T)payload
            };
        }

        public IPayload ReadPayload<T>(Command command) where T : IPayload
        {
            if (command == Command.Version)
            {
                return ReadVersion();
            }
            else if (command == Command.Ping)
            {
                return ReadPing();
            }
            else if (command == Command.Pong)
            {
                return ReadPong();
            }
            else if (command == Command.Alert)
            {
                return ReadAlert();
            }
            else if (command == Command.Addr)
            {
                return ReadAddr();
            }
            else if (command == Command.Inv)
            {
                return ReadInv();
            }
            else if (command == Command.GetAddr)
            {
                return ReadGetAddr();
            }
            else if (command == Command.GetHeaders)
            {
                return ReadGetHeaders();
            }
            else if (command == Command.VerAck)
            {
                return ReadVerAck();
            }

            return null;
        }

        public Magic ReadMagic()
        {
            return (Magic)ReadUInt64();
        }
    }
}