using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Raven.Abstractions.Data;
using Common;
using Raven.Client.Document;

namespace Import
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ImportOptions();
            Parser.Default.ParseArgumentsStrict(args, options);
            var recordCount = 0;
            var invalidRecords = 0;
            const int batchSize = 10000;
            using (var store = Util.GetStore(options))
            {
                using (var batch = store.BulkInsert(options: new BulkInsertOptions {BatchSize = batchSize}))
                using (var reader = new StreamReader(options.File))
                {
                    reader.ReadLine(); //Skip header
                    var stopwatch = Stopwatch.StartNew();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ++recordCount;
                        try
                        {
                            if (recordCount % batchSize == 0)
                            {
                                var elapsed = stopwatch.Elapsed.TotalSeconds;
                                Console.WriteLine($"Processed {recordCount} records in {elapsed:N0}s ({recordCount/elapsed:N0} records/second)");
                            }
                            Process(line, batch);
                        }
                        catch (Exception ex)
                        {
                            ++invalidRecords;
                            Console.WriteLine($"Invalid record '{line}'. {ex.Message}");
                        }
                    }
                }
            }
            Console.WriteLine($"Processed {recordCount} records, {invalidRecords} failed.");
        }

        static void Process(string line, BulkInsertOperation batch)
        {
            var parts = line.Trim().Split(',');
            var name = parts[0].Trim();
            var count = int.Parse(parts[1]);
            var year = int.Parse(parts[2]);
            batch.Store(new Record
            {
                Name = name,
                Count = count,
                Year = year,
            });
        }
    }
}
