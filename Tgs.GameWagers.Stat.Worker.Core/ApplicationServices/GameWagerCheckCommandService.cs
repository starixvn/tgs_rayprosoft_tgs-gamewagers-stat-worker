using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

internal class GameWagerCheckCommandService(
	ILogger<GameWagerCheckCommandService> logger,
	TimeProvider timeProvider,
	ICommandService<GameWagerDataCommand> service) : ICommandService<GameWagerCheckCommand>
{
	public ValueTask ExecuteAsync(GameWagerCheckCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var difference = currentTime.Subtract(command.ClosedOn);

		if (difference.TotalDays > 45)
		{
			logger.LogWarning("LogOn:{logOn} Mesaage:The settlement time of this game platform exceeds the current time by 45 days.({command})",
				DateTime.Now.ToString("o"),
				command);
			return ValueTask.CompletedTask;
		}

		if (command.ClosedState != GameRecord.Handle.Common.Protos.ClosedStates.Closed)
		{
			logger.LogWarning("LogOn:{logOn} Mesaage:This Game Wager has not been settled yet.({command})",
				DateTime.Now.ToString("o"),
				command);
			return ValueTask.CompletedTask;
		}

		return service.ExecuteAsync(
			command: new GameWagerDataCommand(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				ExternalRoundId: command.ExternalRoundId,
				ExternalWagerId: command.ExternalWagerId,
				ExternalGameId: command.ExternalGameId,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				ClosedOn: command.ClosedOn),
			cancellationToken: cancellationToken);
	}
}
