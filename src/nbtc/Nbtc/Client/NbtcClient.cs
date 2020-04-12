using System;
using System.Collections.Generic;
using BeetleX;
using BeetleX.Clients;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;
using Nbtc.Util;
using Version = Nbtc.Network.Payload.Version;

namespace Nbtc.Client
{
    public class NbtcClient
    {
        private readonly AsyncTcpClient _client;
        private readonly MessageStateMachine _state = new MessageStateMachine();
        private readonly MessageProvider _message;
        private readonly NodeWalkerStateMachine _nodewalker;
        private readonly ILogger _logger;
        public event  EventHandler<Message> Received = delegate {  };
        public event  EventHandler<IEnumerable<Message>> Sent = delegate {  };
        public event  EventHandler<string> Event = delegate {  };
        public event  EventHandler<Addr> Addr = delegate {  };
        public event  EventHandler<Exception> Error = delegate {  };
        public event  EventHandler<Version> Version = delegate {  };
        

        public NbtcClient(ILogger logger, MessageProvider message, string hostname, int port)
        {
            _logger = logger.For<NbtcClient>();

            var nw = new NodeWalkerStateMachine();
            nw.OnAddr += OnAddr;
            nw.OnConnect += OnConnect;
            nw.OnHandshake += OnHandshake;
            nw.OnInit += OnInit;
            nw.OnGetAddr += OnGetAddr;
            nw.OnVerackReceived += OnVerackReceived;
            nw.OnVerackSent += OnVerackSent;
            nw.OnVersionReceived += OnVersionReceived;
            nw.OnVersionSent += OnVersionSent;
            nw.OnUnhandledTrigger += OnUnhandledTrigger;
            
            var client = SocketFactory.CreateClient<AsyncTcpClient>(hostname, port);
            client.Connected += Connected;
            client.Disconnected += Disconnected;
            client.DataReceive += DataReceive;
            client.ClientError += ClientError;
            
            _client = client;
            _message = message;
            _nodewalker = nw;

        }

        private void Disconnected(IClient c)
        {
            Event(this, "Disconnected");
        }

        private void Connected(IClient c)
        {
            Event(this, "Connected");
        }

        private void ClientError(IClient c, ClientErrorArgs e)
        {
            Error(this, e.Error);
        }

        private void OnAddr(object sender, Addr a)
        {
            Addr(this, a);
        }
        
        private void OnConnect(object sender, EventArgs e)
        {
            Event(this, "OnConnect");
            _nodewalker.SendVersion();
            Send( _message.Version());
        }
        private void OnHandshake(object sender, EventArgs e)
        {
            Event(this, "OnHandshake");
            _nodewalker.SendGetAddr();
            Send( _message.GetAddr());
        }
        private void OnInit(object sender, EventArgs e)
        {
            Event(this, "OnInit");
        }
        private void OnGetAddr(object sender, EventArgs e)
        {
            Event(this, "OnGetAddr");
            // Thread.Sleep(10000);
            // _nodewalker.Timeout();
        }
        private void OnVerackReceived(object sender, EventArgs e)
        {
            Event(this, "OnVerackReceived");
            _nodewalker.SendVerack();
            Send( _message.VerAck());
        }
        private void OnVersionReceived(object sender, Version v)
        {
            Version(this, v);
        }
        private void OnVersionSent(object sender, EventArgs e)
        {
            Event(this, "OnVersionSent");
        }
        private void OnVerackSent(object sender, EventArgs e)
        {
            Event(this, "OnVerackSent");
            _nodewalker.SetVersion();
        }
        private void OnUnhandledTrigger(object sender, string e)
        {
            Event(this, $"OnUnhandledTrigger : {e}");
        }
        
        private void DataReceive(IClient c, ClientReceiveArgs e)
        {
            try
            {
                _logger.Debug("DataReceive [len: {0}]", e.Stream.Length);
                using (var reader = new MessageReader(_logger, e.Stream, _state, true))
                {
                    foreach (var message in reader.ReadMessages())
                    {
                        MessageReceive(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(this, ex);
            }
        }

        private void MessageReceive(Message message)
        {
            Received(this, message);
            var command = message.Payload.Command;
            var payload = message.Payload;
            switch (command)
            {
                case Command.Version:
                    _nodewalker.ReceiveVersion(payload as Version);
                    break;
                
                case Command.Addr:
                    _nodewalker.ReceiveAddr(payload as Addr);
                    break;
                
                case Command.VerAck:
                    _nodewalker.ReceiveVerack();
                    break;
                
                 default:
                    _nodewalker.ReceiveOther();
                    break;

            }
        }

        public void Run()
        {
            _nodewalker.ConnectSocket();
        }
        private void Send(IEnumerable<Message> msgs)
        {
            var c = _client.Send((s) =>
            {
                using (var writer = new ProtocolWriter(s, true))
                {
                    foreach (var message in msgs)
                    {
                        writer.Write(message);
                        s.Flush();
                    }
                }
            });
            Sent(this, msgs);
        }
    }
}