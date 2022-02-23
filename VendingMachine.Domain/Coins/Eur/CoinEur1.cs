using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain.Coins.Eur
{
	public class CoinEur1 : CoinEurBase
	{
		public override CoinType RepresentedValue => CoinType.Eur1;

		public CoinEur1() : base(new PositiveAmount(1))
		{
		}
	}
}
