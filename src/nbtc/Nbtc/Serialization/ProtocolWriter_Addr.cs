using System;
using System.IO;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter 
    {

        /// <summary>
        /// https://en.bitcoin.it/wiki/Protocol_documentation#addr
        /// 
        /// # addr
        /// Provide information on known nodes of the network. Non-advertised nodes 
        /// should be forgotten after typically 3 hours
        /// 
        /// ## Payload:
        /// ```
        /// Len  | Description | Data type | Comments
        /// 1+   | count       | var_int   | Number of address entries (max: 1000)
        /// 30x? | addr_list   | u32       | Address of other nodes on the network. 
        ///      |             | + IpAddr  | version &lt; 209 will only read the first one. 
        ///      |             | ]         | The u32 is a timestamp (see note below).
        /// ```
        /// 
        /// Note: Starting version 31402, addresses are prefixed with a timestamp. 
        /// If no timestamp is present, the addresses should not be relayed to 
        /// other peers, unless it is indeed confirmed they are up.
        /// 
        /// ## Hexdump example of addr message:
        /// ```
        /// 0000   F9 BE B4 D9 61 64 64 72  00 00 00 00 00 00 00 00   ....addr........
        /// 0010   1F 00 00 00 ED 52 39 9B  01 E2 15 10 4D 01 00 00   .....R9.....M...
        /// 0020   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 FF   ................
        /// 0030   FF 0A 00 00 01 20 8D                               ..... .
        /// ```
        /// 
        /// ## Message Header:
        /// 
        /// ```
        ///  F9 BE B4 D9                                     - Main network magic bytes
        ///  61 64 64 72  00 00 00 00 00 00 00 00            - "addr"
        ///  1F 00 00 00                                     - payload is 31 bytes long
        ///  ED 52 39 9B                                     - payload checksum (little endian)
        /// ```
        /// 
        /// Payload:
        /// ```
        ///  01                                              - 1 address in this message
        /// ```
        /// 
        /// Address:
        /// ```
        ///  E2 15 10 4D                                     - Mon Dec 20 21:50:10 EST 2010 (only when version is >= 31402)
        ///  01 00 00 00 00 00 00 00                         - 1 (NODE_NETWORK service - see version message)
        ///  00 00 00 00 00 00 00 00 00 00 FF FF 0A 00 00 01 - IPv4: 10.0.0.1, IPv6: ::ffff:10.0.0.1 (IPv4-mapped IPv6 address)
        ///  20 8D                                           - port 8333
        /// ```
        /// 
        /// </summary>
        public void Write(Addr addr)
        {
            Write((byte) addr.Addrs.Count);
            foreach (var tna in addr.Addrs)
            {
                Write(tna);
            }
        }

        public void Write(TimedNetworkAddr tna)
        {
            Write(tna.Timestamp);
            Write(tna.NetworkAddr);
        }
    }
}