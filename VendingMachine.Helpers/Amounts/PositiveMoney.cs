using System;
using System.Globalization;

namespace VendingMachine.Helpers.Amounts
{
	/// <summary>
	/// A currency and positive amount.
	/// Amount can be zero.
	/// </summary>
	public sealed class PositiveMoney : IEquatable<PositiveMoney>, IComparable<PositiveMoney>, IEquatable<Money>, IComparable<Money>
		// Class instead of struct, because ORM needs to populate it from multiple columns
	{
		public override string ToString() => $"{this.Currency} {this.Amount.ToDisplayString(CultureInfo.InvariantCulture, $"F{this.Currency.DecimalPlaces}")}";
		public override int GetHashCode() => (int)(this.Amount * 100m * 1000m) + this.Currency.IsoNumericCode; // Include cents, and then move left three spaces to make room for iso numeric code
		public override bool Equals(object other) => (other is PositiveMoney positiveMoney && this.Equals(positiveMoney)) || (other is Money money && this.Equals(money));
		public bool Equals(PositiveMoney other) => !(other is null) && this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount);
		public int CompareTo(PositiveMoney other) => other is null ? 1 : this.CompareTo(other.Currency, other.Amount);
		public bool Equals(Money other) => !(other is null) && this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount);
		public int CompareTo(Money other) => other is null ? 1 : this.CompareTo(other.Currency, other.Amount);
		private int CompareTo(Currency currency, Amount amount)
		{
			var comparison = this.Currency.CompareTo(currency);
			if (comparison == 0) comparison = this.Amount.CompareTo(amount);
			return comparison;
		}
		public static bool operator ==(PositiveMoney one, PositiveMoney two) => one?.Equals(two) ?? two is null;
		public static bool operator !=(PositiveMoney one, PositiveMoney two) => !(one == two);
		public static bool operator ==(PositiveMoney one, NegativeMoney two) => false;
		public static bool operator !=(PositiveMoney one, NegativeMoney two) => !(one == two);
		public static bool operator ==(PositiveMoney one, Money two) => one?.Equals(two) ?? two is null;
		public static bool operator !=(PositiveMoney one, Money two) => !(one == two);
		public static bool operator ==(Money one, PositiveMoney two) => two == one;
		public static bool operator !=(Money one, PositiveMoney two) => !(one == two);
		public void Deconstruct(out Currency currency, out PositiveAmount amount)
		{
			currency = this.Currency;
			amount = this.Amount;
		}

		public Currency Currency { get; }
		public PositiveAmount Amount { get; }
		public byte DecimalPlaces => BitConverter.GetBytes(Decimal.GetBits(this.Amount)[3])[2];

		public PositiveMoney(Currency currency, PositiveAmount amount)
		{
			this.Currency = currency ?? throw new ArgumentNullException(nameof(currency));
			this.Amount = amount;
		}

		/// <summary>
		/// Internal constructor for operator overloads.
		/// Throws if the second currency differs from the first.
		/// </summary>
		internal PositiveMoney(Currency currency, Currency throwIfDifferentCurrency, decimal amount)
			: this(currency, (PositiveAmount)amount)
		{
			if (throwIfDifferentCurrency == null) throw new ArgumentNullException(nameof(throwIfDifferentCurrency));
			if (throwIfDifferentCurrency != this.Currency) throw new InvalidOperationException($"Mismatching currencies {this.Currency} and {throwIfDifferentCurrency}.");
		}

		/// <summary>
		/// Returns a new object whose amount equals the given number of whole smallest units, e.g. for EUR 115 smallest units is 1.15 whole units, for JPY 100 smallest units is 100 whole units.
		/// </summary>
		public static PositiveMoney FromSmallestRepresentableUnits(Currency currency, ulong units) => new PositiveMoney(currency, (PositiveAmount)(units / (decimal)Math.Pow(10, currency.DecimalPlaces)));
		/// <summary>
		/// Returns the number of whole smallest units equal to this amount, e.g. for EUR 1.15 whole units is 115 smallest units, for JPY 100 whole units is 100 smallest units.
		/// Throws if this amount has smaller precision.
		/// </summary>
		public ulong ToSmallestRepresentableUnits() => this.RoundToSmallestRepresentableUnit() == this
			? (ulong)(this.Amount * (decimal)Math.Pow(10, this.Currency.DecimalPlaces))
			: throw new InvalidOperationException($"Money contains smaller precision: {this.ToDisplayString(CultureInfo.InvariantCulture, "G")}. Expected a maximum of {this.Currency.DecimalPlaces} decimal places.");

		/// <summary>
		/// Returns an object whose amount is rounded to the smallest representable unit for this currency, i.e. for EUR two decimals, for JPY zero decimals, rounding away from zero.
		/// </summary>
		public PositiveMoney RoundToSmallestRepresentableUnit()
		{
			return new PositiveMoney(this.Currency, (PositiveAmount)Decimal.Round(this.Amount, this.Currency.DecimalPlaces, MidpointRounding.AwayFromZero));
		}

		/// <summary>
		/// Returns an object whose amount is rounded to the nearest cent, i.e. two decimals, rounding away from zero.
		/// </summary>
		public PositiveMoney RoundToCents()
		{
			return new PositiveMoney(this.Currency, this.Amount.RoundToCents());
		}

		/// <summary>
		/// Returns a string representation for display purposes.
		/// </summary>
		/// <param name="provider">A format provider, such as a culture. By default, the current culture is used.</param>
		/// <param name="format">A decimal format string. See Decimal.ToString(string). By default, F2 (financial with 2 decimals) is used.</param>
		/// <param name="useCurrencySymbol">If true, the currency code is replaced by the currency symbol.</param>
		public string ToDisplayString(IFormatProvider provider = null, string format = "F2", bool useCurrencySymbol = false) => ((Money)this).ToDisplayString(provider, format, useCurrencySymbol);

		// Implicit to decimal
		public static implicit operator decimal(PositiveMoney positiveMoney) => positiveMoney.Amount;

		// Implicit to Amount
		public static implicit operator Amount(PositiveMoney positiveMoney) => positiveMoney.Amount;

		// Implicit to Money, and explicit from (we cannot make assumptions about the sign)
		public static implicit operator Money(PositiveMoney positiveMoney) => new Money(positiveMoney.Currency, positiveMoney.Amount);
		public static explicit operator PositiveMoney(Money positiveMoney) => positiveMoney.AssumePositive();

		// Implicit to PositiveAmount
		public static implicit operator PositiveAmount(PositiveMoney positiveMoney) => positiveMoney.Amount;

		#region Mathematical operators
		// Operators against Money
		public static Money operator +(PositiveMoney one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount + two.Amount);
		public static Money operator -(PositiveMoney one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount - two.Amount);
		public static Money operator *(PositiveMoney one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount * two.Amount);
		public static Money operator /(PositiveMoney one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount / two.Amount);
		public static Money operator +(Money one, PositiveMoney two) => two + one;
		public static Money operator -(Money one, PositiveMoney two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount - two.Amount);
		public static Money operator *(Money one, PositiveMoney two) => two * one;
		public static Money operator /(Money one, PositiveMoney two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount / two.Amount);
		// Operators against PositiveMoney
		public static PositiveMoney operator +(PositiveMoney one, PositiveMoney two) => (one is null || two is null) ? null : new PositiveMoney(one.Currency, two.Currency, one.Amount + two.Amount);
		public static Money operator -(PositiveMoney one, PositiveMoney two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount - two.Amount);
		public static PositiveMoney operator *(PositiveMoney one, PositiveMoney two) => (one is null || two is null) ? null : new PositiveMoney(one.Currency, two.Currency, one.Amount * two.Amount);
		public static PositiveMoney operator /(PositiveMoney one, PositiveMoney two) => (one is null || two is null) ? null : new PositiveMoney(one.Currency, two.Currency, one.Amount / two.Amount);
		// Operators against NegativeMoney
		public static Money operator +(PositiveMoney one, NegativeMoney two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount + two.Amount);
		public static PositiveMoney operator -(PositiveMoney one, NegativeMoney two) => (one is null || two is null) ? null : new PositiveMoney(one.Currency, two.Currency, one.Amount - two.Amount);
		public static NegativeMoney operator *(PositiveMoney one, NegativeMoney two) => (one is null || two is null) ? null : new NegativeMoney(one.Currency, two.Currency, one.Amount * two.Amount);
		public static NegativeMoney operator /(PositiveMoney one, NegativeMoney two) => (one is null || two is null) ? null : new NegativeMoney(one.Currency, two.Currency, one.Amount / two.Amount);
		// Operators against Amount
		public static Money operator +(PositiveMoney one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount + two);
		public static Money operator -(PositiveMoney one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount - two);
		public static Money operator *(PositiveMoney one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount * two);
		public static Money operator /(PositiveMoney one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount / two);
		public static Money operator +(Amount one, PositiveMoney two) => two + one;
		public static Money operator *(Amount one, PositiveMoney two) => two * one;
		// Operators against PositiveAmount
		public static PositiveMoney operator +(PositiveMoney one, PositiveAmount two) => one is null ? null : new PositiveMoney(one.Currency, one.Amount + two);
		public static Money operator -(PositiveMoney one, PositiveAmount two) => one is null ? null : new Money(one.Currency, one.Amount - two);
		public static PositiveMoney operator *(PositiveMoney one, PositiveAmount two) => one is null ? null : new PositiveMoney(one.Currency, one.Amount * two);
		public static PositiveMoney operator /(PositiveMoney one, PositiveAmount two) => one is null ? null : new PositiveMoney(one.Currency, one.Amount / two);
		public static PositiveMoney operator +(PositiveAmount one, PositiveMoney two) => two + one;
		public static PositiveMoney operator *(PositiveAmount one, PositiveMoney two) => two * one;
		// Operators against NegativeAmount
		public static Money operator +(PositiveMoney one, NegativeAmount two) => one is null ? null : new Money(one.Currency, one.Amount + two);
		public static PositiveMoney operator -(PositiveMoney one, NegativeAmount two) => one is null ? null : new PositiveMoney(one.Currency, one.Amount - two);
		public static NegativeMoney operator *(PositiveMoney one, NegativeAmount two) => one is null ? null : new NegativeMoney(one.Currency, one.Amount * two);
		public static NegativeMoney operator /(PositiveMoney one, NegativeAmount two) => one is null ? null : new NegativeMoney(one.Currency, one.Amount / two);
		public static Money operator +(NegativeAmount one, PositiveMoney two) => two + one;
		public static NegativeMoney operator *(NegativeAmount one, PositiveMoney two) => two * one;
		#endregion
	}
}
