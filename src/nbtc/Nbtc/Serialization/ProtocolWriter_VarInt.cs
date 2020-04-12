using System;
using System.IO;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Payload.Version;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter : BinaryWriter
    {

        public void Write(VarInt version)
        {


            byte len = version.Value <= 0xFC ? (byte)version.Value
                    : version.Value <= 0xFFFF ? (byte)0xFD
                    : version.Value <= 0xFFFFFFFF ? (byte)0xFE
                    : (byte)0xFF
                ;

            Write(len);

            if (version.Value <= 0xFC)
            {
            }
            else if (version.Value >= 0xFD && version.Value <= 0xFFFF)
            {
                Write((UInt16)version.Value);
            }
            else if (version.Value > 0xFFFF && version.Value <= 0xFFFFFFFF)
            {
                Write((UInt32)version.Value);
            }
            else
            {
                Write((UInt64)version.Value);
            }
        }
    }
}