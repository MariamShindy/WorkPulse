using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using WorkPulse.API.Authorization;
using WorkPulse.API.Hubs;
using WorkPulse.API.Middleware;
using WorkPulse.API.Services;
using WorkPulse.Application;
using WorkPulse.Application.Abstractions;
using WorkPulse.Domain.Enums;
using WorkPulse.Infrastructure;
using WorkPulse.Infrastructure.Extensions;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console());

// ── Application + Infrastructure ─────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// ── Authentication / JWT ─────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };

        // Allow JWT in SignalR query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) &&
                    ctx.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    ctx.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.RequireAuthenticated, policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy(AuthorizationPolicies.RequireCompanyAdmin, policy =>
        policy
            .RequireAuthenticatedUser()
            .AddRequirements(new CompanyRoleRequirement(
                CompanyMemberRole.Admin,
                CompanyMemberRole.Owner)));

    options.AddPolicy(AuthorizationPolicies.RequireCompanyOwner, policy =>
        policy
            .RequireAuthenticatedUser()
            .AddRequirements(new CompanyRoleRequirement(CompanyMemberRole.Owner)));
});

builder.Services.AddScoped<IAuthorizationHandler, CompanyRoleAuthorizationHandler>();

// ── API infrastructure ────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddScoped<ITaskRealtimeNotifier, RealtimeNotifier>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

// ── Exception handling ────────────────────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ── Health checks ─────────────────────────────────────────────────────────────
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHealthChecks();
}
else
{
    builder.Services
        .AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres")
        .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis");
}

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.SeedDevelopmentDataAsync();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "WorkPulse API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();

app.UseAuthentication();
app.UseMiddleware<TenantMembershipMiddleware>();
app.UseAuthorization();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [] // TODO: add auth filter before production
    });
    app.Services.ScheduleRecurringJobs();
}

app.MapControllers();
app.MapHub<WorkPulseHub>("/hubs/workpulse");
app.MapHealthChecks("/health");

app.Run();

public partial class Program; // exposed for integration test host
