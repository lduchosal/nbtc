using System;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter 
    {

        public void Write(NetworkAddr addr)
        {
            Write(addr.Services);
            Write(addr.Ip);
            var port = BitConverter.GetBytes(addr.Port);
            Array.Reverse(port);
            Write(port);
        }

        public void Write(Service service)
        {
            Write((ulong) service);
        }
        
        public void Write(IPAddress ip)
        {
            var ipv6 = ip.MapToIPv6();
            Write(ipv6.GetAddressBytes());
        }

    }
}