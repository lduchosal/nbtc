namespace Nbtc.Network
{
    public enum Command
    {
        Version,
        VerAck,
        GetHeaders,
        SendHeaders,
        SendCmpct,
        GetAddr,
        Alert,
        Addr,
        Ping,
        Pong,
        Inv,
        FeeFilter
    }
}