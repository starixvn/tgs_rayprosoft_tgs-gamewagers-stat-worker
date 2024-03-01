using System.Threading;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Client;
using Tgs.GameWagers.Common;
using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Core.UnitTest.ApplicationServices;
public class GameWagerCheckCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeLog = NullLogger<GameWagerCheckCommandService>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeService = Substitute.For<ICommandService<GameWagerDataCommand>>();

		var command = new GameWagerCheckCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			WalletId: Guid.NewGuid(),
			ExternalWagerId: "PG-1762394359405350912",
			 CreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"));

		var info = new GameWagerFirstInfo(
			WagerId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			GameProviderId: command.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: command.GameId,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: command.WalletId,
			WagerState: GameWagers.Common.Enums.WagerState.Win,
			WagerType: GameWagers.Common.Enums.WagerType.Normal,
			ClosedState: GameWagers.Common.Enums.ClosedState.Closed,
			VoidState: GameWagers.Common.Enums.VoidState.None,
			CancelState: GameWagers.Common.Enums.CancelState.None,
			FreeGameState: GameWagers.Common.Enums.FreeGameState.None,
			GambleState: GameWagers.Common.Enums.GambleState.None,
			CashoutState: GameWagers.Common.Enums.CashoutState.None,
			ResettleState: GameWagers.Common.Enums.ResettleState.None,
			ExternalRoundId: "1762394359405350912",
			ExternalWagerId: command.ExternalWagerId,
			ExternalCreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalClosedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalResettleOn: DateTimeOffset.Parse("1970-01-01T08:00:00.000+08:00"),
			CurrencyRatio: 1.0m,
			Bet: 0.5m,
			ValidBet: 0.5m,
			Settle: 2.8m,
			Commission: 0.0m,
			Turnover: 0.5m,
			WinLoss: 2.3m,
			CreatedOn: command.CreatedOn,
			ClosedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"),
			UpdatedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"));
		_ = fakeGameWagersClient.GetAsync(
			Arg.Is(command.GameProviderId),
			Arg.Is(command.GameId),
			Arg.Is(command.WalletId),
			Arg.Is(command.ExternalWagerId),
			Arg.Is(command.CreatedOn),
			Arg.Any<CancellationToken>())
			.Returns(info);

		var currentTime = DateTimeOffset.Parse("2024-02-27T16:29:53.000+08:00");
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new GameWagerCheckCommandService(
			fakeLog,
			fakeTimeProvider,
			fakeGameWagersClient,
			fakeService);

		await sut.ExecuteAsync(command);

		await fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerDataCommand(
				GameProviderId: info.GameProviderId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalWagerId: info.ExternalWagerId,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss,
				ClosedOn: info.ClosedOn)),
		   cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_game_wager_cannot_be_found()
	{
		var fakeLog = NullLogger<GameWagerCheckCommandService>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeService = Substitute.For<ICommandService<GameWagerDataCommand>>();

		var command = new GameWagerCheckCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			WalletId: Guid.NewGuid(),
			ExternalWagerId: "PG-1762394359405350912",
			 CreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"));

		_ = fakeGameWagersClient.GetAsync(
			Arg.Is(command.GameProviderId),
			Arg.Is(command.GameId),
			Arg.Is(command.WalletId),
			Arg.Is(command.ExternalWagerId),
			Arg.Is(command.CreatedOn),
			Arg.Any<CancellationToken>())
			.Returns((GameWagerFirstInfo?)null);

		var sut = new GameWagerCheckCommandService(
			fakeLog,
			fakeTimeProvider,
			fakeGameWagersClient,
			fakeService);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.ExecuteAsync(command).AsTask());

		await fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerDataCommand>(),
		   cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_game_bet_has_been_settled_for_more_than_45_days()
	{
		var fakeLog = NullLogger<GameWagerCheckCommandService>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeService = Substitute.For<ICommandService<GameWagerDataCommand>>();

		var command = new GameWagerCheckCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			WalletId: Guid.NewGuid(),
			ExternalWagerId: "PG-1762394359405350912",
			CreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"));

		var info = new GameWagerFirstInfo(
			WagerId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			GameProviderId: command.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: command.GameId,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: command.WalletId,
			WagerState: GameWagers.Common.Enums.WagerState.Win,
			WagerType: GameWagers.Common.Enums.WagerType.Normal,
			ClosedState: GameWagers.Common.Enums.ClosedState.Closed,
			VoidState: GameWagers.Common.Enums.VoidState.None,
			CancelState: GameWagers.Common.Enums.CancelState.None,
			FreeGameState: GameWagers.Common.Enums.FreeGameState.None,
			GambleState: GameWagers.Common.Enums.GambleState.None,
			CashoutState: GameWagers.Common.Enums.CashoutState.None,
			ResettleState: GameWagers.Common.Enums.ResettleState.None,
			ExternalRoundId: "1762394359405350912",
			ExternalWagerId: command.ExternalWagerId,
			ExternalCreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalClosedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalResettleOn: DateTimeOffset.Parse("1970-01-01T08:00:00.000+08:00"),
			CurrencyRatio: 1.0m,
			Bet: 0.5m,
			ValidBet: 0.5m,
			Settle: 2.8m,
			Commission: 0.0m,
			Turnover: 0.5m,
			WinLoss: 2.3m,
			CreatedOn: command.CreatedOn,
			ClosedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"),
			UpdatedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"));
		_ = fakeGameWagersClient.GetAsync(
			Arg.Is(command.GameProviderId),
			Arg.Is(command.GameId),
			Arg.Is(command.WalletId),
			Arg.Is(command.ExternalWagerId),
			Arg.Is(command.CreatedOn),
			Arg.Any<CancellationToken>())
			.Returns(info);

		var currentTime = DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00").AddDays(46);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new GameWagerCheckCommandService(
			fakeLog,
			fakeTimeProvider,
			fakeGameWagersClient,
			fakeService);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		await fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerDataCommand(
				GameProviderId: info.GameProviderId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalWagerId: info.ExternalWagerId,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss,
				ClosedOn: info.ClosedOn)),
		   cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_game_wager_has_not_been_settled_yet()
	{
		var fakeLog = NullLogger<GameWagerCheckCommandService>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeService = Substitute.For<ICommandService<GameWagerDataCommand>>();

		var command = new GameWagerCheckCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			WalletId: Guid.NewGuid(),
			ExternalWagerId: "PG-1762394359405350912",
			 CreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"));

		var info = new GameWagerFirstInfo(
			WagerId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			GameProviderId: command.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: command.GameId,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: command.WalletId,
			WagerState: GameWagers.Common.Enums.WagerState.Running,
			WagerType: GameWagers.Common.Enums.WagerType.Normal,
			ClosedState: GameWagers.Common.Enums.ClosedState.None,
			VoidState: GameWagers.Common.Enums.VoidState.None,
			CancelState: GameWagers.Common.Enums.CancelState.None,
			FreeGameState: GameWagers.Common.Enums.FreeGameState.None,
			GambleState: GameWagers.Common.Enums.GambleState.None,
			CashoutState: GameWagers.Common.Enums.CashoutState.None,
			ResettleState: GameWagers.Common.Enums.ResettleState.None,
			ExternalRoundId: "1762394359405350912",
			ExternalWagerId: command.ExternalWagerId,
			ExternalCreatedOn: DateTimeOffset.Parse("2024-02-27T16:28:53.000+08:00"),
			ExternalClosedOn: DateTimeOffset.Parse("1970-01-01T08:00:00.000+08:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("1970-01-01T08:00:00.000+08:00"),
			ExternalResettleOn: DateTimeOffset.Parse("1970-01-01T08:00:00.000+08:00"),
			CurrencyRatio: 1.0m,
			Bet: 0.5m,
			ValidBet: 0.5m,
			Settle: 2.8m,
			Commission: 0.0m,
			Turnover: 0.5m,
			WinLoss: 2.3m,
			CreatedOn: command.CreatedOn,
			ClosedOn: DateTimeOffset.Parse("1970-01-01T08:00:00.000+08:00"),
			UpdatedOn: DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"));
		_ = fakeGameWagersClient.GetAsync(
			Arg.Is(command.GameProviderId),
			Arg.Is(command.GameId),
			Arg.Is(command.WalletId),
			Arg.Is(command.ExternalWagerId),
			Arg.Is(command.CreatedOn),
			Arg.Any<CancellationToken>())
			.Returns(info);

		var currentTime = DateTimeOffset.Parse("2024-02-27T16:29:53.000+08:00");
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new GameWagerCheckCommandService(
			fakeLog,
			fakeTimeProvider,
			fakeGameWagersClient,
			fakeService);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		await fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerDataCommand(
				GameProviderId: info.GameProviderId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalWagerId: info.ExternalWagerId,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss,
				ClosedOn: info.ClosedOn)),
		   cancellationToken: Arg.Any<CancellationToken>());
	}
}
