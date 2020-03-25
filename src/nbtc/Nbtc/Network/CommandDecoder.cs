using Nbtc.Network;
using System;

namespace Tests.Network
{
    public class CommandDecoder
    {
        public CommandDecoder()
        {
        }

        public Result<Nbtc.Network.Command> Decode(ReadOnlySpan<byte> bytes)
        {
            return Result<Nbtc.Network.Command>.Fail(ErrorEnum.Command);
        }
    }
}