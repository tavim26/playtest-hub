using PlaytestHub.Modules.Access;
using PlaytestHub.Modules.Feedback;
using PlaytestHub.Modules.Identity;
using PlaytestHub.Modules.Reputation;
using PlaytestHub.Modules.Studio;
using PlaytestHub.Shared.Infrastructure;
using PlaytestHub.Shared.Infrastructure.Database;

using PlaytestHub.Shared.Infrastructure.Authentication;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var connectionString = builder.Configuration.GetConnectionString("Database")
                       ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

builder.Services.AddOpenApi();
builder.Services.AddSharedInfrastructure();
builder.Services.AddSharedInfrastructure();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddHealthChecks().AddNpgSql(connectionString);

builder.Services
    .AddIdentityModule(builder.Configuration)
    .AddStudioModule(builder.Configuration)
    .AddAccessModule(builder.Configuration)
    .AddFeedbackModule(builder.Configuration)
    .AddReputationModule(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapIdentityEndpoints();
app.MapStudioEndpoints();
app.MapAccessEndpoints();
app.MapFeedbackEndpoints();
app.MapReputationEndpoints();

app.Run();