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
            _logger.Debug("{$message}", context.Message);

            var msg = context.Message as ListNode;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            var nodes = _nodeProvider.Select(NodeProvider.StatusEnum.New, 100);
            if (nodes.Any())
            {
                foreach (var node in nodes)
                {
                    var walk = new ClientMessage
                    {
                        Hostname = node.Ip,
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