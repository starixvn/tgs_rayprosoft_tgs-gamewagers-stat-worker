namespace Tgs.GameWagers.Stat.Worker.Core;

internal class StatisticalTimeUtnsil : IStatisticalTimeUtnsil
{
	public DateTimeOffset GetStatOn(DateTimeOffset source)
	{
		var utc = source.UtcDateTime;
		var mins = utc.Minute % 5;
		var temp = utc.AddMinutes(-mins);
		return new DateTimeOffset(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, 00, TimeSpan.Zero);
	}
}
