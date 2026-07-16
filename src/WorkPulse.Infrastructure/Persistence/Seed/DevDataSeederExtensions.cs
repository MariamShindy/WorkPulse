using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WorkPulse.Infrastructure.Persistence.Seed;

public static class DevDataSeederExtensions
{
	public static async Task SeedDevelopmentDataAsync(this IServiceProvider scopedServices, CancellationToken ct = default(CancellationToken))
	{
		DevDataSeeder seeder = ActivatorUtilities.CreateInstance<DevDataSeeder>(scopedServices, Array.Empty<object>());
		await seeder.SeedAsync(ct);
	}
}
