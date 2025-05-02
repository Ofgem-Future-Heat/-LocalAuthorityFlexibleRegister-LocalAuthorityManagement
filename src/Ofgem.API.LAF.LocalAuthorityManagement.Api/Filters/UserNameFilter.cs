using Ofgem.LAF.SharedLibrary.Extensions;
using System.Diagnostics.CodeAnalysis;
using Ofgem.API.LAF.LocalAuthorityManagement.Application;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.Filters
{
    [ExcludeFromCodeCoverage]
    public class UserNameFilter(ILogger<UserNameFilter> logger) : IEndpointFilter
    {
        private readonly ILogger _logger = logger;

        async ValueTask<object?> IEndpointFilter.InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            _logger.LogLafInformation(LogEvents.Infrastructure);

            string? username = context.HttpContext.Request.Headers["X-UserName"];

            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogLafError(LogEvents.Infrastructure, "User name was not supplied in the header");
                return Results.Problem("User name was not supplied in the header.");
            }
            var result = await next(context);

            return result;
        }
    }
}
