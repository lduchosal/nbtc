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
        public TimedNetworkAddr ReadTimedNetworkAddr()
        {
            var ts = ReadTimestamp();
            var addr = ReadNetworkAddr();
            
            return new TimedNetworkAddr
            {
                Timestamp = ts,
                NetworkAddr = addr
            };

        }
        public NetworkAddr ReadNetworkAddr()
        {
            var addr = new NetworkAddr
            {
                Services = (Service) ReadUInt64(),
                Ip = ReadIp(),
                Port = ReadPort()
            };

            return addr;
        }

        public ushort ReadPort()
        {
            var bytes = ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public IPAddress ReadIp()
        {
            var bytes = ReadBytes(16);
            //Array.Reverse(bytes);
            var ip = new IPAddress(bytes);
            return ip;
        }

        /// <summary>
        /// The following services are currently assigned:
        /// https://en.bitcoin.it/wiki/Protocol_documentation#version
        /// 
        /// +-------+----------------------+-----------------------------------------------------------------+
        /// | Value | Name                 | Description                                                     |
        /// +-------+----------------------+-----------------------------------------------------------------+
        /// |    1  | NODE_NETWORK         | This node can be asked for full blocks instead of just headers. |
        /// |    2  | NODE_GETUTXO         | See BIP 0064                                                    |
        /// |    4  | NODE_BLOOM           | See BIP 0111                                                    | 
        /// |    8  | NODE_WITNESS         | See BIP 0144                                                    | 
        /// | 1024  | NODE_NETWORK_LIMITED | See BIP 0159                                                    |
        /// +-------+----------------------+-----------------------------------------------------------------+
        /// </summary>
        private Service ReadService()
        {
            return (Service) ReadUInt64();
        }

    }
}