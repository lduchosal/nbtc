using System.Collections.Generic;
using System.Net;

namespace NodeWalker.Message
{
    public class NewSeedNode
    {
        public string Src { get; set; }
        public IEnumerable<(IPAddress, ushort)> Hosts { get; set; }
    }
}