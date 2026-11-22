#!/usr/bin/env bash
# Generates the backend skeleton for playtest-hub (modular monolith, .NET 10).
# Run from anywhere inside the repo:  bash scripts/bootstrap-backend.sh
set -euo pipefail

cd "$(dirname "$0")/.."            # repo root
if [ -d backend ]; then
  echo "backend/ already exists - aborting so nothing is overwritten." >&2
  exit 1
fi

MODULES=(Identity Studio Access Feedback Reputation)
P=PlaytestHub

# Pin the backend to the newest installed .NET 10 (LTS) SDK, even if newer SDKs exist.
SDK10=$(dotnet --list-sdks | awk '/^10\./ {v=$1} END {print v}')
if [ -z "$SDK10" ]; then
  echo "No .NET 10 SDK found. Install it with: ./dotnet-install.sh --channel 10.0" >&2
  exit 1
fi
echo "Using .NET SDK $SDK10"

mkdir -p backend && cd backend

# --- Solution-level files -------------------------------------------------
dotnet new globaljson --sdk-version "$SDK10" --roll-forward latestPatch
dotnet new editorconfig
dotnet new sln -n "$P" --format slnx

cat > Directory.Build.props <<'EOF'
<Project>
  <!-- Applies to every project under backend/ -->
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <!-- Module projects (not their Contracts) and Shared.Infrastructure need ASP.NET Core types -->
  <ItemGroup Condition="('$(MSBuildProjectName.StartsWith(`PlaytestHub.Modules.`))' == 'true' And '$(MSBuildProjectName.EndsWith(`.Contracts`))' != 'true') Or '$(MSBuildProjectName)' == 'PlaytestHub.Shared.Infrastructure'">
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>
EOF

# --- Projects ---------------------------------------------------------------
dotnet new webapi   -n "$P.Api"                   -o "src/$P.Api"
dotnet new classlib -n "$P.Shared.Kernel"         -o "src/Shared/$P.Shared.Kernel"
dotnet new classlib -n "$P.Shared.Infrastructure" -o "src/Shared/$P.Shared.Infrastructure"

for m in "${MODULES[@]}"; do
  dotnet new classlib -n "$P.Modules.$m"           -o "src/Modules/$m/$P.Modules.$m"
  dotnet new classlib -n "$P.Modules.$m.Contracts" -o "src/Modules/$m/$P.Modules.$m.Contracts"
done

dotnet new xunit -n "$P.UnitTests"         -o "tests/$P.UnitTests"
dotnet new xunit -n "$P.IntegrationTests"  -o "tests/$P.IntegrationTests"
dotnet new xunit -n "$P.ArchitectureTests" -o "tests/$P.ArchitectureTests"

find src -name Class1.cs -delete

# --- Project references -----------------------------------------------------
ref() { dotnet add "$1" reference "${@:2}" > /dev/null; }

KERNEL="src/Shared/$P.Shared.Kernel/$P.Shared.Kernel.csproj"
INFRA="src/Shared/$P.Shared.Infrastructure/$P.Shared.Infrastructure.csproj"
API="src/$P.Api/$P.Api.csproj"

ref "$INFRA" "$KERNEL"

MODULE_PROJECTS=()
for m in "${MODULES[@]}"; do
  mod="src/Modules/$m/$P.Modules.$m/$P.Modules.$m.csproj"
  con="src/Modules/$m/$P.Modules.$m.Contracts/$P.Modules.$m.Contracts.csproj"
  ref "$con" "$KERNEL"
  ref "$mod" "$KERNEL" "$INFRA" "$con"
  MODULE_PROJECTS+=("$mod")
done

ref "$API" "${MODULE_PROJECTS[@]}" "$INFRA"
ref "tests/$P.UnitTests/$P.UnitTests.csproj" "${MODULE_PROJECTS[@]}"
ref "tests/$P.IntegrationTests/$P.IntegrationTests.csproj" "$API"
ref "tests/$P.ArchitectureTests/$P.ArchitectureTests.csproj" "$API"

# --- Module entry points (one per module) -----------------------------------
for m in "${MODULES[@]}"; do
  dir="src/Modules/$m/$P.Modules.$m"
  mkdir -p "$dir"/{Domain,Application,Infrastructure,Endpoints}
  touch "$dir"/{Domain,Application,Infrastructure,Endpoints}/.gitkeep
  cat > "$dir/${m}Module.cs" <<EOF
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace $P.Modules.$m;

public static class ${m}Module
{
    public static IServiceCollection Add${m}Module(this IServiceCollection services, IConfiguration configuration)
    {
        // Register this module's DbContext, handlers and validators here.
        return services;
    }

    public static IEndpointRouteBuilder Map${m}Endpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/${m,,}").WithTags("$m");
        group.MapGet("/ping", () => Results.Ok(new { module = "$m", status = "ok" }));
        return app;
    }
}
EOF
done

# --- Api composition root (replaces the WeatherForecast template) ----------
{
  for m in "${MODULES[@]}"; do echo "using $P.Modules.$m;"; done
  cat <<'EOF'

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddIdentityModule(builder.Configuration)
    .AddStudioModule(builder.Configuration)
    .AddAccessModule(builder.Configuration)
    .AddFeedbackModule(builder.Configuration)
    .AddReputationModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapIdentityEndpoints();
app.MapStudioEndpoints();
app.MapAccessEndpoints();
app.MapFeedbackEndpoints();
app.MapReputationEndpoints();

app.Run();
EOF
} > "src/$P.Api/Program.cs"

# --- Add everything to the solution and verify --------------------------------
dotnet sln "$P.slnx" add $(find src tests -name '*.csproj' | sort)

dotnet build "$P.slnx"
dotnet test "$P.slnx" --no-build

echo
echo "Done. Run the API with:  dotnet run --project backend/src/$P.Api"
