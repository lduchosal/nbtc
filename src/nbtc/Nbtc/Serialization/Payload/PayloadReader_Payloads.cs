using System.Collections.Generic;
using Nbtc.Network;
using Nbtc.Network.Payload;

namespace Nbtc.Serialization.Payload
{
    public partial class PayloadReader 
    {
        
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
        public SendCmpct ReadSendCmpct()
        {
            var sendcmpct = ReadByte();
            var version = ReadUInt64();
            return new SendCmpct
            {
                Compatible = sendcmpct,
                Version = version
            };
        }
        
        public FeeFilter ReadFeeFilter()
        {
            var feerate = ReadUInt64();
            return new FeeFilter
            {
                FeeRate = feerate,
            };
        }

        public NotImplementedCommand NotImplementedCommand(Command command)
        {
            return new NotImplementedCommand
            {
                Command = command,
            };
        }
        
        

        public GetAddr ReadGetAddr()
        {
            // No data payload
            return new GetAddr();
        }
        public GetHeaders ReadGetHeaders()
        {
            var version = ReadUInt16();
            var varint = ReadVarInt();
            var hashes = new List<Sha256>();
            for (ulong i = 0; i < varint.Value; i++)
            {
                var bytes = ReadBytes(32);
                var sha = new Sha256();
                hashes.Add(sha);
            }
            var stopbytes = ReadBytes(32);
            var stop = new Sha256();

            return new GetHeaders
            {
                version = version,
                Locators = hashes,
                Stop = stop
            };
        }
        public VerAck ReadVerAck()
        {
            // No data payload
            return new VerAck();
        }
        public SendHeaders ReadSendHeaders()
        {
            // No data payload
            return new SendHeaders();
        }
        
    }
}