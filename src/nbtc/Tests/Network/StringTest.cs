using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void When_Command_Version_Then_Success()
        {
            using var mem = new MemoryStream();
            using var writer = new ProtocolWriter(mem);
            writer.Write(string.Empty);
            writer.Dispose();

            var aresult = mem.ToArray();
            
            var hex = new HexDump();
            var result = hex.Encode(aresult);

            var expected = @"
00000000   00                                                 ·               
";
            Assert.AreEqual(expected, result);
        }
    }
}