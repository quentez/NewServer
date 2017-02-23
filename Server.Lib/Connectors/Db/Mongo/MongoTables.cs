﻿using System.Linq;
using MongoDB.Driver;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db.Mongo
{
    public class MongoTables : ITables
    {
        public MongoTables(IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            // Configure the Mongo connection.
            var clientSettings = new MongoClientSettings
            {
                Servers = configuration.MongoServers.Select(kv => new MongoServerAddress(kv.Key, kv.Value)),
                ApplicationName = "MainApp",
                WriteConcern = WriteConcern.WMajority,
                ReadConcern = ReadConcern.Majority,
                ReadPreference = ReadPreference.Nearest
            };

            // Create the client and get a reference to the Db.
            var client = new MongoClient(clientSettings);
            var database = client.GetDatabase(configuration.MongoDatabaseName);

            // Create references to our tables.
            this.Users = new VersionedMongoTable<CacheUser>(database.GetCollection<CacheUser>("users"));
        }

        public IVersionedTable<CacheUser> Users { get; }
    }
}