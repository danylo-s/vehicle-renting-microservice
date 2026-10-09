using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class ListAvailableVehiclesRequestHandler(
        IUseCase<ListAvailableVehiclesInput> useCase,
        ListAvailableVehiclesPresenter presenter) : IRequestHandler<ListAvailableVehiclesRequest, IWebApiPresenter>
    {
        private readonly IUseCase<ListAvailableVehiclesInput> _useCase = useCase;
        private readonly ListAvailableVehiclesPresenter _presenter = presenter;

        public async Task<IWebApiPresenter> Handle(ListAvailableVehiclesRequest request, CancellationToken cancellationToken)
        {
            await _useCase.Execute(new ListAvailableVehiclesInput());
            return _presenter;
        }
    }
}
