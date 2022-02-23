using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain.Coins.Eur
{
	public class CoinEurCent20 : CoinEurBase
	{
		public override CoinType RepresentedValue => CoinType.EurCent20;

		public CoinEurCent20() : base(new PositiveAmount(0.20m))
		{
		}
	}
}
