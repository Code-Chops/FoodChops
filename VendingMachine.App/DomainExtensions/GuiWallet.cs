using System.Globalization;
using CodeChops.VendingMachine.App.Domain;

namespace CodeChops.VendingMachine.App.DomainExtensions;

public static class GuiWallet
{
	public static string GetName(this WalletType walletName, CultureInfo? culture = null)
	{
		return Resources.WalletName.ResourceManager.GetString(walletName.ToString()!, culture)
			?? throw new Exception($"{nameof(WalletType)} {walletName} not found.");
	}
}