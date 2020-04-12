﻿using System;
using System.Threading;
using Nbtc.Client;
using Nbtc.Util;

namespace Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = new Logger();
            var me = logger.For<Program>();
            
            me.Info("nbtc cli started");

            var message = new MessageProvider();
            string hostname = "127.0.0.1";
            int port = 8333;
            
            var ev = new AutoResetEvent(false);
            var client = new NbtcClient(logger, message, hostname, port);
            
            client.MessageReceived += (o, e) =>
            {
                var command = e.Payload.Command;
                me.Info("MessageReceived : {0}", command);
            };
            
            client.MessagesSent += (o, e) =>
            {
                foreach (var message in e)
                {
                    var command = message.Payload.Command;
                    me.Info("MessageSent : {0}", command);
                }
            };
            client.EventHappened += (o, e) =>
            {
                me.Info("EventHappened : {0}", e);
            };
            
            client.AddrReceived += (o, a) =>
            {
                me.Info("AddrReceived : {0}", a.Addrs.Count);
                foreach (var addr in a.Addrs)
                {
                    me.Info("AddrReceived : {@Addr}", addr);
                }
                ev.Set();
            };

            client.VersionReceived += (o, v) =>
            {
                me.Info("VersionReceived {@Version}", v);
            };
            
            client.ErrorHappened += (o, e) =>
            {
                me.Fatal("ErrorHappened {0}", e);
                ev.Set();
            };
            
            client.Run();
            ev.WaitOne();
            me.Info("nbtc cli stopped");

        }
    }
}