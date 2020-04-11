using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BeetleX;
using BeetleX.Clients;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Nbtc.Client
{
    public class NbtcClient
    {
        private readonly AsyncTcpClient _client;
        private readonly MessageStateMachine _state = new MessageStateMachine();
        private readonly MessageProvider _message;
        private readonly NodeWalker _nodewalker;
        public event  EventHandler<Message> MessageReceived = delegate {  };
        public event  EventHandler<List<Message>> MessagesSent = delegate {  };
        public event  EventHandler<string> EventHappened = delegate {  };
        public event  EventHandler<string> AddrReceived = delegate {  };
        public event  EventHandler<Exception> ErrorHappend = delegate {  };
        

        public NbtcClient(MessageProvider message, string hostname, int port)
        {
            var nodewalker = new NodeWalker();
            nodewalker.OnAddr += OnAddr;
            nodewalker.OnConnect += OnConnect;
            nodewalker.OnHandshake += OnHandshake;
            nodewalker.OnInit += OnInit;
            nodewalker.OnGetAddr += OnGetAddr;
            nodewalker.OnVerackReceived += OnVerackReceived;
            nodewalker.OnVerackSent += OnVerackSent;
            nodewalker.OnVersionReceived += OnVersionReceived;
            nodewalker.OnVersionSent += OnVersionSent;
            nodewalker.OnUnhandledTrigger += OnUnhandledTrigger;
            
            var client = SocketFactory.CreateClient<AsyncTcpClient>(hostname, port);
            client.Connected += Connected;
            client.Disconnected += Disconnected;
            client.DataReceive += DataReceive;
            client.ClientError += ClientError;
            
            _client = client;
            _message = message;
            _nodewalker = nodewalker;

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
            ErrorHappend(this, e.Error);
        }

        private void OnAddr(object sender, EventArgs e)
        {
            AddrReceived(this, "Addr");
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
        private void OnVersionReceived(object sender, EventArgs e)
        {
            EventHappened(this, "OnVersionReceived");
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
        

        public bool IsConnected
        {
            get { return _client.IsConnected; }
        }

        public Exception LastError
        {
            get { return _client.LastError; }
        }

        private void DataReceive(IClient c, ClientReceiveArgs e)
        {
            try
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] NbtcClient DataReceive");
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] NbtcClient DataReceive [len: {e.Stream.Length}]");
                using (var reader = new MessageReader(e.Stream, _state, true))
                {
                    foreach (var message in reader.ReadMessages())
                    {
                        MessageReceive(message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHappend(this, ex);
            }
        }

        private void MessageReceive(Message message)
        {
            MessageReceived(this, message);
            var command = message.Payload.Command;
            switch (command)
            {
                case Command.Version:
                    _nodewalker.ReceiveVersion();
                    break;
                
                case Command.Addr:
                    _nodewalker.ReceiveAddr();
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
        private void Send(List<Message> msgs)
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