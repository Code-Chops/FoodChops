namespace VendingMachine.Domain;

/// <summary>
/// A product stack is a stack of products in the vending machine (in the same compartment behind each other).
/// </summary>
public class ProductStack
{
	public override string ToString() => $"{nameof(ProductStack)}: {this.Product}";

	public Product Product { get; }
	public uint AvailablePortions { get; private set; }

	public ProductStack(Product product, uint availablePortions)
	{
		this.Product = product;
		this.AvailablePortions = availablePortions;
	}

	public void RemoveProduct()
	{
		if (this.AvailablePortions == 0)
		{
			throw new Exception($"{this} has no portions available.");
		}

		this.AvailablePortions--;
	}
}
