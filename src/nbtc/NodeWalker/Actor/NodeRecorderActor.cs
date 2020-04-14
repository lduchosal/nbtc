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
            _logger.Trace("{$message}", context.Message);

            if (context.Message is NewSeedNode nsn)
            {
                _nodeProvider.Insert(
                    nsn.Hosts, 
                    nsn.Src, 
                    SourceTypeEnum.DnsSeed);
                
                return Proto.Actor.Done;
            }
            
            if (context.Message is NewAddrNode nan)
            {
                var addrs = nan.Addrs.Addrs.Select(
                    a =>
                    {
                        var ip = a.NetworkAddr.Ip;
                        return (ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip, a.NetworkAddr.Port);
                    });
                
                _nodeProvider.Insert(
                    addrs, 
                    null, 
                    SourceTypeEnum.Addr, 
                    nan.SrcIdentifier);
                
                return Proto.Actor.Done;
            }
            
            if (context.Message is UserAgentNode vn)
            {
                _nodeProvider.UserAgent(
                    vn.Identifier,
                    vn.Version
                    );
                
                return Proto.Actor.Done;
            }

            if (context.Message is FailedNode fn)
            {
                _nodeProvider.Deactivate(fn.Identifier);
                return Proto.Actor.Done;
            }
            
            if (context.Message is SucceedNode sn)
            {
                _nodeProvider.Deactivate(sn.Identifier);
                return Proto.Actor.Done;
            }

            
            return Proto.Actor.Done;

        }
    }
}