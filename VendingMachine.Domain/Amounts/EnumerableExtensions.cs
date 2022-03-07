namespace CodeChops.VendingMachine.App.Domain.Amounts;

public static class EnumerableExtensions
{
	/// <summary>
	/// Sums the money amounts.
	/// Throws if different currencies are combined.
	/// If a default currency is given and the sequence is empty, a zero-value instance of that currency is returned.
	/// </summary>
	public static Money Sum(this IEnumerable<Money> source, Currency defaultCurrency)
	{
		if (source is null) throw new ArgumentNullException(nameof(source));

		var currency = defaultCurrency;
		var amount = 0m;

		foreach (var money in source)
		{
			if (money.Currency != currency) throw new InvalidOperationException($"Money with different currencies cannot be summed ({currency} vs {money.Currency}).");

			amount += money.Amount.Value;
		}

		return new(currency, amount);
	}

	/// <summary>
	/// Sums the money amounts.
	/// Throws if different currencies are combined.
	/// If a default currency is given and the sequence is empty, a zero-value instance of that currency is returned.
	/// </summary>
	public static Money Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Money> selector, Currency defaultCurrency)
	{
		if (source is null) throw new ArgumentNullException(nameof(source));
		if (selector is null) throw new ArgumentNullException(nameof(selector));

		return source.Select(selector).Sum(defaultCurrency);
	}
}