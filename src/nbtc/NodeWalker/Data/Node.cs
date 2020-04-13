using System;

namespace NodeWalker.Data
{
    public class Node
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        
        public string Src { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Updated { get; set; }
        public NodeProvider.StatusEnum Status { get; set; }
    }
}