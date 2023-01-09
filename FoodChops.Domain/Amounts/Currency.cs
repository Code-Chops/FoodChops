using CodeChops.MagicEnums;

namespace CodeChops.FoodChops.App.Domain.Amounts;

/// <summary>
/// A monetary currency.
/// Can be created by casting, from a currency code string.
/// </summary>
public record Currency : MagicCustomEnum<Currency, Currency>
{
	public static readonly Currency EUR = CreateMember(new("EUR", 978, "€",		2));
	public static readonly Currency USD = CreateMember(new("USD", 840, "$",		2));
	public static readonly Currency GBP = CreateMember(new("GBP", 826, "£",		2));
	public static readonly Currency ARS = CreateMember(new("ARS", 032, "$",		2));
	public static readonly Currency AUD = CreateMember(new("AUD", 036, "$",		2));
	public static readonly Currency BRL = CreateMember(new("BRL", 986, "R$",	2));
	public static readonly Currency CAD = CreateMember(new("CAD", 124, "$",		2));
	public static readonly Currency CHF = CreateMember(new("CHF", 756, "CHF",	2));
	public static readonly Currency CNY = CreateMember(new("CNY", 156, "¥",		2));
	public static readonly Currency CZK = CreateMember(new("CZK", 203, "Kč",	2));
	public static readonly Currency DKK = CreateMember(new("DKK", 208, "kr",	2));
	public static readonly Currency HKD = CreateMember(new("HKD", 344, "HK$",	2));
	public static readonly Currency HRK = CreateMember(new("HRK", 191, "kn",	2));
	public static readonly Currency HUF = CreateMember(new("HUF", 348, "Ft",	2));
	public static readonly Currency ILS = CreateMember(new("ILS", 376, "₪",		2));
	public static readonly Currency ISK = CreateMember(new("ISK", 352, "kr",	2));
	public static readonly Currency JPY = CreateMember(new("JPY", 392, "¥",		0));
	public static readonly Currency MXN = CreateMember(new("MXN", 484, "$",		2));
	public static readonly Currency MYR = CreateMember(new("MYR", 458, "RM",	2));
	public static readonly Currency NOK = CreateMember(new("NOK", 578, "kr",	2));
	public static readonly Currency NZD = CreateMember(new("NZD", 554, "$",		2));
	public static readonly Currency PHP = CreateMember(new("PHP", 608, "₱",		2));
	public static readonly Currency PLN = CreateMember(new("PLN", 985, "zł",	2));
	public static readonly Currency RON = CreateMember(new("RON", 946, "lei",	2));
	public static readonly Currency RUB = CreateMember(new("RUB", 643, "₽",		2));
	public static readonly Currency SEK = CreateMember(new("SEK", 752, "kr",	2));
	public static readonly Currency SGD = CreateMember(new("SGD", 702, "S$",	2));
	public static readonly Currency THB = CreateMember(new("THB", 764, "฿",		2));
	public static readonly Currency TRY = CreateMember(new("TRY", 949, "₺",		2));
	public static readonly Currency TWD = CreateMember(new("TWD", 901, "NT$",	2));
	public static readonly Currency ZAR = CreateMember(new("ZAR", 710, "R",		2));

	public override int GetHashCode() => this.IsoNumericCode;
	public bool Equals(string? other) => String.Equals(this, other, StringComparison.OrdinalIgnoreCase);
	public int CompareTo(string? other) => String.Compare(this, other, StringComparison.OrdinalIgnoreCase);

	public static bool operator >(Currency one, Currency two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) > 0;
	public static bool operator <(Currency one, Currency two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) < 0;
	public static bool operator >=(Currency one, Currency two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) >= 0;
	public static bool operator <=(Currency one, Currency two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) <= 0;
	public static bool operator ==(Currency one, string two) => one.Equals(two);
	public static bool operator !=(Currency one, string two) => !(one == two);
	public static bool operator >(Currency one, string two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) > 0;
	public static bool operator <(Currency one, string two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) < 0;
	public static bool operator >=(Currency one, string two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) >= 0;
	public static bool operator <=(Currency one, string two) => string.Compare(one, two, StringComparison.OrdinalIgnoreCase) <= 0;
	public static bool operator ==(string one, Currency two) => two == one;
	public static bool operator !=(string one, Currency two) => !(one == two);
	public static bool operator >(string one, Currency two) => two < one;
	public static bool operator <(string one, Currency two) => two > one;
	public static bool operator >=(string one, Currency two) => two <= one;
	public static bool operator <=(string one, Currency two) => two >= one;

	/// <summary>
	/// Three letter ISO code.
	/// </summary>
	public string Code { get; }

	/// <summary>
	/// ISO 4217.
	/// </summary>
	public ushort IsoNumericCode { get; }
	public string Symbol { get; }
	public byte DecimalPlaces { get; }

	private Currency(string threeLetterCode, ushort isoNumericCode, string symbol, byte decimalPlaces)
	{
		this.Code = threeLetterCode;
		this.IsoNumericCode = isoNumericCode;
		this.Symbol = symbol;
		this.DecimalPlaces = decimalPlaces;
	}

	// Implicit to string, and explicit from (as we cannot assume that just any string is a currency code)
	public static implicit operator string?(Currency? currency) => currency?.Code;
	public static explicit operator Currency(string currencyCode) => GetSingleMember(currencyCode);
}