using System.Net;

namespace NodeWalker.Message
{
    internal class ClientMessage
    {
        public string Address { get; set; }
        public uint Port { get; set; }
        public uint Identifier { get; set; }
    }
}