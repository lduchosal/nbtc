using System;

namespace Nbtc.Network
{
    /// <summary>
    /// The `version` message
    /// https://en.bitcoin.it/wiki/Protocol_documentation#version
    /// 
    /// When a node creates an outgoing connection, it will immediately advertise its version. 
    /// The remote node will respond with its version. No further communication is possible until 
    /// both peers have exchanged their version.
    ///  
    /// Payload:
    /// 
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// | Field Size | Description  | Data type | Comments                                                     |
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// |      4     | version      | int32_t   | Identifies protocol version being used by the node           |
    /// |      8     | services     | uint64_t  | bitfield of features to be enabled for this connection       |
    /// |      8     | timestamp    | int64_t   | standard UNIX timestamp in seconds                           |
    /// |     26     | addr_recv    | net_addr  | The network address of the node receiving this message       |
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// | Fields below require version ≥ 106                                                                   |
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// |     26     | addr_from    | net_addr  | The network address of the node emitting this message        |
    /// |      8     | nonce        | uint64_t  | Node random nonce, randomly generated every time a version   |
    /// |            |              |           | packet is sent. This nonce is used to detect connections to  |
    /// |            |              |           | self.                                                        |
    /// |      ?     | user_agent   | var_str   | User Agent (0x00 if string is 0 bytes long)                  |
    /// |      4     | start_height | int32_t   | The last block received by the emitting node                 |
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// | Fields below require version ≥ 70001                                                                 |
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// |      1     | relay        | bool      | Whether the remote peer should announce relayed transactions |
    /// |            |              |           | or not, see BIP 0037                                         |
    /// +------------+--------------+-----------+--------------------------------------------------------------+
    /// 
    /// A "verack" packet shall be sent if the version packet was accepted.
    /// </summary>
    public class Version : IPayload
    {
        public int Vversion { get; set; }
        public Service Services { get; set; }
        public ulong Timestamp { get; set; }
        public NetworkAddr Receiver { get; set; }
        public NetworkAddr Sender { get; set; }
        public ulong Nonce { get; set; }
        public string UserAgent { get; set; }
        public int StartHeight { get; set; }
        public bool Relay { get; set; }
        public Command Command { get { return Command.Version; }}
        

    }
}