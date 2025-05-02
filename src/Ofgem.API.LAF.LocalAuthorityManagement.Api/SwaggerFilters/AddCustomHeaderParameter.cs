using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.SwaggerFilters
{
    [ExcludeFromCodeCoverage]
    public class AddCustomHeaderParameter : IOperationFilter
    {
        void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters is null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-UserName",
                In = ParameterLocation.Header,
                Description = "User Name of the Caller",
                Required = false,
            });
        }
    }
}
