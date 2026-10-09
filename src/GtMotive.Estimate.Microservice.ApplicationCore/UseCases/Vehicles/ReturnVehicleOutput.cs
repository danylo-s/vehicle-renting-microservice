using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Output of the return vehicle use case.
    /// </summary>
    /// <param name="vehicle">The returned vehicle.</param>
    public sealed class ReturnVehicleOutput(Vehicle vehicle) : IUseCaseOutput
    {
        /// <summary>
        /// Gets the returned vehicle.
        /// </summary>
        public Vehicle Vehicle { get; } = vehicle;
    }
}
