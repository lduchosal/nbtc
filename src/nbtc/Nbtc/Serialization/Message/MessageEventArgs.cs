using System;

namespace Nbtc.Serialization.Message
{
    public class MessageEventArgs : EventArgs
    {
        public Network.Message Message { get; set; }
        public MessageStatut Result { get; set; }
        public long Length { get; set; }
    }

}