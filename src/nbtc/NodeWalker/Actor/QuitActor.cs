using System;
using System.Threading;
using System.Threading.Tasks;
using Nbtc.Util;
using NodeWalker.Message;
using Proto;

namespace NodeWalker.Actor
{
    public sealed class QuitActor : IActor
    {
        private readonly ILogger _logger;
        private readonly AutoResetEvent _quitev;

        public QuitActor(ILogger logger, AutoResetEvent quitev)
        {
            _logger = logger.For<QuitActor>();
            _quitev = quitev;
        }

        public Task ReceiveAsync(IContext context)
        {
            _logger.Trace("{$message}", context.Message);
        
            if (context.Message is Quit)
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                _quitev.Set();
                return Proto.Actor.Done;
            }

            return Proto.Actor.Done;
        }
    }
}