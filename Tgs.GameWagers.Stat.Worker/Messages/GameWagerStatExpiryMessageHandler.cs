using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Messages;

public class GameWagerStatExpiryMessageHandler(
	ILogger<GameWagerStatExpiryMessageHandler> logger,
	IGameWagerStatDispatcher dispatcher) : IMessageHandler<GameWagersStatWorkerMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatWorkerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		dispatcher.ExtendExpiry(playerCode: data.PlayerCode);

		return ValueTask.CompletedTask;
	}
}
