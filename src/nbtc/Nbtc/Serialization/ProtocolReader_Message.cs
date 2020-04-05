using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using BeetleX.Buffers;
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
        public Message ReadMessage()
        {
            var magic = ReadNetworkId();
            var command = ReadCommand();
            var length = ReadInt32();
            var checksum = ReadUInt32();
            var bpayload = ReadBytes(length);

            var checksum2 = Checksum(bpayload);

            if (checksum != checksum2)
            {
                throw new InvalidDataException("checksum");
            }
            
            using var mem = new MemoryStream(bpayload);
            using var reader = new ProtocolReader(mem);
            var payload = reader.ReadPayload(command);
            
            return new Message
            {
                NetworkId = magic,
                Payload = payload
            };
        }

        public IEnumerable<Message> ReadMessages()
        {
            while (true)
            {
                var stream = this.BaseStream as PipeStream;
                if (stream.Length == 0)
                {
                    break;
                }
                var message = ReadMessage();
                yield return message;
            }
        }


        private UInt32 Checksum(byte[] bytes)
        {
            var sha = SHA256.Create();
            var doublesha = sha.ComputeHash(sha.ComputeHash(bytes));
            return BitConverter.ToUInt32(doublesha, 0);
        }

        public IPayload ReadPayload(Command command)
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
            else if (command == Command.SendHeaders)
            {
                return ReadSendHeaders();
            }
            else if (command == Command.SendCmpct)
            {
                return ReadSendCmpct();
            }
            else if (command == Command.FeeFilter)
            {
                return ReadFeeFilter();
            }

            throw new NotImplementedException(command.ToString());  
        }

        public Network.NetworkId ReadNetworkId()
        {
            return (Network.NetworkId)ReadUInt32();
        }
    }
}