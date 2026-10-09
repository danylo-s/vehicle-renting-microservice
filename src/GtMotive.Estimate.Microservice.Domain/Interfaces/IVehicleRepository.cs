using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Persistence port for the <see cref="Vehicle"/> aggregate.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>A task.</returns>
        /// <exception cref="DuplicateLicensePlateException">Another vehicle has the same license plate.</exception>
        Task AddAsync(Vehicle vehicle);

        /// <summary>
        /// Gets a vehicle by id.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>The vehicle, or null when it does not exist.</returns>
        Task<Vehicle> GetByIdAsync(Guid id);

        /// <summary>
        /// Lists the vehicles that are not rented.
        /// </summary>
        /// <returns>The vehicles.</returns>
        Task<IReadOnlyCollection<Vehicle>> ListNotRentedAsync();

        /// <summary>
        /// Checks whether a customer currently has a rented vehicle.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>True when the customer has an active rental.</returns>
        Task<bool> HasActiveRentalAsync(CustomerId customerId);

        /// <summary>
        /// Saves the rental state of an existing vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>A task.</returns>
        /// <exception cref="CustomerAlreadyHasRentalException">The customer already has another rental.</exception>
        /// <exception cref="VehicleConcurrencyException">The vehicle was modified by another request since it was loaded.</exception>
        Task UpdateAsync(Vehicle vehicle);
    }
}
