using System;
using System.Globalization;
using Newtonsoft.Json;

namespace VendingMachine.Helpers.Amounts
{
	/// <summary>
	/// A positive monetary amount.
	/// Can be zero.
	/// </summary>
	[JsonConverter(typeof(PositiveAmountJsonConverter))]
	public readonly struct PositiveAmount : IEquatable<PositiveAmount>, IComparable<PositiveAmount>, IEquatable<Amount>, IComparable<Amount>
	{
		public override string ToString() => this.ToDisplayString(CultureInfo.InvariantCulture);
		public override int GetHashCode() => this.Value.GetHashCode();
		public override bool Equals(object other) => (other is PositiveAmount positiveAmount && this.Equals(positiveAmount)) || (other is Amount amount && this.Equals(amount));
		public bool Equals(PositiveAmount other) => this.Value.Equals(other.Value);
		public int CompareTo(PositiveAmount other) => this.Value.CompareTo(other.Value);
		public bool Equals(Amount other) => this.Value.Equals(other.Value);
		public int CompareTo(Amount other) => this.Value.CompareTo(other.Value);
		public static bool operator ==(PositiveAmount one, PositiveAmount two) => one.Equals(two);
		public static bool operator !=(PositiveAmount one, PositiveAmount two) => !(one == two);
		// Equality operator with Amount is done via implicit cast
		// Equality operator with NegativeAmount is done via implicit cast

		public decimal Value { get; }

		public PositiveAmount(decimal value)
		{
			if (value < 0m) throw new ArgumentException("Value must be positive.");

			this.Value = value;
		}

		public static bool TryParse(decimal amountDecimal, out PositiveAmount amount)
		{
			if (!Amount.TryParse(amountDecimal, out var convertedAmount) || amountDecimal < 0m)
			{
				amount = default;
				return false;
			}
			amount = (PositiveAmount)convertedAmount;
			return true;
		}
		public static bool TryParse(string amountString, out PositiveAmount amount)
		{
			if (!Amount.TryParse(amountString, out var convertedAmount) || convertedAmount < 0m)
			{
				amount = default;
				return false;
			}
			amount = (PositiveAmount)convertedAmount;
			return true;
		}

		/// <summary>
		/// Returns a new amount whose value equals the given number of whole cents, e.g. 115 cents is 1.15 whole units.
		/// </summary>
		public static PositiveAmount FromCents(ulong cents) => new PositiveAmount(cents / 100m);
		/// <summary>
		/// Returns the number of whole cents equal to this amount, e.g. 1.15 whole units is 115 cents.
		/// Throws if this amount has subcent precision.
		/// </summary>
		public ulong ToCents() => this.RoundToCents() == this ? (ulong)(this.Value * 100m) : throw new InvalidOperationException($"Amount contains subcents: {this.ToDisplayString(CultureInfo.InvariantCulture, "G")}.");
		/// <summary>
		/// Returns an amount whose value is rounded to the nearest cent, i.e. two decimals, rounding away from zero.
		/// </summary>
		public PositiveAmount RoundToCents()
		{
			return (PositiveAmount)Decimal.Round(this.Value, 2, MidpointRounding.AwayFromZero);
		}

		/// <summary>
		/// Returns a string representation for display purposes.
		/// </summary>
		/// <param name="provider">A format provider, such as a culture. By default, the current culture is used.</param>
		/// <param name="format">A decimal format string. See Decimal.ToString(string). By default, F2 (financial with 2 decimals) is used.</param>
		public string ToDisplayString(IFormatProvider provider = null, string format = "F2") => ((Amount)this).ToDisplayString(provider, format);

		// Implicit to decimal, and explicit from (we cannot make assumptions about the sign)
		public static implicit operator decimal(PositiveAmount positiveAmount) => positiveAmount.Value;
		public static implicit operator PositiveAmount(decimal positiveAmount) => new Amount(positiveAmount).AssumePositive();

		// Implicit to Amount, and explicit from (we cannot make assumptions about the sign)
		public static implicit operator Amount(PositiveAmount positiveAmount) => new Amount(positiveAmount.Value);
		public static explicit operator PositiveAmount(Amount positiveAmount) => positiveAmount.AssumePositive();

		#region Mathematical operators
		public static PositiveAmount operator +(PositiveAmount one, PositiveAmount two) => (PositiveAmount)(one.Value + two.Value); // Always positive
		public static PositiveAmount operator *(PositiveAmount one, PositiveAmount two) => (PositiveAmount)(one.Value * two.Value); // Always positive
		public static PositiveAmount operator /(PositiveAmount one, PositiveAmount two) => (PositiveAmount)(one.Value / two.Value); // Always positive
		public static PositiveAmount operator -(PositiveAmount one, NegativeAmount two) => (PositiveAmount)(one.Value - two.Value); // Always positive
		public static NegativeAmount operator *(PositiveAmount one, NegativeAmount two) => (NegativeAmount)(one.Value * two.Value); // Always negative
		public static NegativeAmount operator /(PositiveAmount one, NegativeAmount two) => (NegativeAmount)(one.Value / two.Value); // Always negative
		#endregion
	}

	internal class PositiveAmountJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var amount = (PositiveAmount)value;
			writer.WriteValue(amount.Value);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var amount = (decimal)reader.Value;
			return (PositiveAmount)amount;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(PositiveAmount);
		}
	}
}
