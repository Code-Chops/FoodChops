using CodeChops.MagicEnums;
using CodeChops.FoodChops.App.Domain.Amounts;

namespace CodeChops.FoodChops.App.Domain;

public record ProductType : MagicStringEnum<ProductType>
{
	public static ProductType Candy	{ get; }	= CreateMember();
	public static ProductType Coffee { get; }	= CreateMember();
	public static ProductType Beer { get; }		= CreateMember();
	public static ProductType Soup { get; }		= CreateMember();
}

public class Product
{
	public override string ToString() => $"{nameof(Product)}: {this.Type}, {this.Price}";

	public ProductType Type { get; }
	public PositiveMoney Price { get; }

	public Product(ProductType type, PositiveMoney price)
	{
		this.Type = type;
		this.Price = price;
	}
}