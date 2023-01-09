using CodeChops.FoodChops.App.Domain;
using CodeChops.FoodChops.App.Domain.Amounts;
using Xunit;

namespace CodeChops.FoodChops.Domain.UnitTests;

public class FoodChopsTests
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
			element: new ProductStack(product: new(type: ProductType.Soup, price: new(Currency.EUR, 1m)), availablePortions: 10),
			count: productStackCount);

		var machine = new Machine(
			productStacks.ToList(),
			horizontalProductStackCount,
			availableCoinsWallet: new Wallet(
				type: WalletType.FoodChops,
				coinsWithQuantity: new Dictionary<Coin, uint?>(),
				Currency.EUR),
			userInsertedCoinsWallet: new Wallet(
				type: WalletType.FoodChops,
				coinsWithQuantity: new Dictionary<Coin, uint?>(),
				Currency.EUR),
			theme: MachineTheme.Red);

		Assert.Equal(expectedVerticalProductStackRowCount, (int)machine.ProductStackGridSize.Height);
	}
}