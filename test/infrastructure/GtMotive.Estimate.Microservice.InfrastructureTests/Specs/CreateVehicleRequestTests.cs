using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    public sealed class CreateVehicleRequestTests(GenericInfrastructureTestServerFixture fixture) : InfrastructureTestBase(fixture)
    {
        [Fact]
        public async Task PostVehicleWithoutLicensePlateReturnsValidationProblem()
        {
            using var client = Fixture.Server.CreateClient();
            var body = new { brand = "Seat", model = "Ibiza", manufactureDate = "2024-01-15" };

            using var response = await client.PostAsJsonAsync(new Uri("/api/vehicles", UriKind.Relative), body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem.Errors.Should().ContainKey("LicensePlate");
        }
    }
}
