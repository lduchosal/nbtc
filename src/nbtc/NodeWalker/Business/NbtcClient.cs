using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using BeetleX;
using BeetleX.Clients;
using Nbtc.Client;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;
using Nbtc.Util;
using Version = Nbtc.Network.Payload.Version;
    

namespace NodeWalker.Business
{
    public class NbtcClient : IDisposable
    {
        private readonly AsyncTcpClient _client;
        private readonly MessageStateMachine _state = new MessageStateMachine();
        private readonly MessageProvider _message;
        private readonly NodeWalkerStateMachine _nodewalker;
        private readonly ILogger _logger;
        public event  EventHandler<Nbtc.Network.Message> Received = delegate {  };
        public event  EventHandler<IEnumerable<Nbtc.Network.Message>> Sent = delegate {  };
        public event  EventHandler<string> Event = delegate {  };
        public event  EventHandler Connect = delegate {  };
        public event  EventHandler<string> Disconnect = delegate {  };
        public event  EventHandler<Addr> Addr = delegate {  };
        public event  EventHandler<Exception> Error = delegate {  };
        public event  EventHandler<Version> Version = delegate {  };
        

        public NbtcClient(ILogger logger, MessageProvider message, string address, int port)
        {
            _logger = logger.For<NbtcClient>();

            _logger.Debug("NbtcClient Walking {connect}", new {address, port});

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
            
            var client = SocketFactory.CreateClient<AsyncTcpClient>(address, port);
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
            _logger.Trace("Disconnected");
            Disconnect(this, "Disconnected");
        }

        private void Connected(IClient c)
        {
            _logger.Trace("Connected");
            Event(this, "Connected");
        }

        private void ClientError(IClient c, ClientErrorArgs e)
        {
            if (e.Error is SocketException se)
            {
                Disconnect(this, se.SocketErrorCode.ToString());
            }
            else
            {
                _logger.Trace("ClientError {@e}", e);
                Error(this, e.Error);
            }
        }

        private void OnAddr(object sender, Addr a)
        {
            _logger.Trace("OnAddr");
            Addr(this, a);
        }
        
        private void OnConnect(object sender, EventArgs e)
        {
            _logger.Trace("OnConnect");
            Connect(this, EventArgs.Empty);
            
            bool sent = Send( _message.Version());
            if (sent)
            {
                _nodewalker.SendVersion();
            }

        }
        private void OnHandshake(object sender, EventArgs e)
        {
            _logger.Trace("OnHandshake");
            Event(this, "OnHandshake");
            bool sent = Send( _message.GetAddr());
            if (sent)
            {
                _nodewalker.SendGetAddr();
            }

        }
        private void OnInit(object sender, EventArgs e)
        {
            _logger.Trace("OnInit");
            Event(this, "OnInit");
        }
        private void OnGetAddr(object sender, EventArgs e)
        {
            _logger.Trace("OnGetAddr");
            Event(this, "OnGetAddr");
            // Thread.Sleep(10000);
            // _nodewalker.Timeout();
        }
        private void OnVerackReceived(object sender, EventArgs e)
        {
            _logger.Trace("OnVerackReceived");
            Event(this, "OnVerackReceived");
            
            bool sent = Send( _message.VerAck());
            if (sent)
            {
                _nodewalker.SendVerack();
            }

        }
        private void OnVersionReceived(object sender, Version v)
        {
            _logger.Trace("OnVersionReceived");
            Version(this, v);
        }
        private void OnVersionSent(object sender, EventArgs e)
        {
            _logger.Trace("OnVersionSent");
            Event(this, "OnVersionSent");
        }
        private void OnVerackSent(object sender, EventArgs e)
        {
            _logger.Trace("OnVerackSent");
            Event(this, "OnVerackSent");
            _nodewalker.SetVersion();
        }
        private void OnUnhandledTrigger(object sender, string e)
        {
            _logger.Trace("OnUnhandledTrigger {e}", e);
            Event(this, $"OnUnhandledTrigger : {e}");
        }
        
        private void DataReceive(IClient c, ClientReceiveArgs e)
        {
            try
            {
                _logger.Trace("DataReceive [len: {0}]", e.Stream.Length);
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

        private void MessageReceive(Nbtc.Network.Message message)
        {
            _logger.Trace("MessageReceive {message}", message);
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
            _logger.Trace("Run");
            _nodewalker.ConnectSocket();
        }
        private bool Send(IEnumerable<Nbtc.Network.Message> msgs)
        {
            _logger.Trace("Send {count} messages", msgs.Count());
            bool connected = _client.Connect(out bool newConnection);
            if (!connected)
            {
                return false;
            }
            
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
            return true;
        }

        public void Dispose()
        {
            _logger.Trace("Dispose");
            _client.Dispose();
        }
    }
}