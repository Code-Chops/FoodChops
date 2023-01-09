using CodeChops.FoodChops.App.Domain;
using CodeChops.FoodChops.App.Resources;

namespace CodeChops.FoodChops.App.DomainExtensions;

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