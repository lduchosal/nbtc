using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{

    [TestClass]
    public class HexdumpTest
    {
        [TestMethod]
        public void when_encode_16_then_ok()
        {

            var data = new byte[16];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var result = HexDump.Encode(data);
            var e2 = HexDump.Decode(result);


            Assert.AreEqual(expected, result);;
        }
        [TestMethod]
        public void when_encode_32_then_ok() {

            var data = new byte[32];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);;
        }


        [TestMethod]
        public void when_encode_48_then_ok() {

            var data = new byte[48];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000020   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void when_encode_256_then_ok() {

            var data = new byte[256];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000020   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000030   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000040   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000050   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000060   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000070   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000080   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000090   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
000000A0   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
000000B0   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
000000C0   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
000000D0   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
000000E0   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
000000F0   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void when_encode_8_then_ok() {

            var data = new byte[8];
            var expected = @"
00000000   00 00 00 00 00 00 00 00                            ········        
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void when_encode_0_then_ok() {

            var data = new byte[0];
            var expected = @"
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void when_encode_24_then_ok() {

            var data = new byte[24];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00                            ········        
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void when_encode_40_then_ok() {

            var data = new byte[40];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000020   00 00 00 00 00 00 00 00                            ········        
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void when_encode_40_one_then_ok() {

            var data = Enumerable.Repeat((byte)1, 40).ToArray();

            var expected = @"
00000000   01 01 01 01 01 01 01 01  01 01 01 01 01 01 01 01   ················
00000010   01 01 01 01 01 01 01 01  01 01 01 01 01 01 01 01   ················
00000020   01 01 01 01 01 01 01 01                            ········        
";
            var result = HexDump.Encode(data);
            
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void when_encode_40_61_then_ok() {

            var data = Enumerable.Repeat((byte)'a', 40).ToArray();
            
            var expected = @"
00000000   61 61 61 61 61 61 61 61  61 61 61 61 61 61 61 61   aaaaaaaaaaaaaaaa
00000010   61 61 61 61 61 61 61 61  61 61 61 61 61 61 61 61   aaaaaaaaaaaaaaaa
00000020   61 61 61 61 61 61 61 61                            aaaaaaaa        
";
            var result = HexDump.Encode(data);

            Assert.AreEqual(expected, result);
        }

    }
}
