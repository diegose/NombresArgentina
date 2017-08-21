using CommandLine;
using Common;

namespace Query
{
    public class QueryOptions : Options
    {
        [ValueOption(0)]
        public string Name { get; set; }
    }
}