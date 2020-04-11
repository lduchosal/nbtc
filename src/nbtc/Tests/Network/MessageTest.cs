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

            var message = new Message {
                Magic = Nbtc.Network.NetworkId.MainNet,
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
            var state = new MessageStateMachine();
            
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new MessageReader(mem2, state);
            var result = reader.ReadMessage();
            var mpayload = message.Payload as Ping;
            var rpayload = result.Payload as Ping;

            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(mpayload.Nonce, rpayload.Nonce);
        }

        [TestMethod]
        public void When_Decode_Message_Then_nothing_To_Encode() {

            var message = new Message {
                Magic = Nbtc.Network.NetworkId.MainNet,
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
            var state = new MessageStateMachine();

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new MessageReader(mem2, state);
            var result = reader.ReadMessage();
            var mpayload = message.Payload as Pong;
            var rpayload = result.Payload as Pong;

            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(mpayload.Nonce, rpayload.Nonce);
        }


        [TestMethod]
        public void When_Encode_Decode_Alert_Suceed() {

            var message = new Message {
                Magic = Nbtc.Network.NetworkId.MainNet,
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

            var state = new MessageStateMachine();
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new MessageReader(mem2, state);
            var result = reader.ReadMessage();

            var mpayload = message.Payload as Alert;
            var rpayload = result.Payload as Alert;
            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(mpayload.Data.Length, rpayload.Data.Length);
        }

        [TestMethod]
        public void When_Encode_Decode_Version_Suceed() {

            var message = new Message {
                Magic = Nbtc.Network.NetworkId.MainNet,
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

            var state = new MessageStateMachine();
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new MessageReader(mem2, state);
            var result = reader.ReadMessage();
            
            var mpayload = message.Payload as Version;
            var rpayload = result.Payload as Version;
            
            Assert.AreEqual(message.Magic, result.Magic);
            Assert.AreEqual(mpayload.Receiver.Ip, rpayload.Receiver.Ip);
            Assert.AreEqual(mpayload.Receiver.Port, rpayload.Receiver.Port);
            Assert.AreEqual(mpayload.Receiver.Services, rpayload.Receiver.Services);
            Assert.AreEqual(mpayload.Sender.Ip, rpayload.Sender.Ip);
            Assert.AreEqual(mpayload.Sender.Port, rpayload.Sender.Port);
            Assert.AreEqual(mpayload.Sender.Services, rpayload.Sender.Services);
            Assert.AreEqual(mpayload.Relay, rpayload.Relay);
            Assert.AreEqual(mpayload.Services, rpayload.Services);
            Assert.AreEqual(mpayload.Timestamp, rpayload.Timestamp);
            Assert.AreEqual(mpayload.Vversion, rpayload.Vversion);
            Assert.AreEqual(mpayload.StartHeight, rpayload.StartHeight);
            Assert.AreEqual(mpayload.UserAgent, rpayload.UserAgent);
        }
    }
}