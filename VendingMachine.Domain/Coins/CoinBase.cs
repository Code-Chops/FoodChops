using VendingMachine.Helpers.Amounts;
using VendingMachine.Helpers.Services;

namespace VendingMachine.Domain.Coins
{
	public enum CoinType
	{
		EurCent10,
		EurCent20,
		EurCent50,
		Eur1,
	}

	/// <summary>
	/// This class and it's implementations can also be removed and the concept of a coin could just be a <see cref="Helpers.Amounts.Money"/> class.
	/// But this design is made to be easily extendable in case there is a chance it should be extended. It can be extended by implementing:
	/// - IsSupported (boolean). If, for example, 1 and 2 cents are supported but should be disabled.
	/// - A generic image holder (of type object?). So the application itself can define the images in case different UI's (and techniques) are used.
	/// - Weight / size (in case the coin validation isn't done completely mechanically)
	/// - A name (by using resx)
	/// Etc...
	/// </summary>
	public abstract class CoinBase : IServiceForValue<CoinType>
	{
		public override string ToString() => $"{this.RepresentedValue}: {this.Value}";

		public override int GetHashCode() => this.Value.GetHashCode();
		public override bool Equals(object other) => other is CoinBase coin && this.Equals(coin);

		// Coins should be compared by their coin type, not by their reference equality
		public bool Equals(CoinBase? other) => this.RepresentedValue.Equals(other?.RepresentedValue);
		public int CompareTo(CoinBase? other) => this.RepresentedValue.CompareTo(other?.RepresentedValue);
		public static bool operator ==(CoinBase? one, CoinBase? two) => one?.Equals(two) ?? two is null;
		public static bool operator !=(CoinBase? one, CoinBase? two) => !(one == two);

		public abstract CoinType RepresentedValue { get; }
		public PositiveMoney Value { get; }
		
		public static implicit operator PositiveMoney(CoinBase coin) => coin.Value;
		public static implicit operator decimal(CoinBase coin) => coin.Value;

		private static ServiceDictionary<CoinType, CoinBase> ServiceDictionary { get; } = new ServiceDictionary<CoinType, CoinBase>();

		protected CoinBase(PositiveMoney value)
		{
			this.Value = value;
		}

		public static CoinBase GetCoinByType(CoinType coinType)
		{
			return ServiceDictionary.GetServiceForValue(coinType);
		}
	}

}
