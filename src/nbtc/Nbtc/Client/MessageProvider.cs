using System;
using System.Collections.Generic;
using System.Net;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Client
{
    public class MessageProvider
    {

        public List<Message> version()
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
                            Ip = IPAddress.Parse("::"),
                            Port = 8333
                        },
                    Sender = new NetworkAddr
                        {
                            Services = Service.Network | Service.Witness | Service.NetworkLimited,
                            Ip = IPAddress.Parse("::"),
                            Port = 0
                        },
                    Nonce = nonce,
                    UserAgent =  "/nbtc=0.0.1/",
                    StartHeight = 623518,
                    Relay = true,
                }
                ;

            return new List<Message>
            {
                new Message
                {
                    NetworkId = NetworkId.MainNet,
                    Payload = version
                }
            };
        }

        public List<Message> GetAddr()
        {

            return new List<Message>
            {
                new Message
                {
                    NetworkId = NetworkId.MainNet,
                    Payload = new GetAddr { }
                }
            };
        }

        public List<Message> VerAck()
        {

            return new List<Message>
            {
                new Message
                {
                    NetworkId = NetworkId.MainNet,
                    Payload = new VerAck { }
                }
            };
        }
    }
}