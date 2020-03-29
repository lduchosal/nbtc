using System.Collections.Generic;

namespace Nbtc.Network
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Protocol_documentation#verack
    /// 
    /// # verack
    /// 
    /// The verack message is sent in reply to version. This message consists of only 
    /// a message header with the command string "verack".
    ///
    /// ## Hexdump of the verack message:
    /// ```
    /// 0000   F9 BE B4 D9 76 65 72 61  63 6B 00 00 00 00 00 00   ....verack......
    /// 0010   00 00 00 00 5D F6 E0 E2                            ........
    /// ```
    /// 
    /// ## Message header:
    /// 
    /// ```
    ///  F9 BE B4 D9                          - Main network magic bytes
    ///  76 65 72 61  63 6B 00 00 00 00 00 00 - "verack" command
    ///  00 00 00 00                          - Payload is 0 bytes long
    ///  5D F6 E0 E2                          - Checksum (little endian)
    /// ```
    /// 
    ///  </summary>
    public class VerAck : IPayload
    {
        public Command Command { get { return Command.VerAck;} }
    }
}