using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain.Coins.Eur;

public class CoinEurCent10 : CoinEurBase
{
	public override CoinType RepresentedValue => CoinType.EurCent10;

	public CoinEurCent10() : base(new PositiveAmount(0.10m))
	{
	}
}
