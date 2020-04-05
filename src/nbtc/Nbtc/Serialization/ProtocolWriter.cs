using System.IO;
using Nbtc.Network;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter : BinaryWriter
    {
        

        public ProtocolWriter(Stream output, bool leaveOpen = false)
            : base(output, EncodingCache.UTF8NoBOM, leaveOpen)
        {
        }

        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Write(new byte[] {0});
                return;
            }

            base.Write(value);
        }


        public void Write(Alert version)
        {
            var len = new VarInt
            {
                Value = (ulong)version.Data.Length
            };
            Write(len);
            Write(version.Data);
            
        }

        public void Write(Ping message)
        {
            Write(message.Nonce);
        }

        public void Write(Pong message)
        {
            Write(message.Nonce);
        }

        public void Write(VerAck verack)
        {
            // No data payload
        }
    }
}