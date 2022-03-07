using System.Globalization;

namespace CodeChops.VendingMachine.App.Domain.Amounts;

/// <summary>
/// A monetary amount.
/// </summary>
public readonly struct Amount : IEquatable<Amount>, IComparable<Amount>, IEquatable<PositiveAmount>, IComparable<PositiveAmount>, IEquatable<NegativeAmount>, IComparable<NegativeAmount>
{
	public override string ToString() => this.ToDisplayString(CultureInfo.InvariantCulture);
	public override int GetHashCode() => this.Value.GetHashCode();
	public override bool Equals(object? other) => other is Amount amount && this.Equals(amount)
												|| other is PositiveAmount positiveAmount && positiveAmount.Equals(this)
												|| other is NegativeAmount negativeAmount && negativeAmount.Equals(this);
	public bool Equals(Amount other) => this.Value.Equals(other.Value);
	public int CompareTo(Amount other) => this.Value.CompareTo(other.Value);
	public bool Equals(PositiveAmount other) => this.Value.Equals(other.Value);
	public int CompareTo(PositiveAmount other) => this.Value.CompareTo(other.Value);
	public bool Equals(NegativeAmount other) => this.Value.Equals(other.Value);
	public int CompareTo(NegativeAmount other) => this.Value.CompareTo(other.Value);
	public static bool operator ==(Amount one, Amount two) => one.Equals(two);
	public static bool operator !=(Amount one, Amount two) => !(one == two);

	/// <summary>
	/// The actual value, including the sign, e.g. 500.00 if positive or -500.00 if negative.
	/// </summary>
	public decimal Value { get; }

	public Amount(decimal value)
	{
		this.Value = value;
	}

	public PositiveAmount GetAbsoluteValue() => new(Math.Abs(this.Value));

	public static bool TryParse(decimal amountDecimal, out Amount amount)
	{
		amount = amountDecimal;
		return true;
	}
	public static bool TryParse(string amountString, out Amount amount)
	{
		if (!decimal.TryParse(amountString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var amountDecimal))
		{
			amount = default;
			return false;
		}
		return TryParse(amountDecimal, out amount);
	}

	/// <summary>
	/// Returns a new amount whose value equals the given number of whole cents, e.g. 115 cents is 1.15 whole units.
	/// </summary>
	public static Amount FromCents(long cents) => new(cents / 100m);
	/// <summary>
	/// Returns the number of whole cents equal to this amount, e.g. 1.15 whole units is 115 cents.
	/// Throws if this amount has subcent precision.
	/// </summary>
	public long ToCents() => this.RoundToCents() == this ? (long)(this.Value * 100m) : throw new InvalidOperationException($"Amount contains subcents: {this.ToDisplayString(CultureInfo.InvariantCulture, "G")}.");
	/// <summary>
	/// Returns an amount whose value is rounded to the nearest cent, i.e. two decimals, rounding away from zero.
	/// </summary>
	public Amount RoundToCents()
	{
		return decimal.Round(this.Value, 2, MidpointRounding.AwayFromZero);
	}

	public bool IsPositive(bool includeZero = true)
	{
		return includeZero ? this.Value >= 0m : this.Value > 0m;
	}

	public bool IsPositive(out PositiveAmount positiveAmount, bool includeZero = true)
	{
		var result = this.IsPositive(includeZero);
		positiveAmount = result ? new(this.Value) : new(0m);
		return result;
	}

	public PositiveAmount AssumePositive()
	{
		if (!this.IsPositive(out var positiveAmount)) throw new InvalidOperationException("Not a positive amount.");
		return positiveAmount;
	}

	public bool IsNegative(bool includeZero = true)
	{
		return includeZero ? this.Value <= 0m : this.Value < 0m;
	}

	public bool IsNegative(out NegativeAmount negativeAmount, bool includeZero = true)
	{
		var result = this.IsNegative(includeZero);
		negativeAmount = result ? new(this.Value) : new(0m);
		return result;
	}

	public NegativeAmount AssumeNegative()
	{
		if (!this.IsNegative(out var negativeAmount)) throw new InvalidOperationException("Not a negative amount.");
		return negativeAmount;
	}

	/// <summary>
	/// Returns a string representation for display purposes.
	/// </summary>
	/// <param name="provider">A format provider, such as a culture. By default, the current culture is used.</param>
	/// <param name="format">A decimal format string. See Decimal.ToString(string). By default, F2 (financial with 2 decimals) is used.</param>
	public string ToDisplayString(IFormatProvider provider = null!, string format = "F2")
	{
		return this.Value.ToString(format, provider ?? CultureInfo.CurrentCulture);
	}

	// Implicit to/from decimal
	public static implicit operator decimal(Amount amount) => amount.Value;
	public static implicit operator Amount(decimal value) => new(value);
}