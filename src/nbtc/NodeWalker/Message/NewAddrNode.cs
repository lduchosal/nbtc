using Nbtc.Network.Payload;

namespace NodeWalker.Message
{
    public class NewAddrNode
    {
        public string SrcAdress { get; set; }
        public uint SrcPort { get; set; }
        public uint SrcIdentifier { get; set; }
        
        public Addr Addrs { get; set; }
    }
}