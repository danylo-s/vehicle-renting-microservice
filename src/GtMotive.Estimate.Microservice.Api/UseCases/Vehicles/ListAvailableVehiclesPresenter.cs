using System;
using System.Linq;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class ListAvailableVehiclesPresenter : IWebApiPresenter, IOutputPortStandard<ListAvailableVehiclesOutput>
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(ListAvailableVehiclesOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new OkObjectResult(response.Vehicles.Select(v => new VehicleResponse(v)).ToList());
        }
    }
}
