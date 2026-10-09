namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Identifies a person (e.g. national ID). Normalized so the same person always has the same value.
    /// </summary>
    public sealed record CustomerId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerId"/> class.
        /// </summary>
        /// <param name="value">Raw identifier.</param>
        /// <exception cref="InvalidCustomerIdException">The value is empty.</exception>
        public CustomerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidCustomerIdException("The customer id is required.");
            }

            Value = value.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Gets the normalized identifier.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }
}
