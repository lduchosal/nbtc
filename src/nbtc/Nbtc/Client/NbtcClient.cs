using System.IO;
using System.Net.Sockets;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Nbtc.Client
{
    public class NbtcClient
    {
        private readonly TcpClient _tcp;
        private readonly MessageProvider _message;

        public NbtcClient(MessageProvider message, string hostname, int port)
        {
            _tcp = new TcpClient(hostname, port);
            _message = message;
        }

        public void GetAddr()
        {            
            var stream = _tcp.GetStream();
            var buffered = new BufferedStream(stream);
            var writer = new ProtocolWriter(buffered);
            var reader = new ProtocolReader(buffered);
            var version = _message.version();
            writer.Write(version);
            reader.ReadNext();
        }
    }
}