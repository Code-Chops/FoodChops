using CodeChops.VendingMachine.App.Domain;
using Microsoft.AspNetCore.Components;

namespace CodeChops.VendingMachine.App.Components;

public class WalletComponent : ComponentBase
{
	[Parameter]
	public Wallet Instance { get; set; } = null!;

	[Parameter]
	public Action<Coin?>? OnClick { get; set; }

	[Parameter]
	public Point LocationInPercentages { get; set; }

	[Parameter]
	public string Tooltip { get; set; } = null!;

	internal static MarkupString GetAmountText(decimal? amount)
	{
		return amount is null
			? (MarkupString)"<span class=\"infinite\">∞</span>"
			: new MarkupString(amount.ToString()!);
	}
}