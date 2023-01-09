using CodeChops.FoodChops.App.Domain;
using Xunit;

namespace CodeChops.FoodChops.App.UnitTests;

public class ResourceImagesTests
{
	[Fact]
	public void CoinImagesExistTest()
	{
		var _ = Coin.GetMembers();
	}

	[Fact]
	public void ProductImagesExistTest()
	{
		var _ = ProductType.GetMembers();
	}

	[Fact]
	public void FoodChopsImagesExistTest()
	{
		var _ = MachineTheme.GetMembers();
	}
}