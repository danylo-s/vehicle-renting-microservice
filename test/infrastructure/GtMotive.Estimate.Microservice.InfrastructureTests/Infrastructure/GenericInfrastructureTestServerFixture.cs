using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    public sealed class GenericInfrastructureTestServerFixture : IDisposable
    {
        private readonly IHost _host;

        public GenericInfrastructureTestServerFixture()
        {
            _host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("IntegrationTest")
                .UseDefaultServiceProvider(options => { options.ValidateScopes = true; })
                .ConfigureAppConfiguration((context, builder) => { builder.AddEnvironmentVariables(); })
                .ConfigureWebHost(web => web
                    .UseTestServer()
                    .UseStartup<Startup>())
                .Start();

            Server = _host.GetTestServer();
        }

        public TestServer Server { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
