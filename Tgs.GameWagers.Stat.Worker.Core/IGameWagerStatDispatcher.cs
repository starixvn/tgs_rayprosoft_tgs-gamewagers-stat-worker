using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Worker.Core;

public interface IGameWagerStatDispatcher
{
	ValueTask PushCommandAsync(string playerCode, GameWagerStatCloseCommand command, CancellationToken cancellationToken = default);

	void ExtendExpiry(string playerCode);
}
