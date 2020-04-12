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
        public void When_Encode_Ping_Then_nothing_To_Encode() {

            
            var message = new MessageProvider();
            string hostname = "127.0.0.1";
            int port = 8333;
            
            var ev = new AutoResetEvent(false);
            var logger = new Logger();
            var client = new NbtcClient(logger, message, hostname, port);

            client.MessageReceived += (o, e) =>
            {
                var command = e.Payload.Command;
                Console.WriteLine($"NetworkId : {e.Magic}");
                Console.WriteLine($"Command: {command}");
            };
            client.MessageReceived += (o, e) =>
            {
                var command = e.Payload.Command;
                Console.WriteLine($"NetworkId : {e.Magic}");
                Console.WriteLine($"Command: {command}");
            };
            client.EventHappened += (o, e) =>
            {
                Console.WriteLine($"EventHappened : {e}");
            };
            client.AddrReceived += (o, e) =>
            {
                Console.WriteLine($"AddrReceived : {e}");
                ev.Set();
            };

            client.Run();
            ev.WaitOne();
            
            Thread.Sleep(1000);


        }

    }
}