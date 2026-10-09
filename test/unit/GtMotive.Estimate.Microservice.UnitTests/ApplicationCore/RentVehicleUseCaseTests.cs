using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    public sealed class RentVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 10, 9, 10, 0, 0, TimeSpan.Zero);

        private readonly Mock<IVehicleRepository> _repository = new();
        private readonly Mock<IRentVehicleOutputPort> _outputPort = new();
        private readonly RentVehicleUseCase _useCase;

        public RentVehicleUseCaseTests()
        {
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);

            _useCase = new RentVehicleUseCase(_repository.Object, _outputPort.Object, timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteWhenCustomerAlreadyHasRentalThrowsAndDoesNotUpdate()
        {
            var vehicle = NewVehicle();
            var customerId = new CustomerId("12345678Z");
            _repository.Setup(r => r.GetByIdAsync(vehicle.Id)).ReturnsAsync(vehicle);
            _repository.Setup(r => r.HasActiveRentalAsync(customerId)).ReturnsAsync(true);

            var act = () => _useCase.Execute(new RentVehicleInput(vehicle.Id, customerId));

            await act.Should().ThrowAsync<CustomerAlreadyHasRentalException>();
            vehicle.IsRented.Should().BeFalse();
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Never);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteWhenVehicleIsAvailableRentsItAndCallsStandardOutput()
        {
            var vehicle = NewVehicle();
            var customerId = new CustomerId("12345678Z");
            _repository.Setup(r => r.GetByIdAsync(vehicle.Id)).ReturnsAsync(vehicle);
            _repository.Setup(r => r.HasActiveRentalAsync(customerId)).ReturnsAsync(false);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, customerId));

            vehicle.CurrentRental.Should().Be(new Rental(customerId, Now));
            _repository.Verify(r => r.UpdateAsync(vehicle), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.Is<RentVehicleOutput>(o => o.Vehicle == vehicle)), Times.Once);
        }

        [Fact]
        public async Task ExecuteWhenVehicleDoesNotExistCallsNotFoundOutput()
        {
            var vehicleId = Guid.NewGuid();
            _repository.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync((Vehicle)null);

            await _useCase.Execute(new RentVehicleInput(vehicleId, new CustomerId("12345678Z")));

            _outputPort.Verify(p => p.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Never);
        }

        private static Vehicle NewVehicle()
        {
            return Vehicle.Create("1234ABC", "Seat", "Ibiza", new DateOnly(2024, 1, 15), DateOnly.FromDateTime(Now.UtcDateTime));
        }
    }
}
