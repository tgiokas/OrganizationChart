using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

using DotNetEnv;
using Serilog;

using IntegrationImport.Api.Services;
using IntegrationImport.Application;
using IntegrationImport.Application.Configuration;
using IntegrationImport.Infrastructure;
using IntegrationImport.Infrastructure.Database;
using IntegrationImport.Api.Middlewares;

//ONLY FOR LOCAL RUN
var environment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
    Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

Console.WriteLine($"ENVIRONMENT = {environment}");
Console.WriteLine($"CurrentDirectory = {Directory.GetCurrentDirectory()}");

if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
{
    Env.TraversePath().Load();
    Console.WriteLine("Loaded .env");
}
else
{
    Console.WriteLine(".env not loaded because environment is not Development");
}


var builder = WebApplication.CreateBuilder(args);

var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "postgresql";

// Health checks
builder.Services.AddHealthChecks();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

Log.Information("IntegrationImport is starting...");
builder.Host.UseSerilog();

// Application + Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, dbProvider);

// Bind KeycloakSettings early so we can use it for JWT config
var keycloakSettings = KeycloakSettings.BindFromConfiguration(builder.Configuration);
builder.Services.AddSingleton<KeycloakRoleMapper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakSettings.Authority;
        options.Audience = keycloakSettings.ClientId;
        options.RequireHttpsMetadata = keycloakSettings.RequireHttpsMetadata;

        //change audince =true when go live
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = keycloakSettings.Authority,
            ValidateAudience = false,
            ValidAudiences = [keycloakSettings.ClientId],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var roleMapper = context.HttpContext.RequestServices.GetRequiredService<KeycloakRoleMapper>();
                roleMapper.MapRolesToClaims(context);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "IntegrationImport.Api",
            Version = "v1"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a valid Bearer token."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    });
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


await app.ApplyMigrationsAsync(dbProvider);

app.UseCors("CorsPolicy");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LogMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("Application is starting...");

app.Run();

