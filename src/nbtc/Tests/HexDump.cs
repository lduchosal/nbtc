using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tests
{
    public class HexDump
    {
        private readonly Regex _re =
            new Regex(@"^(?<offset>[0-9a-f]+)\s+(?<hexa>[0-9a-f\s]{48,})\s{3,}(?<dump>.+)$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ReadOnlySpan<byte> Decode(string dump)
        {
            //00000000   01 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ................
            //00000000   01 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ................

            var result = new List<byte>();
            var lines = dump.Split(Environment.NewLine.ToCharArray());
            foreach (var line in lines)
            {
                var capture = _re.Match(line);
                if (!capture.Success) continue;
                var hexa = capture.Groups["hexa"]
                        .Value
                        .Replace(" ", "")
                    ;

                var bytes = Enumerable.Range(0, hexa.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hexa.Substring(x, 2), 16))
                    .ToArray();

                result.AddRange(bytes);
            }

            return result.ToArray();
        }

        public string Encode(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            var bytesLength = bytes.Length;

            var hexChars = "0123456789ABCDEF".ToCharArray();

            var firstHexColumn =
                8 // 8 characters for the address
                + 3; // 3 spaces

            var firstCharColumn = firstHexColumn
                                  + bytesPerLine * 3 // - 2 digit for the hexadecimal value and 1 space
                                  + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                                  + 2; // 2 spaces 

            var lineLength = firstCharColumn
                             + bytesPerLine // - characters to show the ascii value
                             + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            var line = (new string(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            var expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            var result = new StringBuilder(expectedLines * lineLength);
            result.AppendLine();
            for (var i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

                var hexColumn = firstHexColumn;
                var charColumn = firstCharColumn;

                for (var j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        var b = bytes[i + j];
                        line[hexColumn] = hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[b & 0xF];
                        line[charColumn] = b < 32 ? '·' : (char) b;
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                result.Append(line);
            }

            return result.ToString();
        }
    }
}