using System.Drawing;
using VendingMachine.Domain.Coins;
using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain;

public enum MachineColor
{
	Blue,
	Red,
}

public class Machine
{
	public override string ToString() => $"{nameof(Machine)}: {this.Currency}, {nameof(ProductStack)}:{this.ProductStacksByLocation.Length}";

	/// <summary>
	/// A vending machine can support only 1 currency.
	/// </summary>
	public Currency Currency { get; }
	public ProductStack[,] ProductStacksByLocation { get; }
	public Size ProductStackGridSize { get; }
	public Wallet AvailableCoinsWallet { get; }
	public Wallet UserInsertedCoinsWallet { get; }

	/// <summary>
	/// I put this property here (and not in the app logic) because I consider the properties of the visual representation as a part of the domain object itself.
	/// </summary>
	public MachineColor Color { get; }

	public Machine(IReadOnlyList<ProductStack> productStacks, int horizontalProductStackCount, Wallet availableCoinsWallet, Wallet userInsertedCoinsWallet, MachineColor color)
	{
		if (!productStacks.Any())
		{
			throw new Exception("Cannot create vending machine: No product stacks are provided.");
		}

		var currenciesUsed = new[]  { availableCoinsWallet.Currency, userInsertedCoinsWallet.Currency }
			.Concat(productStacks.Select(stack => stack.Product.Price.Currency))
			.Distinct()
			.ToList(); 

		if (currenciesUsed.Count() > 1) throw new Exception("Cannot create vending machine: The currencies of the wallets and products should be the same.");
		this.Currency = currenciesUsed.First();

		// Put product stacks in their slot
		var verticalProductStackCount = (int)Math.Ceiling(productStacks.Count() / (decimal)horizontalProductStackCount);
		this.ProductStacksByLocation = new ProductStack[horizontalProductStackCount, verticalProductStackCount];
		for (var i = 0; i < productStacks.Count; i++)
		{
			this.ProductStacksByLocation[i % horizontalProductStackCount, i / horizontalProductStackCount] = productStacks[i];
		}

		this.ProductStackGridSize = new Size(this.ProductStacksByLocation.GetLength(0), this.ProductStacksByLocation.GetLength(1));

		this.AvailableCoinsWallet = availableCoinsWallet;
		this.UserInsertedCoinsWallet = userInsertedCoinsWallet;

		this.Color = color;
	}

	/// <summary>
	/// Buy a product: checks if amount inserted is correct and returns coins if needed.
	/// </summary>
	/// <returns>A (fictional) wallet that shows the amount transfered</returns>
	public Wallet? BuyProduct(User user, ProductStack productStack)
	{
		var remainingAmount = productStack.Product.Price - this.UserInsertedCoinsWallet.Amount;
		
		// Inserted too little money
		if (remainingAmount > 0) return null;

		// Transfer these coins first so also these coins can be used to give excessive coins back
		this.UserInsertedCoinsWallet.TransferAllCoins(this.AvailableCoinsWallet);
		
		productStack.RemoveProduct();

		// Inserted too much money
		if (remainingAmount < 0)
		{
			return this.AvailableCoinsWallet.GetChangeBack(user.Wallet, (decimal)-remainingAmount);
		}

		return new Wallet(
			name: WalletName.Change,
			coinTypesWithQuantity: new Dictionary<CoinType, uint?>(),
			currency: this.Currency);

	}

	public bool HasProductsAvailable()
	{
		for (var y = 0; y < this.ProductStackGridSize.Height; y++)
		{
			for (var x = 0; x < this.ProductStackGridSize.Width; x++)
			{
				var stack = (ProductStack?)this.ProductStacksByLocation.GetValue(x, y);
				if (stack != null && stack.AvailablePortions > 0)
				{
					return true;
				}
			}
		}

		return false;
	}
}
