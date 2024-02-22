namespace Tgs.GameWagers.Stat.Worker;

public record RedisOptions
{
	public string ConnectionString { get; set; } = "localhost";
}
