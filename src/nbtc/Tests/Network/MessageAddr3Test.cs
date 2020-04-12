using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class MessageAddr3Test
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            
            var dump = @"
0000    F9 BE B4 D9 61 64 64 72    00 00 00 00 00 00 00 00    ....addr........
0010    1F 00 00 00 ED 52 39 9B    01 E2 15 10 4D 01 00 00    .....R9.....M...
0020    00 00 00 00 00 00 00 00    00 00 00 00 00 00 00 FF    ................
0030    FF 0A 00 00 01 20 8D                                  ..... .
";
           
           var hex = new HexDump();
           var original = hex.Decode(dump);
           var state = new MessageStateMachine();
            
            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);

           var message = reader.ReadMessage();
           var addrs = message.Payload as Addr;

           Assert.AreEqual(Command.Addr, message.Payload.Command);
           Assert.IsNotNull(addrs);
           Assert.AreEqual((Int32)1, addrs.Addrs.Count);

           var addr = addrs.Addrs[0];
           Assert.AreEqual((UInt32)1292899810, addr.Timestamp);
           Assert.AreEqual("::ffff:10.0.0.1", addr.NetworkAddr.Ip.ToString());
           Assert.AreEqual((UInt16)8333, addr.NetworkAddr.Port);
           Assert.AreEqual(Service.Network, addr.NetworkAddr.Services);
           
       }
   }
}