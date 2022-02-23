using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain.Coins.Eur
{
	public class CoinEurCent50 : CoinEurBase
	{
		public override CoinType RepresentedValue => CoinType.EurCent50;

		public CoinEurCent50() : base(new PositiveAmount(0.50m))
		{
		}
	}
}
