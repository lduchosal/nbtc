using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class MessageFeeFilterTest
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
0000    f9 be b4 d9 66 65 65 66    69 6c 74 65 72 00 00 00    ????feefilter...
0000    08 00 00 00 e8 0f d1 9f    e8 03 00 00 00 00 00 00    ....?.?.?.......
";
            
            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();
            
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(mem, state);
            var message = reader.ReadMessage();
            var feefilter = message.Payload as FeeFilter;

            Assert.AreEqual(Command.FeeFilter, message.Payload.Command);
            Assert.IsNotNull(feefilter);
            Assert.AreEqual((UInt64)1000, feefilter.FeeRate);
            
        }

    }
}