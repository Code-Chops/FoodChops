namespace VendingMachine.Helpers.Amounts;

public static class EnumerableExtensions
{
	/// <summary>
	/// Sums the money amounts.
	/// Throws if different currencies are combined.
	/// If a default currency is given and the sequence is empty, a zero-value instance of that currency is returned.
	/// </summary>
	public static Money Sum(this IEnumerable<Money> source, Currency defaultCurrency = null)
	{
		if (source is null) throw new ArgumentNullException(nameof(source));

		var currency = defaultCurrency;
		var amount = 0m;

		foreach (var money in source)
		{
			if (money is null) throw new ArgumentException("Source contains a null value.");

			if (currency is null) currency = money.Currency;
			else if (money.Currency != currency) throw new InvalidOperationException($"Money with different currencies cannot be summed ({currency} vs {money.Currency}).");

			amount += money.Amount.Value;
		}

		if (currency is null) throw new InvalidOperationException("At least one money (or the default currency parameter) is required to return the sum.");

		return new Money(currency, amount);
	}

	/// <summary>
	/// Sums the money amounts.
	/// Throws if different currencies are combined.
	/// If a default currency is given and the sequence is empty, a zero-value instance of that currency is returned.
	/// </summary>
	public static Money Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Money> selector, Currency defaultCurrency = null)
	{
		if (source is null) throw new ArgumentNullException(nameof(source));
		if (selector is null) throw new ArgumentNullException(nameof(selector));

		return source.Select(selector).Sum(defaultCurrency);
	}
}
