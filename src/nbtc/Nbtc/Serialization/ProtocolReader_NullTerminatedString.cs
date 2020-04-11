using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nbtc.Network;
using Version = Nbtc.Network.Version;

namespace Nbtc.Serialization
{
    public  sealed partial class PayloadReader 
    {
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