using System.Text.Json;
using StackExchange.Redis;
using Tgs.GameWagers.Stat.Worker.Core;
using Tgs.GameWagers.Stat.Worker.Core.Models;

namespace Tgs.GameWagers.Stat.Worker.Repositories;

internal class GameWagerStatCloseQueueRepository(IDatabase database) : IGameWagerStatCloseQueueRepository
{
	public async ValueTask<bool> ExistAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await database.KeyExistsAsync(GetKey(playerCode)).ConfigureAwait(false);

	public async ValueTask<GameWagerStatCloseAdd?> PopAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var value = await database.ListRightPopLeftPushAsync(
			source: GetKey(playerCode),
			destination: GetKey(playerCode))
			.ConfigureAwait(false);

		return string.IsNullOrEmpty(value) ? null : JsonSerializer.Deserialize<GameWagerStatCloseAdd>(value!)!;
	}

	public async ValueTask PushAsync(string playerCode, GameWagerStatCloseAdd source, CancellationToken cancellationToken = default)
		=> await database.ListLeftPushAsync(GetKey(playerCode), JsonSerializer.Serialize(source)).ConfigureAwait(false);

	public async ValueTask RemoveAsync(string playerCode, GameWagerStatCloseAdd source, CancellationToken cancellationToken = default)
		=> await database.ListRemoveAsync(GetKey(playerCode), JsonSerializer.Serialize(source), 0).ConfigureAwait(false);

	private static string GetKey(string playerCode)
		=> string.Concat(typeof(GameWagerStatCloseQueueRepository).Namespace!.Replace(".", "-").ToLower(), ":", "pending", ":", playerCode);
}
