using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Tests;

namespace Tests.Network
{
    [TestClass]
    public class CommandTest
    {

        [TestMethod]
        public void When_Command_Getaddr_Then_Success()
        {
            var dump = @"
00000000   67 65 74 61 64 64 72 00  00 00 00 00               getaddr.......
";

            var original = HexDump.Decode(dump);
            var decoder = new CommandDecoder();
            var decode = decoder.Decode(original);
            Assert.IsTrue(decode.Valid);

            var result = decode.Data;
            var expected = Command.GetAddr;

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void When_Command_Version_Then_Success()
        {
            var dump = @"
00000000   76 65 72 73 69 6f 6e 00  00 00 00 00               version......";

            // This message is from a satoshi node, morning of May 27 2014
            var original = HexDump.Decode(dump);

            var decoder = new CommandDecoder();
            var decode = decoder.Decode(original);
            Assert.IsTrue(decode.Valid);

            var result = decode.Data;

            var expected = Command.Version;

            Assert.AreEqual(expected, result);
        }

    }
}
