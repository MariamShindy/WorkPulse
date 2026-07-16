using System;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Abstractions.Options;
using WorkPulse.Domain.Repositories;
using WorkPulse.Infrastructure.Jobs;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Persistence.Interceptors;
using WorkPulse.Infrastructure.Services;
using WorkPulse.Infrastructure.Services.Email;
using WorkPulse.Infrastructure.Services.Read;

namespace WorkPulse.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment? environment = null)
	{
		services.AddHttpContextAccessor();
		services.AddAutoMapper(typeof(DependencyInjection).Assembly);
		AddPersistence(services, configuration);
		AddIdentity(services);
		AddCaching(services, configuration);
		if (environment == null || !environment.IsEnvironment("Testing"))
		{
			AddHangfire(services, configuration);
		}
		AddServices(services, configuration, environment);
		return services;
	}

	private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<AuditableEntityInterceptor>();
		services.AddScoped<AuditLogInterceptor>();
		services.AddDbContext<ApplicationDbContext>(delegate(IServiceProvider sp, DbContextOptionsBuilder options)
		{
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), delegate(NpgsqlDbContextOptionsBuilder npgsql)
			{
				npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
				npgsql.EnableRetryOnFailure(3);
				npgsql.CommandTimeout(30);
			});
		});
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped((Func<IServiceProvider, IApplicationDbContext>)((IServiceProvider sp) => sp.GetRequiredService<ApplicationDbContext>()));
	}

	private static void AddIdentity(IServiceCollection services)
	{
		services.AddIdentity<ApplicationUser, ApplicationRole>(delegate(IdentityOptions options)
		{
			options.Password.RequiredLength = 8;
			options.Password.RequireDigit = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireUppercase = true;
			options.Password.RequireNonAlphanumeric = false;
			options.User.RequireUniqueEmail = true;
			options.Lockout.MaxFailedAccessAttempts = 5;
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15L);
		}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
	}

	private static void AddCaching(IServiceCollection services, IConfiguration configuration)
	{
		services.AddStackExchangeRedisCache(delegate(RedisCacheOptions options)
		{
			options.Configuration = configuration.GetConnectionString("Redis");
			options.InstanceName = "WorkPulse_";
		});
		services.AddScoped<ICacheService, CacheService>();
	}

	private static void AddHangfire(IServiceCollection services, IConfiguration configuration)
	{
		services.AddHangfire(delegate(IGlobalConfiguration config)
		{
			config.UsePostgreSqlStorage(delegate(PostgreSqlBootstrapperOptions c)
			{
				c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
			});
		});
		services.AddHangfireServer(delegate(BackgroundJobServerOptions options)
		{
			options.WorkerCount = Environment.ProcessorCount * 2;
			options.Queues = new string[3] { "critical", "default", "low" };
		});
		services.AddScoped<OutboxProcessorJob>();
		services.AddScoped<DeadlineReminderJob>();
		services.AddScoped<SlaMonitorJob>();
	}

	private static void AddServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment? environment)
	{
		services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
		services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
		services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddScoped((Func<IServiceProvider, ITenantContext>)((IServiceProvider sp) => sp.GetRequiredService<TenantContext>()));
		services.AddScoped<TenantContext>();
		services.AddSingleton<IDateTime, DateTimeService>();
		services.AddScoped<ICacheService, CacheService>();
		services.AddScoped<IFileStorageService, LocalFileStorageService>();
		services.AddScoped<IAnalyticsReadService, AnalyticsReadService>();
		services.AddScoped<ISearchReadService, SearchReadService>();
		services.AddScoped<IReportsReadService, ReportsReadService>();
		services.AddScoped<IJwtTokenService, JwtTokenService>();
		services.AddScoped<IUserIdentityService, IdentityUserService>();
		services.AddScoped<IOutboxWriter, OutboxWriter>();
		services.AddScoped<IAutomationEngine, AutomationEngine>();
		services.AddScoped<IExportService, ExportService>();
		services.AddScoped<ITaskRealtimeNotifier, NullTaskRealtimeNotifier>();
		if (environment != null && environment.IsDevelopment())
		{
			services.AddScoped<IEmailService, DevEmailService>();
		}
		else
		{
			services.AddScoped<IEmailService, SmtpEmailService>();
		}
	}
}
