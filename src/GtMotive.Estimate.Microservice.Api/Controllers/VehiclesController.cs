using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public sealed class VehiclesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
        {
            var presenter = await _mediator.Send(request);
            return presenter.ActionResult;
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<VehicleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAvailable()
        {
            var presenter = await _mediator.Send(new ListAvailableVehiclesRequest());
            return presenter.ActionResult;
        }

        [HttpPost("{id:guid}/rent")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Rent(Guid id, [FromBody] RentVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var presenter = await _mediator.Send(request with { VehicleId = id });
            return presenter.ActionResult;
        }

        [HttpPost("{id:guid}/return")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Return(Guid id)
        {
            var presenter = await _mediator.Send(new ReturnVehicleRequest(id));
            return presenter.ActionResult;
        }
    }
}
