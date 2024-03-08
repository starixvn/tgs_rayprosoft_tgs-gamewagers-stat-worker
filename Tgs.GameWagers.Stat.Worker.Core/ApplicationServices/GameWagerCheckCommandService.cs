using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Client;
using Tgs.GameWagers.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

internal class GameWagerCheckCommandService(
	ILogger<GameWagerCheckCommandService> logger,
	TimeProvider timeProvider,
	IGameWagersClient gameWagersClient,
	ICommandService<GameWagerDataCommand> service) : ICommandService<GameWagerCheckCommand>
{
	public async ValueTask ExecuteAsync(GameWagerCheckCommand command, CancellationToken cancellationToken = default)
	{
		GameWagerFirstInfo? info;
		try
		{
			info = await gameWagersClient.GameWagerFirstGetAsync(
			   gameProviderId: command.GameProviderId,
			   gameId: command.GameId,
			   walletId: command.WalletId,
			   externalWagerId: command.ExternalWagerId,
			   createdOn: command.CreatedOn,
			   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Mesaage:This game wager cannot be found.({command})",
				DateTime.Now.ToString("o"),
				command);
			throw;
		}

		if (info == null)
		{
			logger.LogError("LogOn:{logOn} Mesaage:This game wager cannot be found.({command})",
				DateTime.Now.ToString("o"),
				command);

			throw new ArgumentOutOfRangeException($"This game wager cannot be found.({JsonSerializer.Serialize(command)})");
		}

		var currentTime = timeProvider.GetUtcNow();
		var difference = currentTime.Subtract(info.ClosedOn);

		if (difference.TotalDays > 45)
		{
			logger.LogWarning("LogOn:{logOn} Mesaage:This game wager({wagerId}) has been settled for more than 45 days.({totalDays})",
				DateTime.Now.ToString("o"),
				info.WagerId,
				difference.TotalDays);

			throw new Exception($"This game bet({info.WagerId}) has been settled for more than 45 days.({difference.TotalDays})");
		}

		if (info.ClosedState != GameWagers.Common.Enums.ClosedState.Closed)
		{
			logger.LogWarning("LogOn:{logOn} Mesaage:This game wager{wagerId} has not been settled yet.({closedState})",
				DateTime.Now.ToString("o"),
				info.WagerId,
				info.ClosedState);

			throw new Exception($"This game wager{info.WagerId} has not been settled yet.({info.ClosedState})");
		}

		await service.ExecuteAsync(
		   command: new GameWagerDataCommand(
			   GameProviderId: info.GameProviderId,
			   GameId: info.GameId,
			   ResellerId: info.ResellerId,
			   LicenseeId: info.LicenseeId,
			   BrandId: info.BrandId,
			   PlayerCode: info.PlayerCode,
			   WalletId: info.WalletId,
			   ExternalRoundId: info.ExternalRoundId,
			   ExternalWagerId: info.ExternalWagerId,
			   CurrencyRatio: info.CurrencyRatio,
			   Bet: info.Bet,
			   ValidBet: info.ValidBet,
			   Settle: info.Settle,
			   Commission: info.Commission,
			   Turnover: info.Turnover,
			   WinLoss: info.WinLoss,
			   ClosedOn: info.ClosedOn),
		   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}
