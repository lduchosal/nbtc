using System.Linq;
using System.Threading.Tasks;
using Nbtc.Client;
using Nbtc.Util;
using NodeWalker.Message;
using Proto;

namespace NodeWalker.Actor
{
    public class DnsSeederActor : IActor
    {
        private readonly ILogger _logger;
        private readonly DnsSeeder _seeder;
        private readonly PID _nodeRecorderPid;

        public DnsSeederActor(ILogger logger, PID nodeRecorderPid)
        {
            _nodeRecorderPid = nodeRecorderPid;
            _logger = logger.For<DnsSeederActor>();
            _seeder = new DnsSeeder(logger);
            
        }

        public Task ReceiveAsync(IContext context)
        {
            
            _logger.Trace("{$message}", context.Message);
            
            var msg = context.Message as Seed;
            if (msg == null)
            {
                return Proto.Actor.Done;
            }

            foreach (var seed in _seeder.Seed())
            {
                context.Send(_nodeRecorderPid, 
                    new NewSeedNode
                    {
                        Src = seed.Item1,
                        Hosts = seed.Item2.Select(address => (address, (ushort)8333))
                    });
            }
            return Proto.Actor.Done;

        }
    }
}