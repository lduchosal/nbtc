using System.Linq;
using System.Threading;
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
        private readonly PID _nodeRecorderPid;
        
        public ClientActor(ILogger logger, PID nodeRecorderPid)
        {
            _logger = logger.For<ClientActor>();
            _message = new MessageProvider();
            _nodeRecorderPid = nodeRecorderPid;
        }

        public Task ReceiveAsync(IContext context)
        {
            _logger.Trace("{$message}", context.Message);

            var msg = context.Message as ClientMessage;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            var end = new AutoResetEvent(false);
            var identifier = msg.Identifier;
            var address = msg.Address;
            var port = msg.Port;
            using (var client = new NbtcClient(_logger, _message, address, (int)port))
            {

                client.Connect += (o, e) =>
                {
                    _logger.Debug("Connected to {@host}", new { address, port});
                };
                
                client.Sent += (o, e) =>
                {
                    _logger.Trace("Sent : {0}", e.Count());
                    foreach (var message in e)
                    {
                        var command = message.Payload.Command;
                        _logger.Trace("Sent : {$commnad}", command);
                    }
                };

                client.Event += (o, e) =>
                {
                    _logger.Trace("Event : {0}", e);
                };

                client.Disconnect += (o, reason) =>
                {
                    _logger.Trace("Disconnected : {0}", reason);
                    _logger.Debug("Disconnected from {@host}", new { address, port, reason});
                    context.Send(_nodeRecorderPid, new FailedNode
                    {
                        Identifier = identifier,
                    });
                    end.Set();
                };

                client.Addr += (o, a) =>
                {
                    _logger.Debug("Get Addr {@addr}", new { address, port, countaddr = a.Addrs.Count });

                    _logger.Trace("Addr : {0}", a.Addrs.Count);
                    foreach (var addr in a.Addrs)
                    {
                        _logger.Trace("Addr : {@Addr}", addr);
                    }

                    context.Send(_nodeRecorderPid, new NewAddrNode
                    {
                        SrcAdress = address,
                        SrcPort = port,
                        SrcIdentifier = identifier,
                        Addrs = a 
                        
                    });
                    context.Send(_nodeRecorderPid, new SucceedNode()
                    {
                        Identifier = identifier,
                    });
                    end.Set();
                };

                client.Version += (o, v) =>
                {
                    _logger.Trace("Version {@Version}", v);
                    context.Send(_nodeRecorderPid, new UserAgentNode
                    {
                        Identifier = identifier,
                        Version = v.UserAgent
                    });

                    
                };

                client.Error += (o, e) =>
                {
                    _logger.Debug("Error {0}", e);
                    context.Send(_nodeRecorderPid, new FailedNode
                    {
                        Identifier = identifier,
                    });
                    end.Set();
                };
                
                client.Run();
                end.WaitOne();
            }

            return Proto.Actor.Done;

        }
    }
}