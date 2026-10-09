using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class ListAvailableVehiclesRequest : IRequest<IWebApiPresenter>
    {
    }
}
