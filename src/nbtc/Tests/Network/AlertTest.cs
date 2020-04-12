using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbtc.Network;
using Nbtc.Serialization;
using Nbtc.Util;

namespace Tests.Network
{
    [TestClass]
    public class AlertTest
    {

        [TestMethod]
        public void When_Encode_alert_Then_nothing_To_Encode()
        {

            var hex = new HexDump();

            var alerts = new byte [0];
            var alert = new Alert
            {
                Data  = alerts
            };

            var data = new byte[] {0};
            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(alert);
                }
                var expected = hex.Encode(data);
                var result = hex.Encode(mem.ToArray());
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Encode_one_alert_Then_Encode()
        {

            var bytes = @"
00000000   10 54 68 69 73 20 69 73  20 61 6E 20 61 6C 65 72   ·This is an aler
00000010   74                                                 t               
";
            var hex = new HexDump();
            var data = hex.Decode(bytes).ToArray();

            var alerts = Encoding.UTF8.GetBytes("This is an alert");
            var alert = new Alert
            {
                Data  = alerts
            };

            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(alert);
                }
                var expected = hex.Encode(data);
                var result = hex.Encode(mem.ToArray());
                Assert.AreEqual(expected, result);
            }
        }

        
        [TestMethod]
        public void When_Encode_big_alert_Then_Encode()
        {

            var bytes = @"
00000000   FD B3 05 0A 31 32 33 34  35 36 37 38 39 30 31 32   ý³··123456789012
00000010   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
00000020   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
00000030   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000040   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
00000050   37 38 39 30 0A 31 32 33  34 35 36 37 38 39 30 31   7890·12345678901
00000060   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000070   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
00000080   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
00000090   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
000000A0   36 37 38 39 30 0A 31 32  33 34 35 36 37 38 39 30   67890·1234567890
000000B0   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
000000C0   37 38 39 30 31 32 33 34  35 36 37 38 39 30 31 32   7890123456789012
000000D0   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
000000E0   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
000000F0   35 36 37 38 39 30 0A 31  32 33 34 35 36 37 38 39   567890·123456789
00000100   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
00000110   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 31   6789012345678901
00000120   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000130   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
00000140   34 35 36 37 38 39 30 0A  31 32 33 34 35 36 37 38   4567890·12345678
00000150   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
00000160   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000170   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
00000180   37 38 39 30 31 32 33 34  35 36 37 38 39 30 31 32   7890123456789012
00000190   33 34 35 36 37 38 39 30  0A 31 32 33 34 35 36 37   34567890·1234567
000001A0   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
000001B0   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
000001C0   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
000001D0   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 31   6789012345678901
000001E0   32 33 34 35 36 37 38 39  30 0A 31 32 33 34 35 36   234567890·123456
000001F0   37 38 39 30 31 32 33 34  35 36 37 38 39 30 31 32   7890123456789012
00000200   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
00000210   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
00000220   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000230   31 32 33 34 35 36 37 38  39 30 0A 31 32 33 34 35   1234567890·12345
00000240   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 31   6789012345678901
00000250   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000260   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
00000270   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
00000280   30 31 32 33 34 35 36 37  38 39 30 0A 31 32 33 34   01234567890·1234
00000290   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
000002A0   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
000002B0   37 38 39 30 31 32 33 34  35 36 37 38 39 30 31 32   7890123456789012
000002C0   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
000002D0   39 30 31 32 33 34 35 36  37 38 39 30 0A 31 32 33   901234567890·123
000002E0   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
000002F0   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
00000300   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 31   6789012345678901
00000310   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000320   38 39 30 31 32 33 34 35  36 37 38 39 30 0A 31 32   8901234567890·12
00000330   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
00000340   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
00000350   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000360   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
00000370   37 38 39 30 31 32 33 34  35 36 37 38 39 30 0A 31   78901234567890·1
00000380   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000390   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
000003A0   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
000003B0   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
000003C0   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 0A   678901234567890·
000003D0   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
000003E0   37 38 39 30 31 32 33 34  35 36 37 38 39 30 31 32   7890123456789012
000003F0   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
00000400   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
00000410   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000420   0A 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   ·123456789012345
00000430   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 31   6789012345678901
00000440   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000450   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
00000460   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
00000470   30 0A 31 32 33 34 35 36  37 38 39 30 31 32 33 34   0·12345678901234
00000480   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000490   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
000004A0   37 38 39 30 31 32 33 34  35 36 37 38 39 30 31 32   7890123456789012
000004B0   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
000004C0   39 30 0A 31 32 33 34 35  36 37 38 39 30 31 32 33   90·1234567890123
000004D0   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
000004E0   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
000004F0   36 37 38 39 30 31 32 33  34 35 36 37 38 39 30 31   6789012345678901
00000500   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000510   38 39 30 0A 31 32 33 34  35 36 37 38 39 30 31 32   890·123456789012
00000520   33 34 35 36 37 38 39 30  31 32 33 34 35 36 37 38   3456789012345678
00000530   39 30 31 32 33 34 35 36  37 38 39 30 31 32 33 34   9012345678901234
00000540   35 36 37 38 39 30 31 32  33 34 35 36 37 38 39 30   5678901234567890
00000550   31 32 33 34 35 36 37 38  39 30 31 32 33 34 35 36   1234567890123456
00000560   37 38 39 30 0A 31 32 33  34 35 36 37 38 39 30 31   7890·12345678901
00000570   32 33 34 35 36 37 38 39  30 31 32 33 34 35 36 37   2345678901234567
00000580   38 39 30 31 32 33 34 35  36 37 38 39 30 31 32 33   8901234567890123
00000590   34 35 36 37 38 39 30 31  32 33 34 35 36 37 38 39   4567890123456789
000005A0   30 31 32 33 34 35 36 37  38 39 30 31 32 33 34 35   0123456789012345
000005B0   36 37 38 39 30 0A                                  67890·          
";
            var hex = new HexDump();
            var data = hex.Decode(bytes).ToArray();

            var alerts = Encoding.UTF8.GetBytes(@"
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
12345678901234567890123456789012345678901234567890123456789012345678901234567890
");
            var alert = new Alert
            {
                Data  = alerts
            };

            using (var mem = new MemoryStream())
            {
                using (var writer = new ProtocolWriter(mem))
                {
                    writer.Write(alert);
                }
                var expected = hex.Encode(data);
                var result = hex.Encode(mem.ToArray());
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void When_Decode_alert_Then_nothing_To_Encode()
        {

            var data = new byte[] {0};
            var logger = new Logger();
            using var mem = new MemoryStream(data);
            using var reader = new PayloadReader(logger, mem);

            var result = reader.ReadAlert();
            var alerts = new byte [0];
            
            var expected = new Alert {
                    Data  = alerts
                }
                ;

            Assert.AreEqual(expected.Data.Length, result.Data.Length);
        }
    }
}