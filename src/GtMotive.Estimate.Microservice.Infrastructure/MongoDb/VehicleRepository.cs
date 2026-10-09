using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    public sealed class VehicleRepository : IVehicleRepository
    {
        private const string CollectionName = "vehicles";

        private readonly IMongoCollection<VehicleDocument> _collection;

        // Optimistic concurrency: version of each vehicle as it was loaded in this scope (one per request).
        private readonly Dictionary<Guid, long> _loadedVersions = [];

        public VehicleRepository(MongoService mongoService)
        {
            ArgumentNullException.ThrowIfNull(mongoService);

            _collection = mongoService.Database.GetCollection<VehicleDocument>(CollectionName);
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            try
            {
                await _collection.InsertOneAsync(ToDocument(vehicle, version: 0));
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new DuplicateLicensePlateException($"A vehicle with license plate {vehicle.LicensePlate} already exists.", ex);
            }

            _loadedVersions[vehicle.Id] = 0;
        }

        public async Task<Vehicle> GetByIdAsync(Guid id)
        {
            var document = await _collection.Find(d => d.Id == id).FirstOrDefaultAsync();
            return document is null ? null : Track(document);
        }

        public async Task<IReadOnlyCollection<Vehicle>> ListNotRentedAsync()
        {
            var documents = await _collection.Find(d => d.CurrentRental == null).ToListAsync();
            return documents.ConvertAll(Track);
        }

        public async Task<bool> HasActiveRentalAsync(CustomerId customerId)
        {
            ArgumentNullException.ThrowIfNull(customerId);

            return await _collection.Find(d => d.CurrentRental.CustomerId == customerId.Value).AnyAsync();
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (!_loadedVersions.TryGetValue(vehicle.Id, out var loadedVersion))
            {
                throw new InvalidOperationException($"The vehicle {vehicle.Id} must be loaded through this repository before it is updated.");
            }

            // Saved only if nobody changed the vehicle since it was loaded: two employees renting
            // or returning the same vehicle at the same time cannot overwrite each other.
            var filter = Builders<VehicleDocument>.Filter;
            var sameVersion = filter.Eq(d => d.Version, loadedVersion);
            if (loadedVersion == 0)
            {
                // Documents stored before versioning have no version field and are read as 0.
                sameVersion = filter.Or(sameVersion, filter.Exists(d => d.Version, exists: false));
            }

            ReplaceOneResult result;
            try
            {
                result = await _collection.ReplaceOneAsync(
                    filter.And(filter.Eq(d => d.Id, vehicle.Id), sameVersion),
                    ToDocument(vehicle, loadedVersion + 1));
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new CustomerAlreadyHasRentalException($"The customer {vehicle.CurrentRental?.CustomerId} already has a rented vehicle.", ex);
            }

            if (result.MatchedCount == 0)
            {
                throw new VehicleConcurrencyException($"The vehicle {vehicle.Id} was modified by another request. Reload it and try again.");
            }

            _loadedVersions[vehicle.Id] = loadedVersion + 1;
        }

        internal static void CreateIndexes(IMongoDatabase database)
        {
            // One active rental per customer, enforced by the database under concurrent requests.
            // $type (not $exists) so vehicles without a rental (null) are left out of the index.
            var oneRentalPerCustomer = new CreateIndexModel<VehicleDocument>(
                Builders<VehicleDocument>.IndexKeys.Ascending(d => d.CurrentRental.CustomerId),
                new CreateIndexOptions<VehicleDocument>
                {
                    Name = "ux_currentRental_customerId",
                    Unique = true,
                    PartialFilterExpression = Builders<VehicleDocument>.Filter.Type(d => d.CurrentRental.CustomerId, BsonType.String),
                });

            var uniqueLicensePlate = new CreateIndexModel<VehicleDocument>(
                Builders<VehicleDocument>.IndexKeys.Ascending(d => d.LicensePlate),
                new CreateIndexOptions { Name = "ux_licensePlate", Unique = true });

            database.GetCollection<VehicleDocument>(CollectionName)
                .Indexes.CreateMany([oneRentalPerCustomer, uniqueLicensePlate]);
        }

        private static VehicleDocument ToDocument(Vehicle vehicle, long version)
        {
            return new VehicleDocument
            {
                Id = vehicle.Id,
                Version = version,
                LicensePlate = vehicle.LicensePlate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                ManufactureDate = vehicle.ManufactureDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                CurrentRental = vehicle.CurrentRental is null
                    ? null
                    : new RentalDocument
                    {
                        CustomerId = vehicle.CurrentRental.CustomerId.Value,
                        RentedAt = vehicle.CurrentRental.RentedAt.UtcDateTime,
                    },
            };
        }

        private static Vehicle ToDomain(VehicleDocument document)
        {
            var rental = document.CurrentRental is null
                ? null
                : new Rental(new CustomerId(document.CurrentRental.CustomerId), new DateTimeOffset(document.CurrentRental.RentedAt, TimeSpan.Zero));

            return Vehicle.Restore(
                document.Id,
                document.LicensePlate,
                document.Brand,
                document.Model,
                DateOnly.FromDateTime(document.ManufactureDate),
                rental);
        }

        private Vehicle Track(VehicleDocument document)
        {
            _loadedVersions[document.Id] = document.Version;
            return ToDomain(document);
        }

        private sealed class VehicleDocument
        {
            [BsonId]
            public Guid Id { get; set; }

            [BsonElement("version")]
            public long Version { get; set; }

            [BsonElement("licensePlate")]
            public string LicensePlate { get; set; }

            [BsonElement("brand")]
            public string Brand { get; set; }

            [BsonElement("model")]
            public string Model { get; set; }

            [BsonElement("manufactureDate")]
            public DateTime ManufactureDate { get; set; }

            [BsonElement("currentRental")]
            public RentalDocument CurrentRental { get; set; }
        }

        private sealed class RentalDocument
        {
            [BsonElement("customerId")]
            public string CustomerId { get; set; }

            [BsonElement("rentedAt")]
            public DateTime RentedAt { get; set; }
        }
    }
}
