using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    public sealed class RentVehicleTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
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
