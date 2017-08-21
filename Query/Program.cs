using System;
using System.Linq;
using CommandLine;
using Common;

namespace Query
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new QueryOptions();
            Parser.Default.ParseArgumentsStrict(args, options);
            using (var store = Util.GetStore(options))
            using (var session = store.OpenSession())
            {
                var records = session.Query<Record, AllNames>()
                    .Where(x => x.Name == options.Name)
                    .ToDictionary(x => x.Year, x => x.Count);
                if (!records.Any())
                    return;
                var births = session.Query<BirthsPerYear.Result, BirthsPerYear>()
                    .ToDictionary(x => x.Year, x => x.Count);
                var range = Enumerable.Range(1922, 2015 - 1922 + 1);
                var maxPopularity = range.Aggregate(0M,
                    (currentMax, year) => Math.Max(currentMax,
                        GetPopularity(records.GetValueOrDefault(year), births.GetValueOrDefault(year))));
                foreach (var year in range) //DB has those years
                {
                    var nameCount = records.GetValueOrDefault(year);
                    var birthCount = births.GetValueOrDefault(year);
                    var popularity = GetPopularity(nameCount, birthCount);
                    if (popularity == maxPopularity)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{year}: {nameCount,6:N0} — {popularity,7:P2} de {birthCount,9:N0}");
                    Console.ResetColor();
                }
            }
        }

        static decimal GetPopularity(int nameCount, int birthCount)
        {
            return nameCount > 0 && birthCount > 0
                ? nameCount / (decimal) birthCount
                : 0;
        }
    }
}
