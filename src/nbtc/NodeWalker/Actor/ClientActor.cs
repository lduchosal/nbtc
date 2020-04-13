using System.Threading.Tasks;
using Nbtc.Client;
using Nbtc.Util;
using NodeWalker.Business;
using NodeWalker.Message;
using Proto;

namespace NodeWalker.Actor
{
    public sealed class ClientActor : IActor
    {
        private readonly ILogger _logger;
        private readonly MessageProvider _message;
        private readonly PID _newAddressPid;
        private readonly PID _failedNodePid;
        
        public ClientActor(ILogger logger)
        {
            _logger = logger.For<ClientActor>();
            _message = new MessageProvider();

        }

        public Task ReceiveAsync(IContext context)
        {
            _logger.Debug("{$message}", context.Message);

            var msg = context.Message as ClientMessage;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            var identifier = msg.Identifier;
            var hostname = msg.Hostname;
            var port = msg.Port;
            using (var client = new NbtcClient(_logger, _message, hostname, port))
            {

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

                    context.Send(_newAddressPid, new Address { Addrs = a });
                };

                client.Version += (o, v) =>
                {
                    _logger.Info("VersionReceived {@Version}", v);
                };

                client.Error += (o, e) =>
                {
                    _logger.Fatal("ErrorHappened {0}", e);
                    context.Send(_failedNodePid, new FailedNode
                    {
                        Identifier = identifier,
                        Hostname = hostname,
                        Port = port
                    });
                };
                
                client.Run();
            }

            return Proto.Actor.Done;

        }
    }
}