using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Adds a new vehicle to the fleet.
    /// </summary>
    /// <param name="repository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="telemetry">Telemetry.</param>
    public sealed class CreateVehicleUseCase(
        IVehicleRepository repository,
        IOutputPortStandard<CreateVehicleOutput> outputPort,
        TimeProvider timeProvider,
        IAppLogger<CreateVehicleUseCase> logger,
        ITelemetry telemetry) : IUseCase<CreateVehicleInput>
    {
        private readonly IVehicleRepository _repository = repository;
        private readonly IOutputPortStandard<CreateVehicleOutput> _outputPort = outputPort;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly IAppLogger<CreateVehicleUseCase> _logger = logger;
        private readonly ITelemetry _telemetry = telemetry;

        /// <inheritdoc />
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            var vehicle = Vehicle.Create(input.LicensePlate, input.Brand, input.Model, input.ManufactureDate, today);

            await _repository.AddAsync(vehicle);

            _logger.LogInformation("Vehicle {VehicleId} added to the fleet.", vehicle.Id);
            _telemetry.TrackVehicleEvent("VehicleCreated", vehicle);

            _outputPort.StandardHandle(new CreateVehicleOutput(vehicle));
        }
    }
}
