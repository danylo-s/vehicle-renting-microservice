using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Thrown when a vehicle was modified by another request between loading and saving it.
    /// </summary>
    public sealed class VehicleConcurrencyException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConcurrencyException"/> class.
        /// </summary>
        public VehicleConcurrencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public VehicleConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
