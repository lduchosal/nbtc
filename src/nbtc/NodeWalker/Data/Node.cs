using System;
using System.Net;

namespace NodeWalker.Data
{
    public class Node
    {
        public uint Id { get; set; }
        
        public string Ip { get; set; }
        public uint Port { get; set; }
        
        public string Src { get; set; }
        public uint? SrcId { get; set; }
        public SourceTypeEnum SrcType { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Updated { get; set; }
        public StatusEnum Status { get; set; }
    }
    
    public enum StatusEnum : int
    {
        New = 0,
        Done = 1,
        Deleted = 2,
        Deactivate = 4,
    }
    public enum SourceTypeEnum : int
    {
        DnsSeed = 0,
        Addr = 1,
    }
}