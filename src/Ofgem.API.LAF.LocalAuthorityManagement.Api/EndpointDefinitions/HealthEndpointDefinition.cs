using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Abstractions;
using System.Diagnostics.CodeAnalysis;
using Ofgem.API.LAF.LocalAuthorityManagement.Application;
using Ofgem.LAF.SharedLibrary.Extensions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.EndpointDefinitions
{
    [ExcludeFromCodeCoverage]
    public class HealthEndpointDefinition : IEndpointDefinition
    {
        
        public void RegisterEndpoints(WebApplication app)
        {
            app.MapGet("/api/health", Health).WithOpenApi();
            app.MapGet("/api/Healthfullcheck", Healthfullcheck).WithOpenApi();
        }

        /// <summary>
        /// Tests that the api is available
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        private async Task<IResult> Health(ILogger<HealthEndpointDefinition> logger)
        {
            logger.LogLafInformation(LogEvents.Health);

            return await Task.Run(() => TypedResults.Ok("Ok"));
        }

        /// <summary>
        /// Tests the calls to the CREATE, GET, UPDATE and  DELETE methods
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<IResult> Healthfullcheck(IMediator mediator, ILogger<HealthEndpointDefinition> logger)
        {
            logger.LogLafInformation(LogEvents.HealthFull);

            try
            {
                logger.LogLafInformation(LogEvents.CreateProfile);
                var createLocalAuthorityCommand = new Application.LocalAuthority.Commands.Create() 
                { 
                    Name = "Health full check Test", 
                    OnsCode = "12345" ,
                    Email = "test@test.gove.uk"
                };

                var createdLocalAuthority = await mediator.Send(createLocalAuthorityCommand) ?? throw new ArgumentException($"Health full check - creation of test local authority failed");

                logger.LogLafInformation(LogEvents.GetProfile);

                var getLocalAuthorityRequest = new Application.LocalAuthority.Queries.Get() { LocalAuthorityId = createdLocalAuthority.LocalAuthorityId };

                var localAuthorityResult1 = await mediator.Send(getLocalAuthorityRequest);

                if (!localAuthorityResult1.IsSuccess)
                {
                    throw new ArgumentException(
                        $"Health full check - retrieval of test local authority: {createdLocalAuthority.LocalAuthorityId} failed");
                }

                logger.LogLafInformation(LogEvents.GetProfile);

                var newName = "Health full check Test Update";

                var updateLocalAuthorityCommand = new Application.LocalAuthority.Commands.Update() 
                { 
                    Name = newName, 
                    LocalAuthorityId = createdLocalAuthority.LocalAuthorityId,
                    OnsCode = "98765",
                    Email = "test2@test.gov.uk"                    
                };

                _ = await mediator.Send(updateLocalAuthorityCommand) ?? throw new ArgumentException($"Health full check - update of test local authority: {createdLocalAuthority.LocalAuthorityId} failed");

                logger.LogLafInformation(LogEvents.GetProfile);

                getLocalAuthorityRequest = new Application.LocalAuthority.Queries.Get() { LocalAuthorityId = createdLocalAuthority.LocalAuthorityId };

                var localAuthorityResult2 = await mediator.Send(getLocalAuthorityRequest);

                if (localAuthorityResult2.IsSuccess && localAuthorityResult2.Value.Name != newName)
                {
                    throw new ArgumentException(
                        $"Health full check - update of  test local authority: {createdLocalAuthority.LocalAuthorityId} name property failed.");
                }

                logger.LogLafInformation(LogEvents.DeleteProfile);
                var deleteLocalAuthorityCommand = new Application.LocalAuthority.Commands.Delete() { LocalAuthorityId = createdLocalAuthority.LocalAuthorityId };
                await mediator.Send(deleteLocalAuthorityCommand);


                logger.LogLafInformation(LogEvents.GetProfile);
                getLocalAuthorityRequest = new Application.LocalAuthority.Queries.Get() { LocalAuthorityId = createdLocalAuthority.LocalAuthorityId };
                var localAuthorityResult3 = await mediator.Send(getLocalAuthorityRequest);
                if(localAuthorityResult3.IsSuccess)
                {
                    throw new ArgumentException($"Health full check - created test local authority: {createdLocalAuthority.LocalAuthorityId} is still in the DB");
                }
            }
            catch (Exception ex)
            {
                logger.LogLafError(ex, LogEvents.HealthFull, "Health full check LocalAuthorities responded with {Message}", ex.Message);
                return TypedResults.StatusCode(500);
            }

            logger.LogLafInformation(LogEvents.HealthFull, "Health full check result OK");
            return TypedResults.Ok("Ok");
        }
    }
}
