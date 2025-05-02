using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics.CodeAnalysis;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public class CustomTelemetryInitialiser : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = "LAF-LocalAuthority-Api";
        }
    }
}
