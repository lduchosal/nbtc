using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using System.Net;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Payload;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class Version2Test
    
    {
        [TestMethod]
        public void When_Encode_Version_Message_Then_Message_Equal()
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

            var version = new Version
            {
                Vversion = 70002,
                Services = Service.Network,
                Timestamp = 1401217254,
                Receiver = new NetworkAddr
                {
                    Services = Service.Network,
                    Ip = IPAddress.Parse("0.0.0.0"),
                    Port = 0
                },
                Sender = new NetworkAddr
                {
                    Services = Service.Network,
                    Ip = IPAddress.Parse("fd87:d87e:eb43:64f2:2cf5:4dca:5941:2db7"),
                    Port = 8333
                },
                Nonce = 0xE83EE8FCCF20D947,
                UserAgent = "/Satoshi:0.9.99/",
                StartHeight = 0x00049F2C,
                Relay = true
            };

            using (var mem = new MemoryStream())
            {
                using (var protocol = new ProtocolWriter(mem))
                {
                    protocol.Write(version);
                }

                var aoriginasl = hex.Encode(original.ToArray());
                var amem = hex.Encode(mem.ToArray());
                Assert.AreEqual(aoriginasl, amem);
            }
        }

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
            var logger = new Logger();

            using (var read = new MemoryStream(original.ToArray()))
            using (var write = new MemoryStream())
            {
                using (var reader = new PayloadReader(logger, read))
                using (var writer = new ProtocolWriter(write))
                {
                    var version = reader.ReadVersion();
                    writer.Write(version);
                }

                var aoriginasl = hex.Encode(original.ToArray());
                var amem = hex.Encode(write.ToArray());
                Assert.AreEqual(aoriginasl, amem);
            }
        }
    }
}