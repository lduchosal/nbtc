using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Version = Nbtc.Network.Payload.Version;

namespace Nbtc.Serialization
{
    public sealed partial class ProtocolWriter 
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
        
        public void Write(Inv inv)
        {
            var vecs = inv.Inventories;
            var varint = new VarInt
            {
                Value = (ulong)vecs.Count
            };
            Write(varint);
            foreach (var vec in vecs)
            {               
                Write(vec);
            }
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
        /// <param name="vec"></param>
        public void Write(InventoryVector vec)
        {
            Write(vec.ObjectType);
            var hash = $"{vec.Hash}\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            var bytes = Encoding.ASCII.GetBytes(hash);
            Write(bytes, 0, 32);
        }

        private void Write(ObjectType ot)
        {
            Write((UInt32)ot);
        }
    }
}