using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Domain.Coins;
using VendingMachine.Helpers.Amounts;
using Xunit;

namespace VendingMachine.Domain.Tests
{
	public class VendingMachineTests
	{
		/// <summary>
		/// Tests the construction (and therefore valid state) of the vending machine.
		/// </summary>
		[Theory]
		[InlineData(3, 3, 7)]
		[InlineData(2, 3, 6)]
		[InlineData(1, 9, 9)]
		public void ConstructionProductStackRowsTest(int expectedVerticalProductStackRowCount, int horizontalProductStackCount, int productStackCount)
		{
			var productStacks = Enumerable.Repeat(
				element: new ProductStack(product: new Product(type: ProductType.ChickenSoup, price: new PositiveMoney(Currency.Eur, 1m)), availablePortions: 10),
				count: productStackCount);

			var machine = new Machine(
				productStacks.ToList(),
				horizontalProductStackCount,
				availableCoinsWallet: new Wallet(
					name: WalletName.VendingMachine,
					coinTypesWithQuantity: new Dictionary<CoinType, uint?>(),
					Currency.Eur),
				userInsertedCoinsWallet: new Wallet(
					name: WalletName.VendingMachine,
					coinTypesWithQuantity: new Dictionary<CoinType, uint?>(),
					Currency.Eur),
				color: MachineColor.Red);

			Assert.Equal(expectedVerticalProductStackRowCount, machine.ProductStackGridSize.Height);
		}
	}
}
