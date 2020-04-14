using System;
using Nbtc.Util;
using Proto;

namespace NodeWalker.Actor
{
    public class LogExceptionStrategy : ISupervisorStrategy
    {
        private readonly ILogger _logger;

        public LogExceptionStrategy(ILogger logger)
        {
            _logger = logger;
        }

        public void HandleFailure(
            ISupervisor supervisor,
            PID child,
            RestartStatistics rs,
            Exception reason)
        {
            _logger.Trace("{@reason}", reason);
            supervisor.StopChildren(child);
        }
    }
}