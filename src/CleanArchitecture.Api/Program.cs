using CleanArchitecture.Api;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseInfrastructure();

app.UseSwagger(options =>
{
    string prefix = $"{Environment.GetEnvironmentVariable("SWAGGER_PREFIX")}";
    prefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"/{prefix}";
    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        swaggerDoc.Servers = [new OpenApiServer { Url = $"https://{httpReq.Host.Value}{prefix}" }]);
});
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("v1/swagger.json", $"Reminders API"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
