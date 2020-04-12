using System;
using System.Collections.Generic;

namespace Nbtc.Network.Payload
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
    /// </summary>
    public class Inv : IPayload
    {
        public List<InventoryVector> Inventories { get; set; }
        public Command Command => Command.Inv;
    }

    /// <summary>
    /// Inventory Vectors
    /// 
    /// Inventory vectors are used for notifying other nodes about objects they have or data which is being requested.
    /// 
    /// Inventory vectors consist of the following data format:
    /// ```
    /// +-------+-------------+------------+-----------------------------+
    /// | Size  | Description | Data type  | Comments                    | 
    /// +-------+-------------+------------+-----------------------------+
    /// | 4     | type        | uint32_t   | Identifies the object type  |
    /// |       |             |            | linked to this inventory    |
    /// +-------+-------------+------------+-----------------------------+
    /// | 32    | hash        | char[32]   | Hash of the object          |
    /// +-------+-------------+------------+-----------------------------+
    /// ```
    /// 
    /// The object type is currently defined as one of the following possibilities:
    /// ```
    /// +-------+--------------------+-------------------------------------------------------+
    /// | Value | Name               | Description                                           |
    /// +-------+--------------------+-------------------------------------------------------+
    /// | 0     | ERROR              | Any data of with this number may be ignored           |
    /// +-------+--------------------+-------------------------------------------------------+
    /// | 1     | MSG_TX             | Hash is related to a transaction                      |
    /// +-------+--------------------+-------------------------------------------------------+
    /// | 2     | MSG_BLOCK          | Hash is related to a data block                       |
    /// +-------+--------------------+-------------------------------------------------------+
    /// | 3     | MSG_FILTERED_BLOCK | Hash of a block header; identical to MSG_BLOCK.       |
    /// |       |                    | Only to be used in getdata message. Indicates the     |
    /// |       |                    | reply should be a merkleblock message rather than a   |
    /// |       |                    | block message; this only works if a bloom filter      |
    /// |       |                    | has been set.                                         |
    /// +-------+--------------------+-------------------------------------------------------+
    /// | 4     | MSG_CMPCT_BLOCK	 | Hash of a block header; identical to MSG_BLOCK. Only  |
    /// |       |                    | to be used in getdata message.                        |
    /// |       |                    | Indicates the reply should be a cmpctblock message.   |
    /// |       |                    | See BIP 152 for more info.                            |
    /// +-------+--------------------+-------------------------------------------------------+
    /// ```
    /// Other Data Type values are considered reserved for future implementations.
    /// 
    /// </summary>
    public class InventoryVector
    {
        public ObjectType ObjectType { get; set; }
        public string Hash { get; set; }
    }

    public enum ObjectType : UInt32
    {
        Error,
        MsgTx,
        MsgBlock,
        MsgFilteredBlock,
        MsgCmpctBlock
    }
}