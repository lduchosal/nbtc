using System;
using System.Collections.Generic;
using System.Net;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Version = Nbtc.Network.Payload.Version;

namespace Nbtc.Client
{
    public class MessageProvider
    {

        public IEnumerable<Message> Version()
        {

            var now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var rng = new Random();
            var bytes = new byte[64];
            rng.NextBytes(bytes);
            var nonce = BitConverter.ToUInt64(bytes, 0);

            var version = new Version {
                    Vversion = 70015,
                    Services = Service.Network | Service.Witness | Service.NetworkLimited,
                    Timestamp = now,
                    Receiver = new NetworkAddr
                        {
                            Services = Service.Network | Service.Witness | Service.NetworkLimited,
                            Ip = IPAddress.Parse("0.0.0.0"),
                            Port = 0
                        },
                    Sender = new NetworkAddr
                        {
                            Services = Service.Network | Service.Witness | Service.NetworkLimited,
                            Ip = IPAddress.Parse("0.0.0.0"),
                            Port = 0
                        },
                    Nonce = nonce,
                    UserAgent =  "/nbtc=0.0.1/",
                    StartHeight = 623518,
                    Relay = true,
                }
                ;

            yield return new Message
            {
                Magic = NetworkId.MainNet,
                Payload = version
            };
        }

        public IEnumerable<Message> GetAddr()
        {
            yield return new Message
            {
                Magic = NetworkId.MainNet,
                Payload = new GetAddr { }
            };
        }

        public IEnumerable<Message> Ping()
        {
            yield return new Message 
            {
                Magic = NetworkId.MainNet,
                Payload = new Ping
                {
                    Nonce = 1234
                }
            };
        }

        public IEnumerable<Message> VerAck()
        {
            yield return new Message
            {
                Magic = NetworkId.MainNet,
                Payload = new VerAck { }
            };
        }
    }
}