using CommandLine;
using Commands;
using Common;

namespace Query
{
    public class QueryOptions : Options
    {
        [ValueOption(0)]
        public string Name { get; set; }
    }
}