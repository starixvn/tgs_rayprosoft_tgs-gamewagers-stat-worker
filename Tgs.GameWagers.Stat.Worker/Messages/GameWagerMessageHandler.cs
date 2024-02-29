using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Messages;

public class GameWagerMessageHandler(
	ILogger<GameWagerMessageHandler> logger,
	ICommandService<GameWagerCheckCommand> service) : IMessageHandler<GameWagersFirstsMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersFirstsMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				data.WagerId,
				GameProviderId = data.GameProviderId.ToGuid(),
				data.GameId,
				data.PlayerCode,
				WalletId = data.WalletId.ToGuid(),
				data.ExternalWagerId,
				CreatedOn = data.CreatedOn.ToDateTimeOffset(),
				ClosedOn = data.ClosedOn.ToDateTimeOffset()
			});

		return service.ExecuteAsync(
			command: new GameWagerCheckCommand(
				GameProviderId: data.GameProviderId.ToGuid(),
				GameId: data.GameId,
				WalletId: data.WalletId.ToGuid(),
				ExternalWagerId: data.ExternalWagerId,
				CreatedOn: data.CreatedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
