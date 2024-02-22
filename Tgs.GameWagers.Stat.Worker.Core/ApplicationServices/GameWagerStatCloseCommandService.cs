using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

internal class GameWagerStatCloseCommandService(
	ILogger<GameWagerStatCloseCommandService> logger,
	IGameWagerStatCloseQueueRepository gameWagerStatCloseQueueRepository,
	IGameWagerStatCloseRepository gameWagerStatCloseRepository) : ICommandService<GameWagerStatCloseCommand>
{
	public async ValueTask ExecuteAsync(GameWagerStatCloseCommand command, CancellationToken cancellationToken = default)
	{
		GameWagerStatCloseAdd? source;
		do
		{
			source = await gameWagerStatCloseQueueRepository.PopAsync(
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("LogOn:{logOn} Mesaage:This player({playerCode}) gets information.({source})",
				DateTime.Now.ToString("o"),
				command.PlayerCode,
				source);

			if (source is not null)
			{
				await gameWagerStatCloseRepository.AddAsync(
					source: source,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await gameWagerStatCloseQueueRepository.RemoveAsync(
					playerCode: command.PlayerCode,
					source: source,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
		} while (source != null);
	}
}
