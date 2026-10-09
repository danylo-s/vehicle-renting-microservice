using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Rents a vehicle to a customer. A customer can only have one rented vehicle at a time.
    /// </summary>
    /// <param name="repository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="telemetry">Telemetry.</param>
    public sealed class RentVehicleUseCase(
        IVehicleRepository repository,
        IRentVehicleOutputPort outputPort,
        TimeProvider timeProvider,
        IAppLogger<RentVehicleUseCase> logger,
        ITelemetry telemetry) : IUseCase<RentVehicleInput>
    {
        private readonly IVehicleRepository _repository = repository;
        private readonly IRentVehicleOutputPort _outputPort = outputPort;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly IAppLogger<RentVehicleUseCase> _logger = logger;
        private readonly ITelemetry _telemetry = telemetry;

        /// <inheritdoc />
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await _repository.GetByIdAsync(input.VehicleId);
            if (vehicle is null)
            {
                _outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            if (await _repository.HasActiveRentalAsync(input.CustomerId))
            {
                throw new CustomerAlreadyHasRentalException($"The customer {input.CustomerId} already has a rented vehicle.");
            }

            vehicle.Rent(input.CustomerId, _timeProvider.GetUtcNow());

            await _repository.UpdateAsync(vehicle);

            _logger.LogInformation("Vehicle {VehicleId} rented.", vehicle.Id);
            _telemetry.TrackVehicleEvent("VehicleRented", vehicle);

            _outputPort.StandardHandle(new RentVehicleOutput(vehicle));
        }
    }
}
