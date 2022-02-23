using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Domain;

namespace VendingMachine.App.DomainExtensions
{
	public static class GuiWallet
	{
		public static string GetName(this WalletName walletName, CultureInfo? culture = null)
		{
			return Resources.WalletName.ResourceManager.GetString(walletName.ToString(), culture)
				?? throw new Exception($"{nameof(WalletName)} {walletName} not found.");
		}
	}
}
