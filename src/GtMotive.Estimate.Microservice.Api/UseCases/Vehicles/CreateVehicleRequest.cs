using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed record CreateVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Gets the license plate.
        /// </summary>
        [Required]
        [StringLength(15)]
        public string LicensePlate { get; init; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Brand { get; init; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Model { get; init; }

        /// <summary>
        /// Gets the manufacture date (yyyy-MM-dd). It cannot be in the future or more than 5 years ago.
        /// </summary>
        [Required]
        public DateOnly? ManufactureDate { get; init; }
    }
}
