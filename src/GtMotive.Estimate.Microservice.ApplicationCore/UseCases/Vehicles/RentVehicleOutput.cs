using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Output of the rent vehicle use case.
    /// </summary>
    /// <param name="vehicle">The rented vehicle.</param>
    public sealed class RentVehicleOutput(Vehicle vehicle) : IUseCaseOutput
    {
        /// <summary>
        /// Gets the rented vehicle.
        /// </summary>
        public Vehicle Vehicle { get; } = vehicle;
    }
}
