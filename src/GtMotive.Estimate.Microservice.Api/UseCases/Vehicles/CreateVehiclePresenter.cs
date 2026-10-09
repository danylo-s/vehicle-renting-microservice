using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class CreateVehiclePresenter : IWebApiPresenter, IOutputPortStandard<CreateVehicleOutput>
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(CreateVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new ObjectResult(new VehicleResponse(response.Vehicle))
            {
                StatusCode = StatusCodes.Status201Created,
            };
        }
    }
}
