using CodeChops.MagicEnums;
using CodeChops.VendingMachine.App.Domain.Amounts;

namespace CodeChops.VendingMachine.App.Domain;

public record ProductType : MagicStringEnum<ProductType>
{
	public static ProductType Tea	{ get; }		= CreateMember();
	public static ProductType Espresso { get; }		= CreateMember();
	public static ProductType Juice { get; }		= CreateMember();
	public static ProductType ChickenSoup { get; }	= CreateMember();
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
