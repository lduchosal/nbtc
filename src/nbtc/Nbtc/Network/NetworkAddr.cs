using System;
using System.Net;

namespace Nbtc.Network
{
    public struct NetworkAddr
    {
        public Service Services { get; set; }
        public IPAddress Ip { get; set; }
        public UInt16 Port { get; set; }
    }
}