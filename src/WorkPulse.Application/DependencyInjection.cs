using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Services;

namespace WorkPulse.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		Assembly assembly = typeof(DependencyInjection).Assembly;
		services.AddMediatR(delegate(MediatRServiceConfiguration cfg)
		{
			cfg.RegisterServicesFromAssembly(assembly);
			cfg.AddOpenBehavior(typeof(LoggingBehavior<, >));
			cfg.AddOpenBehavior(typeof(ValidationBehavior<, >));
			cfg.AddOpenBehavior(typeof(TransactionBehavior<, >));
		});
		services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped, null, includeInternalTypes: true);
		services.AddAutoMapper(assembly);
		services.AddScoped<ITaskCollaborationService, TaskCollaborationService>();
		return services;
	}
}
