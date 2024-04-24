using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Worker.Core.UnitTest.ApplicationServices;

public class GameWagerBiddingCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeLog = NullLogger<GameWagerBiddingCommandService>.Instance;
		var fakeGameWagerStatCloseBiddingRepository = Substitute.For<IGameWagerStatCloseBiddingRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameWagerBiddingCommand(
			"4ucmqplxb029");

		var token = Environment.MachineName;
		_ = fakeGameWagerStatCloseBiddingRepository.BiddingAsync(
			Arg.Is(command.PlayerCode),
			Arg.Is(Environment.MachineName),
			Arg.Any<CancellationToken>())
			.Returns(token);

		var sut = new GameWagerBiddingCommandService(
			fakeLog,
			fakeGameWagerStatCloseBiddingRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is($"gamewagers.stat.worker.command.{token}"),
			data: Arg.Is(new GameWagersStatWorkerMessage
			{
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is($"gamewagers.stat.worker.expiry.{token}"),
			data: Arg.Is(new GameWagersStatWorkerMessage
			{
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void The_bidding_process_failed_for_this_player()
	{
		var fakeLog = NullLogger<GameWagerBiddingCommandService>.Instance;
		var fakeGameWagerStatCloseBiddingRepository = Substitute.For<IGameWagerStatCloseBiddingRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameWagerBiddingCommand(
			"4ucmqplxb029");

		var token = string.Empty;
		_ = fakeGameWagerStatCloseBiddingRepository.BiddingAsync(
			Arg.Is(command.PlayerCode),
			Arg.Is(Environment.MachineName),
			Arg.Any<CancellationToken>())
			.Returns(token);

		var sut = new GameWagerBiddingCommandService(
			fakeLog,
			fakeGameWagerStatCloseBiddingRepository,
			fakeMessageSender);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is($"gamewagers.stat.worker.command.{token}"),
			data: Arg.Is(new GameWagersStatWorkerMessage
			{
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is($"gamewagers.stat.worker.expiry.{token}"),
			data: Arg.Is(new GameWagersStatWorkerMessage
			{
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());
	}
}
