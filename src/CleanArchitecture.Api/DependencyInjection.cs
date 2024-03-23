using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        string currentEnvironment = configuration.GetSection("CICD:CurrentEnvironment").Value!;
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = $"Reminders API ({currentEnvironment})", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Autenticación JWT (Bearer)",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme,
                        },
                    }, new List<string>()
                },
            });
        });
        services.AddProblemDetails();

        return services;
    }
}