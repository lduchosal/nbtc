using System;
using System.IO;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter : BinaryWriter
    {
        public ProtocolWriter(Stream output)
            : base(output)
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

    }
}