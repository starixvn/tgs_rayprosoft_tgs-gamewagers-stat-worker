namespace Tgs.GameWagers.Stat.Worker.Core.ApplicationServices;
public record GameWagerCheckCommand(
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn);
