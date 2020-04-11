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
                
                Console.WriteLine($"ReadMessages [StateMachine : {_machine.State}]");
                var result = _machine.Bytes(length);
                Console.WriteLine($"ReadMessages [StateMachine : {_machine.State}]");
                Console.WriteLine($"ReadMessages [result : {result.Statut}]");

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
            Console.WriteLine($"OnUnHandled [unhandled: {unhandled}]");
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

            Console.WriteLine($"OnMessage [magic: {magic}]");
            Console.WriteLine($"OnMessage [command: {command}]");
            Console.WriteLine($"OnMessage [length: {length}]");
            Console.WriteLine($"OnMessage [checksum: {checksum}]");

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
            if (blength != length)
            {
                Console.WriteLine($"OnChecksum [length: {length}]");
                Console.WriteLine($"OnChecksum [blength: {blength}]");
                mea.Result = MessageStatut.Failed;
                return;
            }

            var checksum = mea.Message.Checksum;
            var checksum2 = Checksum(bpayload);
            if (checksum != checksum2)
            {
                Console.WriteLine($"OnChecksum [checksum: {checksum}]");
                Console.WriteLine($"OnChecksum [checksum2: {checksum2}]");

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
            using var reader = new PayloadReader(mem);
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