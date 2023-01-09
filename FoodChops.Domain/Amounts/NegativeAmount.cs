using System.Globalization;

namespace CodeChops.FoodChops.App.Domain.Amounts;

/// <summary>
/// A negative monetary amount.
/// Can be zero.
/// </summary>
public readonly struct NegativeAmount : IEquatable<NegativeAmount>, IComparable<NegativeAmount>, IEquatable<Amount>, IComparable<Amount>
{
	public override string ToString() => this.ToDisplayString(CultureInfo.InvariantCulture);
	public override int GetHashCode() => this.Value.GetHashCode();
	public override bool Equals(object? other) => other is NegativeAmount negativeAmount && this.Equals(negativeAmount) || other is Amount amount && this.Equals(amount);
	public bool Equals(NegativeAmount other) => this.Value.Equals(other.Value);
	public int CompareTo(NegativeAmount other) => this.Value.CompareTo(other.Value);
	public bool Equals(Amount other) => this.Value.Equals(other.Value);
	public int CompareTo(Amount other) => this.Value.CompareTo(other.Value);
	public static bool operator ==(NegativeAmount one, NegativeAmount two) => one.Equals(two);
	public static bool operator !=(NegativeAmount one, NegativeAmount two) => !(one == two);
	// Equality operator with Amount is done via implicit cast
	// Equality operator with PositiveAmount is done via implicit cast

	/// <summary>
	/// Signed.
	/// </summary>
	public decimal Value { get; }

	/// <summary>
	/// Constructs a new instance from the given value. The value must be negative, unless isAbsolute=true, in which case it must be positive.
	/// </summary>
	public NegativeAmount(decimal value, bool isAbsolute = false)
		: this(isAbsolute ? decimal.Negate(value) : value)
	{
	}

	private NegativeAmount(decimal value)
	{
		if (value > 0m) throw new ArgumentException($"Trying to create a {nameof(NegativeAmount)} with a positive value greater than zero.");

		this.Value = value;
	}

	public PositiveAmount GetAbsoluteValue() => new(Math.Abs(this.Value));

	public static bool TryParse(decimal amountDecimal, out NegativeAmount amount)
	{
		if (!Amount.TryParse(amountDecimal, out var convertedAmount) || amountDecimal > 0m)
		{
			amount = default;
			return false;
		}
		amount = (NegativeAmount)convertedAmount;
		return true;
	}
	public static bool TryParse(string amountString, out NegativeAmount amount)
	{
		if (!Amount.TryParse(amountString, out var convertedAmount) || convertedAmount > 0m)
		{
			amount = default;
			return false;
		}
		amount = (NegativeAmount)convertedAmount;
		return true;
	}

	/// <summary>
	/// Returns a new amount whose value equals the given number of whole cents, e.g. -115 cents is -1.15 whole units.
	/// </summary>
	public static NegativeAmount FromCents(long cents) => new(cents / 100m);
	/// <summary>
	/// Returns the number of whole cents equal to this amount, e.g. -1.15 whole units is -115 cents.
	/// Throws if this amount has subcent precision.
	/// </summary>
	public long ToCents() => this.RoundToCents() == this ? (long)(this.Value * 100m) : throw new InvalidOperationException($"Amount contains subcents: {this.ToDisplayString(CultureInfo.InvariantCulture, "G")}.");
	/// <summary>
	/// Returns an amount whose value is rounded to the nearest cent, i.e. two decimals, rounding away from zero.
	/// </summary>
	public NegativeAmount RoundToCents()
	{
		return (NegativeAmount)decimal.Round(this.Value, 2, MidpointRounding.AwayFromZero);
	}

	/// <summary>
	/// Returns a string representation for display purposes.
	/// </summary>
	/// <param name="provider">A format provider, such as a culture. By default, the current culture is used.</param>
	/// <param name="format">A decimal format string. See Decimal.ToString(string). By default, F2 (financial with 2 decimals) is used.</param>
	public string ToDisplayString(IFormatProvider provider = null!, string format = "F2") => ((Amount)this).ToDisplayString(provider, format);

	// Implicit to decimal, and explicit from (we cannot make assumptions about the sign)
	public static implicit operator decimal(NegativeAmount negativeAmount) => negativeAmount.Value;
	public static explicit operator NegativeAmount(decimal negativeAmount) => new(negativeAmount);

	// Implicit to Amount, and explicit from (we cannot make assumptions about the sign)
	public static implicit operator Amount(NegativeAmount negativeAmount) => new(negativeAmount.Value);
	public static explicit operator NegativeAmount(Amount negativeAmount) => negativeAmount.AssumeNegative();

	#region Mathematical operators
	public static NegativeAmount operator +(NegativeAmount one, NegativeAmount two) => (NegativeAmount)(one.Value + two.Value); // Always negative
	public static PositiveAmount operator *(NegativeAmount one, NegativeAmount two) => (PositiveAmount)(one.Value * two.Value); // Always positive
	public static PositiveAmount operator /(NegativeAmount one, NegativeAmount two) => (PositiveAmount)(one.Value / two.Value); // Always positive
	public static NegativeAmount operator -(NegativeAmount one, PositiveAmount two) => (NegativeAmount)(one.Value - two.Value); // Always negative
	public static NegativeAmount operator *(NegativeAmount one, PositiveAmount two) => (NegativeAmount)(one.Value * two.Value); // Always negative
	public static NegativeAmount operator /(NegativeAmount one, PositiveAmount two) => (NegativeAmount)(one.Value / two.Value); // Always negative
	#endregion
}