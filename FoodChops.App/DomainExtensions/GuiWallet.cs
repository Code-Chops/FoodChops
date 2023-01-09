using System.Globalization;
using CodeChops.FoodChops.App.Domain;

namespace CodeChops.FoodChops.App.DomainExtensions;

public static class GuiWallet
{
	public static string GetName(this WalletType walletName, CultureInfo? culture = null)
	{
		return Resources.WalletName.ResourceManager.GetString(walletName.ToString()!, culture)
			?? throw new Exception($"{nameof(WalletType)} {walletName} not found.");
	}
}