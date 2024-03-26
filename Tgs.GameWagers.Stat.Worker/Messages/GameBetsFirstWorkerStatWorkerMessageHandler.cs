using Tgs.GameBets.First.Worker.Common.Protos;
using Tgs.GameBets.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Messages;

public class GameBetsFirstWorkerStatWorkerMessageHandler(
	ILogger<GameBetsFirstWorkerStatWorkerMessageHandler> logger,
	ICommandService<GameWagerCheckCommand> service) : IMessageHandler<GameBetsFirstWorkerStatMessage>
{
	public ValueTask HandleAsync(string subject, GameBetsFirstWorkerStatMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				GameProviderId = data.GameProviderId.ToGuid(),
				data.GameId,
				data.PlayerCode,
				WalletId = data.WalletId.ToGuid(),
				data.ExternalRoundId,
				data.ExternalBetId,
				ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset()
			});

		return service.ExecuteAsync(
			command: new GameWagerCheckCommand(
				GameProviderId: data.GameProviderId.ToGuid(),
				GameId: data.GameId,
				WalletId: data.WalletId.ToGuid(),
				ExternalBetId: data.ExternalBetId,
				ExternalCreatedOn: data.ExternalCreatedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
