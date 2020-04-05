using System;

namespace Nbtc.Network
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Protocol_documentation#feefilter
    /// 
    /// feefilter
    /// The payload is always 8 bytes long and it encodes 64 bit integer value
    /// (LSB / little endian) of feerate. The value represents a minimal fee and
    /// is expressed in satoshis per 1000 bytes.
    /// 
    /// Upon receipt of a "feefilter" message, the node will be permitted,
    /// but not required, to filter transaction invs for transactions that fall
    /// below the feerate provided in the feefilter message interpreted as
    /// satoshis per kilobyte.
    /// 
    /// The fee filter is additive with a bloom filter for transactions
    /// so if an SPV client were to load a bloom filter and send a feefilter message,
    /// transactions would only be relayed if they passed both filters.
    /// 
    /// Inv's generated from a mempool message are also subject to a fee filter if it exists.
    /// Feature discovery is enabled by checking protocol version >= 70013
    /// 
    /// See BIP 133 for more information.
    /// </summary>
    public class FeeFilter : IPayload
    {
        /// <summary>
        /// The value represents a minimal fee and is expressed in satoshis per 1000 bytes.
        /// </summary>
        public UInt64 FeeRate { get; set; }
        public Command Command { get { return Command.FeeFilter; }}
    }
}