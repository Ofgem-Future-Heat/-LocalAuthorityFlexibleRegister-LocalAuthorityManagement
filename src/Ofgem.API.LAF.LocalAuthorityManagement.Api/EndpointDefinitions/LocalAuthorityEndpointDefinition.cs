using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application;
using System.Diagnostics.CodeAnalysis;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;
using Ofgem.LAF.SharedLibrary.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.EndpointDefinitions;
[ExcludeFromCodeCoverage]
public class LocalAuthorityEndpointDefinition : IEndpointDefinition
{
    public void RegisterEndpoints(WebApplication app)
    {
        var localAuthorities = app.MapGroup("/api/LocalAuthorities");

        localAuthorities.MapGet("/{id}", Get).WithName("GetLocalAuthorityById").WithOpenApi();

        localAuthorities.MapGet("/by-ons-code/{onsCode}", GetByOnsCode).WithName("GetByOnsCode").WithOpenApi();

        localAuthorities.MapGet("/by-name/{name}", GetByName).WithName("GetByName").WithOpenApi();

        localAuthorities.MapGet("/", GetAll).WithOpenApi();

        localAuthorities.MapGet("/{onsCode}/localauthorities", GetAssociatedLocalAuthorities).WithOpenApi();

        //select la.[Name] from StatementOfIntents soi,
        //DesignatedLas dla,
        //    LocalAuthorities la
        //    where soi.LocalAuthorityId = '78214A5F-7EA2-403C-9177-B63A7B489912'

        //and dla.StatementOfIntentId = soi.StatementOfIntentId

        //and la.LocalAuthorityId = dla.LocalAuthorityId

        localAuthorities.MapPost("/GetFiltered", GetFiltered).WithOpenApi();

        localAuthorities.MapPost("/", Create).WithOpenApi();

        localAuthorities.MapPut("/{id}", UpdateLocalAuthority).WithOpenApi();

        localAuthorities.MapPut("/by-ons-code", UpdateLocalAuthorityByOnsCode).WithName("OutByOnsCode").WithOpenApi();

        localAuthorities.MapDelete("/{id}", DeleteLocalAuthority).WithOpenApi();
    }

    private async Task<IResult> GetAssociatedLocalAuthorities(
            IMediator mediator,
            string onsCode,
            ILogger<LocalAuthorityEndpointDefinition> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfileAssociatedLocalAuthorities);
        GetAssociatedLocalAuthorities query = new()
        {
            OnsCode = onsCode
        };

        var localAuthorities = await mediator.Send(query);
        return TypedResults.Ok(localAuthorities);
    }

    private async Task<IResult> GetFiltered(Ofgem.LAF.SharedLibrary.Models.ProfilesFilter filter, IMediator mediator,
        ILogger<LocalAuthorityEndpointDefinition> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfile);

        GetFiltered getFilteredLocalAuthorityRequest = new()
        {
            Filter = filter
        };

        var localAuthorities = await mediator.Send(getFilteredLocalAuthorityRequest);

        return TypedResults.Ok(localAuthorities);
    }

    private async Task<NoContent> DeleteLocalAuthority(Guid id, IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.DeleteProfile);
        var deleteLocalAuthorityCommand = new Application.LocalAuthority.Commands.Delete() { LocalAuthorityId = id };
        await mediator.Send(deleteLocalAuthorityCommand);

        return TypedResults.NoContent();
    }

    private async Task<IResult> UpdateLocalAuthority(Ofgem.LAF.SharedLibrary.Models.LocalAuthority localAuthority, Guid id, IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.UpdateProfile);
        var updateLocalAuthorityCommand = new Application.LocalAuthority.Commands.Update() { Name = localAuthority.Name, LocalAuthorityId = id };
        var updatedLocalAuthority = await mediator.Send(updateLocalAuthorityCommand);

        return updatedLocalAuthority == null ? TypedResults.NotFound() : TypedResults.Ok(updatedLocalAuthority);
    }

    private async Task<IResult> UpdateLocalAuthorityByOnsCode(Ofgem.LAF.SharedLibrary.Models.LocalAuthority localAuthority,
                                        IMediator mediator,
                                        ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.UpdateProfile);
        var updateLocalAuthorityCommand = new Application.LocalAuthority.Commands.UpdateByOnsCode()
        {
            Name = localAuthority.Name,
            Email = localAuthority.Email!,
            OnsCode = localAuthority.OnsCode
        };
        var updatedLocalAuthority = await mediator.Send(updateLocalAuthorityCommand);

        return updatedLocalAuthority == null ? TypedResults.NotFound() : TypedResults.Ok(updatedLocalAuthority);
    }
    private async Task<IResult> Create(Ofgem.LAF.SharedLibrary.Models.LocalAuthority localAuthority, IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.CreateProfile);
        var createLocalAuthorityCommand = new Application.LocalAuthority.Commands.Create()
        {
            LocalAuthorityId = Guid.NewGuid(),
            Name = localAuthority.Name,
            Email = localAuthority.Email,
            OnsCode = localAuthority.OnsCode
        };
        var createdLocalAuthority = await mediator.Send(createLocalAuthorityCommand);

        return Results.CreatedAtRoute("GetLocalAuthorityById", new { Id = createdLocalAuthority.LocalAuthorityId }, createdLocalAuthority);
    }

    private async Task<IResult> GetAll(IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfile);

        GetAll getAllLocalAuthoritiesRequest = new();

        var localAuthorities = await mediator.Send(getAllLocalAuthoritiesRequest);

        return TypedResults.Ok(localAuthorities);
    }

    private async Task<IResult> Get(Guid id, IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfile);

        var getLocalAuthorityRequest = new Get() { LocalAuthorityId = id };

        Result<LocalAuthority?> localAuthorityResult = await mediator.Send(getLocalAuthorityRequest);

        if (localAuthorityResult.IsSuccess) return TypedResults.Ok(localAuthorityResult.Value);

        return  TypedResults.NotFound(id);
    }

    private async Task<IResult> GetByOnsCode(string onsCode, IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfile);
        GetByOnsCode getLocalAuthorityByOnsCodeRequest = new()
        { OnsCode = onsCode };

        var result = await mediator.Send(getLocalAuthorityByOnsCodeRequest);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Pared());
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    private async Task<IResult> GetByName(string name, IMediator mediator, ILogger<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfile);

        GetByName getLocalAuthorityByNameRequest = new()
        { Name = name };
        var result = await mediator.Send(getLocalAuthorityByNameRequest);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value.Pared());
        }

        return TypedResults.NotFound();
    }
}