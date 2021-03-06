using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;
using Nbtc.Util;

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

            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();
            
            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);


            var result = reader.ReadCommand();
            Assert.AreEqual(Command.GetAddr, result);
        }

        [TestMethod]
        public void When_Command_Version_Then_Success()
        {
            var dump = @"
00000000   76 65 72 73 69 6f 6e 00  00 00 00 00               version......";

            // This message is from a satoshi node, morning of May 27 2014
            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();

            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);


            var command = reader.ReadCommand();
            var expected = Command.Version;

            Assert.AreEqual(expected, command);
        }
    }
}

