﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Util;

namespace Tests.Utils
{
    [TestClass]
    public class HexdumpTest
    {
        [TestMethod]
        public void When_Encode_null_Then_null()
        {
            byte[] data = null;
            var expected = @"<null>";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Encode_16_Then_ok()
        {
            var data = new byte[16];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var hex = new HexDump();
            var result = hex.Encode(data);


            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Encode_32_Then_ok()
        {
            var data = new byte[32];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void When_Encode_48_Then_ok()
        {
            var data = new byte[48];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000020   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void When_Encode_256_Then_ok()
        {
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
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void When_Encode_8_Then_ok()
        {
            var data = new byte[8];
            var expected = @"
00000000   00 00 00 00 00 00 00 00                            ········        
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Encode_0_Then_ok()
        {
            var data = new byte[0];
            var expected = @"
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void When_Encode_24_Then_ok()
        {
            var data = new byte[24];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00                            ········        
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Encode_40_Then_ok()
        {
            var data = new byte[40];
            var expected = @"
00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ················
00000020   00 00 00 00 00 00 00 00                            ········        
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Encode_40_one_Then_ok()
        {
            var data = Enumerable.Repeat((byte) 1, 40).ToArray();

            var expected = @"
00000000   01 01 01 01 01 01 01 01  01 01 01 01 01 01 01 01   ················
00000010   01 01 01 01 01 01 01 01  01 01 01 01 01 01 01 01   ················
00000020   01 01 01 01 01 01 01 01                            ········        
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Encode_40_61_Then_ok()
        {
            var data = Enumerable.Repeat((byte) 'a', 40).ToArray();

            var expected = @"
00000000   61 61 61 61 61 61 61 61  61 61 61 61 61 61 61 61   aaaaaaaaaaaaaaaa
00000010   61 61 61 61 61 61 61 61  61 61 61 61 61 61 61 61   aaaaaaaaaaaaaaaa
00000020   61 61 61 61 61 61 61 61                            aaaaaaaa        
";
            var hex = new HexDump();
            var result = hex.Encode(data);

            Assert.AreEqual(expected, result);
        }
    }
}