using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Worker;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatWorker(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddSingleton(TimeProvider.System)
		.AddGameWagersStatWorkerCore()
		.AddGameWagersStatWorkerRepositories(
			dbContextOptions: dbContextOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
