using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PlaytestHub.Shared.Infrastructure.Validation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder) =>
        builder
            .AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();
}