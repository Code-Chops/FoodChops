using CodeChops.VendingMachine.App.Domain;
using CodeChops.VendingMachine.App.DomainExtensions;
using Xunit;

namespace CodeChops.VendingMachine.App.Tests;

public class ResourceImagesTests
{
	[Fact]
	public void CoinImagesExistTest()
	{
		var allCoinImagesExist = Coin.GetEnumerable()
			.All(coinType => coinType.GetImage() != null);

		Assert.True(allCoinImagesExist);
	}

	[Fact]
	public void ProductImagesExistTest()
	{
		var allProductImagesExist = ProductType.GetEnumerable()
			.All(productType => productType.GetImage() != null);

		Assert.True(allProductImagesExist);
	}

	[Fact]
	public void VendingMachineImagesExistTest()
	{
		var allVendingMachineImagesExist = MachineColor.GetEnumerable()
			.All(color => color.GetImage() != null);

		Assert.True(allVendingMachineImagesExist);
	}
}