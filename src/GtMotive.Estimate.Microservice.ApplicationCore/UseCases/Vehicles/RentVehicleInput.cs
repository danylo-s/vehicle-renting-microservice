using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Input of the rent vehicle use case.
    /// </summary>
    public sealed class RentVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleInput"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle to rent.</param>
        /// <param name="customerId">Customer renting the vehicle.</param>
        public RentVehicleInput(Guid vehicleId, CustomerId customerId)
        {
            VehicleId = vehicleId;
            CustomerId = customerId;
        }

        /// <summary>
        /// Gets the vehicle to rent.
        /// </summary>
        public Guid VehicleId { get; }

        /// <summary>
        /// Gets the customer renting the vehicle.
        /// </summary>
        public CustomerId CustomerId { get; }
    }
}
