using System;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Common
{
    public static class Util
    {
        public static IDocumentStore GetStore(Options options)
        {
            var uri = new Uri(options.ConnectionString);
            var store = uri.Scheme == Uri.UriSchemeFile
                ? (IDocumentStore) new EmbeddableDocumentStore {DataDirectory = uri.LocalPath, DefaultDatabase = options.Database }
                : new DocumentStore {Url = uri.ToString(), DefaultDatabase = options.Database };
            store.Initialize();
            store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(options.Database);
            return store;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}