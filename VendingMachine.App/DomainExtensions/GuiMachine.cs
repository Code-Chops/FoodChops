using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.App.Resources;
using VendingMachine.Domain;

namespace VendingMachine.App.DomainExtensions
{
	internal static class GuiMachine
	{
		public static Size ProductWindowSize = new Size(345, 445);
		public static Point PriceLocation = new Point(465, 262);
		public static Point ProductWindowOffset = new Point(44, 41);

		public static Point BuyButtonOffset = new Point(453, 90);
		public static Size BuyButtonSize = new Size(54, 120);

		private static Point _inBetweenProductsOffset = new Point(10, 10);

		public static string GetImage(this MachineColor color)
		{
			var machineImageName = $"{nameof(VendingMachine)}{color.ToString()}";
			var image = (byte[]?)Images.ResourceManager.GetObject(machineImageName)
			            ?? throw new Exception($"{nameof(machineImageName)} {machineImageName} not found.");

			return Convert.ToBase64String(image);
		}

		public static Point GetProductStackOffsetInPixels(this Machine machine, Point productOffset)
		{
			var productSize = GuiProduct.GetSizeInPixels(machine);

			return new Point(
				x: productOffset.X * (productSize.Width + _inBetweenProductsOffset.X) + ProductWindowOffset.X + _inBetweenProductsOffset.X / 2,
				y: productOffset.Y * (productSize.Height + _inBetweenProductsOffset.Y) + ProductWindowOffset.Y + _inBetweenProductsOffset.Y / 2);
		}

		/// <summary>
		/// Returns NULL if no message is available.
		/// </summary>
		public static MarkupString? GetMessageIfAvailable(this Machine machine)
		{
			if (machine.AvailableCoinsWallet.Amount == 0) return new MarkupString($"{Messages.OutOfOrder}: {Messages.NoCoinsAvailable}.");

			if (!machine.HasProductsAvailable()) return new MarkupString($"{Messages.OutOfOrder}: {Messages.NoProductsAvailable}.");

			return null;
		}

	}
}
