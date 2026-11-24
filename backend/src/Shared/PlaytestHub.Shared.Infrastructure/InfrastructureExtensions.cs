using Microsoft.Extensions.DependencyInjection;
using PlaytestHub.Shared.Infrastructure.Errors;
using System.Text.Json.Serialization;

namespace PlaytestHub.Shared.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }
}