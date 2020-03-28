using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class ProtocolReader 
    {
        public string ReadVarString()
        {
            var len = ReadByte();
            var content = ReadBytes(len);
            return Encoding.ASCII.GetString(content);
        }

    }
}