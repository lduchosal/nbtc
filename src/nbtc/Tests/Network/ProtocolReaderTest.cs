using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Nbtc.Serialization;
using Serilog;

namespace Tests.Network
{
    [TestClass]
    public class ProtocolReaderTest
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
00000000   72 11 01 00 01 00 00 00  00 00 00 00 e6 e0 84 53   ver.service.time
00000000   00 00 00 00 01 00 00 00  00 00 00 00 00 00 00 00   ................
00000000   00 00 00 00 00 00 ff ff  00 00 00 00 00 00 01 00   ................
00000000   00 00 00 00 00 00 fd 87  d8 7e eb 43 64 f2 2c f5   ................
00000000   4d ca 59 41 2d b7 20 8d  47 d9 20 cf fc e8 3e e8   .........nonce..
00000000   10 2f 53 61 74 6f 73 68  69 3a 30 2e 39 2e 39 39   .useragent......
00000000   2f 2c 9f 04 00 01                                  height.relay.   
    ";
            var hex = new HexDump();
            var original = hex.Decode(dump);
            
            using (var read = new MemoryStream(original.ToArray()))
            using (var write = new MemoryStream())
            {
                using (var reader = new ProtocolReader(read))
                {
                    var version = reader.ReadVersion();
                    Assert.AreEqual(70002, version.Vversion);
                    Assert.AreEqual(Service.Network, version.Services);
                    Assert.AreEqual((UInt64)1401217254, version.Timestamp);
                    Assert.AreEqual(IPAddress.Parse("::ffff:0:0"), version.Receiver.Ip);
                    Assert.AreEqual(0, version.Receiver.Port);
                    Assert.AreEqual(Service.Network, version.Receiver.Services);
                    Assert.AreEqual(IPAddress.Parse("fd87:d87e:eb43:64f2:2cf5:4dca:5941:2db7"), version.Sender.Ip);
                    Assert.AreEqual(8333, version.Sender.Port);
                    Assert.AreEqual(Service.Network, version.Sender.Services);
                    Assert.AreEqual((UInt64)16735069437859780935, version.Nonce);
                    Assert.AreEqual("/Satoshi:0.9.99/", version.UserAgent);
                    Assert.AreEqual(302892, version.StartHeight);
                    Assert.AreEqual(true, version.Relay);
                }

            }

        }
    }
}
