using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaytestHub.Modules.Studio.Application.Services;
using PlaytestHub.Modules.Studio.Endpoints;
using PlaytestHub.Modules.Studio.Infrastructure.Database;
using PlaytestHub.Shared.Infrastructure.Database;

namespace PlaytestHub.Modules.Studio;

public static class StudioModule
{
    public static IServiceCollection AddStudioModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<StudioDbContext>(configuration, StudioDbContext.Schema);

        services.AddScoped<IGameService, GameService>();
        services.AddValidatorsFromAssembly(typeof(StudioModule).Assembly, includeInternalTypes: true);

        return services;
    }

    public static IEndpointRouteBuilder MapStudioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studio")
            .WithTags("Studio")
            .RequireAuthorization();

        group.MapGameEndpoints();

        return app;
    }
}