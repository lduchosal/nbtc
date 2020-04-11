using System;
using Nbtc.Network;

namespace Nbtc.Serialization
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; set; }
        public MessageStatut Result { get; set; }
        public long Length { get; set; }
    }

}