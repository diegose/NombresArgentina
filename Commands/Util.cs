using System;
using Raven.Client;
using Raven.Client.Document;

namespace Commands
{
    public static class Util
    {
        public static IDocumentStore GetStore(Options options)
        {
            var uri = new Uri(options.ConnectionString);
            var store = new DocumentStore {Url = uri.ToString(), DefaultDatabase = options.Database };
            store.Initialize();
            store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(options.Database);
            return store;
        }
    }
}