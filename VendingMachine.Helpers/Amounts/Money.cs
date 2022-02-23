using System.Globalization;

namespace VendingMachine.Helpers.Amounts;

/// <summary>
/// A currency and amount.
/// </summary>
public sealed class Money : IEquatable<Money>, IComparable<Money>, IEquatable<PositiveMoney>, IComparable<PositiveMoney>, IEquatable<NegativeMoney>, IComparable<NegativeMoney>
	// Class instead of struct, because ORM needs to populate it from multiple columns
{
	public override string ToString() => $"{{{this.Currency} {this.Amount.ToDisplayString(CultureInfo.InvariantCulture, $"F{this.Currency.DecimalPlaces}")}}}";
	public override int GetHashCode() => (int)(this.Amount * 100m * 1000m) + this.Currency.IsoNumericCode; // Include cents, and then move left three spaces to make room for iso numeric code
	public override bool Equals(object other) => (other is Money money && this.Equals(money)) ||
												(other is PositiveMoney positiveMoney && positiveMoney.Equals(this)) ||
												(other is NegativeMoney negativeMoney && negativeMoney.Equals(this));
	public bool Equals(Money other) => !(other is null) && this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount);
	public int CompareTo(Money other) => other is null ? 1 : this.CompareTo(other.Currency, other.Amount);
	public bool Equals(PositiveMoney other) => !(other is null) && this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount); // This direction cannot be defined in the more specific class
	public int CompareTo(PositiveMoney other) => other is null ? 1 : this.CompareTo(other.Currency, other.Amount); // This direction cannot be defined in the more specific class
	public bool Equals(NegativeMoney other) => !(other is null) && this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount); // This direction cannot be defined in the more specific class
	public int CompareTo(NegativeMoney other) => other is null ? 1 : this.CompareTo(other.Currency, other.Amount); // This direction cannot be defined in the more specific class
	private int CompareTo(Currency currency, Amount amount)
	{
		var comparison = this.Currency.CompareTo(currency);
		if (comparison == 0) comparison = this.Amount.CompareTo(amount);
		return comparison;
	}
	public static bool operator ==(Money one, Money two) => one?.Equals(two) ?? two is null;
	public static bool operator !=(Money one, Money two) => !(one == two);
	public void Deconstruct(out Currency currency, out Amount amount)
	{
		currency = this.Currency;
		amount = this.Amount;
	}

	public Currency Currency { get; }
	public Amount Amount { get; }
	public byte DecimalPlaces => BitConverter.GetBytes(Decimal.GetBits(this.Amount)[3])[2];

	public Money(Currency currency, Amount amount)
	{
		this.Currency = currency ?? throw new ArgumentNullException(nameof(currency));
		this.Amount = amount;
	}

	/// <summary>
	/// Internal constructor for operator overloads.
	/// Throws if the second currency differs from the first.
	/// </summary>
	internal Money(Currency currency, Currency throwIfDifferentCurrency, decimal amount)
		: this(currency, amount)
	{
		if (throwIfDifferentCurrency == null) throw new ArgumentNullException(nameof(throwIfDifferentCurrency));
		if (throwIfDifferentCurrency != this.Currency) throw new InvalidOperationException($"Mismatching currencies {this.Currency} and {throwIfDifferentCurrency}.");
	}

	/// <summary>
	/// Returns a new object whose amount equals the given number of whole smallest units, e.g. for EUR 115 smallest units is 1.15 whole units, for JPY 100 smallest units is 100 whole units.
	/// </summary>
	public static Money FromSmallestRepresentableUnits(Currency currency, long units) => new Money(currency, units / (decimal)Math.Pow(10, currency.DecimalPlaces));
	/// <summary>
	/// Returns the number of whole smallest units equal to this amount, e.g. for EUR 1.15 whole units is 115 smallest units, for JPY 100 whole units is 100 smallest units.
	/// Throws if this amount has smaller precision.
	/// </summary>
	public long ToSmallestRepresentableUnits() => this.RoundToSmallestRepresentableUnit() == this 
		? (long)(this.Amount * (decimal)Math.Pow(10, this.Currency.DecimalPlaces)) 
		: throw new InvalidOperationException($"Money contains smaller precision: {this.ToDisplayString(CultureInfo.InvariantCulture, "G")}. Expected a maximum of {this.Currency.DecimalPlaces} decimal places.");
	
	/// <summary>
	/// Returns an object whose amount is rounded to the smallest representable unit for this currency, i.e. for EUR two decimals, for JPY zero decimals, rounding away from zero.
	/// </summary>
	public Money RoundToSmallestRepresentableUnit()
	{
		return new Money(this.Currency, Decimal.Round(this.Amount, this.Currency.DecimalPlaces, MidpointRounding.AwayFromZero));
	}

	/// <summary>
	/// Returns an object whose amount is rounded to the nearest cent, i.e. two decimals, rounding away from zero.
	/// </summary>
	public Money RoundToCents()
	{
		return new Money(this.Currency, this.Amount.RoundToCents());
	}

	public bool IsPositive(bool includeZero = true) => this.Amount.IsPositive(includeZero);
	public bool IsPositive(out PositiveMoney positiveMoney, bool includeZero = true)
	{
		var result = this.Amount.IsPositive(out var positiveAmount, includeZero);
		positiveMoney = new PositiveMoney(this.Currency, positiveAmount);
		return result;
	}
	public PositiveMoney AssumePositive()
	{
		if (!this.IsPositive(out var positiveMoney)) throw new InvalidOperationException("Not a positive amount.");
		return positiveMoney;
	}

	public bool IsNegative(bool includeZero = true) => this.Amount.IsNegative(includeZero);
	public bool IsNegative(out NegativeMoney negativeMoney, bool includeZero = true)
	{
		var result = this.Amount.IsNegative(out var negativeAmount, includeZero);
		negativeMoney = new NegativeMoney(this.Currency, negativeAmount);
		return result;
	}
	public NegativeMoney AssumeNegative()
	{
		if (!this.IsNegative(out var negativeMoney)) throw new InvalidOperationException("Not a negative amount.");
		return negativeMoney;
	}

	/// <summary>
	/// Returns a string representation for display purposes.
	/// </summary>
	/// <param name="provider">A format provider, such as a culture. By default, the current culture is used.</param>
	/// <param name="format">A decimal format string. See Decimal.ToString(string). By default, F2 (financial with 2 decimals) is used.</param>
	/// <param name="useCurrencySymbol">If true, the currency code is replaced by the currency symbol.</param>
	public string ToDisplayString(IFormatProvider provider = null, string format = "F2", bool useCurrencySymbol = false)
	{
		var currency = useCurrencySymbol ? this.Currency.Symbol : this.Currency.Code;
		var amount = this.Amount.ToDisplayString(provider, format);
		return $"{currency} {amount}";
	}

	// Implicit to decimal
	public static implicit operator decimal(Money money) => money.Amount;

	// Implicit to Amount
	public static implicit operator Amount(Money money) => money.Amount;

	// Explicit to PositiveAmount (we cannot make assumptions about the sign)
	public static explicit operator PositiveAmount(Money positiveMoney) => positiveMoney.Amount.AssumePositive();

	// Explicit to NegativeAmount (we cannot make assumptions about the sign)
	public static explicit operator NegativeAmount(Money negativeMoney) => negativeMoney.Amount.AssumeNegative();

	#region Mathematical operators
	// Operators against Money
	public static Money operator +(Money one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount + two.Amount);
	public static Money operator -(Money one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount - two.Amount);
	public static Money operator *(Money one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount * two.Amount);
	public static Money operator /(Money one, Money two) => (one is null || two is null) ? null : new Money(one.Currency, two.Currency, one.Amount / two.Amount);
	// Operators against Amount
	public static Money operator +(Money one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount + two);
	public static Money operator -(Money one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount - two);
	public static Money operator *(Money one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount * two);
	public static Money operator /(Money one, Amount two) => one is null ? null : new Money(one.Currency, one.Amount / two);
	public static Money operator +(Amount one, Money two) => two + one;
	public static Money operator *(Amount one, Money two) => two * one;
	#endregion
}
