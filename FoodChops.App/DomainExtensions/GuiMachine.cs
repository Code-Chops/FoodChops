using CodeChops.FoodChops.App.Domain;
using Microsoft.AspNetCore.Components;

namespace CodeChops.FoodChops.App.DomainExtensions;

internal static class GuiMachine
{
	public static Size ProductWindowSize			{ get; } = new(180, 280);
	public static Point PriceLocation				{ get; } = new(273, 253);
	public static Point ProductWindowOffset			{ get; } = new(75, 41);

	public static Point BuyButtonOffset				{ get; } = new(270, 33);
	public static Size BuyButtonSize				{ get; } = new(53, 135);

	private static Point InBetweenProductsOffset	{ get; } = new(10, 50);

	public static string GetImage(this MachineTheme theme)
	{
		var machineImageName = $"VendingMachine{theme}";
		var image = (byte[]?)Resources.Images.ResourceManager.GetObject(machineImageName)
					?? throw new Exception($"{nameof(machineImageName)} {machineImageName} not found.");

		return Convert.ToBase64String(image);
	}

	public static Point GetProductStackOffsetInPixels(this Machine machine, Point productOffset)
	{
		var productSize = GuiProduct.GetSizeInPixels(machine);

		return new Point(
			x: productOffset.X * (productSize.Width		+ InBetweenProductsOffset.X) + ProductWindowOffset.X + InBetweenProductsOffset.X / 2,
			y: productOffset.Y * (productSize.Height	+ InBetweenProductsOffset.Y) + ProductWindowOffset.Y + InBetweenProductsOffset.Y / 2);
	}

	/// <summary>
	/// Returns NULL if no message is available.
	/// </summary>
	public static MarkupString? GetMessageIfAvailable(this Machine machine)
	{
		if (machine.AvailableCoinsWallet.Amount == 0) 
			return new MarkupString($"{Resources.Messages.OutOfOrder}: {Resources.Messages.NoCoinsAvailable}.");

		if (!machine.HasProductsAvailable()) 
			return new MarkupString($"{Resources.Messages.OutOfOrder}: {Resources.Messages.NoProductsAvailable}.");

		return null;
	}
}
