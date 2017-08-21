using System.Linq;
using Raven.Client.Indexes;

namespace Common
{
    public class BirthsPerYear : AbstractIndexCreationTask<Record, BirthsPerYear.Result>
    {
        public class Result
        {
            public int Year { get; set; }
            public int Count { get; set; }
        }

        public BirthsPerYear()
        {
            Map = records =>
                from record in records
                select new
                {
                    record.Year,
                    record.Count
                };
            Reduce = results =>
                from result in results
                group result by new { result.Year }
                into g
                select new
                {
                    g.Key.Year,
                    Count = g.Sum(x => x.Count)
                };
        }
    }
}