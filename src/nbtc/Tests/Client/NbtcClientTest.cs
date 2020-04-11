using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Client;

namespace Tests.Client
{
    [TestClass]
    public class NbtcClientTest
    {
        
        [TestMethod]
        [Ignore]
        public void When_Encode_Ping_Then_nothing_To_Encode() {

            
            var message = new MessageProvider();
            string hostname = "104.198.116.235";
            int port = 8333;
            
            var ev = new AutoResetEvent(false);
            var client = new NbtcClient(message, hostname, port);
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
            };

            client.Run();
            ev.WaitOne();


        }

    }
}