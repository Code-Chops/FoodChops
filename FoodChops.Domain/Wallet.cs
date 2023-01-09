using CodeChops.MagicEnums;
using CodeChops.FoodChops.App.Domain.Amounts;

namespace CodeChops.FoodChops.App.Domain;

public record WalletType : MagicStringEnum<WalletType>
{
	public static WalletType User			{ get; } = CreateMember();
	public static WalletType FoodChops { get; } = CreateMember();
	public static WalletType UserInserted	{ get; } = CreateMember();
	public static WalletType Change			{ get; } = CreateMember();
}


/// <summary>
/// A coin quantity of NULL means an unlimited amount of coins.
/// </summary>
public record Wallet
{
	public override string ToString() => $"{nameof(Wallet)} {nameof(this.Type)}:{this.Type}, {this.Amount?.ToString() ?? "∞"}{this.Currency}";

	public WalletType Type { get; }

	/// <summary>
	/// Ordered by coin value. Make public getter that is of IReadOnlyDictionary, so data cannot be altered from outside.
	/// </summary>
	public IReadOnlyDictionary<Coin, uint?> OrderedCoinsWithQuantity =>
		this.CoinsWithQuantity
			.OrderByDescending(coinWithQuantity => (decimal)coinWithQuantity.Key.Value)
			.ToDictionary(
				keySelector: coinWithQuantity => coinWithQuantity.Key,
				elementSelector: coinWithQuantity => coinWithQuantity.Value);

	private Dictionary<Coin, uint?> CoinsWithQuantity { get; }

	public Currency Currency { get; }

	public decimal? Amount => this.CoinsWithQuantity.Values.Any(quantity => quantity is null)
		? null
		: this.CoinsWithQuantity.Sum(coinAndQuantity => coinAndQuantity.Key.Value * coinAndQuantity.Value);

	public static implicit operator decimal?(Wallet wallet) => wallet.Amount;

	public Wallet(WalletType type, Dictionary<Coin, uint?> coinsWithQuantity, Currency? currency = null)
	{
		// Get the implementation of the coin type and create a dictionary with quantity as value
		this.CoinsWithQuantity = coinsWithQuantity;
		this.Type = type;
		this.Currency = ValidateWalletCurrency();

		Currency ValidateWalletCurrency()
		{
			var uniqueCoinCurrencies = this.CoinsWithQuantity
				.Keys
				.Select(coin => coin.Value.Currency)
				.Distinct()
				.ToList();

			if (uniqueCoinCurrencies.Count > 1)
			{
				var coinCurrenciesText = String.Join(", ", uniqueCoinCurrencies);
				throw new Exception($"Cannot create wallet: You cannot use multiple currencies ({coinCurrenciesText}) in one wallet.");
			}

			if (currency is null)
			{
				if (!this.CoinsWithQuantity.Any())
				{
					throw new Exception("Cannot create wallet: Currency is unknown when no coins are provided.");
				}

				return this.CoinsWithQuantity.First().Key.Value.Currency;
			}
			else
			{
				if (uniqueCoinCurrencies.Any() && !String.Equals(uniqueCoinCurrencies.Single(), currency, StringComparison.OrdinalIgnoreCase))
				{
					throw new Exception($"Cannot create wallet: Provided currency ({currency}) does not match the currency of the coins in the wallet.");
				}

				return currency;
			}
		}
	}

	/// <summary>
	/// Transfer all coins of the source wallet.
	/// </summary>
	public void TransferAllCoinsTo(Wallet destinationWallet)
	{
		// Create a new (materialized) list so this collection won't be modified.
		// Therefore the enumeration below can procede.
		var coinsAndQuantity = this.CoinsWithQuantity
			.Select(coinAndQuantity => (coinAndQuantity.Key, coinAndQuantity.Value))
			.ToList();

		foreach (var (coin, quantity) in coinsAndQuantity)
		{
			if (quantity is null)
			{
				throw new Exception($"Unable to transfer all money to another wallet because coin quantity of {coin} is infinite.");
			}

			for (var i = 0; i < quantity; i++)
			{
				this.TransferOneCoin(destinationWallet, coin);
			}
		}
	}

	/// <summary>
	/// Deposit coins using the smallest quantity of coins possible.
	/// </summary>
	public Wallet? GetChangeBack(Wallet destinationWallet, decimal deposit)
	{
		// Create a new (materialized) list so this collection won't be modified.
		// Therefore the enumeration below can procede.
		var wallet = this.CalculateChangeWithLowestCoinQuantity(deposit);

		if (wallet is null) return null;

		foreach (var (coin, quantity) in wallet.CoinsWithQuantity)
		{
			for (var i = 0; i < quantity; i++)
			{
				this.TransferOneCoin(destinationWallet, coin);
			}
		}

		return wallet;
	}

	/// <summary>
	/// I know transferring one coin at a time is not the most efficient, but it does it's job.
	/// </summary>
	public bool TransferOneCoin(Wallet destinationWallet, Coin coin)
	{
		if (destinationWallet.Currency != this.Currency)
		{
			throw new Exception("The currency should be the same of the wallets that transfer money.");
		}

		if (!this.CoinsWithQuantity.TryGetValue(coin, out var sourceWalletCoinQuantity) || sourceWalletCoinQuantity == 0)
		{
			return false;
		}

		return this.RemoveCoin(coin) && destinationWallet.AddCoin(coin);
	}

	private bool RemoveCoin(Coin coin)
	{
		// Add the coin to the destination wallet
		if (!this.CoinsWithQuantity.TryGetValue(coin, out var coinQuantity))
		{
			return false;
		}

		switch (coinQuantity)
		{
			case null:
				break;
			case 0:
				throw new Exception($"{coin} should already have been removed from {this}");
			case 1:
				this.CoinsWithQuantity.Remove(coin);
				break;
			default:
				this.CoinsWithQuantity[coin]--;
				break;
		}

		return true;
	}

	private bool AddCoin(Coin coin)
	{
		// Add the coin to the wallet
		if (this.CoinsWithQuantity.TryGetValue(coin, out var destinationCoinQuantity))
		{
			if (destinationCoinQuantity != null) this.CoinsWithQuantity[coin]++;
		}
		else
		{
			this.CoinsWithQuantity.Add(coin, 1);
		}

		return true;
	}

	/// <summary>
	/// This idea is based on Depth-first search (DFS). It tries to use the coins with the highest denomination first by going down the nodes (the left number in each group, see below).
	/// If that doesn't succeed (coins are depleted or exceed the remaining amount) it goes up one node and tries to use coins with a lower denomination.
	/// I tried to use Breadth-first search (BFS) first: It seemed to be the best fit as this method looks for the shortest path, but it appeared to be too slow.
	/// 
	/// If 4 different coins are provided:
	/// 
	/// Node row:
	///	0																0
	///	1							1									2								3									4
	///	2			6			7		8		9			10		11		12		13			14		15		16		17			18		19		20		21	
	/// 3	22	23	24	25		....
	/// 
	/// </summary>
	public Wallet? CalculateChangeWithLowestCoinQuantity(decimal deposit)
	{
		var changeWallet = new Wallet(
			type: WalletType.Change,
			coinsWithQuantity: new Dictionary<Coin, uint?>(),
			currency: this.Currency);

		var availableCoinsWallet = new Wallet(
			type: WalletType.FoodChops,
			coinsWithQuantity: new Dictionary<Coin, uint?>(this.CoinsWithQuantity),
			currency: this.Currency);

		return GoIntoNode(deposit) ? changeWallet : null;


		bool GoIntoNode(decimal remainingDeposit, Coin? coin = null)
		{
			if (coin != null)
			{
				if (remainingDeposit < coin.Value) return false;

				var transferSuccess = availableCoinsWallet.TransferOneCoin(changeWallet, coin);
				if (!transferSuccess) return false;

				remainingDeposit -= coin.Value;
				if (remainingDeposit == 0) return true;
			}

			foreach (var coinToCheck in availableCoinsWallet.OrderedCoinsWithQuantity.Keys)
			{
				if (coin != null && coinToCheck.Value > coin.Value) continue;

				var succeeded = GoIntoNode(remainingDeposit, coinToCheck);
				if (succeeded) return true;
			}

			if (coin != null)
			{
				var transferSuccess = changeWallet.TransferOneCoin(availableCoinsWallet, coin);
				if (!transferSuccess) throw new Exception("Transferring back the change to the available coins didn't succeed.");
			}

			return false;
		}
	}
}