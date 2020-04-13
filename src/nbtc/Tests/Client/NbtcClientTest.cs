using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Client;
using Nbtc.Util;

namespace Tests.Client
{
    [TestClass]
    public class NbtcClientTest
    {
        
        [TestMethod]
        public void When_Run_Localhost_Returns_AddrList() {

            string hostname = "127.0.0.1";
            int port = 8333;

            var nw = new NodeWalker.NodeWalker(hostname, port);
            nw.Run();
            
            Thread.Sleep(1000);
        }

    }
}