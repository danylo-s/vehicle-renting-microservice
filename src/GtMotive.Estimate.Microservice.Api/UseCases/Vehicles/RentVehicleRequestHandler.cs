using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class RentVehicleRequestHandler(
        IUseCase<RentVehicleInput> useCase,
        RentVehiclePresenter presenter) : IRequestHandler<RentVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<RentVehicleInput> _useCase = useCase;
        private readonly RentVehiclePresenter _presenter = presenter;

        public async Task<IWebApiPresenter> Handle(RentVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var input = new RentVehicleInput(request.VehicleId, new CustomerId(request.CustomerId));
            await _useCase.Execute(input);
            return _presenter;
        }
    }
}
