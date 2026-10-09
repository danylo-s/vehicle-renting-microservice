using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Output of the create vehicle use case.
    /// </summary>
    /// <param name="vehicle">The created vehicle.</param>
    public sealed class CreateVehicleOutput(Vehicle vehicle) : IUseCaseOutput
    {
        /// <summary>
        /// Gets the created vehicle.
        /// </summary>
        public Vehicle Vehicle { get; } = vehicle;
    }
}
