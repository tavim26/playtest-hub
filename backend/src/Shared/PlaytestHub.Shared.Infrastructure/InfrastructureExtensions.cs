using Microsoft.Extensions.DependencyInjection;
using PlaytestHub.Shared.Infrastructure.Errors;

namespace PlaytestHub.Shared.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}