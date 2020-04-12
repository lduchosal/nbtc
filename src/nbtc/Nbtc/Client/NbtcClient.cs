using System;
using System.Collections.Generic;
using BeetleX;
using BeetleX.Clients;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;
using Version = Nbtc.Network.Version;

namespace Nbtc.Client
{
    public class NbtcClient
    {
        private readonly AsyncTcpClient _client;
        private readonly MessageStateMachine _state = new MessageStateMachine();
        private readonly MessageProvider _message;
        private readonly NodeWalkerStateMachine _nodewalker;
        private readonly ILogger _logger;
        public event  EventHandler<Message> MessageReceived = delegate {  };
        public event  EventHandler<IEnumerable<Message>> MessagesSent = delegate {  };
        public event  EventHandler<string> EventHappened = delegate {  };
        public event  EventHandler<Addr> AddrReceived = delegate {  };
        public event  EventHandler<Exception> ErrorHappened = delegate {  };
        public event  EventHandler<Version> VersionReceived = delegate {  };
        

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
            EventHappened(this, "Disconnected");
        }

        private void Connected(IClient c)
        {
            EventHappened(this, "Connected");
        }

        private void ClientError(IClient c, ClientErrorArgs e)
        {
            ErrorHappened(this, e.Error);
        }

        private void OnAddr(object sender, Addr a)
        {
            AddrReceived(this, a);
        }
        
        private void OnConnect(object sender, EventArgs e)
        {
            EventHappened(this, "OnConnect");
            _nodewalker.SendVersion();
            Send( _message.Version());
        }
        private void OnHandshake(object sender, EventArgs e)
        {
            EventHappened(this, "OnHandshake");
            _nodewalker.SendGetAddr();
            Send( _message.GetAddr());
        }
        private void OnInit(object sender, EventArgs e)
        {
            EventHappened(this, "OnInit");
        }
        private void OnGetAddr(object sender, EventArgs e)
        {
            EventHappened(this, "OnGetAddr");
            // Thread.Sleep(10000);
            // _nodewalker.Timeout();
        }
        private void OnVerackReceived(object sender, EventArgs e)
        {
            EventHappened(this, "OnVerackReceived");
            _nodewalker.SendVerack();
            Send( _message.VerAck());
        }
        private void OnVersionReceived(object sender, Version v)
        {
            VersionReceived(this, v);
        }
        private void OnVersionSent(object sender, EventArgs e)
        {
            EventHappened(this, "OnVersionSent");
        }
        private void OnVerackSent(object sender, EventArgs e)
        {
            EventHappened(this, "OnVerackSent");
            _nodewalker.SetVersion();
        }
        private void OnUnhandledTrigger(object sender, string e)
        {
            EventHappened(this, $"OnUnhandledTrigger : {e}");
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
                ErrorHappened(this, ex);
            }
        }

        private void MessageReceive(Message message)
        {
            MessageReceived(this, message);
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
                
                case Command.Inv:
                    _nodewalker.ReceiveOther();
                    break;
                
                case Command.Alert:
                    _nodewalker.ReceiveOther();
                    break;
                
                case Command.Ping:
                    _nodewalker.ReceiveOther();
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
            MessagesSent(this, msgs);
        }
    }
}