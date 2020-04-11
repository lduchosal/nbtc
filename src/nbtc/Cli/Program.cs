using System;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            string hostname = "127.0.0.1";
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
            
            client.AddrReceived += (o, a) =>
            {
                Console.WriteLine($"AddrReceived [Addrs {a.Addrs.Count}]");
                foreach (var addr in a.Addrs)
                {
                    Console.WriteLine($"AddrReceived [Addr: {addr}]");
                }
                ev.Set();
            };

            
            client.VersionReceived += (o, v) =>
            {
                Console.WriteLine($"VersionReceived [UserAgent   : {v.UserAgent}]");
                Console.WriteLine($"VersionReceived [Timestamp   : {v.Timestamp}]");
                Console.WriteLine($"VersionReceived [Version     : {v.Vversion}]");
                Console.WriteLine($"VersionReceived [StartHeight : {v.StartHeight}]");
                Console.WriteLine($"VersionReceived [Services    : {v.Services}]");
                Console.WriteLine($"VersionReceived [Receiver    : {v.Receiver.ToString()}]");
                Console.WriteLine($"VersionReceived [Sender      : {v.Sender.ToString()}]");
            };
            
            client.ErrorHappened += (o, e) =>
            {
                Console.WriteLine($"ErrorHappened : {e.ToString()}");
                ev.Set();
            };
            
            client.Run();
            ev.WaitOne();
            Console.WriteLine($"Stopping program");


        }
    }
}