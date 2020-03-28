using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class AddrTest
    {


        [TestMethod]
        public void When_Encode_addr_Then_Nothing_To_Encode()
        {

            var hex = new HexDump();

            var addrs = new List<TimedNetworkAddr>();
            var addr = new Addr
            {
                Addrs = addrs
            };

            var data = new byte[] {0};
            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(addr);
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
00000000   01 15 CD 5B 07 01 00 00  00 00 00 00 00 00 00 00   ··Í[············
00000010   00 00 00 00 00 00 00 00  00 00 00 00 01 04 D2      ··············Ò 
";
            var hex = new HexDump();
            var data = hex.Decode(bytes).ToArray();

            var addrs = new List<TimedNetworkAddr>
            {
                new TimedNetworkAddr
                {
                    Timestamp = 123456789,
                    NetworkAddr = new NetworkAddr
                    {
                        Ip = IPAddress.IPv6Loopback,
                        Port = 1234,
                        Services = Service.Network
                    }
                }
            };
            
            var addr = new Addr
            {
                Addrs = addrs
            };

            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(addr);
                }
                var expected = hex.Encode(data);
                var result = hex.Encode(mem.ToArray());
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Decode_Addr_Then_Nothing_To_Encode()
        {

            var data = new byte[] {0};
            using (var mem = new MemoryStream(data))
            using (var reader = new ProtocolReader(mem))
            {
                var result = reader.ReadAddr();
                var addrs = new List<TimedNetworkAddr>();
                var expected = new Addr {
                        Addrs = addrs
                    }
                    ;

                Assert.AreEqual(expected.Addrs.Count, result.Addrs.Count);

            }


        }
        
        [TestMethod]
        public void When_Decode_multiple_addr_Then_Encode()
        {

            var hex = new HexDump();

            // Expected
            var addrs = new List<TimedNetworkAddr>
            {
                new TimedNetworkAddr
                {
                    Timestamp = 1,
                    NetworkAddr = new NetworkAddr
                    {
                        Ip = IPAddress.IPv6None,
                        Port = 1,
                        Services = Service.Bloom
                    }
                },
                new TimedNetworkAddr
                {
                    Timestamp = 2,
                    NetworkAddr = new NetworkAddr
                    {
                        Ip = IPAddress.IPv6Any,
                        Port = 2,
                        Services = Service.Network
                    }
                }
            };
                
            var expected = new Addr {
                    Addrs = addrs
                }
            ;
            
            var bytes = @"
00000000   02 01 00 00 00 04 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 01 02   ················
00000020   00 00 00 01 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000030   00 00 00 00 00 00 00 00  00 00 00 00 02            ·············
";

            var data = hex.Decode(bytes).ToArray();
            using (var mem = new MemoryStream(data))
            using (var reader = new ProtocolReader(mem))
            {
                var result = reader.ReadAddr();
                
                Assert.AreEqual(expected.Addrs.Count, result.Addrs.Count);
                
                Assert.AreEqual(expected.Addrs[0].Timestamp, result.Addrs[0].Timestamp);
                Assert.AreEqual(expected.Addrs[0].NetworkAddr.Ip, result.Addrs[0].NetworkAddr.Ip);
                Assert.AreEqual(expected.Addrs[0].NetworkAddr.Port, result.Addrs[0].NetworkAddr.Port);
                Assert.AreEqual(expected.Addrs[0].NetworkAddr.Services, result.Addrs[0].NetworkAddr.Services);
                
                Assert.AreEqual(expected.Addrs[1].Timestamp, result.Addrs[1].Timestamp);
                Assert.AreEqual(expected.Addrs[1].NetworkAddr.Ip, result.Addrs[1].NetworkAddr.Ip);
                Assert.AreEqual(expected.Addrs[1].NetworkAddr.Port, result.Addrs[1].NetworkAddr.Port);
                Assert.AreEqual(expected.Addrs[1].NetworkAddr.Services, result.Addrs[1].NetworkAddr.Services);

            }
        }
    }
}