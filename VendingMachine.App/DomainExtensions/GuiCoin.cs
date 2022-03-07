using CodeChops.VendingMachine.App.Domain;
using CodeChops.VendingMachine.App.Resources;

namespace CodeChops.VendingMachine.App.DomainExtensions;

internal static class GuiCoin
{
	public static string GetImage(this Coin coin)
	{
		var coinImageName = $"Coin{coin}";
		var image = (byte[]?)Images.ResourceManager.GetObject(coinImageName)
					?? throw new Exception($"{nameof(coinImageName)} {coinImageName} not found.");

		return Convert.ToBase64String(image);
	}
}