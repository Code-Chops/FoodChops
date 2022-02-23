using VendingMachine.App.Resources;
using VendingMachine.Domain.Coins;

namespace VendingMachine.App.DomainExtensions;

internal static class GuiCoin
{
	public static string GetImage(this CoinType coinType)
	{
		var coinImageName = $"Coin{coinType.ToString()}";
		var image = (byte[]?)Images.ResourceManager.GetObject(coinImageName)
					?? throw new Exception($"{nameof(coinImageName)} {coinImageName} not found.");

		return Convert.ToBase64String(image);
	}
}
