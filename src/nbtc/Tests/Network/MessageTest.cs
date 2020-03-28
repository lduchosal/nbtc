using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class MessageTest
    {
        
        [TestMethod]
        public void When_Encode_Message_Then_nothing_To_Encode() {

            var message = new Message<Ping> {
                Magic = Magic.MainNet,
                Payload = new Ping
                {
                    Nonce = 0
                }
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var result = reader.ReadMessage<Ping>();
            
            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(message.Payload.Nonce, result.Payload.Nonce);
        }

        [TestMethod]
        public void When_Decode_Message_Then_nothing_To_Encode() {

            var message = new Message<Pong> {
                Magic = Magic.MainNet,
                Payload = new Pong
                {
                    Nonce = 0
                }
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var result = reader.ReadMessage<Pong>();
            
            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(message.Payload.Nonce, result.Payload.Nonce);
        }


        [TestMethod]
        public void When_Encode_Decode_Alert_Suceed() {

            var message = new Message<Alert> {
                Magic = Magic.MainNet,
                Payload = new Alert
                {
                    Data = new byte [0]
                }
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var result = reader.ReadMessage<Alert>();
            
            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(message.Payload.Data.Length, result.Payload.Data.Length);
        }

        [TestMethod]
        public void When_Encode_Decode_Version_Suceed() {

            var message = new Message<Version> {
                Magic = Magic.MainNet,
                Payload = new Version
                {
                    Nonce = 1,
                    Receiver = new NetworkAddr
                    {
                        Ip = IPAddress.IPv6None,
                        Port = 2,
                        Services = Service.Bloom
                    },
                    Sender = new NetworkAddr
                    {
                        Ip = IPAddress.IPv6Any,
                        Port = 3,
                        Services = Service.Network
                    },
                    Relay = true,
                    Services = Service.Witness,
                    Timestamp = 4,
                    Vversion = 5,
                    StartHeight = 6,
                    UserAgent = "UserAgent"
                }
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var result = reader.ReadMessage<Version>();
            
            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(message.Payload.Receiver.Ip, result.Payload.Receiver.Ip);
            Assert.AreEqual(message.Payload.Receiver.Port, result.Payload.Receiver.Port);
            Assert.AreEqual(message.Payload.Receiver.Services, result.Payload.Receiver.Services);
            Assert.AreEqual(message.Payload.Sender.Ip, result.Payload.Sender.Ip);
            Assert.AreEqual(message.Payload.Sender.Port, result.Payload.Sender.Port);
            Assert.AreEqual(message.Payload.Sender.Services, result.Payload.Sender.Services);
            Assert.AreEqual(message.Payload.Relay, result.Payload.Relay);
            Assert.AreEqual(message.Payload.Services, result.Payload.Services);
            Assert.AreEqual(message.Payload.Timestamp, result.Payload.Timestamp);
            Assert.AreEqual(message.Payload.Vversion, result.Payload.Vversion);
            Assert.AreEqual(message.Payload.StartHeight, result.Payload.StartHeight);
            Assert.AreEqual(message.Payload.UserAgent, result.Payload.UserAgent);
        }
    }
}