using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.GameWagers.Stat.Worker.Core;
using Tgs.GameWagers.Stat.Worker.Entities;
using Tgs.GameWagers.Stat.Worker.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatWorkerRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<IGameWagerStatCloseRepository, GameWagerStatCloseRepository>()
		.AddTransient<IGameBetFirstRepository, GameBetFirstRepository>()
		.AddSingleton<IGameWagerStatCloseBiddingRepository>(sp => ActivatorUtilities.CreateInstance<GameWagerStatCloseBiddingRepository>(sp, redisDatabaseResolver(sp)))
		.AddSingleton<IGameWagerStatCloseQueueRepository>(sp => ActivatorUtilities.CreateInstance<GameWagerStatCloseQueueRepository>(sp, redisDatabaseResolver(sp)));
}
