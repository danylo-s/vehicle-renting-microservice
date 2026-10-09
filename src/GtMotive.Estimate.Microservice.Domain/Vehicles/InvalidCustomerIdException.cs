using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Thrown when a customer id is not valid.
    /// </summary>
    public sealed class InvalidCustomerIdException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCustomerIdException"/> class.
        /// </summary>
        public InvalidCustomerIdException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCustomerIdException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public InvalidCustomerIdException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCustomerIdException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidCustomerIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
