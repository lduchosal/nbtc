using System.Linq;
using System.Threading.Tasks;
using Nbtc.Util;
using NodeWalker.Data;
using NodeWalker.Message;
using Proto;

namespace NodeWalker.Actor
{
    public sealed class NodeListerActor : IActor
    {
        private readonly ILogger _logger;
        private readonly NodeProvider _nodeProvider;
        private readonly PID _clientPid;
        private readonly PID _quitPid;

        public NodeListerActor(ILogger logger, NodeProvider nodeProvider, PID clientPid, PID quitPid)
        {
            _nodeProvider = nodeProvider;
            _clientPid = clientPid;
            _quitPid = quitPid;
            _logger = logger.For<NodeListerActor>();
        }

        public Task ReceiveAsync(IContext context)
        {
            _logger.Trace("{$message}", context.Message);

            var msg = context.Message as ListNode;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            int max = 1000;

            _logger.Debug("Listing top {max} nodes", max);

            var nodes = _nodeProvider.Select(StatusEnum.New, max);
            
            _logger.Trace("List with {count} nodes", nodes.Count());
            
            if (nodes.Any())
            {
                foreach (var node in nodes)
                {
                    var walk = new ClientMessage
                    {
                        Address = node.Ip,
                        Identifier = node.Id,
                        Port = node.Port,
                    };
                    context.Send(_clientPid, walk);
                }
                return Proto.Actor.Done;
            }
            
            return Proto.Actor.Done;

        }
    }
}