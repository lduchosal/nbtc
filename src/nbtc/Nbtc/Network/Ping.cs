using System;

namespace Nbtc.Network
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Protocol_documentation#ping
    /// 
    /// ping
    /// The ping message is sent primarily to confirm that the TCP/IP connection is still valid. 
    /// An error in transmission is presumed to be a closed connection and the address is 
    /// removed as a current peer.
    /// 
    /// Payload:
    /// ```
    /// +------------+--------------+-----------+--------------+
    /// | Field Size |  Description | Data type | Comments     |
    /// +------------+--------------+-----------+--------------+
    /// | 8          |  nonce       | uint64_t  | random nonce |
    /// +------------+--------------+-----------+--------------+
    /// ```
    /// 
    /// </summary>
    public class Ping : IPayload
    {
        public UInt64 Nonce { get; set; }
        public Command Command { get { return Command.Ping; }}
    }
}