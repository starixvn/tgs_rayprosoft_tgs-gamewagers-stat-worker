using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Messages;

public class GameWagerPlayerCheckMessageHandler(
	ILogger<GameWagerPlayerCheckMessageHandler> logger,
	ICommandService<GameWagerPlayerCheckCommand> service) : IMessageHandler<GameWagersStatWorkerMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatWorkerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new GameWagerPlayerCheckCommand(
				PlayerCode: data.PlayerCode),
			cancellationToken: cancellationToken);
	}
}
