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