using CleanArchitecture.Api;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation(builder.Configuration)
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

string currentEnvironment = builder.Configuration["CICD:CurrentEnvironment"]!;

var app = builder.Build();
{
    app.UseExceptionHandler();
    app.UseInfrastructure();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", $"Reminders API ({currentEnvironment})");
        options.RoutePrefix = "reminders/swagger";
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}