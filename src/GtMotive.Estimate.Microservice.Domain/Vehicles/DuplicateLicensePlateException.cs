using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Thrown when a vehicle with the same license plate already exists in the fleet.
    /// </summary>
    public sealed class DuplicateLicensePlateException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateLicensePlateException"/> class.
        /// </summary>
        public DuplicateLicensePlateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateLicensePlateException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public DuplicateLicensePlateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateLicensePlateException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public DuplicateLicensePlateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
