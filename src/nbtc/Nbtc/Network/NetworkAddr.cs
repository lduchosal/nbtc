using System.Net;

namespace Nbtc.Network
{
    public struct NetworkAddr
    {
        public Service Services { get; set; }
        public IPAddress Ip { get; set; }
        public int Port { get; set; }
    }
}