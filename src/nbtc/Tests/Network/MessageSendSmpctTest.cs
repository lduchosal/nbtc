using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class MessageSendSmpctTest
    {
        [TestMethod]
        public void When_Encode_Decode_Message_Then_Message_Equal()
        {
            var dump = @"
0000    f9 be b4 d9 73 65 6e 64    63 6d 70 63 74 00 00 00    ????sendcmpct...
0000    09 00 00 00 e9 2f 5e f8    00 02 00 00 00 00 00 00    ....?/^?........
0000    00 1f 03 e5 68 31 23 86    1c                         ...?h1#..       
";
            
            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();
            
            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);


var message = reader.ReadMessage();
            var sendcmpct = message.Payload as SendCmpct;

            Assert.AreEqual(Command.SendHeaders, message.Payload.Command);
            Assert.IsNotNull(sendcmpct);
            Assert.AreEqual(0, sendcmpct.Compatible);
            Assert.AreEqual((UInt64)2, sendcmpct.Version);
        }

    }
}