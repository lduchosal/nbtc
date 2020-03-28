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

    }
}