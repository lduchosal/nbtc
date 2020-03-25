using System;
using System.Collections.Generic;
using System.Text;

namespace Nbtc.Network
{

    public enum Command
    {
        Version,
        VerAck,
        GetHeaders,
        GetAddr,
        Alert,
        Addr,
        Ping,
        Pong,
        Inv,
    }


}
