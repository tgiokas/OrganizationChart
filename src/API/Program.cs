using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using ExternalIntegrations.OrganizationChart.Application;
using ExternalIntegrations.OrganizationChart.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Health checks
builder.Services.AddHealthChecks();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

Log.Information("OrganizationChart is starting...");
builder.Host.UseSerilog();

// Add Application services
builder.Services.AddApplicationServices();

// Infrastructure services (DB, Repositories, Kafka, HttpClient)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Authentication — validates partner's JWT from Keycloak (client_credentials)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["KEYCLOAK_AUTHORITY"]
            ?? throw new InvalidOperationException("KEYCLOAK_AUTHORITY is not configured");
        options.RequireHttpsMetadata = false; // TODO: set to true in production

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["KEYCLOAK_AUDIENCE"] ?? "account",
            ValidateIssuer = true,
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
