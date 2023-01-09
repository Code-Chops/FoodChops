using System.Globalization;
using CodeChops.FoodChops.App.Domain;
using CodeChops.FoodChops.App.Resources;

namespace CodeChops.FoodChops.App.DomainExtensions;

internal static class GuiProduct
{
	public static string GetImage(this ProductType productType)
	{
		var productImageName = productType.ToString();
		var image = (byte[]?)Images.ResourceManager.GetObject(productImageName)
					?? throw new Exception($"{nameof(productImageName)} {productImageName} not found.");

		return Convert.ToBase64String(image);
	}

	public static Size GetSizeInPixels(Machine machine)
	{
		var width = GuiMachine.ProductWindowSize.Width / machine.ProductStackGridSize.Width - 10;
		var height = GuiMachine.ProductWindowSize.Height / machine.ProductStackGridSize.Height - 10;

		var dimension = Math.Min(width, height);
		
		return new Size(
			width: dimension,
			height: dimension);
	}

	public static string? GetName(this ProductType productType, CultureInfo? culture = null)
	{
		return Products.ResourceManager.GetString(productType.ToString(), culture);
	}
}