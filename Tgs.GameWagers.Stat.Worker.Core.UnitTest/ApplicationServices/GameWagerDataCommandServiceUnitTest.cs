using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Worker.Core.Models;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Core.UnitTest.ApplicationServices;
public class GameWagerDataCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{

		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatCloseQueueRepository = Substitute.For<IGameWagerStatCloseQueueRepository>();
		var fakeService = Substitute.For<ICommandService<GameWagerBiddingCommand>>();

		var command = new GameWagerDataCommand(
			Guid.NewGuid(),
			1,
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid(),
			"4ucmqplxb029",
			Guid.NewGuid(),
			"1762394359405350912",
			"PG-1762394359405350912",
			1.0m,
			0.5m,
			0.5m,
			2.8m,
			0.0m,
			0.5m,
			2.3m,
			 DateTimeOffset.Parse("2024-02-27T16:29:11.267+08:00"));

		var statOn = new DateTimeOffset(2024, 02, 27, 16, 25, 00, 000, TimeSpan.FromHours(+8));
		_ = fakeStatisticalTimeUtnsil.GetStatOn(
			Arg.Is(command.ClosedOn))
			.Returns(statOn);

		var sut = new GameWagerDataCommandService(
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatCloseQueueRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		await fakeGameWagerStatCloseQueueRepository
			.Received(1)
			.PushAsync(
			playerCode: Arg.Is(command.PlayerCode),
			source: Arg.Is(new GameWagerStatCloseAdd(
			GameProviderId: command.GameProviderId,
			GameId: command.GameId,
			ResellerId: command.ResellerId,
			LicenseeId: command.LicenseeId,
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			WalletId: command.WalletId,
			ExternalRoundId: command.ExternalRoundId,
			ExternalBetId: command.ExternalBetId,
			StatOn: statOn,
			Bet: command.Bet,
			ValidBet: command.ValidBet,
			Settle: command.Settle,
			Commission: command.Commission,
			Turnover: command.Turnover,
			WinLoss: command.WinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		await fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerBiddingCommand(command.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
