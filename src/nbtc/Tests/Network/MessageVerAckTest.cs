using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using System.Net;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;
using Nbtc.Util;
using Version = Nbtc.Network.Payload.Version;

namespace Tests.Network
{
    [TestClass]
    public class MessageVerAckTest
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
00000000   f9 be b4 d9 76 65 72 61  63 6b 00 00 00 00 00 00   ????verack......
00000000   00 00 00 00 5d f6 e0 e2                            ....]???        
";
            
            var hex = new HexDump();
            var original = hex.Decode(dump);

            var state = new MessageStateMachine();
            var logger = new Logger();
            using var read = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, read, state);
            var message = reader.ReadMessage();
            var verAck = message.Payload as VerAck;

            Assert.IsNotNull(verAck);
            Assert.AreEqual(Command.VerAck, message.Payload.Command);
            
        }
    }
}