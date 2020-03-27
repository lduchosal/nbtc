using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public sealed class ProtocolReader : BinaryReader
    {
        public ProtocolReader(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
        }

        public ProtocolReader(Stream output)
            : base(output)
        {
        }

        public Version ReadVersion()
        {
            var version = new Version
            {
                Vversion = ReadInt32(),
                Services = (Service) ReadUInt64(),
                Timestamp = ReadUInt64(),
                Receiver = ReadNetworkAddr(),
                Sender = ReadNetworkAddr(),
                Nonce = ReadUInt64(),
                UserAgent = ReadVarString(),
                StartHeight = ReadInt32(),
                Relay = ReadBoolean()
            };

            return version;
        }

        public string ReadVarString()
        {
            var len = ReadByte();
            var content = ReadBytes(len);
            return Encoding.ASCII.GetString(content);
        }

        public string ReadNullTerminatedString()
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = ReadByte()) != 0x00) bytes.Add(b);

            return Encoding.ASCII.GetString(bytes.ToArray());
        }

        public NetworkAddr ReadNetworkAddr()
        {
            var addr = new NetworkAddr
            {
                Services = (Service) ReadUInt64(),
                Ip = ReadIp(),
                Port = ReadPort()
            };

            return addr;
        }

        public ushort ReadPort()
        {
            var bytes = ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public IPAddress ReadIp()
        {
            var bytes = ReadBytes(16);
            var ip = new IPAddress(bytes);
            return ip;
        }

        public Command ReadCommand()
        {
            var scommand = ReadNullTerminatedString();
            var command = (Command) Enum.Parse(typeof(Command), scommand, true);
            return command;
        }
    }
}