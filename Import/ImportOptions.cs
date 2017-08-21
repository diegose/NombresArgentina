using CommandLine;
using Common;

namespace Import
{
    public class ImportOptions : Options
    {
        [ValueOption(0)]
        public string File { get; set; }
    }
}