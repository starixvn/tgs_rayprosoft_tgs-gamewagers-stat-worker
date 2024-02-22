using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;

internal class GameWagerBiddingCommandService(
	ILogger<GameWagerBiddingCommandService> logger,
	IGameWagerStatCloseBiddingRepository biddingRepository,
	IMessageSender messageSender) : ICommandService<GameWagerBiddingCommand>
{
	public async ValueTask ExecuteAsync(GameWagerBiddingCommand command, CancellationToken cancellationToken = default)
	{
		var token = await biddingRepository.BiddingAsync(
			playerCode: command.PlayerCode,
			token: Environment.MachineName,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var publishSubject = string.IsNullOrWhiteSpace(token) ?
			throw new Exception($"The bidding process failed for this player.({command.PlayerCode})")
			: $"gamewagers.stat.worker.{token}";

		var message = new GameWagersStatWorkerMessage
		{
			PlayerCode = command.PlayerCode
		};

		await messageSender.PublishAsync(
			subject: publishSubject,
			data: message,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			publishSubject,
			message);
	}
}
