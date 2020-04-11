using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class GetAddrTest
    {
        
        [TestMethod]
        public void When_Encode_GetAddr_Then_nothing_To_Encode() {

            var message = new GetAddr {
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new PayloadReader(mem2);
            var getaddr = reader.ReadGetAddr();
            Assert.IsNotNull(getaddr);
            Assert.AreEqual(Command.GetAddr, getaddr.Command);
        }

        [TestMethod]
        public void When_Decode_GetAddr_one_Then_nothing_To_Encode() {

            var message = new GetAddr {
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new PayloadReader(mem2);
            var getaddr = reader.ReadGetAddr();
            Assert.IsNotNull(getaddr);
            Assert.AreEqual(Command.GetAddr, getaddr.Command);
        }
    }
}