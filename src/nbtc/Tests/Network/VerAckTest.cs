using System.IO;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class VerAckTest
    {
        
        [TestMethod]
        public void When_Encode_VerAck_Then_nothing_To_Encode() {

            var message = new VerAck {
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var verack = reader.ReadVerAck();
            Assert.IsNotNull(verack);
            Assert.AreEqual(Command.VerAck, verack.Command);
        }

        [TestMethod]
        public void When_Decode_VerAck_one_Then_nothing_To_Encode() {

            var message = new VerAck {
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new ProtocolReader(mem2);
            var verack = reader.ReadVerAck();
            Assert.IsNotNull(verack);
            Assert.AreEqual(Command.VerAck, verack.Command);
        }
    }
}