using VendingMachine.App.DomainExtensions;
using VendingMachine.Domain;
using VendingMachine.Domain.Coins;
using Xunit;

namespace VendingMachine.App.Tests;

public class ResourceImagesTests
{
	[Fact]
	public void CoinImagesExistTest()
	{
		var allCoinImagesExist = GetEnumValues<CoinType>()
			.All(coinType => GuiCoin.GetImage(coinType) != null);

		Assert.True(allCoinImagesExist);
	}

	[Fact]
	public void ProductImagesExistTest()
	{
		var allProductImagesExist = GetEnumValues<ProductType>()
			.All(productType => GuiProduct.GetImage(productType) != null);

		Assert.True(allProductImagesExist);
	}

	[Fact]
	public void VendingMachineImagesExistTest()
	{
		var allVendingMachineImagesExist = GetEnumValues<MachineColor>()
			.All(color => GuiMachine.GetImage(color) != null);

		Assert.True(allVendingMachineImagesExist);
	}

	private static IEnumerable<TEnum> GetEnumValues<TEnum>()
	{
		return Enum.GetValues(typeof(TEnum))
			.Cast<TEnum>()
			.ToList();
	}
}

