using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class MessageSendHeadersTest
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {
            var dump = @"
0000    f9 be b4 d9 73 65 6e 64    68 65 61 64 65 72 73 00    ????sendheaders.
0000    00 00 00 00 5d f6 e0 e2                               ....]???        
";
            
            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();
            
            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);


var message = reader.ReadMessage();
            var sendheaders = message.Payload as SendHeaders;

            Assert.AreEqual(Command.SendHeaders, message.Payload.Command);
            Assert.IsNotNull(sendheaders);
        }
    }
}