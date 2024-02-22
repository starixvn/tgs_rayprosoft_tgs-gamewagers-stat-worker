using System.Reflection;
using Tgs.GameWagers.Stat.Worker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatWorkerCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton<IStatisticalTimeUtnsil, StatisticalTimeUtnsil>();
}
