using System;
using System.Threading;
using Nbtc.Client;

namespace Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("nbtc cli");
            
            var message = new MessageProvider();
            string hostname = "104.198.116.235";
            int port = 8333;
            
            var ev = new AutoResetEvent(false);
            var client = new NbtcClient(message, hostname, port);
            client.MessageReceived += (o, e) =>
            {
                var command = e.Payload.Command;
                Console.WriteLine($"MessageReceived : {command}");
            };
            
            client.MessagesSent += (o, e) =>
            {
                foreach (var message in e)
                {
                    var command = message.Payload.Command;
                    Console.WriteLine($"MessageSent : {command}");
                }
            };
            client.EventHappened += (o, e) =>
            {
                Console.WriteLine($"EventHappened : {e}");
            };
            
            client.AddrReceived += (o, e) =>
            {
                Console.WriteLine($"AddrReceived : {e}");
                Console.WriteLine($"AddrReceived : Stopping program");
                ev.Set();
            };
            client.ErrorHappend += (o, e) =>
            {
                Console.WriteLine($"ErrorHappend : {e.ToString()}");
                Console.WriteLine($"ErrorHappend : Stopping program");
                ev.Set();
            };
            
            client.Run();
            ev.WaitOne();


        }
    }
}