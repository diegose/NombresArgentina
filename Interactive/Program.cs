using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Interactive
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = Load(args[0]);
            var range = Enumerable.Range(1922, 2015 - 1922 + 1);
            while (true)
            {
                Console.Write("Name: ");
                var name = Console.ReadLine()?.Normalize();
                if (string.IsNullOrWhiteSpace(name))
                    return;
                var byName = data.NamesByYear.GetValueOrDefault(name);
                if (byName == null)
                    Console.WriteLine("Not found");
                else
                {
                    //var maxPopularity = range.Aggregate(0M,
                    //    (currentMax, year) => Math.Max(currentMax,
                    //        GetPopularity(data.NamesByYear.GetValueOrDefault(name)?.GetValueOrDefault(year) ?? 0,
                    //            data.PeopleByYear.GetValueOrDefault(year))));
                    //foreach (var year in range) //DB has those years
                    //{
                    //    var nameCount = byName.GetValueOrDefault(year);
                    //    var birthCount = data.PeopleByYear.GetValueOrDefault(year);
                    //    var popularity = GetPopularity(nameCount, birthCount);
                    //    if (popularity == maxPopularity)
                    //        Console.ForegroundColor = ConsoleColor.Yellow;
                    //    Console.WriteLine($"{year}: {nameCount,6:N0} — {popularity,7:P2} de {birthCount,9:N0}");
                    //    Console.ResetColor();
                    //}
                }
            }
        }

        static Data Load(string fileName)
        {
            var jsonFileName = fileName + ".json";
            if (File.Exists(jsonFileName))
                using (var reader = new StreamReader(jsonFileName))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return JsonSerializer.Create().Deserialize<Data>(jsonReader);
                }
            using (var reader = new StreamReader(fileName))
            using (var writer = new StreamWriter(jsonFileName))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var namesByYear = new Dictionary<string, Dictionary<int, int>>(10_000);
                var peopleByYear = new Dictionary<int, int>(100);
                reader.ReadLine(); //Skip header
                var stopwatch = Stopwatch.StartNew();
                string line;
                var recordCount = 0;
                var invalidRecords = 0;
                const int batchSize = 1_000_000;
                while ((line = reader.ReadLine()) != null)
                {
                    ++recordCount;
                    try
                    {
                        if (recordCount % batchSize == 0)
                        {
                            var elapsed = stopwatch.Elapsed.TotalSeconds;
                            Console.WriteLine($"Processed {recordCount} records in {elapsed:N0}s ({recordCount / elapsed:N0} records/second)");
                        }
                        var parts = line.Trim().Split(',');
                        var count = int.Parse(parts[1]);
                        var year = int.Parse(parts[2]);
                        var fullName = parts[0];
                        var names = fullName.Split(' ').Where(x => x.Length > 0).Select(Normalize);
                        peopleByYear[year] = peopleByYear.GetValueOrDefault(year) + count;
                        foreach (var name in names)
                        {
                            var byName = namesByYear.GetValueOrDefault(name);
                            if (byName == null)
                            {
                                byName = new Dictionary<int, int>();
                                namesByYear[name] = byName;
                            }
                            byName[year] = byName.GetValueOrDefault(year) + count;
                        }
                    }
                    catch (Exception ex)
                    {
                        ++invalidRecords;
                        Console.WriteLine($"Invalid record '{line}'. {ex.Message}");
                    }
                }
                Console.WriteLine($"Processed {recordCount} records, {invalidRecords} failed.");
                var data = new Data
                {
                    NamesByYear = namesByYear.ToDictionary(x => x.Key,
                        x => x.Value.Select(v => new[] {v.Key, v.Value}).ToArray()),
                    PeopleByYear = peopleByYear
                };
                JsonSerializer.Create().Serialize(jsonWriter, data);
                return data;
            }
        }

        private static string Normalize(string name) => name.ToLower().Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u');
        static decimal GetPopularity(int nameCount, int birthCount) => nameCount > 0 && birthCount > 0
            ? nameCount / (decimal)birthCount
            : 0;

        public class Data
        {
            public Dictionary<string, int[][]> NamesByYear { get; set; }
            public Dictionary<int, int> PeopleByYear { get; set; }
        }
    }
}
