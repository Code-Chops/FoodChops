using System.Globalization;
using VendingMachine.Domain;

namespace VendingMachine.App.DomainExtensions;

public static class GuiWallet
{
	public static string GetName(this WalletName walletName, CultureInfo? culture = null)
	{
		return Resources.WalletName.ResourceManager.GetString(walletName.ToString(), culture)
			?? throw new Exception($"{nameof(WalletName)} {walletName} not found.");
	}
}
