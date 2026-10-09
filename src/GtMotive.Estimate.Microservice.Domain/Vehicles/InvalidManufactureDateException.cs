using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Thrown when a manufacture date is in the future or too old for the fleet.
    /// </summary>
    public sealed class InvalidManufactureDateException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidManufactureDateException"/> class.
        /// </summary>
        public InvalidManufactureDateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidManufactureDateException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public InvalidManufactureDateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidManufactureDateException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidManufactureDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
