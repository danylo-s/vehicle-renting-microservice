using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class ReturnVehicleRequestHandler(
        IUseCase<ReturnVehicleInput> useCase,
        ReturnVehiclePresenter presenter) : IRequestHandler<ReturnVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<ReturnVehicleInput> _useCase = useCase;
        private readonly ReturnVehiclePresenter _presenter = presenter;

        public async Task<IWebApiPresenter> Handle(ReturnVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            await _useCase.Execute(new ReturnVehicleInput(request.VehicleId));
            return _presenter;
        }
    }
}
