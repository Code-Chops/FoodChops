using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain.Coins.Eur;

public abstract class CoinEurBase : CoinBase
{
	protected CoinEurBase(PositiveAmount amount) : base(new PositiveMoney(Currency.Eur, amount))
	{
	}
}
