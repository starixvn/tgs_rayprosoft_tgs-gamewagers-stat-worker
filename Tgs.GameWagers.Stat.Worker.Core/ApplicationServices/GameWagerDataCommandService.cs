using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
internal class GameWagerDataCommandService(
	IStatisticalTimeUtnsil utnsil,
	IGameWagerStatCloseQueueRepository queueRepository,
	ICommandService<GameWagerBiddingCommand> service) : ICommandService<GameWagerDataCommand>
{
	public async ValueTask ExecuteAsync(GameWagerDataCommand command, CancellationToken cancellationToken = default)
	{
		var statOn = utnsil.GetStatOn(command.ClosedOn);

		var add = new GameWagerStatCloseAdd(
			GameProviderId: command.GameProviderId,
			GameId: command.GameId,
			ResellerId: command.ResellerId,
			LicenseeId: command.LicenseeId,
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			WalletId: command.WalletId,
			ExternalRoundId: command.ExternalRoundId,
			ExternalWagerId: command.ExternalWagerId,
			StatOn: statOn,
			Bet: command.Bet,
			ValidBet: command.ValidBet,
			Settle: command.Settle,
			Commission: command.Commission,
			Turnover: command.Turnover,
			WinLoss: command.WinLoss);

		await queueRepository.PushAsync(
			playerCode: command.PlayerCode,
			source: add,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await service.ExecuteAsync(
			command: new GameWagerBiddingCommand(command.PlayerCode),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
