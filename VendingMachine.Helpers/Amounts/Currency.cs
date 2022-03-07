using System.Reflection;
using Newtonsoft.Json;

namespace VendingMachine.Helpers.Amounts;

/// <summary>
/// A monetary currency.
/// Can be created by casting, from a currency code string or from an ISO numeric currency code (u)short.
/// </summary>
public sealed class Currency : IEquatable<Currency>, IComparable<Currency>, IEquatable<string>, IComparable<string> // Class to avoid struct's unremovable public default constructor
{
	public static readonly Currency Eur = new Currency("EUR", 978, "€", 2);
	public static readonly Currency Usd = new Currency("USD", 840, "$", 2);
	public static readonly Currency Gbp = new Currency("GBP", 826, "£", 2);

	public static readonly Currency Ars = new Currency("ARS", 032, "$", 2);
	public static readonly Currency Aud = new Currency("AUD", 036, "$", 2);
	public static readonly Currency Brl = new Currency("BRL", 986, "R$", 2);
	public static readonly Currency Cad = new Currency("CAD", 124, "$", 2);
	public static readonly Currency Chf = new Currency("CHF", 756, "CHF", 2);
	public static readonly Currency Cny = new Currency("CNY", 156, "¥", 2);
	public static readonly Currency Czk = new Currency("CZK", 203, "Kč", 2);
	public static readonly Currency Dkk = new Currency("DKK", 208, "kr", 2);
	public static readonly Currency Hkd = new Currency("HKD", 344, "HK$", 2);
	public static readonly Currency Hrk = new Currency("HRK", 191, "kn", 2);
	public static readonly Currency Huf = new Currency("HUF", 348, "Ft", 2);
	public static readonly Currency Ils = new Currency("ILS", 376, "₪", 2);
	public static readonly Currency Isk = new Currency("ISK", 352, "kr", 2);
	public static readonly Currency Jpy = new Currency("JPY", 392, "¥", 0);
	public static readonly Currency Mxn = new Currency("MXN", 484, "$", 2);
	public static readonly Currency Myr = new Currency("MYR", 458, "RM", 2);
	public static readonly Currency Nok = new Currency("NOK", 578, "kr", 2);
	public static readonly Currency Nzd = new Currency("NZD", 554, "$", 2);
	public static readonly Currency Php = new Currency("PHP", 608, "₱", 2);
	public static readonly Currency Pln = new Currency("PLN", 985, "zł", 2);
	public static readonly Currency Ron = new Currency("RON", 946, "lei", 2);
	public static readonly Currency Rub = new Currency("RUB", 643, "₽", 2);
	public static readonly Currency Sek = new Currency("SEK", 752, "kr", 2);
	public static readonly Currency Sgd = new Currency("SGD", 702, "S$", 2);
	public static readonly Currency Thb = new Currency("THB", 764, "฿", 2);
	public static readonly Currency Try = new Currency("TRY", 949, "₺", 2);
	public static readonly Currency Twd = new Currency("TWD", 901, "NT$", 2);
	public static readonly Currency Zar = new Currency("ZAR", 710, "R", 2);

	private static readonly IReadOnlyDictionary<string, Currency> CurrenciesByCode = typeof(Currency)
		.GetFields(BindingFlags.Public | BindingFlags.Static) // Static fields
		.Where(field => field.Name.Length == 3) // With 3-character name
		.ToDictionary(field => field.Name, field => (Currency)field.GetValue(null), comparer: StringComparer.OrdinalIgnoreCase);

	private static readonly IReadOnlyDictionary<ushort, Currency> CurrenciesByIsoNumericCurrencyCode = CurrenciesByCode
		.ToDictionary(pair => pair.Value.IsoNumericCode, pair => pair.Value);

	public override string ToString() => this.Code;
	public override int GetHashCode() => this.IsoNumericCode;
	public override bool Equals(object other) => (other is Currency currencyOther && this.Equals(currencyOther)) || (other is string stringOther && this.Equals(stringOther));
	public bool Equals(Currency other) => ReferenceEquals(this, other); // Instances are fixed, so we can use reference equality for performance
	public int CompareTo(Currency other) => String.CompareOrdinal(this, other);
	public bool Equals(string other) => String.Equals(this, other, StringComparison.OrdinalIgnoreCase);
	public int CompareTo(string other) => String.Compare(this, other, StringComparison.OrdinalIgnoreCase);
	public static bool operator ==(Currency one, Currency two) => one?.Equals(two) ?? two is null;
	public static bool operator !=(Currency one, Currency two) => !(one == two);
	public static bool operator >(Currency one, Currency two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) > 0;
	public static bool operator <(Currency one, Currency two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) < 0;
	public static bool operator >=(Currency one, Currency two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) >= 0;
	public static bool operator <=(Currency one, Currency two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) <= 0;
	public static bool operator ==(Currency one, string two) => one?.Equals(two) ?? two is null;
	public static bool operator !=(Currency one, string two) => !(one == two);
	public static bool operator >(Currency one, string two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) > 0;
	public static bool operator <(Currency one, string two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) < 0;
	public static bool operator >=(Currency one, string two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) >= 0;
	public static bool operator <=(Currency one, string two) => String.Compare(one, two, StringComparison.OrdinalIgnoreCase) <= 0;
	public static bool operator ==(string one, Currency two) => two == one;
	public static bool operator !=(string one, Currency two) => !(one == two);
	public static bool operator >(string one, Currency two) => two < one;
	public static bool operator <(string one, Currency two) => two > one;
	public static bool operator >=(string one, Currency two) => two <= one;
	public static bool operator <=(string one, Currency two) => two >= one;

	public string Code { get; }
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

	private static Currency GetByCode(string code) => CurrenciesByCode.TryGetValue(code, out var currency)
		? currency
		: throw new KeyNotFoundException($"Currency {code} is not defined.");
	private static Currency GetByIsoNumericCode(ushort isoNumericCode) => CurrenciesByIsoNumericCurrencyCode.TryGetValue(isoNumericCode, out var currency)
		? currency
		: throw new KeyNotFoundException($"Currency {isoNumericCode} is not defined.");

	public static bool TryParse(string currencyCode, out Currency currency) => CurrenciesByCode.TryGetValue(currencyCode, out currency);
	public static bool TryParse(ushort isoNumericCurrencyCode, out Currency currency) => CurrenciesByIsoNumericCurrencyCode.TryGetValue(isoNumericCurrencyCode, out currency);

	// Implicit to string, and explicit from (as we cannot assume that just any string is a currency code)
	public static implicit operator string(Currency currency) => currency?.Code;
	public static explicit operator Currency(string currencyCode) => GetByCode(currencyCode);

	// Implicit to ushort, and explicit from (as we cannot assume that just any ushort is an ISO numeric currency code)
	public static explicit operator Currency(ushort isoNumericCurrencyCode) => GetByIsoNumericCode(isoNumericCurrencyCode);

	// Implicit to short, and explicit from (as we cannot assume that just any short is an ISO numeric currency code)
	public static explicit operator Currency(short isoNumericCurrencyCode) => GetByIsoNumericCode((ushort)isoNumericCurrencyCode);
}