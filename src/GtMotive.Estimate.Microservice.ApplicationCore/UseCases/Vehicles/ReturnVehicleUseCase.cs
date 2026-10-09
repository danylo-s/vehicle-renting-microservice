using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Returns a rented vehicle.
    /// </summary>
    /// <param name="repository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    public sealed class ReturnVehicleUseCase(
        IVehicleRepository repository,
        IReturnVehicleOutputPort outputPort) : IUseCase<ReturnVehicleInput>
    {
        private readonly IVehicleRepository _repository = repository;
        private readonly IReturnVehicleOutputPort _outputPort = outputPort;

        /// <inheritdoc />
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await _repository.GetByIdAsync(input.VehicleId);
            if (vehicle is null)
            {
                _outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            vehicle.Return();

            await _repository.UpdateAsync(vehicle);

            _outputPort.StandardHandle(new ReturnVehicleOutput(vehicle));
        }
    }
}
