using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Worker.Core;
using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.GameWagers.Stat.Worker.Entities;

namespace Tgs.GameWagers.Stat.Worker.Repositories;

internal class GameWagerStatCloseRepository(TgsContext context) : IGameWagerStatCloseRepository
{
	public async ValueTask AddAsync(GameWagerStatCloseAdd source, CancellationToken cancellationToken = default)
	{
		context.ChangeTracker.Clear();

		var gameWagerStatClose = await context.GameWagerStatCloses.Where(
			predicate: close => close.StatOn == source.StatOn.UtcDateTime &&
				close.ResellerId == source.ResellerId &&
				close.LicenseeId == source.LicenseeId &&
				close.BrandId == source.BrandId &&
				close.PlayerCode == source.PlayerCode &&
				close.GameProviderId == source.GameProviderId &&
				close.GameId == source.GameId)
			.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (gameWagerStatClose is null)
		{
			var entity = new GameWagerStatClose
			{
				ResellerId = source.ResellerId,
				LicenseeId = source.LicenseeId,
				BrandId = source.BrandId,
				PlayerCode = source.PlayerCode,
				GameProviderId = source.GameProviderId,
				GameId = source.GameId,
				StatOn = source.StatOn.UtcDateTime,
				Count = 1,
				Bet = source.Bet,
				ValidBet = source.ValidBet,
				Settle = source.Settle,
				Commission = source.Commission,
				Turnover = source.Turnover,
				WinLoss = source.WinLoss,
				CreatedOn = DateTime.UtcNow,
				UpdatedOn = DateTime.UtcNow
			};
			_ = await context.GameWagerStatCloses.AddAsync(
				entity: entity,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			gameWagerStatClose.Count += 1;
			gameWagerStatClose.Bet += source.Bet;
			gameWagerStatClose.ValidBet += source.ValidBet;
			gameWagerStatClose.Settle += source.Settle;
			gameWagerStatClose.Commission += source.Commission;
			gameWagerStatClose.Turnover += source.Turnover;
			gameWagerStatClose.WinLoss += source.WinLoss;
			gameWagerStatClose.UpdatedOn = DateTime.UtcNow;
		}

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
