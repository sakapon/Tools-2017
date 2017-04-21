using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogPicker
{
    class Program
    {
        const string LogName_System = "System";
        static readonly HashSet<long> InstanceIds = new HashSet<long> { 7001, 7002 };
        const int SpanMonths = 3;

        static readonly string[] ColumnNames = new[] { "レベル", "日付と時刻", "ソース", "イベント ID", "タスクのカテゴリ" };

        static int Main(string[] args)
        {
            if (!EventLog.Exists(LogName_System))
            {
                Console.WriteLine("The \"System\" event log is not found.");
                return 101;
            }

            var entries = GetEventLogEntries();

            var fileName = $"{Environment.MachineName}-{DateTime.Now:yyyyMMdd}.csv";
            CsvFile.WriteRecordsByArray(fileName, entries.Select(ToColumnValues), ColumnNames, Encoding.UTF8);

            return 0;
        }

        static EventLogEntry[] GetEventLogEntries()
        {
            var startDate = DateTime.Now.Date.AddMonths(-SpanMonths);

            using (var el = new EventLog(LogName_System))
            {
                return el.Entries.Cast<EventLogEntry>()
                    .Where(e => InstanceIds.Contains(e.InstanceId))
                    .SkipWhile(e => e.TimeGenerated < startDate)
                    .Reverse()
                    .ToArray();
            }
        }

        static string[] ToColumnValues(EventLogEntry e) =>
            new[]
            {
                ToString(e.EntryType),
                e.TimeGenerated.ToString(),
                e.Source,
                e.InstanceId.ToString(),
                e.Category,
                e.Message
            };

        static string ToString(EventLogEntryType value)
        {
            switch (value)
            {
                case EventLogEntryType.Error:
                    return "エラー";
                case EventLogEntryType.Warning:
                    return "警告";
                case EventLogEntryType.Information:
                    return "情報";
                case EventLogEntryType.SuccessAudit:
                    return "成功の監査";
                case EventLogEntryType.FailureAudit:
                    return "失敗の監査";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
