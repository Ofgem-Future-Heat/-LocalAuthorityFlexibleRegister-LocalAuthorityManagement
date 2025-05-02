using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Extensions;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.SwaggerFilters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Configuration.AddAzureAppConfiguration(
    options => options.Connect(new Uri($"https://{builder.Configuration["AppConfig:Name"]}.azconfig.io"), new DefaultAzureCredential())
        .UseFeatureFlags()
        .Select(KeyFilter.Any)
        .Select(KeyFilter.Any, builder.Configuration["AppConfig:LabelFilter"])
        .ConfigureKeyVault(kvOptions =>
        {
            kvOptions.SetCredential(new DefaultAzureCredential());
        })
        .ConfigureRefresh(refresh =>
        {
            refresh.Register("Laf:SentinelKey", true).SetCacheExpiration(TimeSpan.FromSeconds(30));
        }));

builder.RegisterServices();
builder.Services.AddLogsConfiguration(builder.Configuration);
builder.Services.AddAzureAppConfiguration();

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AddCustomHeaderParameter>();
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


var app = builder.Build();

app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.RegisterEndpointDefinitions();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
