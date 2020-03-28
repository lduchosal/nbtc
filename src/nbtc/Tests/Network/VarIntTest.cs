using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;

namespace Tests.Network
{
    [TestClass]
    public class VarIntTest
    {

        [TestMethod]
        public void When_Decode_VarInt_0x00_Then_1_byte()
        {

            var data = new byte[] {0x00, 0x00, 0x00, 0x00};

            using var mem = new MemoryStream(data);
            using (var reader = new ProtocolReader(mem)) {
                var varint = reader.ReadVarInt();
                var result = varint.Value;
                Assert.AreEqual(result,(UInt64)0x00);

            }
        }

        [TestMethod]
        public void When_Decode_VarInt_0xfc_Then_1_byte()
        {

            var data = new byte[] {0xfc, 0x00, 0x00, 0x00 };
            using var mem = new MemoryStream(data);
            using (var reader = new ProtocolReader(mem))
            {
                var varint = reader.ReadVarInt();
                var result = varint.Value;

                Assert.AreEqual(result, (UInt64)0xfc);
            }
        }

        [TestMethod]
        public void When_Decode_VarInt_0xfd_Then_3_byte()
        {

            var data = new byte[] {0xfd, 0xfe, 0x00, 0x00};
            using var mem = new MemoryStream(data);
            using (var reader = new ProtocolReader(mem))
            {
                var varint = reader.ReadVarInt();
                var result = varint.Value;

                Assert.AreEqual(result, (UInt64)0x00fe);
            }
        }

        [TestMethod]
        public void When_Decode_VarInt_0xfd_fe_01_Then_3_byte()
        {

            var data = new byte[] {0xfd, 0xfe, 0x01, 0x00};
            using var mem = new MemoryStream(data);
            using (var reader = new ProtocolReader(mem))
            {
                var varint = reader.ReadVarInt();
                var result = varint.Value;

                Assert.AreEqual(result, (UInt64)0x01fe);
            }
        }


        [TestMethod]
        public void When_Decode_VarInt_0xfe_Then_5_byte()
        {

            var data = new byte[] {0xfe, 0x03, 0x02, 0x01, 0x00};
            using var mem = new MemoryStream(data);
            using (var reader = new ProtocolReader(mem))
            {
                var varint = reader.ReadVarInt();
                var result = varint.Value;

                Assert.AreEqual(result, (UInt64)0x00010203);
            }
        }

        [TestMethod]
        public void When_Decode_VarInt_0xff_Then_9_byte()
        {

            var data = new byte[] {0xff, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00};
            using var mem = new MemoryStream(data);
            using (var reader = new ProtocolReader(mem))
            {
                var varint = reader.ReadVarInt();
                var result = varint.Value;

                Assert.AreEqual(result, (UInt64)0x0001020304050607);
            }
        }


        [TestMethod]
        public void When_Decode_VarInt_0xff_Too_Small_Then_Fail_ParseError_VarInt_ff()
        {

            var data = new byte[] {0xff, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02};
            using var mem = new MemoryStream(data);
            using var reader = new ProtocolReader(mem);

            Assert.ThrowsException<EndOfStreamException>(reader.ReadVarInt);

        }


        [TestMethod]
        public void When_Decode_VarInt_0xfe_Too_Small_Then_Fail_ParseError_VarInt_fe()
        {
            var data = new byte[] {0xfe, 0x07, 0x06, 0x05};
            using var mem = new MemoryStream(data);
            using var reader = new ProtocolReader(mem);
            Assert.ThrowsException<EndOfStreamException>(reader.ReadVarInt);

        }

        [TestMethod]
        public void When_Decode_VarInt_0xfd_Too_Small_Then_Fail_ParseError_VarInt_fd()
        {
            var data = new byte[] {0xfd, 0x07};
            using var mem = new MemoryStream(data);
            using var reader = new ProtocolReader(mem);
            Assert.ThrowsException<EndOfStreamException>(reader.ReadVarInt);
        }

        [TestMethod]
        public void When_Encode_VarInt_0xffffffffffffffff_Then_Size_9()
        {

            var varint = new VarInt { Value = 0xFFFFFFFFFFFFFFFF };
            using (var mem = new MemoryStream()) 
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(varint);
                }

                var hex = new HexDump();
                var aresult = mem.ToArray();
                var result = hex.Encode(aresult);

                var aexpected = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
                var expected = hex.Encode(aexpected);
                
                Assert.AreEqual(expected, result);
            }    
        }

        [TestMethod]
        public void When_Encode_VarInt_0xFFFFFF_Then_Size_5()
        {
            var varint = new VarInt { Value = 0xFFFFFFFF };
            using (var mem = new MemoryStream(new byte[5])) 
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(varint);
                }


                var hex = new HexDump();
                var aresult = mem.ToArray();
                var result = hex.Encode(aresult);

                var aexpected = new byte[] {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};
                var expected = hex.Encode(aexpected);

                Assert.AreEqual(expected, result);
            }    
        }

        [TestMethod]
        public void When_Encode_VarInt_0xffff_Then_Size_3()
        {

            var varint = new VarInt { Value = 0xFFFF };
            using (var mem = new MemoryStream(3))
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(varint);
                }

                var hex = new HexDump();
                var aresult = mem.ToArray();
                var result = hex.Encode(aresult);

                var aexpected = new byte[] {0xFD, 0xFF, 0xFF};
                var expected = hex.Encode(aexpected);
                
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Encode_VarInt_0xfd_Then_Size_3()
        {

            var varint = new VarInt { Value = 0xFD };
            using (var mem = new MemoryStream(3))
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(varint);
                }

                var hex = new HexDump();
                var aresult = mem.ToArray();
                var result = hex.Encode(aresult);

                var aexpected = new byte[] {0xFD, 0xFD, 0x00};
                var expected = hex.Encode(aexpected);
                
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Encode_VarInt_0xf0_Then_Size_1()
        {

            var varint = new VarInt { Value = 0xF0 };
            using (var mem = new MemoryStream(1))
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(varint);
                }

                var hex = new HexDump();
                var aresult = mem.ToArray();
                var result = hex.Encode(aresult);

                var aexpected = new byte[] {0xF0};
                var expected = hex.Encode(aexpected);
                
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Encode_VarInt_0xfc_Then_Size_1()
        {

            var varint = new VarInt { Value = 0xFC };
            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(varint);
                }

                var hex = new HexDump();
                var aresult = mem.ToArray();
                var result = hex.Encode(aresult);

                var aexpected = new byte[] {0xFC};
                var expected = hex.Encode(aexpected);
                
                Assert.AreEqual(expected, result);
            }
        }
    }
}
