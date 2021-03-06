using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nbtc.Network;
using Nbtc.Util;

namespace Nbtc.Serialization.Message
{
    public  sealed partial class MessageReader : BinaryReader
    {
        private readonly ILogger _logger;

        public MessageReader(ILogger logger, Stream output, MessageStateMachine machine, bool leaveOpen = false)
            : base(output, EncodingCache.UTF8NoBOM, leaveOpen)
        {
            _logger = logger.For<MessageReader>();
            _machine = machine;
            _machine.OnMessage += OnMessage;
            _machine.OnChecksum += OnChecksum;
            _machine.OnPayload += OnPayload;
            _machine.OnUnHandled += OnUnHandled;
        }

        protected override void Dispose(bool disposing)
        {
            _machine.OnMessage -= OnMessage;
            _machine.OnChecksum -= OnChecksum;
            _machine.OnPayload -= OnPayload;
            _machine.OnUnHandled -= OnUnHandled;
            base.Dispose(disposing);
        }

        public Command ReadCommand()
        {
            var scommand = ReadNullTerminatedString(12);
            var parsed = Enum.TryParse<Command>(scommand, true, out Command command);
            if (!parsed)
            {
                command = Command.Unknown;
                _logger.Debug("ReadCommand {0}", new { scommand, command});
                return command;
            }
            return command;
        }
        
        public string ReadNullTerminatedString(int len)
        {
            var bytes = ReadBytes(len);
            var bstring = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                if (bytes[i] == 0x00) break;
                bstring.Add(bytes[i]);
            }
            return Encoding.ASCII.GetString(bstring.ToArray());
        }
    }
}