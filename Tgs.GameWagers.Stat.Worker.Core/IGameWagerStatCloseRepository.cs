using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameWagers.Stat.Worker.Core.Models;

namespace Tgs.GameWagers.Stat.Worker.Core;
public interface IGameWagerStatCloseRepository
{
	public ValueTask AddAsync(GameWagerStatCloseAdd source, CancellationToken cancellationToken = default);
}
