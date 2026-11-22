using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PlaytestHub.Modules.Identity.Application.Dtos;
using PlaytestHub.Modules.Identity.Application.Services;
using PlaytestHub.Shared.Infrastructure.Authentication;
using PlaytestHub.Shared.Infrastructure.Errors;
using PlaytestHub.Shared.Infrastructure.Validation;

namespace PlaytestHub.Modules.Identity.Endpoints;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapPost("/register", RegisterAsync)
            .WithRequestValidation<RegisterRequest>()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", LoginAsync)
            .WithRequestValidation<LoginRequest>()
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization()
            .Produces<UserResponse>();
    }

    private static async Task<IResult> RegisterAsync(RegisterRequest request, IUserService userService)
    {
        var result = await userService.RegisterAsync(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> LoginAsync(LoginRequest request, IUserService userService)
    {
        var result = await userService.LoginAsync(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> GetCurrentUserAsync(ClaimsPrincipal principal, IUserService userService)
    {
        var result = await userService.GetByIdAsync(principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }
}