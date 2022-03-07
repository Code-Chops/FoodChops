using System.Globalization;
using CodeChops.VendingMachine.App.Domain;
using CodeChops.VendingMachine.App.Resources;

namespace CodeChops.VendingMachine.App.DomainExtensions;

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
		return new Size(
			width: GuiMachine.ProductWindowSize.Width / machine.ProductStackGridSize.Width - 10,
			height: GuiMachine.ProductWindowSize.Height / machine.ProductStackGridSize.Height - 10);
	}

	public static string? GetName(this ProductType productType, CultureInfo? culture = null)
	{
		return Products.ResourceManager.GetString(productType.ToString(), culture);
	}
}