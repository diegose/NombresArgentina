using CommandLine;

namespace Common
{
    public class Options
    {
        [Option('c', "connection", Required = true)]
        public string ConnectionString { get; set; }

        [Option('d', "database", Required = true)]
        public string Database { get; set; }
    }
}