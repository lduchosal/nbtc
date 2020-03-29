using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class ProtocolReader : BinaryReader
    {
        public ProtocolReader(Stream output)
            : base(output)
        {
        }

        public Command ReadCommand()
        {
            var scommand = ReadNullTerminatedString(12);
            var command = (Command) Enum.Parse(typeof(Command), scommand, true);
            return command;
        }
        
        public Alert ReadAlert()
        {
            var varlen = ReadVarInt();
            var bytes = ReadBytes((int)varlen.Value);

            return new Alert
            {
                Data = bytes
            };
        }

        public Ping ReadPing()
        {
            var nonce = ReadUInt64();
            return new Ping
            {
                Nonce = nonce
            };
        }

        public Pong ReadPong()
        {
            var nonce = ReadUInt64();
            return new Pong
            {
                Nonce = nonce
            };
        }
        

        public GetAddr ReadGetAddr()
        {
            throw new NotImplementedException();
        }
        public GetHeaders ReadGetHeaders()
        {
            throw new NotImplementedException();
        }
        public VerAck ReadVerAck()
        {
            // No data payload
            return new VerAck();
        }
    }
}