using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Active rental of a vehicle.
    /// </summary>
    public sealed record Rental
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// </summary>
        /// <param name="customerId">Customer renting the vehicle.</param>
        /// <param name="rentedAt">Start of the rental.</param>
        public Rental(CustomerId customerId, DateTimeOffset rentedAt)
        {
            ArgumentNullException.ThrowIfNull(customerId);

            CustomerId = customerId;
            RentedAt = rentedAt;
        }

        /// <summary>
        /// Gets the customer renting the vehicle.
        /// </summary>
        public CustomerId CustomerId { get; }

        /// <summary>
        /// Gets the start of the rental.
        /// </summary>
        public DateTimeOffset RentedAt { get; }
    }
}
