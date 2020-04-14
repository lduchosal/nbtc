using System.Threading;
using System.Threading.Tasks;
using Nbtc.Util;
using NodeWalker.Data;
using NodeWalker.Message;
using Proto;

namespace NodeWalker.Actor
{
    public class HeartbeatActor : IActor
    {
        private readonly ILogger _logger;
        private readonly NodeProvider _nodeProvider;
        private readonly PID _nodelisterPid;

        public HeartbeatActor(ILogger logger, NodeProvider nodeProvider, PID nodelisterPid)
        {
            _logger = logger.For<HeartbeatActor>();
            _nodeProvider = nodeProvider;
            _nodelisterPid = nodelisterPid;
        }

        public Task ReceiveAsync(IContext context)
        {
            _logger.Trace("{$message}", context.Message);

            var msg = context.Message as Started;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            while (true)
            {
                context.Send(_nodelisterPid, new ListNode());
                Thread.Sleep(10000);
            }
            return Proto.Actor.Done;

        }
    }
}