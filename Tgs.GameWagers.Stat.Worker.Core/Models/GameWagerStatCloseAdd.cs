namespace Tgs.GameWagers.Stat.Worker.Core.Models;
public record GameWagerStatCloseAdd(
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	Guid GameProviderId,
	int GameId,
	DateTimeOffset StatOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss);
