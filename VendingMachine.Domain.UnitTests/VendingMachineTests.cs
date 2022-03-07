using CodeChops.VendingMachine.App.Domain;
using CodeChops.VendingMachine.App.Domain.Amounts;
using Xunit;

namespace CodeChops.VendingMachine.Domain.Tests;

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
			element: new ProductStack(product: new(type: ProductType.ChickenSoup, price: new(Currency.EUR, 1m)), availablePortions: 10),
			count: productStackCount);

		var machine = new Machine(
			productStacks.ToList(),
			horizontalProductStackCount,
			availableCoinsWallet: new Wallet(
				type: WalletType.VendingMachine,
				coinsWithQuantity: new Dictionary<Coin, uint?>(),
				Currency.EUR),
			userInsertedCoinsWallet: new Wallet(
				type: WalletType.VendingMachine,
				coinsWithQuantity: new Dictionary<Coin, uint?>(),
				Currency.EUR),
			color: MachineColor.Red);

		Assert.Equal(expectedVerticalProductStackRowCount, (int)machine.ProductStackGridSize.Height);
	}
}