using System.Linq;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;

namespace Tests.Network
{
    public static class ProtocolReaderExt
    {
        public static Message ReadMessage(this MessageReader reader)
        {
            return reader.ReadMessages().FirstOrDefault();
        }
        
    }
}