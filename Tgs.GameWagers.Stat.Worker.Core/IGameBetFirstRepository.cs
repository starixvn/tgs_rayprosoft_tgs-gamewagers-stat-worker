using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameWagers.Stat.Worker.Core.Models;

namespace Tgs.GameWagers.Stat.Worker.Core;

public interface IGameBetFirstRepository
{
	ValueTask<GameBetFirstInfo?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);
}
