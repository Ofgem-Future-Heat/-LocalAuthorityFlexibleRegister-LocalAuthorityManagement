using Microsoft.EntityFrameworkCore;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Data.Repositories;
using Serilog;
using System.Reflection;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;
using Ofgem.LAF.SharedLibrary.Context;
using System.Diagnostics.CodeAnalysis;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class MinimalApiExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        // set up serilog
        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDb");

        var configuration = builder.Configuration.GetConnectionString("Default") 
                            ?? throw new ArgumentException("Missing db connection string");

        if (useInMemoryDb)
        {
            builder.Services.AddDbContext<LafContext>(options =>
            {
                options.UseInMemoryDatabase(configuration);
            });
        }
        else
        {
            builder.Services.AddDbContext<LafContext>(options => options.UseSqlServer(configuration));
        }

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ILocalAuthorityRepository, LocalAuthorityRepository>();
        builder.Services.AddScoped<IStatementOfIntentRepository, StatementOfIntentRepository>();
        builder.Services.AddScoped<IAssessmentNoteRepository, AssessmentNoteRepository>();
        builder.Services.AddScoped<IDeclarationsRepository, DeclarationsRepository>();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Create).GetTypeInfo().Assembly));
    }

    /// <summary>
    /// Uses reflection to register all the endpoints we have defined
    /// </summary>
    /// <param name="app"></param>
    public static void RegisterEndpointDefinitions(this WebApplication app)
    {
        var endpointDefinitions = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpointDefinition))
                        && !t.IsAbstract
                        && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IEndpointDefinition>();

        foreach (var endpointdef in endpointDefinitions)
        {
            endpointdef.RegisterEndpoints(app);
        }
    }
}