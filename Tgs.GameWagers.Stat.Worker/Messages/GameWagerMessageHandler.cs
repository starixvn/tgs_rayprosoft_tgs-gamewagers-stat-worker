using Grpc.Net.Client.Balancer;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Messages;

public class GameWagerMessageHandler(
	ILogger<GameWagerMessageHandler> logger,
	ICommandService<GameWagerCheckCommand> service) : IMessageHandler<GameWagerPublish>
{
	public ValueTask HandleAsync(string subject, GameWagerPublish data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var loggerInfo = new
		{
			data.GameProviderCode,
			data.SubGameProviderCode,
			data.GameId,
			data.GameCategory,
			data.GameType,
			data.ResellerCode,
			data.LicenseeCode,
			data.BrandCode,
			data.PlayerCode,
			data.Username,
			data.CurrencyCode,
			data.ExternalRoundId,
			data.ExternalWagerId
		};

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			loggerInfo);

		return service.ExecuteAsync(
			command: new GameWagerCheckCommand(
				GameProviderId: data.GameProviderId.ToGuid(),
				GameProviderCode: data.GameProviderCode,
				GameId: data.GameId,
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				PlayerCode: data.PlayerCode,
				Username: data.Username,
				CurrencyCode: data.CurrencyCode,
				WalletId: data.WalletId.ToGuid(),
				WagerState: data.WagerState,
				WagerType: data.WagerType,
				ClosedState: data.ClosedState,
				VoidState: data.VoidState,
				CancelState: data.CancelState,
				FreeGameState: data.FreeGameState,
				GambleState: data.GambleState,
				CashoutState: data.CashoutState,
				ResettleState: data.ResettleState,
				ExternalRoundId: data.ExternalRoundId,
				ExternalWagerId: data.ExternalWagerId,
				ExternalGameId: data.ExternalGameId,
				CurrencyRatio: data.CurrencyRatio.ToDecimal().GetValueOrDefault(0),
				Bet: data.Bet.ToDecimal().GetValueOrDefault(0),
				ValidBet: data.ValidBet.ToDecimal().GetValueOrDefault(0),
				Settle: data.Settle.ToDecimal().GetValueOrDefault(0),
				Commission: data.Commission.ToDecimal().GetValueOrDefault(0),
				Turnover: data.Turnover.ToDecimal().GetValueOrDefault(0),
				WinLoss: data.WinLoss.ToDecimal().GetValueOrDefault(0),
				ClosedOn: data.ClosedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
