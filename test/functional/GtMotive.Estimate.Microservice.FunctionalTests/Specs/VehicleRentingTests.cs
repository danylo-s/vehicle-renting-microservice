using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    public sealed class VehicleRentingTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        private const string CustomerId = "12345678Z";

        [Fact]
        public async Task CustomerCanOnlyHaveOneRentedVehicleAtATime()
        {
            var firstVehicleId = await CreateVehicle("1111AAA");
            var secondVehicleId = await CreateVehicle("2222BBB");

            var firstRent = await Rent(firstVehicleId, CustomerId);
            firstRent.Should().BeOfType<OkObjectResult>();

            var rentSecondWhileFirstIsRented = () => Rent(secondVehicleId, CustomerId);
            await rentSecondWhileFirstIsRented.Should().ThrowAsync<CustomerAlreadyHasRentalException>();

            var returnFirst = await Return(firstVehicleId);
            returnFirst.Should().BeOfType<OkObjectResult>();

            var secondRent = await Rent(secondVehicleId, CustomerId);
            secondRent.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<VehicleResponse>()
                .Which.RentedBy.Should().Be(CustomerId);
        }

        [Fact]
        public async Task SameVehicleRentedConcurrentlyForDifferentCustomersOnlyOneSucceeds()
        {
            var vehicleId = await CreateVehicle("3333CCC");

            var failures = await Task.WhenAll(
                Enumerable.Range(0, 10).Select(_ => TryRent(vehicleId, NewCustomerId())));

            failures.Count(f => f is null).Should().Be(1);
            failures.Where(f => f is not null).Should()
                .OnlyContain(f => f is VehicleNotAvailableException || f is VehicleConcurrencyException);
        }

        [Fact]
        public async Task SameCustomerRentingDifferentVehiclesConcurrentlyOnlyOneSucceeds()
        {
            var customerId = NewCustomerId();
            var vehicleIds = new List<Guid>();
            for (var i = 0; i < 5; i++)
            {
                vehicleIds.Add(await CreateVehicle($"4444DD{i}"));
            }

            var failures = await Task.WhenAll(vehicleIds.Select(id => TryRent(id, customerId)));

            failures.Count(f => f is null).Should().Be(1);
            failures.Where(f => f is not null).Should().AllBeOfType<CustomerAlreadyHasRentalException>();
        }

        [Fact]
        public async Task StaleReturnCannotEraseANewerRental()
        {
            var vehicleId = await CreateVehicle("5555EEE");
            await Rent(vehicleId, NewCustomerId());

            await Fixture.UsingRepository<IVehicleRepository>(async staleRepository =>
            {
                // A slow request loads the rented vehicle...
                var staleVehicle = await staleRepository.GetByIdAsync(vehicleId);

                // ...meanwhile the vehicle is returned and rented to someone else.
                await Return(vehicleId);
                await Rent(vehicleId, NewCustomerId());

                staleVehicle.Return();
                var saveStaleReturn = () => staleRepository.UpdateAsync(staleVehicle);
                await saveStaleReturn.Should().ThrowAsync<VehicleConcurrencyException>();
            });

            await Fixture.UsingRepository<IVehicleRepository>(async repository =>
            {
                var vehicle = await repository.GetByIdAsync(vehicleId);
                vehicle.IsRented.Should().BeTrue();
            });
        }

        [Fact]
        public async Task SameLicensePlateWrittenDifferentlyCannotBeAddedTwice()
        {
            await CreateVehicle("6666 fff");

            var addSamePlateAgain = () => CreateVehicle("6666-FFF");

            await addSamePlateAgain.Should().ThrowAsync<DuplicateLicensePlateException>();
        }

        [Fact]
        public async Task VehicleStoredBeforeVersioningCanBeRentedAndReturned()
        {
            var vehicleId = Guid.NewGuid();
            await Fixture.UsingRepository<MongoService>(mongo => mongo.Database
                .GetCollection<BsonDocument>("vehicles")
                .InsertOneAsync(new BsonDocument
                {
                    ["_id"] = new BsonBinaryData(vehicleId, GuidRepresentation.Standard),
                    ["licensePlate"] = "7777GGG",
                    ["brand"] = "Seat",
                    ["model"] = "Arona",
                    ["manufactureDate"] = DateTime.UtcNow.Date.AddYears(-1),
                    ["currentRental"] = BsonNull.Value,
                }));

            (await Rent(vehicleId, NewCustomerId())).Should().BeOfType<OkObjectResult>();
            (await Return(vehicleId)).Should().BeOfType<OkObjectResult>();
        }

        private static string NewCustomerId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private async Task<DomainException> TryRent(Guid vehicleId, string customerId)
        {
            try
            {
                await Rent(vehicleId, customerId);
                return null;
            }
            catch (DomainException ex)
            {
                return ex;
            }
        }

        private async Task<Guid> CreateVehicle(string licensePlate)
        {
            var request = new CreateVehicleRequest
            {
                LicensePlate = licensePlate,
                Brand = "Seat",
                Model = "Ibiza",
                ManufactureDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1),
            };

            var result = await Send<CreateVehicleRequest>(request);

            return ((VehicleResponse)((ObjectResult)result).Value).Id;
        }

        private Task<IActionResult> Rent(Guid vehicleId, string customerId)
        {
            return Send(new RentVehicleRequest { VehicleId = vehicleId, CustomerId = customerId });
        }

        private Task<IActionResult> Return(Guid vehicleId)
        {
            return Send(new ReturnVehicleRequest(vehicleId));
        }

        private async Task<IActionResult> Send<TRequest>(TRequest request)
            where TRequest : IRequest<IWebApiPresenter>
        {
            IActionResult result = null;

            await Fixture.UsingHandlerForRequestResponse<TRequest, IWebApiPresenter>(async handler =>
            {
                var presenter = await handler.Handle(request, CancellationToken.None);
                result = presenter.ActionResult;
            });

            return result;
        }
    }
}
