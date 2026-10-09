using System;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    public sealed class MongoService : IDisposable
    {
        public MongoService(IOptions<MongoDbSettings> options)
        {
            // Driver 3.x requires an explicit Guid representation.
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            MongoClient = new MongoClient(options.Value.ConnectionString);
            Database = MongoClient.GetDatabase(options.Value.MongoDbDatabaseName);

            // Runs once per process (singleton). If MongoDB is unreachable the constructor throws
            // and the container retries on the next resolution.
            VehicleRepository.CreateIndexes(Database);
        }

        public MongoClient MongoClient { get; }

        public IMongoDatabase Database { get; }

        public void Dispose()
        {
            MongoClient.Dispose();
        }
    }
}
