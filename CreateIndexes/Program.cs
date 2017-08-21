using CommandLine;
using Common;
using Raven.Client.Indexes;

namespace CreateIndexes
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArgumentsStrict(args, options);
            using (var store = Util.GetStore(options))
                IndexCreation.CreateIndexes(typeof(AllNames).Assembly, store);
        }
    }
}
