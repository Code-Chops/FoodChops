using CodeChops.VendingMachine.App.Domain;
using Microsoft.AspNetCore.Components;

namespace CodeChops.VendingMachine.App.DomainExtensions;

internal static class GuiMachine
{
	public static Size ProductWindowSize			{ get; } = new(345, 445);
	public static Point PriceLocation				{ get; } = new(465, 262);
	public static Point ProductWindowOffset			{ get; } = new(44, 41);

	public static Point BuyButtonOffset				{ get; } = new(453, 90);
	public static Size BuyButtonSize				{ get; } = new(54, 120);

	private static Point InBetweenProductsOffset	{ get; } = new(10, 10);

	public static string GetImage(this MachineColor color)
	{
		var machineImageName = $"{nameof(VendingMachine)}{color}";
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
		if (machine.AvailableCoinsWallet.Amount == 0) return new MarkupString($"{Resources.Messages.OutOfOrder}: {Resources.Messages.NoCoinsAvailable}.");

		if (!machine.HasProductsAvailable()) return new MarkupString($"{Resources.Messages.OutOfOrder}: {Resources.Messages.NoProductsAvailable}.");

		return null;
	}
}
