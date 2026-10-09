using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed record RentVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Gets the vehicle id. Taken from the route, not from the body.
        /// </summary>
        [JsonIgnore]
        public Guid VehicleId { get; init; }

        /// <summary>
        /// Gets the customer id (e.g. national ID) of the person renting the vehicle.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string CustomerId { get; init; }
    }
}
