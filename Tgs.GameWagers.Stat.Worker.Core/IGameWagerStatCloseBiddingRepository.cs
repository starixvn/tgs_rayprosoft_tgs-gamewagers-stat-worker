namespace Tgs.GameWagers.Stat.Worker.Core;

public interface IGameWagerStatCloseBiddingRepository
{
	ValueTask<string> BidAsync(string playerCode, string token, CancellationToken cancellationToken = default);

	ValueTask<string?> GetTokenAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask DiscardAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<bool> ExtendAsync(string playerCode, CancellationToken cancellationToken = default);
}
