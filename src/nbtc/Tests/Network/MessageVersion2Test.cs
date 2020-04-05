using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using System.Net;
using Nbtc.Serialization;
using Nbtc.Util;
using Version = Nbtc.Network.Version;

namespace Tests.Network
{
    [TestClass]
    public class MessageVersion2Test
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
0000    f9 be b4 d9 76 65 72 73    69 6f 6e 00 00 00 00 00    ????version.....
0000    62 00 00 00 c0 d6 01 43    7f 11 01 00 09 04 00 00    b...??.C........
0000    00 00 00 00 e9 23 81 5e    00 00 00 00 09 04 00 00    ....?#.^........
0000    00 00 00 00 00 00 00 00    00 00 00 00 00 00 00 00    ................
0000    00 00 00 00 20 8d 09 04    00 00 00 00 00 00 00 00    .... ...........
0000    00 00 00 00 00 00 00 00    00 00 00 00 00 00 00 00    ................
0000    e6 c9 81 f1 57 68 bb 34    0c 2f 6e 62 74 63 3d 30    ??.?Wh?4./nbtc=0
0000    2e 30 2e 31 2f 9e 83 09    00 01                      .0.1/.....      
";
            
            var hex = new HexDump();
            var original = hex.Decode(dump);

            using var read = new MemoryStream(original.ToArray());
            using var reader = new ProtocolReader(read);
            var message = reader.ReadMessage();
            var version = message.Payload as Version;

            Assert.AreEqual(Command.Version, message.Payload.Command);
            
            Assert.AreEqual(70015, version.Vversion);
            Assert.AreEqual(Service.Network | Service.Witness | Service.NetworkLimited, version.Services);
            Assert.AreEqual((ulong) 1585521641, version.Timestamp);
            Assert.AreEqual(IPAddress.Parse("::"), version.Receiver.Ip);
            Assert.AreEqual(8333, version.Receiver.Port);
            Assert.AreEqual(Service.Network | Service.Witness | Service.NetworkLimited, version.Receiver.Services);
            Assert.AreEqual(IPAddress.Parse("::"), version.Sender.Ip);
            Assert.AreEqual(0, version.Sender.Port);
            Assert.AreEqual(Service.Network | Service.Witness | Service.NetworkLimited, version.Sender.Services);
            Assert.AreEqual((UInt64)3799745437540403686, version.Nonce);
            Assert.AreEqual("/nbtc=0.0.1/", version.UserAgent);
            Assert.AreEqual(623518, version.StartHeight);
            Assert.AreEqual(true, version.Relay);
        }
    }
}