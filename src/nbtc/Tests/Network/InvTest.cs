using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Payload;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class InvTest
    {
        
        [TestMethod]
        public void When_Encode_Inv_Then_nothing_To_Encode() {

            var message = new Inv {
                Inventories = new List<InventoryVector>()
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            var logger = new Logger();
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new PayloadReader(logger, mem2);



var inv = reader.ReadInv();
            Assert.IsNotNull(inv);
            Assert.AreEqual(Command.Inv, inv.Command);
            Assert.AreEqual(message.Inventories.Count, inv.Inventories.Count);
        }

        [TestMethod]
        public void When_Decode_Inv_one_Then_nothing_To_Encode() {

            var message = new Inv {
                Inventories = new List<InventoryVector>
                {
                    new InventoryVector
                    {
                        ObjectType = ObjectType.MsgTx,
                        Hash = "hash"
                    },
                    new InventoryVector
                    {
                        ObjectType = ObjectType.Error,
                        Hash = "error"
                    }
                }
            };
            using var mem = new MemoryStream();
            using (var writer = new ProtocolWriter(mem))
            {
                writer.Write(message);
            }

            var logger = new Logger();
            using var mem2 = new MemoryStream(mem.ToArray());
            using var reader = new PayloadReader(logger, mem2);



var inv = reader.ReadInv();
            Assert.IsNotNull(inv);
            Assert.AreEqual(Command.Inv, inv.Command);
            Assert.AreEqual(message.Inventories.Count, inv.Inventories.Count);
            Assert.AreEqual(message.Inventories[0].ObjectType, inv.Inventories[0].ObjectType);
            Assert.AreEqual(message.Inventories[0].Hash, inv.Inventories[0].Hash);
            Assert.AreEqual(message.Inventories[1].ObjectType, inv.Inventories[1].ObjectType);
            Assert.AreEqual(message.Inventories[1].Hash, inv.Inventories[1].Hash);
        }
    }
}