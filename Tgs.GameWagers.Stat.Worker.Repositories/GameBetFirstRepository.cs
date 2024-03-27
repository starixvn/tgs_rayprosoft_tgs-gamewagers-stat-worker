using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Worker.Core;
using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.GameWagers.Stat.Worker.Entities;

namespace Tgs.GameWagers.Stat.Worker.Repositories;

internal class GameBetFirstRepository(
	TgsContext context) : IGameBetFirstRepository
{
	public async ValueTask<GameBetFirstInfo?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameBetFirsts
		.Where(gbs => gbs.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbs.GameProviderId == gameProviderId &&
			gbs.GameId == gameId &&
			gbs.WalletId == walletId &&
			gbs.ExternalBetId == externalBetId)
		.Select(gbs => new GameBetFirstInfo(
				gbs.Id,
				gbs.GameBetId,
				gbs.GameProviderId,
				gbs.GameProviderConfigurationId,
				gbs.GameId,
				gbs.ResellerId,
				gbs.LicenseeId,
				gbs.BrandId,
				gbs.PlayerCode,
				gbs.CurrencyCode,
				gbs.WalletId,
				gbs.ExternalRoundId,
				gbs.ExternalBetId,
				gbs.ExternalCreatedOn,
				gbs.ExternalClosedOn,
				gbs.ExternalPayoffOn,
				gbs.ExternalResettleOn,
				gbs.CurrencyRatio,
				gbs.Bet,
				gbs.ValidBet,
				gbs.Settle,
				gbs.Commission,
				gbs.Turnover,
				gbs.WinLoss,
				gbs.Jackpot,
				gbs.CreatedOn,
				gbs.ClosedOn,
				gbs.UpdatedOn))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
