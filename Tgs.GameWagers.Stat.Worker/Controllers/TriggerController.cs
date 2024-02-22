using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Worker.Controllers;
[Route("[controller]")]
[ApiController]
public class TriggerController : ControllerBase
{
	/// <summary>
	/// Triggers the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	[HttpPost("{playerCode}")]
	public async ValueTask TriggerAsync(
		[FromServices] ICommandService<GameWagerBiddingCommand> service,
		string playerCode)
		=> await service.ExecuteAsync(
			command: new GameWagerBiddingCommand(
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);
}
