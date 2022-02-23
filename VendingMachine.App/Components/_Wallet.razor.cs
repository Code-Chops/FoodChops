using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Microsoft.AspNetCore.Components;
using VendingMachine.Domain;
using VendingMachine.Domain.Coins;

namespace VendingMachine.App.Components;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
public class WalletComponent : ComponentBase
{
	[Parameter]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	public Wallet Instance { get; set; }

	[Parameter]
	public Action<CoinBase>? OnClick { get; set; }

	[Parameter]
	public Point LocationInPercentages { get; set; }

	[Parameter]
	public string Tooltip { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

	internal static MarkupString GetAmountText(decimal? amount)
	{
		return amount is null
			? (MarkupString)"<span class='oi oi-infinity'></span>"
			: new MarkupString(amount.ToString());
	}
    }
