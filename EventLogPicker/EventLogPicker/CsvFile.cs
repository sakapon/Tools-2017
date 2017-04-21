using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EventLogConsole
{
    /// <summary>
    /// Provides a set of methods to access CSV files.
    /// </summary>
    /// <remarks>
    /// Source:
    /// https://github.com/sakapon/Bellona.Analysis/blob/master/Bellona/Analysis/IO/CsvFile.cs
    /// </remarks>
    public static class CsvFile
    {
        static readonly Regex QualifyingFieldPattern = new Regex("^.*[,\"].*$");

        public static string ToLine(IEnumerable<string> fields) => string.Join(",",
            fields
                .Select(f => f.Replace("\"", "\"\""))
                .Select(f => QualifyingFieldPattern.Replace(f, "\"$&\""))
        );

        static IEnumerable<string> WriteRecordsByArray(this IEnumerable<string[]> records, string[] columnNames)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));
            if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));

            return Enumerable.Repeat(columnNames, 1)
                .Concat(records)
                .Select(ToLine);
        }

        public static void WriteRecordsByArray(Stream stream, IEnumerable<string[]> records, string[] columnNames, Encoding encoding = null) =>
            TextFile.WriteLines(stream, records.WriteRecordsByArray(columnNames), encoding);

        public static void WriteRecordsByArray(string path, IEnumerable<string[]> records, string[] columnNames, Encoding encoding = null) =>
            TextFile.WriteLines(path, records.WriteRecordsByArray(columnNames), encoding);
    }

    public static class TextFile
    {
        public static readonly Encoding UTF8N = new UTF8Encoding();
        public static readonly Encoding ShiftJIS = Encoding.GetEncoding("shift_jis");

        public static IEnumerable<string> ReadLines(this Stream stream, Encoding encoding = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var reader = new StreamReader(stream, encoding ?? UTF8N))
            {
                while (!reader.EndOfStream)
                    yield return reader.ReadLine();
            }
        }

        public static IEnumerable<string> ReadLines(string path, Encoding encoding = null) =>
            File.ReadLines(path, encoding ?? UTF8N);

        public static void WriteLines(this Stream stream, IEnumerable<string> lines, Encoding encoding = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (lines == null) throw new ArgumentNullException(nameof(lines));

            using (var writer = new StreamWriter(stream, encoding ?? UTF8N))
            {
                foreach (var line in lines)
                    writer.WriteLine(line);
            }
        }

        public static void WriteLines(string path, IEnumerable<string> lines, Encoding encoding = null) =>
            File.WriteAllLines(path, lines, encoding ?? UTF8N);
    }
}
