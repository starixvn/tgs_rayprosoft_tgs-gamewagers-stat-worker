using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Core.UnitTest.ApplicationServices;

public class GameWagerCheckCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<GameWagerCheckCommandService>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeService = Substitute.For<ICommandService<GameWagerDataCommand>>();

		var command = new GameWagerCheckCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1762394359405350912",
			ExternalBetId: "PG-1762394359405350912",
			CurrencyRatio: 1.0m,
			Bet: 0.5m,
			ValidBet: 0.5m,
			Settle: 2.8m,
			Commission: 0.0m,
			Turnover: 0.5m,
			WinLoss: 2.3m,
			ClosedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"));

		var currentTime = DateTimeOffset.Parse("2024-02-27T16:29:53.000+08:00");
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new GameWagerCheckCommandService(
			fakeLog,
			fakeTimeProvider,
			fakeService);

		await sut.ExecuteAsync(command);

		await fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerDataCommand(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				WalletId: command.WalletId,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				ClosedOn: command.ClosedOn)),
		   cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_has_been_settled_for_more_than_45_days()
	{
		var fakeLog = NullLogger<GameWagerCheckCommandService>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeService = Substitute.For<ICommandService<GameWagerDataCommand>>();

		var command = new GameWagerCheckCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1762394359405350912",
			ExternalBetId: "PG-1762394359405350912",
			CurrencyRatio: 1.0m,
			Bet: 0.5m,
			ValidBet: 0.5m,
			Settle: 2.8m,
			Commission: 0.0m,
			Turnover: 0.5m,
			WinLoss: 2.3m,
			ClosedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"));

		var currentTime = DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00").AddDays(46);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new GameWagerCheckCommandService(
			fakeLog,
			fakeTimeProvider,
			fakeService);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		await fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerDataCommand(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				WalletId: command.WalletId,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				ClosedOn: command.ClosedOn)),
		   cancellationToken: Arg.Any<CancellationToken>());
	}
}
