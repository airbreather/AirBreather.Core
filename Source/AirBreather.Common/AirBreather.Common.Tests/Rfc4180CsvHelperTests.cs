﻿using System;
using System.IO;
using System.Text;

using AirBreather.IO;

using Xunit;

namespace AirBreather.Tests
{
    public sealed class Rfc4180CsvHelperTests
    {
        [Fact]
        public void TestCsvReading()
        {
            const string CSVText = "Hello,\"Wor\"\"ld\"\r\nhow,\"are,\",you\n\r\n\n\r\n\r\r\n,doing,\"to\"\"\"\"d\"\"ay\",\rI,am,,,,fine\r,,,\nasdf";
            var bytes = Encoding.UTF8.GetBytes(CSVText);
            string[][] expectedRows =
            {
                new[] { "Hello", "Wor\"ld" },
                new[] { "how", "are,", "you" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                new[] { string.Empty, "doing", "to\"\"d\"ay", string.Empty },
                new[] { "I", "am", string.Empty, string.Empty, string.Empty, "fine" },
                new[] { string.Empty, string.Empty, string.Empty, string.Empty },
                new[] { "asdf" },
            };
            using (var stream = new MemoryStream(bytes))
            {
                int i = 0;
                Rfc4180CsvHelper.ReadUtf8CsvFile(stream, row => Do(row, expectedRows[i++]));
            }

            void Do(Rfc4180CsvRow fields, string[] expected)
            {
                string[] actual = new string[fields.FieldCount];
                for (int i = 0; i < actual.Length; i++)
                {
                    actual[i] = Encoding.UTF8.GetString(fields[i]);
                }

                Assert.Equal(expected, actual);

                int j = 0;
                foreach (var field in fields)
                {
                    actual[j++] = Encoding.UTF8.GetString(field);
                }

                Assert.Equal(expected, actual);
            }
        }
    }
}
