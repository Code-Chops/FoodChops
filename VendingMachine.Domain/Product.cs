using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain;

public enum ProductType
{
	Tea,
	Espresso,
	Juice,
	ChickenSoup,
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
