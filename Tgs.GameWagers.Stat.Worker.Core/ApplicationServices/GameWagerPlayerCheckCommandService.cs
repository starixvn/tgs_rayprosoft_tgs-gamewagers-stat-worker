using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

internal class GameWagerPlayerCheckCommandService(
	IGameWagerStatCloseQueueRepository gameWagerStatCloseQueueRepository,
	ICommandService<GameWagerBiddingCommand> service) : ICommandService<GameWagerPlayerCheckCommand>
{
	public async ValueTask ExecuteAsync(GameWagerPlayerCheckCommand command, CancellationToken cancellationToken = default)
	{
		if (await gameWagerStatCloseQueueRepository.ExistAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			await service.ExecuteAsync(
				command: new GameWagerBiddingCommand(
					command.PlayerCode),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
