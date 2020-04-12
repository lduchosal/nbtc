using System.Collections.Generic;
using Nbtc.Network;
using Nbtc.Network.Payload;

namespace Nbtc.Serialization.Payload
{
    public partial class PayloadReader
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
        public GetHeaders ReadGetHeaders()
        {
            var version = ReadUInt32();
            var varint = ReadVarInt();
            var hashes = ReadSha256s(varint);
            var stop = ReadSha256();

            return new GetHeaders
            {
                version = version,
                Locators = hashes,
                Stop = stop
            };
        }

        public List<Sha256> ReadSha256s(VarInt varint)
        {
            var hashes = new List<Sha256>();
            for (ulong i = 0; i < varint.Value; i++)
            {
                var sha = ReadSha256();
                hashes.Add(sha);
            }

            return hashes;
        }
        
        public Sha256 ReadSha256()
        {
            var bytes = ReadBytes(32);
            var sha = new Sha256 { Hash = bytes };
            return sha;
        }
    }
}