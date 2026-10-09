using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Lists the vehicles of the fleet that can be rented today.
    /// </summary>
    /// <param name="repository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    public sealed class ListAvailableVehiclesUseCase(
        IVehicleRepository repository,
        IOutputPortStandard<ListAvailableVehiclesOutput> outputPort,
        TimeProvider timeProvider) : IUseCase<ListAvailableVehiclesInput>
    {
        private readonly IVehicleRepository _repository = repository;
        private readonly IOutputPortStandard<ListAvailableVehiclesOutput> _outputPort = outputPort;
        private readonly TimeProvider _timeProvider = timeProvider;

        /// <inheritdoc />
        public async Task Execute(ListAvailableVehiclesInput input)
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            var vehicles = await _repository.ListNotRentedAsync();

            var available = vehicles.Where(v => v.IsAvailable(today)).ToList();

            _outputPort.StandardHandle(new ListAvailableVehiclesOutput(available));
        }
    }
}
