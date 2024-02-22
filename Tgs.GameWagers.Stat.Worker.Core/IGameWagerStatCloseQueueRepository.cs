using Tgs.GameWagers.Stat.Worker.Core.Models;

namespace Tgs.GameWagers.Stat.Worker.Core;

public interface IGameWagerStatCloseQueueRepository
{
	ValueTask PushAsync(string playerCode, GameWagerStatCloseAdd source, CancellationToken cancellationToken = default);

	ValueTask<GameWagerStatCloseAdd?> PopAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask RemoveAsync(string playerCode, GameWagerStatCloseAdd source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(string playerCode, CancellationToken cancellationToken = default);
}
