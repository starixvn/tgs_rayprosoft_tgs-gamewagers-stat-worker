using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

internal class GameWagerCheckCommandService(
	ILogger<GameWagerCheckCommandService> logger,
	TimeProvider timeProvider,
	ICommandService<GameWagerDataCommand> service) : ICommandService<GameWagerCheckCommand>
{
	public async ValueTask ExecuteAsync(GameWagerCheckCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var difference = currentTime.Subtract(command.ClosedOn);

		if (difference.TotalDays > 45)
		{
			logger.LogWarning("LogOn:{logOn} Mesaage:This game wager({wagerId}) has been settled for more than 45 days.({totalDays})",
				DateTime.Now.ToString("o"),
				command.ExternalBetId,
				difference.TotalDays);

			throw new Exception($"This game bet({command.ExternalBetId}) has been settled for more than 45 days.({difference.TotalDays})");
		}

		await service.ExecuteAsync(
		   command: new GameWagerDataCommand(
			   GameProviderId: command.GameProviderId,
			   GameId: command.GameId,
			   ResellerId: command.ResellerId,
			   LicenseeId: command.LicenseeId,
			   BrandId: command.BrandId,
			   PlayerCode: command.PlayerCode,
			   WalletId: command.WalletId,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   CurrencyRatio: command.CurrencyRatio,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   ClosedOn: command.ClosedOn),
		   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}
