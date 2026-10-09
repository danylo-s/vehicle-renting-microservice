using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    public sealed class CreateVehicleRequestHandler(
        IUseCase<CreateVehicleInput> useCase,
        CreateVehiclePresenter presenter) : IRequestHandler<CreateVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<CreateVehicleInput> _useCase = useCase;
        private readonly CreateVehiclePresenter _presenter = presenter;

        public async Task<IWebApiPresenter> Handle(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var input = new CreateVehicleInput(request.LicensePlate, request.Brand, request.Model, request.ManufactureDate.GetValueOrDefault());
            await _useCase.Execute(input);
            return _presenter;
        }
    }
}
