using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Worker.Core.Models;

namespace Tgs.GameWagers.Stat.Worker.Core.UnitTest.ApplicationServices;
public class GameWagerStatCloseCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<GameWagerStatCloseCommandService>.Instance;
		var fakeGameWagerStatCloseQueueRepository = Substitute.For<IGameWagerStatCloseQueueRepository>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();

		var command = new GameWagerStatCloseCommand(
			"4ucmqplxb029");
		var source = new GameWagerStatCloseAdd(
			Guid.NewGuid(),
			1,
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid(),
			"4ucmqplxb029",
			Guid.NewGuid(),
			"1762394359405350912",
			"PG-1762394359405350912",
			new DateTimeOffset(2024, 02, 27, 16, 25, 00, 000, TimeSpan.FromHours(+8)),
			0.5m,
			0.5m,
			2.8m,
			0.0m,
			0.5m,
			2.3m);
		_ = fakeGameWagerStatCloseQueueRepository.PopAsync(
			Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(source, (GameWagerStatCloseAdd?)null);

		var sut = new GameWagerStatCloseCommandService(
			fakeLog,
			fakeGameWagerStatCloseQueueRepository,
			fakeGameWagerStatCloseRepository);

		await sut.ExecuteAsync(command);

		await fakeGameWagerStatCloseQueueRepository
			.Received(1)
			.RemoveAsync(
			playerCode: Arg.Is(command.PlayerCode),
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
