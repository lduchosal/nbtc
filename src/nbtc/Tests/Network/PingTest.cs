using System.IO;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class PingTest
    {
        
        [TestMethod]
        public void When_Encode_Ping_Then_nothing_To_Encode() {

            var message = new Ping {
                Nonce =  0
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var ping = reader.ReadPing();
            Assert.AreEqual(message.Nonce, ping.Nonce);
        }

        [TestMethod]
        public void When_Decode_Ping_one_Then_nothing_To_Encode() {

            var message = new Ping {
                Nonce =  72340172838076673
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var ping = reader.ReadPing();
            Assert.AreEqual(message.Nonce, ping.Nonce);
        }
    }
}