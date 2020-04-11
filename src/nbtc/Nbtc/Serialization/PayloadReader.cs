using System;
using System.Collections.Generic;
using System.IO;
using Nbtc.Network;

namespace Nbtc.Serialization
{
    public partial class PayloadReader : BinaryReader
    {
        public PayloadReader(Stream output, bool leaveOpen = false)
            : base(output, EncodingCache.UTF8NoBOM, leaveOpen)
        {
        }

        public IPayload ReadPayload(Command command)
        {
            if (command == Command.Version)
            {
                return ReadVersion();
            }
            else if (command == Command.Ping)
            {
                return ReadPing();
            }
            else if (command == Command.Pong)
            {
                return ReadPong();
            }
            else if (command == Command.Alert)
            {
                return ReadAlert();
            }
            else if (command == Command.Addr)
            {
                return ReadAddr();
            }
            else if (command == Command.Inv)
            {
                return ReadInv();
            }
            else if (command == Command.GetAddr)
            {
                return ReadGetAddr();
            }
            else if (command == Command.GetHeaders)
            {
                return ReadGetHeaders();
            }
            else if (command == Command.VerAck)
            {
                return ReadVerAck();
            }
            else if (command == Command.SendHeaders)
            {
                return ReadSendHeaders();
            }
            else if (command == Command.SendCmpct)
            {
                return ReadSendCmpct();
            }
            else if (command == Command.FeeFilter)
            {
                return ReadFeeFilter();
            }

            return NotImplementedCommand(command);

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
        //
        // public byte[] ReadNext()
        // {
        //     var buffer = new byte[1024];
        //     int read = -1;
        //     var mem = new MemoryStream();
        //     var stream = this.BaseStream as BufferedStream;
        //     
        //     while(stream != null && stream.CanRead)
        //     {
        //         read = Read(buffer, 0, buffer.Length);
        //         mem.Write(buffer, 0, read);
        //     } 
        //
        //     return mem.ToArray();
        // }
    }
}