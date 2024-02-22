using System;
using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Worker;
using Tgs.GameWagers.Stat.Worker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatWorker(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions,
		Action<GameWagersStatWorkerSettingsOptions, IServiceProvider> gameWagersStatWorkerSettingsOptions)
		=> services
		.AddOptions<GameWagersStatWorkerSettingsOptions>()
		.Configure(gameWagersStatWorkerSettingsOptions)
		.Services
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
