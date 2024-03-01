using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
public record GameWagerPlayerCheckCommand(
	string PlayerCode);