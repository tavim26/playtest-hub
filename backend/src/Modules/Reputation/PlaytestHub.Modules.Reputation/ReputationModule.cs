using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PlaytestHub.Modules.Reputation;

public static class ReputationModule
{
    public static IServiceCollection AddReputationModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register this module's DbContext, handlers and validators here.
        return services;
    }

    public static IEndpointRouteBuilder MapReputationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reputation").WithTags("Reputation");
        group.MapGet("/ping", () => Results.Ok(new { module = "Reputation", status = "ok" }));
        return app;
    }
}
