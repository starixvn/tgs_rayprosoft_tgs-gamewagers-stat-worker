namespace Tgs.GameWagers.Stat.Worker.Core;
public record GameWagersStatWorkerSettingsOptions
{
	public int ChannelExpiry { get; set; } = 210;
	public int BiddingExpiry { get; set; } = 180;
}
