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
    public class MessageVersion1Test
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
00000000   f9 be b4 d9 76 65 72 73  69 6f 6e 00 00 00 00 00   ????version.....
00000000   67 00 00 00 17 64 43 7f                            g....dC.        
00000000   7f 11 01 00 09 04 00 00  00 00 00 00 8c 1f 81 5e   ...............^
00000000   00 00 00 00 09 04 00 00  00 00 00 00 00 00 00 00   ................
00000000   00 00 00 00 00 00 ff ff  b9 e1 e2 ce 20 8d 09 04   ......?????? ...
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ................
00000000   00 00 00 00 00 00 00 00  ba e9 52 6b 1c 7c 77 74   ........??Rk.|wt
00000000   11 2f 53 61 74 6f 73 68  69 3a 30 2e 31 39 2e 39   ./Satoshi:0.19.9
00000000   39 2f 9e 83 09 00 01                               9/.....         
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
            Assert.AreEqual((ulong) 1585520524, version.Timestamp);
            Assert.AreEqual(IPAddress.Parse("::ffff:185.225.226.206"), version.Receiver.Ip);
            Assert.AreEqual(8333, version.Receiver.Port);
            Assert.AreEqual(Service.Network | Service.Witness | Service.NetworkLimited, version.Receiver.Services);
            Assert.AreEqual(IPAddress.Parse("::"), version.Sender.Ip);
            Assert.AreEqual(0, version.Sender.Port);
            Assert.AreEqual(Service.Network | Service.Witness | Service.NetworkLimited, version.Sender.Services);
            Assert.AreEqual((UInt64)8392312892129733050, version.Nonce);
            Assert.AreEqual("/Satoshi:0.19.99/", version.UserAgent);
            Assert.AreEqual(623518, version.StartHeight);
            Assert.AreEqual(true, version.Relay);
        }
    }
}