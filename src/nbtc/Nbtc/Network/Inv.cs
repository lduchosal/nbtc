using System.Collections.Generic;

namespace Nbtc.Network
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Protocol_documentation#inv
    /// 
    /// # inv
    /// 
    /// Allows a node to advertise its knowledge of one or more objects. It can be received unsolicited, 
    /// or in reply to getblocks.
    /// 
    /// Payload (maximum 50,000 entries, which is just over 1.8 megabytes):
    /// 
    /// ```
    /// +------+-------------+------------+-----------------------------+
    /// | Size | Description | Data type  | Comments                    |
    /// +------+-------------+------------+-----------------------------+
    /// |   1+ | count       | var_int    | Number of inventory entries |
    /// | 36x? | inventory   | inv_vect[] | Inventory vectors           |
    /// +------+-------------+------------+-----------------------------+
    /// ```
    ///     /// </summary>
    public class Inv : IPayload
    {
        public byte[] Data { get; set; }
        public Command Command { get { return Command.Inv;} }
    }
}