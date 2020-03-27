using System;
using System.IO;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public sealed class ProtocolWriter : BinaryWriter
    {
        public ProtocolWriter(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
        }

        public ProtocolWriter(Stream output)
            : base(output)
        {
        }

        public void Write(Version version)
        {
            Write(version.Vversion);
            Write(version.Services);
            Write(version.Timestamp);
            Write(version.Receiver);
            Write(version.Sender);
            Write(version.Nonce);
            Write(version.UserAgent);
            Write(version.StartHeight);
            Write(version.Relay);
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

        public void Write(NetworkAddr addr)
        {
            Write(addr.Services);
            var ip = addr.Ip.MapToIPv6();
            Write(ip.GetAddressBytes());
            var port = BitConverter.GetBytes(addr.Port);
            Array.Reverse(port);
            Write(port);
        }

        public void Write(Service service)
        {
            Write((ulong) service);
        }
    }
}