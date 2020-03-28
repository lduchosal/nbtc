using System;
using System.IO;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter 
    {

        public void Write(Version version)
        {
            Write(version.Vversion);
            Write(version.Services);
            Write(version.Timestamp);
            Write(version.Receiver);
            Write(version.Sender);
            Write(version.Nonce);
            Write(version.UserAgent);
            Write(version.StartHeight);
            Write(version.Relay);
        }

    }
}