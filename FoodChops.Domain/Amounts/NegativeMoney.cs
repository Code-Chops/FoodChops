﻿using System.Globalization;

namespace CodeChops.FoodChops.App.Domain.Amounts;

/// <summary>
/// A currency and negative amount.
/// Amount can be zero.
/// </summary>
public readonly struct NegativeMoney : IEquatable<NegativeMoney>, IComparable<NegativeMoney>, IEquatable<Money>, IComparable<Money>
{
	public override string ToString() => $"{{{this.Currency} {this.Amount.ToDisplayString(CultureInfo.InvariantCulture, $"F{this.Currency.DecimalPlaces}")}}}";
	public override int GetHashCode() => (int)(this.Amount * 100m * 1000m) + this.Currency.IsoNumericCode; // Include cents, and then move left three spaces to make room for iso numeric code
	public override bool Equals(object? other) => other is NegativeMoney negativeMoney && this.Equals(negativeMoney) || other is Money money && this.Equals(money);
	public bool Equals(NegativeMoney other) => this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount);
	public int CompareTo(NegativeMoney other) => this.CompareTo(other.Currency, other.Amount);
	public bool Equals(Money other) => this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount);
	public int CompareTo(Money other) => this.CompareTo(other.Currency, other.Amount);
	private int CompareTo(Currency currency, Amount amount)
	{
		var comparison = this.Currency.CompareTo(currency);
		if (comparison == 0) comparison = this.Amount.CompareTo(amount);
		return comparison;
	}
	public static bool operator ==(NegativeMoney one, NegativeMoney two) => one.Equals(two);
	public static bool operator !=(NegativeMoney one, NegativeMoney two) => !(one == two);
	public static bool operator ==(NegativeMoney one, PositiveMoney two) => false;
	public static bool operator !=(NegativeMoney one, PositiveMoney two) => !(one == two);
	public static bool operator ==(NegativeMoney one, Money two) => one.Equals(two);
	public static bool operator !=(NegativeMoney one, Money two) => !(one == two);
	public static bool operator ==(Money one, NegativeMoney two) => two == one;
	public static bool operator !=(Money one, NegativeMoney two) => !(one == two);
	public void Deconstruct(out Currency currency, out NegativeAmount amount)
	{
		currency = this.Currency;
		amount = this.Amount;
	}

	public Currency Currency { get; }
	public NegativeAmount Amount { get; }
	public byte DecimalPlaces => BitConverter.GetBytes(decimal.GetBits(this.Amount)[3])[2];

	public NegativeMoney(Currency currency, NegativeAmount amount)
	{
		this.Currency = currency;
		this.Amount = amount;
	}

	/// <summary>
	/// Internal constructor for operator overloads.
	/// Throws if the second currency differs from the first.
	/// </summary>
	internal NegativeMoney(Currency currency, Currency throwIfDifferentCurrency, decimal amount)
		: this(currency, (NegativeAmount)amount)
	{
		if (throwIfDifferentCurrency != this.Currency) throw new InvalidOperationException($"Mismatching currencies {this.Currency} and {throwIfDifferentCurrency}.");
	}

	/// <summary>
	/// Returns a new object whose amount equals the given number of whole smallest units, e.g. for EUR 115 smallest units is 1.15 whole units, for JPY 100 smallest units is 100 whole units.
	/// </summary>
	public static NegativeMoney FromSmallestRepresentableUnits(Currency currency, long units) => new(currency, (NegativeAmount)(units / (decimal)Math.Pow(10, currency.DecimalPlaces)));
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
	public NegativeMoney RoundToSmallestRepresentableUnit()
	{
		return new NegativeMoney(this.Currency, (NegativeAmount)decimal.Round(this.Amount, this.Currency.DecimalPlaces, MidpointRounding.AwayFromZero));
	}

	/// <summary>
	/// Returns an object whose amount is rounded to the nearest cent, i.e. two decimals, rounding away from zero.
	/// </summary>
	public NegativeMoney RoundToCents()
	{
		return new NegativeMoney(this.Currency, this.Amount.RoundToCents());
	}

	/// <summary>
	/// Returns a string representation for display purposes.
	/// </summary>
	/// <param name="provider">A format provider, such as a culture. By default, the current culture is used.</param>
	/// <param name="format">A decimal format string. See Decimal.ToString(string). By default, F2 (financial with 2 decimals) is used.</param>
	/// <param name="useCurrencySymbol">If true, the currency code is replaced by the currency symbol.</param>
	public string ToDisplayString(IFormatProvider provider = null!, string format = "F2", bool useCurrencySymbol = false) => ((Money)this).ToDisplayString(provider, format, useCurrencySymbol);

	public static implicit operator decimal(NegativeMoney negativeMoney) => negativeMoney.Amount;
	public static implicit operator Amount(NegativeMoney negativeMoney) => negativeMoney.Amount;

	// Implicit to Money, and explicit from (we cannot make assumptions about the sign)
	public static implicit operator Money(NegativeMoney negativeMoney) => new(negativeMoney.Currency, negativeMoney.Amount);
	public static explicit operator NegativeMoney(Money negativeMoney) => negativeMoney.AssumeNegative();

	// Implicit to NegativeAmount
	public static implicit operator NegativeAmount(NegativeMoney negativeMoney) => negativeMoney.Amount;

	#region Mathematical operators
	// Operators against Money
	public static Money operator +(NegativeMoney one, Money two) => new(one.Currency, two.Currency, one.Amount + two.Amount);
	public static Money operator -(NegativeMoney one, Money two) => new(one.Currency, two.Currency, one.Amount - two.Amount);
	public static Money operator *(NegativeMoney one, Money two) => new(one.Currency, two.Currency, one.Amount * two.Amount);
	public static Money operator /(NegativeMoney one, Money two) => new(one.Currency, two.Currency, one.Amount / two.Amount);
	public static Money operator +(Money one, NegativeMoney two) => two + one;
	public static Money operator -(Money one, NegativeMoney two) => new(one.Currency, two.Currency, one.Amount - two.Amount);
	public static Money operator *(Money one, NegativeMoney two) => two * one;
	public static Money operator /(Money one, NegativeMoney two) => new(one.Currency, two.Currency, one.Amount / two.Amount);
	// Operators against PositiveMoney
	public static Money operator +(NegativeMoney one, PositiveMoney two) => new(one.Currency, two.Currency, one.Amount + two.Amount);
	public static NegativeMoney operator -(NegativeMoney one, PositiveMoney two) => new(one.Currency, two.Currency, one.Amount - two.Amount);
	public static NegativeMoney operator *(NegativeMoney one, PositiveMoney two) => new(one.Currency, two.Currency, one.Amount * two.Amount);
	public static NegativeMoney operator /(NegativeMoney one, PositiveMoney two) => new(one.Currency, two.Currency, one.Amount / two.Amount);
	// Operators against NegativeMoney
	public static NegativeMoney operator +(NegativeMoney one, NegativeMoney two) => new(one.Currency, two.Currency, one.Amount + two.Amount);
	public static Money operator -(NegativeMoney one, NegativeMoney two) => new(one.Currency, two.Currency, one.Amount - two.Amount);
	public static PositiveMoney operator *(NegativeMoney one, NegativeMoney two) => new(one.Currency, two.Currency, one.Amount * two.Amount);
	public static PositiveMoney operator /(NegativeMoney one, NegativeMoney two) => new(one.Currency, two.Currency, one.Amount / two.Amount);
	// Operators against Amount
	public static Money operator +(NegativeMoney one, Amount two) => new(one.Currency, one.Amount + two);
	public static Money operator -(NegativeMoney one, Amount two) => new(one.Currency, one.Amount - two);
	public static Money operator *(NegativeMoney one, Amount two) => new(one.Currency, one.Amount * two);
	public static Money operator /(NegativeMoney one, Amount two) => new(one.Currency, one.Amount / two);
	public static Money operator +(Amount one, NegativeMoney two) => two + one;
	public static Money operator *(Amount one, NegativeMoney two) => two * one;
	// Operators against PositiveAmount
	public static Money operator +(NegativeMoney one, PositiveAmount two) => new(one.Currency, one.Amount + two);
	public static NegativeMoney operator -(NegativeMoney one, PositiveAmount two) => new(one.Currency, one.Amount - two);
	public static NegativeMoney operator *(NegativeMoney one, PositiveAmount two) => new(one.Currency, one.Amount * two);
	public static NegativeMoney operator /(NegativeMoney one, PositiveAmount two) => new(one.Currency, one.Amount / two);
	public static Money operator +(PositiveAmount one, NegativeMoney two) => two + one;
	public static NegativeMoney operator *(PositiveAmount one, NegativeMoney two) => two * one;
	// Operators against NegativeAmount
	public static NegativeMoney operator +(NegativeMoney one, NegativeAmount two) => new(one.Currency, one.Amount + two);
	public static Money operator -(NegativeMoney one, NegativeAmount two) => new(one.Currency, one.Amount - two);
	public static PositiveMoney operator *(NegativeMoney one, NegativeAmount two) => new(one.Currency, one.Amount * two);
	public static PositiveMoney operator /(NegativeMoney one, NegativeAmount two) => new(one.Currency, one.Amount / two);
	public static NegativeMoney operator +(NegativeAmount one, NegativeMoney two) => two + one;
	public static Money operator *(NegativeAmount one, NegativeMoney two) => two * one;
	#endregion
}