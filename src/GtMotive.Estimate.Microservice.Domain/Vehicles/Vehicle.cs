using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Vehicle of the fleet. Aggregate root that owns its current rental.
    /// </summary>
    public sealed class Vehicle
    {
        /// <summary>
        /// Maximum age, in years, of a vehicle in the fleet.
        /// </summary>
        public const int MaxAgeInYears = 5;

        private Vehicle(Guid id, string licensePlate, string brand, string model, DateOnly manufactureDate, Rental currentRental)
        {
            Id = id;
            LicensePlate = licensePlate;
            Brand = brand;
            Model = model;
            ManufactureDate = manufactureDate;
            CurrentRental = currentRental;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        public string LicensePlate { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateOnly ManufactureDate { get; }

        /// <summary>
        /// Gets the current rental, or null when the vehicle is not rented.
        /// </summary>
        public Rental CurrentRental { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the vehicle is currently rented.
        /// </summary>
        public bool IsRented => CurrentRental is not null;

        /// <summary>
        /// Creates a new vehicle for the fleet.
        /// </summary>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="today">Current date.</param>
        /// <returns>The new vehicle.</returns>
        /// <exception cref="InvalidManufactureDateException">The date is in the future or older than <see cref="MaxAgeInYears"/> years.</exception>
        public static Vehicle Create(string licensePlate, string brand, string model, DateOnly manufactureDate, DateOnly today)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate);
            ArgumentException.ThrowIfNullOrWhiteSpace(brand);
            ArgumentException.ThrowIfNullOrWhiteSpace(model);

            if (manufactureDate > today)
            {
                throw new InvalidManufactureDateException($"The manufacture date {manufactureDate:yyyy-MM-dd} cannot be in the future.");
            }

            if (IsTooOld(manufactureDate, today))
            {
                throw new InvalidManufactureDateException($"The fleet cannot contain vehicles manufactured more than {MaxAgeInYears} years ago.");
            }

            return new Vehicle(Guid.NewGuid(), licensePlate.Trim(), brand.Trim(), model.Trim(), manufactureDate, null);
        }

        /// <summary>
        /// Rebuilds a vehicle from persisted state. Does not validate the age rule.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="currentRental">Current rental or null.</param>
        /// <returns>The vehicle.</returns>
        public static Vehicle Restore(Guid id, string licensePlate, string brand, string model, DateOnly manufactureDate, Rental currentRental)
        {
            return new Vehicle(id, licensePlate, brand, model, manufactureDate, currentRental);
        }

        /// <summary>
        /// Checks whether the vehicle can be rented: not rented and not older than <see cref="MaxAgeInYears"/> years.
        /// </summary>
        /// <param name="today">Current date.</param>
        /// <returns>True when the vehicle is available.</returns>
        public bool IsAvailable(DateOnly today)
        {
            return !IsRented && !IsTooOld(ManufactureDate, today);
        }

        /// <summary>
        /// Rents the vehicle to a customer.
        /// </summary>
        /// <param name="customerId">Customer renting the vehicle.</param>
        /// <param name="now">Current instant.</param>
        /// <exception cref="VehicleNotAvailableException">The vehicle is rented or too old.</exception>
        public void Rent(CustomerId customerId, DateTimeOffset now)
        {
            ArgumentNullException.ThrowIfNull(customerId);

            if (IsRented)
            {
                throw new VehicleNotAvailableException($"The vehicle {Id} is already rented.");
            }

            if (IsTooOld(ManufactureDate, DateOnly.FromDateTime(now.UtcDateTime)))
            {
                throw new VehicleNotAvailableException($"The vehicle {Id} is older than {MaxAgeInYears} years and cannot be rented.");
            }

            CurrentRental = new Rental(customerId, now);
        }

        /// <summary>
        /// Returns the vehicle, ending the current rental.
        /// </summary>
        /// <exception cref="VehicleNotRentedException">The vehicle is not rented.</exception>
        public void Return()
        {
            if (!IsRented)
            {
                throw new VehicleNotRentedException($"The vehicle {Id} is not rented.");
            }

            CurrentRental = null;
        }

        private static bool IsTooOld(DateOnly manufactureDate, DateOnly today)
        {
            return manufactureDate < today.AddYears(-MaxAgeInYears);
        }
    }
}
