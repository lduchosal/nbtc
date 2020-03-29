using System.IO;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Client;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Client
{
    [TestClass]
    public class NbtcClientTest
    {
        
        [TestMethod]
        public void When_Encode_Ping_Then_nothing_To_Encode() {

            var message = new MessageProvider();
            string hostname = "127.0.0.1";
            int port = 8333;
                
            var client = new NbtcClient(message, hostname, port);
            client.GetAddr();
        }

    }
}