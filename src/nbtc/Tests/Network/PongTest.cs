using System.IO;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Payload;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class PongTest
    {
        
        [TestMethod]
        public void When_Encode_Pong_Then_nothing_To_Encode() {

            var message = new Pong {
                Nonce =  0
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            var logger = new Logger();
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new PayloadReader(logger, mem2);



            var pong = reader.ReadPong();
            Assert.AreEqual(message.Nonce, pong.Nonce);
            Assert.AreEqual(Command.Pong, pong.Command);
        }

        [TestMethod]
        public void When_Decode_Pong_one_Then_nothing_To_Encode() {

            var message = new Pong {
                Nonce =  72340172838076673
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            var logger = new Logger();
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new PayloadReader(logger, mem2);



var pong = reader.ReadPong();
            Assert.AreEqual(message.Nonce, pong.Nonce);
            Assert.AreEqual(Command.Pong, pong.Command);
            
        }
    }
}