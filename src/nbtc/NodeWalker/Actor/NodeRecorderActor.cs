using System.Linq;
using System.Threading.Tasks;
using Nbtc.Util;
using NodeWalker.Data;
using NodeWalker.Message;
using Proto;

namespace NodeWalker.Actor
{
    public sealed class NodeRecorderActor : IActor
    {
        private readonly ILogger _logger;
        private readonly NodeProvider _nodeProvider;

        public NodeRecorderActor(ILogger logger, NodeProvider nodeProvider)
        {
            _nodeProvider = nodeProvider;
            _logger = logger.For<NodeRecorderActor>();
        }

        public Task ReceiveAsync(IContext context)
        {
            _logger.Debug("{$message}", context.Message);

            var msg = context.Message as NewNode;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            var hosts = msg.Hosts.Select((ip,port) => (ip.ToString(), port));
            _nodeProvider.BulkInsert(hosts, msg.Src, 0);
            
            return Proto.Actor.Done;

        }
    }
}