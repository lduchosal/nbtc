using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Nbtc.Network;

namespace Nbtc.Serialization
{
    public  sealed partial class MessageReader
    {

        private readonly MessageStateMachine _machine;

        /// <summary>
        /// https://en.bitcoin.it/wiki/Protocol_documentation
        /// 
        /// Message structure
        /// ```
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// | Field Size | Description | Data type | Comments                                        |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    4       | magic       | uint32_t  | Magic value indicating message origin network,  |
        /// |            |             |           | and used to seek to next message when stream    |
        /// |            |             |           | state is unknown                                |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |   12       | command     | char[12]  | ASCII string identifying the packet content,    |
        /// |            |             |           | NULL padded (non-NULL padding results in packet |
        /// |            |             |           | rejected)                                       |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    4       | length      | uint32_t  | Length of payload in number of bytes            |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    4       | checksum    | uint32_t  | First 4 bytes of sha256(sha256(payload))        |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// |    ?       | payload     | uchar[]   | The actual data                                 |
        /// +------------+-------------+-----------+-------------------------------------------------+
        /// ```
        /// 
        /// </summary>
        /// <returns></returns>
        
        public IEnumerable<Message> ReadMessages()
        {

            while (this.BaseStream.Length > 0)
            {
                long length = this.BaseStream.Length;

                _logger.Debug("ReadMessages {@len}", length);
                var result = _machine.Bytes(length);
                _logger.Debug("ReadMessages {@result}", result);

                if (result.Statut == MessageStatut.Failed)
                {
                    throw new InvalidDataException(result.Error);
                }
                else if (result.Statut == MessageStatut.Succeed)
                {
                    yield return result.Message;
                }
                else if (result.Statut == MessageStatut.Missing)
                {
                    yield break;
                }
            }
        }
        
        private void OnUnHandled(object sender, string unhandled)
        {
            _logger.Fatal("OnUnHandled [unhandled: {0}]", unhandled);
            throw new InvalidProgramException(unhandled);
        }
        
        private void OnMessage(object sender, MessageEventArgs mea)
        {
            if (this.BaseStream.Length < 24)
            {
                mea.Result = MessageStatut.Missing;
                return;
            }

            var magic = ReadNetworkId();
            var command = ReadCommand();
            var length = ReadInt32();
            var checksum = ReadUInt32();

            _logger.Debug("OnMessage : {@Message}", new { Magic = magic, Command = command, Length = length, Checksum = checksum});

            if (command == Command.Unknwon)
            {
                throw new InvalidProgramException();
            }

            mea.Message.Magic = magic;
            mea.Message.Command = command;
            mea.Message.Length = length;
            mea.Message.Checksum = checksum;
            
            mea.Result = MessageStatut.Succeed;
            mea.Length = this.BaseStream.Length;

        }

        private void OnChecksum(object sender, MessageEventArgs mea)
        {
            var length = mea.Message.Length;
            if (this.BaseStream.Length < length)
            {
                mea.Result = MessageStatut.Missing;
                return;
            }
            
            var bpayload = ReadBytes(length);
            var blength = bpayload.Length;
            
            _logger.Debug("OnChecksum : Len {@Len}", new
            {
                Message = length, 
                Payload = blength
            });

            if (blength != length)
            {
                mea.Result = MessageStatut.Failed;
                return;
            }

            var checksum = mea.Message.Checksum;
            var checksum2 = Checksum(bpayload);
            
            
            _logger.Debug("OnChecksum : Checksum {@Checksum}", new
            {
                Message = checksum, 
                Payload = checksum2
            });
            
            if (checksum != checksum2)
            {

                mea.Result = MessageStatut.Failed;
                return;
            }
            
            mea.Message.BPayload = bpayload;
            mea.Result = MessageStatut.Succeed;

        }

        private void OnPayload(object sender, MessageEventArgs mea) 
        {
            var command = mea.Message.Command;
            using var mem = new MemoryStream(mea.Message.BPayload);
            using var reader = new PayloadReader(_logger, mem);
            var payload = reader.ReadPayload(command);

            if (payload is NotImplementedCommand)
            {
                mea.Result = MessageStatut.Failed;
                return;
            }
            
            mea.Message.Payload = payload;
            mea.Result = MessageStatut.Succeed;
        }
        
        private UInt32 Checksum(byte[] bytes)
        {
            var sha = SHA256.Create();
            var doublesha = sha.ComputeHash(sha.ComputeHash(bytes));
            return BitConverter.ToUInt32(doublesha, 0);
        }


        public Network.NetworkId ReadNetworkId()
        {
            return (Network.NetworkId)ReadUInt32();
        }
    }
}