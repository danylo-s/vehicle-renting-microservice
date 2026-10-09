using System.Collections.Generic;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Output of the list available vehicles use case.
    /// </summary>
    /// <param name="vehicles">The available vehicles.</param>
    public sealed class ListAvailableVehiclesOutput(IReadOnlyCollection<Vehicle> vehicles) : IUseCaseOutput
    {
        /// <summary>
        /// Gets the available vehicles.
        /// </summary>
        public IReadOnlyCollection<Vehicle> Vehicles { get; } = vehicles;
    }
}
