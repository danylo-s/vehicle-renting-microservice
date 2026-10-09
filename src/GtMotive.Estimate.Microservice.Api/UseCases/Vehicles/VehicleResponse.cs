using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class VehicleResponse
    {
        public VehicleResponse(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            Id = vehicle.Id;
            LicensePlate = vehicle.LicensePlate;
            Brand = vehicle.Brand;
            Model = vehicle.Model;
            ManufactureDate = vehicle.ManufactureDate;
            RentedBy = vehicle.CurrentRental?.CustomerId.Value;
            RentedAt = vehicle.CurrentRental?.RentedAt;
        }

        /// <summary>
        /// Gets the vehicle id.
        /// </summary>
        [Required]
        public Guid Id { get; }

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        [Required]
        public string LicensePlate { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        [Required]
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        [Required]
        public string Model { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        [Required]
        public DateOnly ManufactureDate { get; }

        /// <summary>
        /// Gets the customer currently renting the vehicle, or null.
        /// </summary>
        public string RentedBy { get; }

        /// <summary>
        /// Gets the start of the current rental, or null.
        /// </summary>
        public DateTimeOffset? RentedAt { get; }
    }
}
