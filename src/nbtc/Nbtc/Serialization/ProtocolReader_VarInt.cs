using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class PayloadReader 
    {
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