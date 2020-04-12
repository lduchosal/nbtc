using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using System.Net;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class Version3Test
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
00000000   72 11 01 00 01 00 00 00  00 00 00 00 7a 18 81 5e   r...........z..^
00000000   00 00 00 00 01 00 00 00  00 00 00 00 00 00 00 00   ................
00000000   00 00 00 00 00 00 ff ff  ff ff ff ff 00 00 01 00   ......??????....
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ................
00000000   ff ff ff ff ff ff 00 00  62 bb 89 8d 65 5f 61 06   ??????..b?..e_a.
00000000   0c 2f 6e 62 74 63 3d 30  2e 30 2e 31 2f 61 81 08   ./nbtc=0.0.1/a..
00000000   00 00                                              ..              
    ";
            
            var hex = new HexDump();
            var original = hex.Decode(dump);

            var logger = new Logger();
            using var read = new MemoryStream(original.ToArray());
            using var reader = new PayloadReader(logger, read);
var version = reader.ReadVersion();

            Assert.AreEqual(70002, version.Vversion);
            Assert.AreEqual(Service.Network, version.Services);
            Assert.AreEqual((ulong) 1585518714, version.Timestamp);
            Assert.AreEqual(IPAddress.Parse("::ffff:255.255.255.255"), version.Receiver.Ip);
            Assert.AreEqual(0, version.Receiver.Port);
            Assert.AreEqual(Service.Network, version.Receiver.Services);
            Assert.AreEqual(IPAddress.Parse("::ffff:255.255.255.255"), version.Sender.Ip);
            Assert.AreEqual(0, version.Sender.Port);
            Assert.AreEqual(Service.Network, version.Sender.Services);
            Assert.AreEqual((UInt64)459753526739450722, version.Nonce);
            Assert.AreEqual("/nbtc=0.0.1/", version.UserAgent);
            Assert.AreEqual(557409, version.StartHeight);
            Assert.AreEqual(false, version.Relay);
        }
    }
}
