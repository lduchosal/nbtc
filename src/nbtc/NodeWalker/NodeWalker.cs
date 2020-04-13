using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nbtc.Util;
using NodeWalker.Actor;
using NodeWalker.Data;
using NodeWalker.Message;
using Proto;
using Serilog;
using ILogger = Nbtc.Util.ILogger;

namespace NodeWalker
{
    public class NodeWalker : IDisposable
    {
        private readonly ILogger _logger;
        private readonly AutoResetEvent _quitev;

        public NodeWalker(string hostname, in int port)
        {
            ILogger logger =  new Logger();
            var quitev = new AutoResetEvent(false);
            var conn = new ConnectionFactory();
            var nodeProvider = new NodeProvider(logger,  conn);

            var strategy = new LogExceptionStrategy(logger);
            var rootContext = new RootContext();
            var loggerFactory = new LoggerFactory()
                    .AddSerilog();
            
            Proto.Log.SetLoggerFactory(loggerFactory);

            var nodeRecorder = Props.FromProducer(() => new NodeRecorderActor(logger, nodeProvider));
            var nodeRecorderPid = rootContext.Spawn(nodeRecorder);

            var dnsseeder = Props.FromProducer(() => new DnsSeederActor(logger, nodeRecorderPid));        

            var dnsseederPid = rootContext.Spawn(dnsseeder);

            var quit = Props.FromProducer(() => new QuitActor(logger, quitev));
            var quitPid = rootContext.Spawn(quit);

            var client = Props.FromProducer(() => new ClientActor(logger));
            var clientPid = rootContext.Spawn(client);

            var nodelister = Props.FromProducer(() => new NodeListerActor(logger, nodeProvider, clientPid, quitPid));
            var nodelisterPid = rootContext.Spawn(nodelister);

            var heartbeat = Props.FromProducer(() => new HeartbeatActor(logger, nodeProvider, nodelisterPid));
            var heartbeatPid = rootContext.Spawn(heartbeat);

            rootContext.Send(dnsseederPid, new Seed {});
            
            _quitev = quitev;
            _logger = logger.For<NodeWalker>();;
        }

        public void Run()
        {
            _logger.Info("nbtc cli started");
            _logger.Debug("Waiting...");
            _quitev.WaitOne();
            _logger.Info("nbtc cli stopped");
        }

        public void Dispose()
        {
            _logger?.Dispose();
            _quitev?.Dispose();
        }
    }

}