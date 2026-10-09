using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Api.DependencyInjection
{
    public static class UserInterfaceExtensions
    {
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            // Each presenter is registered once per scope and exposed as its output port,
            // so the handler and the use case share the same instance.
            services.AddScoped<CreateVehiclePresenter>();
            services.AddScoped<IOutputPortStandard<CreateVehicleOutput>>(sp => sp.GetRequiredService<CreateVehiclePresenter>());

            services.AddScoped<ListAvailableVehiclesPresenter>();
            services.AddScoped<IOutputPortStandard<ListAvailableVehiclesOutput>>(sp => sp.GetRequiredService<ListAvailableVehiclesPresenter>());

            services.AddScoped<RentVehiclePresenter>();
            services.AddScoped<IRentVehicleOutputPort>(sp => sp.GetRequiredService<RentVehiclePresenter>());

            services.AddScoped<ReturnVehiclePresenter>();
            services.AddScoped<IReturnVehicleOutputPort>(sp => sp.GetRequiredService<ReturnVehiclePresenter>());

            return services;
        }
    }
}
