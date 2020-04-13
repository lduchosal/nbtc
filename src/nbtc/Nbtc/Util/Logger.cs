using System;
using System.Net;
using Nbtc.Client;
using Nbtc.Network;
using Nbtc.Serialization.Message;
using Serilog;
using Serilog.Events;
using Version = Nbtc.Network.Payload.Version;

namespace Nbtc.Util
{
    public interface ILogger : IDisposable
    {
        ILogger For<T>();
        void Debug(string template, params object[] values);
        void Info(string template, params object[] values);
        void Fatal(string template, params object[] values);
    }

    public class Logger : ILogger
    {
        public Logger()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .Destructure
                    .ByTransforming<TimedNetworkAddr>(
                    r => r.NetworkAddr )
                .Destructure
                    .ByTransforming<NetworkAddr>(
                        r => new { Ip = r.Ip.ToString(), Port = r.Port })
                .Destructure
                    .ByTransforming<Version>(
                        r => new { V = r.Vversion, U = r.UserAgent, H = r.StartHeight })
                .Destructure
                    .ByTransforming<Message>(
                        r => new { r.Magic, r.Command, r.Length })
                .Destructure
                    .ByTransforming<MessageResult>(
                        r => new { r.Statut, r.Error })
                .Destructure
                .ByTransforming<IPAddress>(
                    r => r.ToString())

                .Destructure
                .ByTransforming<SeedElement>(
                    r => r.Value)

            
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                
                .MinimumLevel.Debug()
                
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger()

                ;
        }
        public ILogger For<T>()
        {
            var serilog = Serilog.Log.ForContext<T>();
            var logger = new Logger(serilog);
            return logger;
        } 

        private Serilog.ILogger _inner;
        public Logger(Serilog.ILogger inner)
        {
            _inner = inner;
        }
        public void Debug(string template, params object[] values)
        {
            if (_inner.IsEnabled(LogEventLevel.Debug))
            {
                _inner.Debug(template, values);
            }
        }
        public void Info(string template, params object[] values)
        {
            if (_inner.IsEnabled(LogEventLevel.Information))
            {
                _inner.Information(template, values);
            }
        }
        public void Fatal(string template, params object[] values)
        {
            if (_inner.IsEnabled(LogEventLevel.Fatal))
            {
                _inner.Fatal(template, values);
            }
        }
        public void Dispose()
        {
            Serilog.Log.CloseAndFlush();
        }
    }
}