using System;

namespace Nbtc.Network
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
    /// </summary>
    public class Message<T> where T : IPayload
    {
        public Magic Magic { get; set; }
        public T Payload { get; set; }
    }
    
    /// <summary>
    /// https://en.bitcoin.it/wiki/Protocol_documentation
    /// 
    /// Known magic values:
    /// ```
    /// +-----------+-------------+-------------------+
    /// | Network   | Magic value | Sent over wire as |
    /// +-----------+-------------+-------------------+
    /// | main      | 0xD9B4BEF9  | F9 BE B4 D9       |
    /// +-----------+-------------+-------------------+
    /// | testnet   | 0xDAB5BFFA  | FA BF B5 DA       |
    /// +-----------+-------------+-------------------+
    /// | testnet3  | 0x0709110B  | 0B 11 09 07       |
    /// +-----------+-------------+-------------------+
    /// | namecoin  | 0xFEB4BEF9  | F9 BE B4 FE       |
    /// +-----------+-------------+-------------------+
    /// ```
    ///
    /// </summary>
    public enum Magic : UInt32 {
        MainNet = 0xD9B4BEF9,
        TestNet = 0x0709110B,
        RegTest = 0xDAB5BFFA,
    }
    
    public interface IPayload
    {
        Command Command { get; }
    }


}