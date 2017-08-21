using System.Linq;
using Raven.Client.Indexes;

namespace Common
{
    public class AllNames : AbstractIndexCreationTask<Record>
    {
        public AllNames()
        {
            Map = records =>
                from record in records
                from given in record.Name.Split(' ')
                where given.Length > 0
                select new
                {
                    Name = given.ToLower().Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u'),
                    record.Year,
                    record.Count
                };
            Reduce = results =>
                from result in results
                group result by new { result.Name, result.Year }
                into g
                select new
                {
                    g.Key.Name,
                    g.Key.Year,
                    Count = g.Sum(x => x.Count)
                };
        }
    }
}