using System;
using System.Net;

namespace Nbtc.Network
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Protocol_documentation#Network_address
    /// 
    /// Network address
    /// When a network address is needed somewhere, this structure is used. 
    /// Network addresses are not prefixed with a timestamp in the version message.
    ///
    /// ```
    /// +------------+-------------+-----------+----------------------------------------------------------------+
    /// | Field Size | Description | Data type | Comments                                                       |
    /// +------------+-------------+-----------+----------------------------------------------------------------+
    /// |     4      | time        | uint32    | the Time (version >= 31402). Not present in version message.   |
    /// +------------+-------------+-----------+----------------------------------------------------------------+
    /// |     8      | services    | uint64_t  | same service(s) listed in version                              |
    /// +------------+-------------+-----------+----------------------------------------------------------------+
    /// |    16      | IPv6/4      | char[16]  | IPv6 address. Network byte order. The original client only     |
    /// |            |             |           | supported IPv4 and only read the last 4 bytes to get the IPv4  |
    /// |            |             |           | address. However, the IPv4 address is written into the message |
    /// |            |             |           | as a 16 byte IPv4-mapped IPv6 address                          |
    /// |            |             |           |                                                                |
    /// |            |             |           | (12 bytes 00 00 00 00 00 00 00 00 00 00 FF FF, followed by the |
    /// |            |             |           | 4 bytes of the IPv4 address).                                  |
    /// +------------+-------------+-----------+----------------------------------------------------------------+
    /// |      2     | port        | uint16_t  | port number, network byte order                                |
    /// +------------+-------------+-----------+----------------------------------------------------------------+
    /// ```
    /// 
    /// Hexdump example of Network address structure
    ///
    /// ```
    /// 0000   01 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  ................
    /// 0010   00 00 FF FF 0A 00 00 01  20 8D                    ........ .
    /// ```
    ///
    /// Network address:
    /// ```
    ///  01 00 00 00 00 00 00 00                         - 1 (NODE_NETWORK: see services listed under version command)
    ///  00 00 00 00 00 00 00 00 00 00 FF FF 0A 00 00 01 - IPv6: ::ffff:a00:1 or IPv4: 10.0.0.1
    ///  20 8D                                           - Port 8333
    /// ```
    /// 
    /// </summary>
    public struct NetworkAddr
    {
        public Service Services { get; set; }
        public IPAddress Ip { get; set; }
        public ushort Port { get; set; }
    }
}