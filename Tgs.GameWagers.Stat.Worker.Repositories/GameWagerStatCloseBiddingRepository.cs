using StackExchange.Redis;
using Tgs.GameWagers.Stat.Worker.Core;

namespace Tgs.GameWagers.Stat.Worker.Repositories;

internal class GameWagerStatCloseBiddingRepository(IDatabase database) : IGameWagerStatCloseBiddingRepository
{
	public async ValueTask<string> BidAsync(string playerCode, string token, CancellationToken cancellationToken = default)
	{
		var redisKey = GetKey(playerCode);

		var isGot = await database.StringSetAsync(
			key: GetKey(playerCode),
			value: token,
			expiry: TimeSpan.FromMinutes(10D),
			when: When.NotExists)
			.ConfigureAwait(false);

		return isGot
			? token
			: (string?)await database.StringGetAsync(redisKey)
			.ConfigureAwait(false)
			?? string.Empty;
	}

	public async ValueTask DiscardAsync(string playerCode, CancellationToken cancellationToken = default)
		=> _ = await database.KeyDeleteAsync(GetKey(playerCode)).ConfigureAwait(false);

	public async ValueTask<bool> ExtendAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await database.KeyExpireAsync(
			key: GetKey(playerCode),
			expiry: TimeSpan.FromMinutes(10D),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);

	public async ValueTask<string?> GetTokenAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await database.StringGetAsync(GetKey(playerCode)).ConfigureAwait(false);

	private static string GetKey(string playerCode)
		=> string.Concat(typeof(GameWagerStatCloseBiddingRepository).Namespace!.Replace(".", "-").ToLower(), ":", "bidding", ":", playerCode);
}
