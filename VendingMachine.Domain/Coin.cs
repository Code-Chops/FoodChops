using CodeChops.MagicEnums;
using CodeChops.VendingMachine.App.Domain.Amounts;

namespace CodeChops.VendingMachine.App.Domain;

public record Coin : MagicCustomEnum<Coin, Money>
{
	public static Coin EurCent10 { get; }	= CreateMember(new(Currency.EUR, 0.10m));
	public static Coin EurCent20 { get; }	= CreateMember(new(Currency.EUR, 0.20m));
	public static Coin EurCent50 { get; }	= CreateMember(new(Currency.EUR, 0.50m));
	public static Coin Eur1 { get; }		= CreateMember(new(Currency.EUR, 1.00m));
}