using CodeChops.FoodChops.App.Domain;
using Microsoft.AspNetCore.Components;

namespace CodeChops.FoodChops.App.Components;

public class WalletComponent : ComponentBase
{
	[Parameter] public required Wallet Instance { get; set; } = null!;
	[Parameter] public required Action<Coin?>? OnClick { get; set; }
	[Parameter] public required Point LocationInPercentages { get; set; }
	[Parameter] public string Tooltip { get; set; } = null!;
	[Parameter] public string Message { get; set; } = null!;

	internal static MarkupString GetAmountText(decimal? amount)
	{
		return amount is null
			? (MarkupString)"<span class=\"infinite\">∞</span>"
			: new MarkupString(amount.ToString()!);
	}
}