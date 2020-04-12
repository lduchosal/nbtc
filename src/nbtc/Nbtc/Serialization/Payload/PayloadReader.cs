using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Util;

namespace Nbtc.Serialization.Payload
{
    public partial class PayloadReader : BinaryReader
    {
        private readonly ILogger _logger;

        public PayloadReader(ILogger logger, Stream output, bool leaveOpen = false)
            : base(output, EncodingCache.UTF8NoBOM, leaveOpen)
        {
            _logger = logger.For<PayloadReader>();
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

            return NotImplementedCommand(command);

        }
        
        public string ReadNullTerminatedString(int len)
        {
            var bytes = ReadBytes(len);
            var bstring = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                if (bytes[i] == 0x00) break;
                bstring.Add(bytes[i]);
            }
            return Encoding.ASCII.GetString(bstring.ToArray());
        }

        public string ReadVarString()
        {
            var len = ReadByte();
            var content = ReadBytes(len);
            return Encoding.ASCII.GetString(content);
        }
        
        
        /// <summary>
        /// https://en.bitcoin.it/wiki/Protocol_documentation#Transaction_Verification
        /// 
        /// Variable length integer
        /// Integer can be encoded depending on the represented value to save space. 
        /// Variable length integers always precede an array/vector of a type of data 
        /// that may vary in length. Longer numbers are encoded in little endian.
        /// 
        /// +----------------+-----------------+----------------------------------------------+
        /// | Value          | Storage length  |  Format                                      |
        /// +----------------+-----------------+----------------------------------------------+
        /// | < 0xFD         | 1               |  uint8_t                                     |
        /// +----------------+-----------------+----------------------------------------------+
        /// | <= 0xFFFF      | 3               |  0xFD followed by the length as uint16_t     |
        /// +----------------+-----------------+----------------------------------------------+
        /// | <= 0xFFFF FFFF | 5               |  0xFE followed by the length as uint32_t     |
        /// +----------------+-----------------+----------------------------------------------+
        /// | -              | 9               |  0xFF followed by the length as uint64_t     |
        /// +----------------+-----------------+----------------------------------------------+
        /// 
        /// If you're reading the Satoshi client code (BitcoinQT) it refers to this 
        /// encoding as a "CompactSize". Modern BitcoinQT also has the CVarInt class 
        /// which implements an even more compact integer for the purpose of local 
        /// storage (which is incompatible with "CompactSize" described here). 
        /// CVarInt is not a part of the protocol.
        /// </summary>
        /// <returns></returns>
        public VarInt ReadVarInt()
        {
            var varlen = ReadByte();
            UInt64 value =
                varlen == 0xFD ? ReadUInt16()
                : varlen == 0xFE ? ReadUInt32()
                : varlen == 0xFF ? ReadUInt64()
                : varlen;
            
            return new VarInt
            {
                Value = value
            };
        }

    }
}