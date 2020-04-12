using System;
using System.Collections.Generic;

namespace Nbtc.Network.Payload
{
    /// <summary>
    /// https://github.com/rust-bitcoin/rust-bitcoin/blob/45140a3251d9eca8d17baf7a4e900a4ac5baae3b/src/network/message_blockdata.rs
    /// The `getheaders` message
    /// 
    /// https://en.bitcoin.it/wiki/Protocol_documentation
    /// getheaders 
    /// Return a headers packet containing the headers of blocks starting right after the last known hash 
    /// in the block locator object, up to hash_stop or 2000 blocks, whichever comes first. To receive the 
    /// next block headers, one needs to issue getheaders again with a new block locator object. 
    /// Keep in mind that some clients may provide headers of blocks which are invalid if the block 
    /// locator object contains a hash on the invalid branch.
    /// 
    /// Payload:
    /// 
    /// +---------------+--------------------------+-------------+-------------------------------------------------------+
    /// | Field Size    | Description              | Data type   | Comments                                              | 
    /// +---------------+--------------------------+-------------+-------------------------------------------------------+ 
    /// |     4         | version                  | uint32_t    | the protocol version                                  |
    /// +---------------+--------------------------+-------------+-------------------------------------------------------+ 
    /// |     1+        | hash count               | var_int     | number of block locator hash entries                  |
    /// +---------------+--------------------------+-------------+-------------------------------------------------------+ 
    /// |     32+       | block locator hashes     | char[32]    | block locator object; newest back to genesis block    |
    /// |               |                          |             | (dense to start, but then sparse)                     |
    /// +---------------+--------------------------+-------------+-------------------------------------------------------+ 
    /// |     32        | hash_stop                | char[32]    | hash of the last desired block header;                |
    /// |               |                          |             | set to zero to get as many blocks as possible (2000)  |
    /// +---------------+--------------------------+-------------+-------------------------------------------------------+ 
    /// 
    /// For the block locator object in this packet, the same rules apply as for the getblocks packet.
    /// 
    ///
    /// </summary>
    public class GetHeaders : IPayload
    {
        /// The protocol version
        public UInt32 version { get; set; }
        /// Locator hashes --- ordered newest to oldest. The remote peer will
        /// reply with its longest known chain, starting from a locator hash
        /// if possible and block 1 otherwise.
        public List<Sha256> Locators { get; set; }
        /// References the header to stop at, or zero to just fetch the maximum 2000 headers
        public Sha256 Stop{ get; set; }
        public Command Command => Command.GetHeaders;
    }
}