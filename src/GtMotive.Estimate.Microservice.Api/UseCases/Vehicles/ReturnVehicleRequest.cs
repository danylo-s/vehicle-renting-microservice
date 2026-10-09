using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed record ReturnVehicleRequest(Guid VehicleId) : IRequest<IWebApiPresenter>;
}
