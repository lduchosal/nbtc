using System;

namespace Nbtc.Network
{
    /// <summary>
    /// The following services are currently assigned:
    /// https://en.bitcoin.it/wiki/Protocol_documentation#version
    /// 
    /// +-------+----------------------+-----------------------------------------------------------------+
    /// | Value | Name                 | Description                                                     |
    /// +-------+----------------------+-----------------------------------------------------------------+
    /// |    1  | NODE_NETWORK         | This node can be asked for full blocks instead of just headers. |
    /// |    2  | NODE_GETUTXO         | See BIP 0064                                                    |
    /// |    4  | NODE_BLOOM           | See BIP 0111                                                    | 
    /// |    8  | NODE_WITNESS         | See BIP 0144                                                    | 
    /// | 1024  | NODE_NETWORK_LIMITED | See BIP 0159                                                    |
    /// +-------+----------------------+-----------------------------------------------------------------+
    /// </summary>
    [Flags]
    public enum Service : ulong
    {
        Network = 1,
        GetUtxo = 2,
        Bloom = 4,
        Witness = 8,
        NetworkLimited = 1024
    }
}