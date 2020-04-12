using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class TimedNetworkAddrTest
    {
        [TestMethod]
        public void When_Encode_timednetworkaddr_Then_Encode()
        {

            var hex = new HexDump();

            var tna = new TimedNetworkAddr
            {
                Timestamp = 1234,
                NetworkAddr = new NetworkAddr
                {
                    Ip = IPAddress.IPv6None,
                    Port = 1234,
                    Services = Service.NetworkLimited
                }
            };

            var bytes = @"
00000000   D2 04 00 00 00 04 00 00  00 00 00 00 00 00 00 00   Ò···············
00000010   00 00 00 00 00 00 00 00  00 00 00 00 04 D2         ·············Ò  
";
            var data = hex.Decode(bytes).ToArray();
            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(tna);
                }
                var expected = hex.Encode(data);
                var result = hex.Encode(mem.ToArray());
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Encode_one_addr_Then_Encode()
        {

            var bytes = @"
00000000   15 CD 5B 07 01 00 00 00  00 00 00 00 00 00 00 00   ·Í[·············
00000010   00 00 00 00 00 00 00 00  00 00 00 01 04 D2         ·············Ò  
";
            var hex = new HexDump();
            var data = hex.Decode(bytes).ToArray();

            var tna = new TimedNetworkAddr
            {
                Timestamp = 123456789,
                NetworkAddr = new NetworkAddr
                {
                    Ip = IPAddress.IPv6Loopback,
                    Port = 1234,
                    Services = Service.Network
                }
            };
            
            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(tna);
                }
                var expected = hex.Encode(data);
                var result = hex.Encode(mem.ToArray());
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Decode_addr_Then_nothing_To_Encode()
        {

            var bytes = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00         ·············Ò  
";
            var hex = new HexDump();
            var data = hex.Decode(bytes).ToArray();
            var logger = new Logger();

            using (var mem = new MemoryStream(data))
            using (var reader = new PayloadReader(logger, mem))
            {
                var result = reader.ReadTimedNetworkAddr();
                var expected = new TimedNetworkAddr
                {
                    Timestamp = 0,
                    NetworkAddr = new NetworkAddr
                    {
                        Ip = IPAddress.Parse("::"),
                        Port = 0,
                        Services = (Service) 0
                    }
                };

                Assert.AreEqual(expected.Timestamp, result.Timestamp);
                Assert.AreEqual(expected.NetworkAddr.Ip, result.NetworkAddr.Ip);
                Assert.AreEqual(expected.NetworkAddr.Port, result.NetworkAddr.Port);
                Assert.AreEqual(expected.NetworkAddr.Services, result.NetworkAddr.Services);

            }


        }
    }
}