using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class ProtocolReader : BinaryReader
    {
        public ProtocolReader(Stream output, bool leaveOpen = false)
            : base(output, EncodingCache.UTF8NoBOM, leaveOpen)
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
        
        public byte[] ReadNext()
        {
            var buffer = new byte[1024];
            int read = -1;
            var mem = new MemoryStream();
            var stream = this.BaseStream as BufferedStream;
            
            while(stream != null && stream.CanRead)
            {
                read = Read(buffer, 0, buffer.Length);
                mem.Write(buffer, 0, read);
            } 

            return mem.ToArray();
        }
    }
}