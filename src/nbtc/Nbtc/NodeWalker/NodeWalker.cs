using System;
using System.Threading;
using Nbtc.Client;
using Nbtc.Util;

namespace Nbtc.NodeWalker
{
    public class NodeWalker : IDisposable
    {
        private readonly ILogger _logger;
        private readonly AutoResetEvent _ev;
        private readonly NbtcClient _client;

        public NodeWalker(string hostname, in int port)
        {
            var logger =  new Logger().For<NodeWalker>();

            var message = new MessageProvider();
            var ev = new AutoResetEvent(false);
            var client = new NbtcClient(logger, message, hostname, port);
            
            client.Received += (o, e) =>
            {
                var command = e.Payload.Command;
                _logger.Info("MessageReceived : {0}", command);
            };
            
            client.Sent += (o, e) =>
            {
                foreach (var message in e)
                {
                    var command = message.Payload.Command;
                    _logger.Info("MessageSent : {0}", command);
                }
            };
            client.Event += (o, e) =>
            {
                _logger.Info("EventHappened : {0}", e);
            };
            
            client.Addr += (o, a) =>
            {
                _logger.Info("AddrReceived : {0}", a.Addrs.Count);
                foreach (var addr in a.Addrs)
                {
                    _logger.Info("AddrReceived : {@Addr}", addr);
                }
                ev.Set();
            };

            client.Version += (o, v) =>
            {
                _logger.Info("VersionReceived {@Version}", v);
            };
            
            client.Error += (o, e) =>
            {
                _logger.Fatal("ErrorHappened {0}", e);
                ev.Set();
            };

            _client = client;
            _ev = ev;
            _logger = logger;
        }

        public void Run()
        {
            _logger.Info("nbtc cli started");
            _client.Run();
            _ev.WaitOne();
            _logger.Info("nbtc cli stopped");
        }

        public void Dispose()
        {
            _logger?.Dispose();
            _ev?.Dispose();
            _client?.Dispose();
        }
    }
}