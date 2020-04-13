using System.Collections.Generic;
using System.Net;

namespace NodeWalker.Message
{
    public class NewNode
    {
        public string Src { get; set; }
        public IEnumerable<(IPAddress, int)> Hosts { get; set; }
    }
}