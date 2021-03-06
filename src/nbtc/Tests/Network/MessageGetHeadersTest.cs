﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Network.Payload;
using Nbtc.Serialization;
using Nbtc.Serialization.Message;
using Nbtc.Serialization.Payload;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class MessageGetHeadersTest
    {
        [TestMethod]
        public void When_Encode_Decode_Version_Message_Then_Message_Equal()
        {

            #region Addr data

            var dump = @"
0000    f9 be b4 d9 67 65 74 68    65 61 64 65 72 73 00 00    ????getheaders..
0000    05 04 00 00 50 84 4e 45                               ....P.NE        
0000    7f 11 01 00 1f 69 08 c8    30 f2 8c fb 91 f5 40 b3    .....i.?0?.?.?@?
0000    1d 6d d6 86 17 51 89 15    0a 4b 90 0b 00 00 00 00    .m?..Q...K......
0000    00 00 00 00 00 29 78 73    bc 4b e5 53 bf c0 f9 f4    .....)xs?K?S????
0000    e2 e9 47 fe a0 f9 39 dd    b3 55 3f 0e 00 00 00 00    ??G???9ݳU?.....
0000    00 00 00 00 00 48 bd 44    d7 8b bc 3e 83 5b 52 29    .....H?D?.?>.[R)
0000    33 15 2f d5 6c 6a ff 38    74 c4 08 0a 00 00 00 00    3./?lj?8t?......
0000    00 00 00 00 00 ac 11 88    21 ec c4 00 27 78 79 eb    .....?..!??.'xy?
0000    56 e3 52 d4 36 c6 1d 34    53 31 6b 00 00 00 00 00    V?R?6?.4S1k.....
0000    00 00 00 00 00 94 fa 0d    45 70 b7 60 68 af c3 1b    ......?.Ep?`h??.
0000    ff 8b 0b a1 3c 5c 96 49    b3 f0 1c 02 00 00 00 00    ?..?<\.I??......
0000    00 00 00 00 00 40 fa 64    77 f5 1c 50 85 4d 96 dc    .....@?dw?.P.M.?
0000    e7 70 2b 3e 4b 4a 1e 44    a3 8b ed 0b 00 00 00 00    ?p+>KJ.D?.?.....
0000    00 00 00 00 00 60 48 54    c8 62 40 40 52 82 f5 96    .....`HT?b@@R.?.
0000    85 cf 68 75 2a 83 6d 84    0a c0 14 12 00 00 00 00    .?hu*.m..?......
0000    00 00 00 00 00 12 f4 c6    69 3d 9f d6 28 ec 29 c2    ......??i=.?(?)?
0000    6b a8 ac c3 61 7e ba d0    6b df ed 11 00 00 00 00    k???a~??k??.....
0000    00 00 00 00 00 08 7b 8d    ad 7f 58 4f 0a 6c ed 16    ......{.?.XO.l?.
0000    f4 fe 3b b6 bf be f4 f3    93 24 7d 10 00 00 00 00    ??;?????.$}.....
0000    00 00 00 00 00 07 d1 6e    1d f2 07 4a 5f e8 c2 1a    ......?n.?.J_??.
0000    12 d4 28 ef 80 28 ea 3c    e6 35 40 03 00 00 00 00    .?(?.(?<?5@.....
0000    00 00 00 00 00 c4 b6 44    a5 54 89 39 26 39 52 7a    .....ĶD?T.9&9Rz
0000    3d 3f 8e 2c d7 36 8e 6f    9c a3 5d 0c 00 00 00 00    =?.,?6.o.?].....
0000    00 00 00 00 00 5a dd ff    38 45 70 67 8a 3c de 1b    .....Z??8Epg.<?.
0000    b6 ff 1b 79 aa 6c f7 59    1b 48 59 08 00 00 00 00    ??.y?l?Y.HY.....
0000    00 00 00 00 00 15 2f 4f    02 56 90 1c 77 d2 f2 eb    ....../O.V..w???
0000    dd 88 26 b2 4e ff 1d ec    08 57 54 02 00 00 00 00    ?.&?N?.?.WT.....
0000    00 00 00 00 00 3b ba f7    91 19 02 9f 37 65 48 ef    .....;??....7eH?
0000    ad d4 3f f7 ba c8 58 ee    fb 20 1c 07 00 00 00 00    ??????X?? ......
0000    00 00 00 00 00 a2 33 bd    ad 99 53 13 1b 63 a8 dd    .....?3??.S..c??
0000    26 24 92 c6 f7 bd 90 7d    fc 53 7a 12 00 00 00 00    &$.???.}?Sz.....
0000    00 00 00 00 00 8c e0 77    a9 93 5a 53 45 63 47 e5    ......?w?.ZSEcG?
0000    32 c6 2f 2e 6f ad 8c 9d    be ae ac 12 00 00 00 00    2?/.o?..???.....
0000    00 00 00 00 00 db 5d e0    d0 8c aa 5e 45 a2 19 34    .....?]??.?^E?.4
0000    e9 d4 94 ef 14 77 48 1e    6c c3 59 01 00 00 00 00    ??.?.wH.l?Y.....
0000    00 00 00 00 00 e7 f9 f9    d5 ec 51 9c f0 96 7c f0    .....?????Q.?.|?
0000    3a f3 9c fb 25 a1 c8 b6    69 4f 76 05 00 00 00 00    :?.?%?ȶiOv.....
0000    00 00 00 00 00 5d 6a 93    80 c9 20 43 8c 5a 59 ff    .....]j..? C.ZY?
0000    b2 40 a4 33 60 90 e3 de    d5 0b 1f 13 00 00 00 00    ?@?3`.???.......
0000    00 00 00 00 00 be 0a 9b    0a 0b 35 7b 80 90 20 bd    .....?....5{.. ?
0000    6d 61 b2 1c 1d a0 6b de    d7 69 37 0e 00 00 00 00    ma?..?k??i7.....
0000    00 00 00 00 00 20 2f 93    25 e8 a5 1b 8b 50 4d 55    ..... /.%?..PMU
0000    04 b9 7c 52 e8 2b 69 e3    3b 15 91 06 00 00 00 00    .?|R?+i?;.......
0000    00 00 00 00 00 4e af 96    2e 07 37 c2 6c 8c 22 0b    .....N?...7?l...
0000    62 9d ea f1 f6 55 cd 74    e7 21 0c 02 00 00 00 00    b.???U?t?!......
0000    00 00 00 00 00 c9 45 22    8a 25 ad 53 b1 bc 3d b3    .....?E..%?S??=?
0000    ba f5 eb 94 9e ac 24 b8    63 65 aa 05 00 00 00 00    ???..?$?ce?.....
0000    00 00 00 00 00 a3 d8 5c    e3 ca a9 8d fd 3a fc d4    .....??\?ʩ.?:??
0000    3c bf ef 0b 76 bb d6 2c    8e a6 72 09 00 00 00 00    <??.v??,.?r.....
0000    00 00 00 00 00 c4 7f 79    4d 6f 05 a2 36 e7 5f 00    .....?.yMo.?6?_.
0000    a2 1d fb ec 3a 4e 06 f0    41 d9 ca 09 00 00 00 00    ?.??:N.?A??.....
0000    00 00 00 00 00 2f 4c 8b    19 53 ad dd 0f e7 45 91    ...../L..S??.?E.
0000    5e 43 4c 8b 5b 62 77 f4    96 38 5e 10 00 00 00 00    ^CL.[bw?.8^.....
0000    00 00 00 00 00 c1 28 bb    a4 21 94 78 92 4b 9c 46    .....?(??!.x.K.F
0000    11 b4 43 bc 3d 2e 5c 4f    9a 48 46 15 00 00 00 00    .?C?=.\O.HF.....
0000    00 00 00 00 00 3e ee 36    7a da 1a e0 d4 30 a6 12    .....>?6z?.??0?.
0000    1c 7f 8a 5e e3 be 36 9a    7d b1 c6 11 00 00 00 00    ...^?6.}??.....
0000    00 00 00 00 00 02 9a 6f    0b 0e 4c 26 bd 4b 4c 95    .......o..L&?KL.
0000    7d 94 15 b2 56 e2 3b fc    08 da e3 ec 08 00 00 00    }..?V?;?.???....
0000    00 00 00 00 00 00 c8 bf    e8 b3 de a9 a6 64 88 e2    ......ȿ?ީ?d.?
0000    0c db ac c3 ac bb 74 29    45 d1 43 15 56 4b 16 01    .۬ì?t)E?C.VK..
0000    00 00 00 00 00 6f e2 8c    0a b6 f1 b3 72 c1 a6 a2    .....o?..??r???
0000    46 ae 63 f7 4f 93 1e 83    65 e1 5a 08 9c 68 d6 19    F?c?O...e?Z..h?.
0000    00 00 00 00 00 00 00 00    00 00 00 00 00 00 00 00    ................
0000    00 00 00 00 00 00 00 00    00 00 00 00 00 00 00 00    ................
0000    00 00 00 00 00                                        .....           
";

            #endregion

            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();

            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);

            var message = reader.ReadMessage();
            var getheaders = message.Payload as GetHeaders;

            Assert.AreEqual(Command.GetHeaders, message.Payload.Command);
            Assert.IsNotNull(getheaders);
            Assert.AreEqual((UInt32) 70015, getheaders.version);

        }

        [TestMethod]
        public void When_Decode_getheaders_valid_dump_Then_Decode_ok()
        {

            var dump = @"
00000000   f9 be b4 d9 67 65 74 68  65 61 64 65 72 73 00 00   main.getheaders.
00000010   65 00 00 00 8A 17 D6 CB  71 11 01 00 02 10 10 10   siz.has.ver.c.bl
00000020   10 11 11 11 11 12 12 12  12 13 13 13 13 14 14 14   ock1.block1.bloc
00000030   14 15 15 15 15 16 16 16  16 00 00 00 00 20 20 20   block1.block1..b
00000040   20 21 21 21 21 22 22 22  22 23 23 23 23 24 24 24   lock2.block2.blo
00000050   24 25 25 25 25 26 26 26  26 00 00 00 00 30 30 30   ck2.block2.blo.s
00000060   30 31 31 31 31 32 32 32  32 33 33 33 33 34 34 34   top.stop.stop.st
00000070   34 35 35 35 35 36 36 36  36 00 00 00 00            top.stop.sto
"; //  0 x 8A 17 D6 CB   

            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();

            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new MessageReader(logger, mem, state);

            var message = reader.ReadMessage();
            var getheaders = message.Payload as GetHeaders;

            Assert.IsNotNull(getheaders);
            Assert.AreEqual((UInt32)70001, getheaders.version);
            Assert.AreEqual(0x02, getheaders.Locators.Count);

            var locator1 = getheaders.Locators[0];
            var hash1 = new byte[]
            {
                0x10, 0x10, 0x10, 0x10, 0x11, 0x11, 0x11, 0x11, 0x12, 0x12, 0x12, 0x12, 0x13, 0x13, 0x13, 0x13,
                0x14, 0x14, 0x14, 0x14, 0x15, 0x15, 0x15, 0x15, 0x16, 0x16, 0x16, 0x16, 0x00, 0x00, 0x00, 0x00,
            };
            var slocator1 = hex.Encode(locator1.Hash);
            var shahs1 = hex.Encode(hash1);
            Assert.AreEqual(shahs1, slocator1);
            

            var locator2 = getheaders.Locators[1];
            var hash2 = new byte[]
            {
                0x20, 0x20, 0x20, 0x20, 0x21, 0x21, 0x21, 0x21, 0x22, 0x22, 0x22, 0x22, 0x23, 0x23, 0x23, 0x23,
                0x24, 0x24, 0x24, 0x24, 0x25, 0x25, 0x25, 0x25, 0x26, 0x26, 0x26, 0x26, 0x00, 0x00, 0x00, 0x00,
            };
            
            var slocator2 = hex.Encode(locator1.Hash);
            var shahs2 = hex.Encode(hash1);
            Assert.AreEqual(shahs2, slocator2);

            var stop = getheaders.Stop;
            var hash3 = new byte[]
            {
                0x30, 0x30, 0x30, 0x30, 0x31, 0x31, 0x31, 0x31, 0x32, 0x32, 0x32, 0x32, 0x33, 0x33, 0x33, 0x33,
                0x34, 0x34, 0x34, 0x34, 0x35, 0x35, 0x35, 0x35, 0x36, 0x36, 0x36, 0x36, 0x00, 0x00, 0x00, 0x00,
            };
            
            var slocator3 = hex.Encode(stop.Hash);
            var shahs3 = hex.Encode(hash3);
            Assert.AreEqual(shahs3, slocator3);

        }

        [TestMethod]
        public void When_Decode_Encode_getheaders_Then_same()
        {

            var dump = @"
00000000                            71 11 01 00 02 10 10 10           ver.c.bl
00000010   10 11 11 11 11 12 12 12  12 13 13 13 13 14 14 14   ock1.block1.bloc
00000020   14 15 15 15 15 16 16 16  16 00 00 00 00 20 20 20   block1.block1..b
00000030   20 21 21 21 21 22 22 22  22 23 23 23 23 24 24 24   lock2.block2.blo
00000040   24 25 25 25 25 26 26 26  26 00 00 00 00 30 30 30   ck2.block2.blo.s
00000050   30 31 31 31 31 32 32 32  32 33 33 33 33 34 34 34   top.stop.stop.st
00000060   34 35 35 35 35 36 36 36  36 00 00 00 00            top.stop.sto
";

            var hex = new HexDump();
            var original = hex.Decode(dump);
            var state = new MessageStateMachine();

            var logger = new Logger();
            using var mem = new MemoryStream(original.ToArray());
            using var reader = new PayloadReader(logger, mem, true);

            var getheaders = reader.ReadGetHeaders();

            Assert.IsNotNull(getheaders);


            using (var mem2 = new MemoryStream())
            {
                using (var protocol = new ProtocolWriter(mem2))
                {
                    protocol.Write(getheaders);
                }

                var aoriginasl = hex.Encode(original.ToArray());
                var result = hex.Encode(mem2.ToArray());

                Assert.AreEqual(aoriginasl, result);
            }
        }

        [TestMethod]
        public void When_Encode_getheaders_Then_same()
        {

            var dump = @"
00000000                            71 11 01 00 02 10 10 10           ver.c.bl
00000010   10 11 11 11 11 12 12 12  12 13 13 13 13 14 14 14   ock1.block1.bloc
00000020   14 15 15 15 15 16 16 16  16 00 00 00 00 20 20 20   block1.block1..b
00000030   20 21 21 21 21 22 22 22  22 23 23 23 23 24 24 24   lock2.block2.blo
00000040   24 25 25 25 25 26 26 26  26 00 00 00 00 30 30 30   ck2.block2.blo.s
00000050   30 31 31 31 31 32 32 32  32 33 33 33 33 34 34 34   top.stop.stop.st
00000060   34 35 35 35 35 36 36 36  36 00 00 00 00            top.stop.sto
";

            var hex = new HexDump();
            var original = hex.Decode(dump);

            var getheaders = new GetHeaders
            {
                version = 70001,
                Locators = new List<Sha256>
                {
                    new Sha256
                    {
                        Hash = new byte[]
                        {
                            0x10, 0x10, 0x10, 0x10, 0x11, 0x11, 0x11, 0x11,
                            0x12, 0x12, 0x12, 0x12, 0x13, 0x13, 0x13, 0x13,
                            0x14, 0x14, 0x14, 0x14, 0x15, 0x15, 0x15, 0x15,
                            0x16, 0x16, 0x16, 0x16, 0x00, 0x00, 0x00, 0x00
                        }
                    },
                    new Sha256
                    {
                        Hash = new byte[]
                        {
                            0x20, 0x20, 0x20, 0x20, 0x21, 0x21, 0x21, 0x21,
                            0x22, 0x22, 0x22, 0x22, 0x23, 0x23, 0x23, 0x23,
                            0x24, 0x24, 0x24, 0x24, 0x25, 0x25, 0x25, 0x25,
                            0x26, 0x26, 0x26, 0x26, 0x00, 0x00, 0x00, 0x00
                        }
                    }
                },
                Stop = new Sha256
                {
                    Hash = new byte[]
                    {
                        0x30, 0x30, 0x30, 0x30, 0x31, 0x31, 0x31, 0x31,
                        0x32, 0x32, 0x32, 0x32, 0x33, 0x33, 0x33, 0x33,
                        0x34, 0x34, 0x34, 0x34, 0x35, 0x35, 0x35, 0x35,
                        0x36, 0x36, 0x36, 0x36, 0x00, 0x00, 0x00, 0x00
                    }
                }
            };

            using (var mem2 = new MemoryStream())
            {
                using (var protocol = new ProtocolWriter(mem2))
                    protocol.Write(getheaders);

                var aoriginasl = hex.Encode(original.ToArray());
                var result = hex.Encode(mem2.ToArray());

                Assert.AreEqual(aoriginasl, result);
            }
        }
    }
}
