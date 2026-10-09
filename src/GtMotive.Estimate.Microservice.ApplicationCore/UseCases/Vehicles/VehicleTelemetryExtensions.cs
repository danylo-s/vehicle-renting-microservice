using System.Collections.Generic;
using System.Globalization;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Tracks vehicle business events. Only the vehicle id is sent: the customer id is personal data.
    /// </summary>
    internal static class VehicleTelemetryExtensions
    {
        /// <summary>
        /// Tracks a vehicle event.
        /// </summary>
        /// <param name="telemetry">Telemetry.</param>
        /// <param name="eventName">Event name.</param>
        /// <param name="vehicle">Vehicle.</param>
        public static void TrackVehicleEvent(this ITelemetry telemetry, string eventName, Vehicle vehicle)
        {
            telemetry.TrackEvent(eventName, new Dictionary<string, string>
            {
                ["VehicleId"] = vehicle.Id.ToString("D", CultureInfo.InvariantCulture),
            });
        }
    }
}
