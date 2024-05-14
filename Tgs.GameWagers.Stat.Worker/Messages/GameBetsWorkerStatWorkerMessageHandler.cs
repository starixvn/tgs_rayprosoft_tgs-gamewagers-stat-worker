using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Messages;

public class GameBetsWorkerStatWorkerMessageHandler(
	ILogger<GameBetsWorkerStatWorkerMessageHandler> logger,
	ICommandService<GameWagerCheckCommand> service) : IMessageHandler<GameBets.Worker.Common.Protos.GameBetsWorkerPublish>
{
	public ValueTask HandleAsync(string subject, GameBets.Worker.Common.Protos.GameBetsWorkerPublish data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				GameBetId = data.GameBetId.ToGuid(),
				GameProviderId = data.GameProviderId.ToGuid(),
				data.GameId,
				ResellerId = data.ResellerId.ToGuid(),
				LicenseeId = data.LicenseeId.ToGuid(),
				BrandId = data.BrandId.ToGuid(),
				data.PlayerCode,
				data.CurrencyCode,
				WalletId = data.WalletId.ToGuid(),
				data.ExternalRoundId,
				data.ExternalBetId,
				ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset()
			});

		return service.ExecuteAsync(
			command: new GameWagerCheckCommand(
				GameProviderId: data.GameProviderId.ToGuid(),
				GameId: data.GameId,
				ResellerId: data.ResellerId.ToGuid(),
				LicenseeId: data.LicenseeId.ToGuid(),
				BrandId: data.BrandId.ToGuid(),
				PlayerCode: data.PlayerCode,
				WalletId: data.WalletId.ToGuid(),
				ExternalRoundId: data.ExternalRoundId,
				ExternalBetId: data.ExternalBetId,
				CurrencyRatio: data.CurrencyRatio.ToDecimal().GetValueOrDefault(0.0m),
				Bet: data.Bet.ToDecimal().GetValueOrDefault(0.0m),
				ValidBet: data.ValidBet.ToDecimal().GetValueOrDefault(0.0m),
				Settle: data.Settle.ToDecimal().GetValueOrDefault(0.0m),
				Commission: data.Commission.ToDecimal().GetValueOrDefault(0.0m),
				Turnover: data.Turnover.ToDecimal().GetValueOrDefault(0.0m),
				WinLoss: data.WinLoss.ToDecimal().GetValueOrDefault(0.0m),
				ClosedOn: data.ClosedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
