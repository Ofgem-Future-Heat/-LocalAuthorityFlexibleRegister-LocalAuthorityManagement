namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.Abstractions;

public interface IEndpointDefinition
{
    void RegisterEndpoints(WebApplication app);
}