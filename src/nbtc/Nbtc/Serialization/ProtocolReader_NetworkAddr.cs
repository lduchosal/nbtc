using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class ProtocolReader
    {
        
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