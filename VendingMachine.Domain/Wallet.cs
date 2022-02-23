using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using VendingMachine.Domain.Coins;
using VendingMachine.Helpers.Amounts;

namespace VendingMachine.Domain
{
	public enum WalletName
	{
		User,
		VendingMachine,
		UserInserted,
		Change,
	}

	/// <summary>
	/// A coin quantity of NULL means an unlimited amount of coins.
	/// </summary>
	public class Wallet
	{
		public override string ToString() => $"{nameof(Wallet)} {nameof(Name)}:{this.Name.ToString()}, {this.Amount?.ToString() ?? "∞"}{this.Currency}";

		public WalletName Name { get; }

		/// <summary>
		/// Ordered by coin value. Make public getter that is of IReadOnlyDictionary, so data cannot be altered from outside.
		/// </summary>
		public IReadOnlyDictionary<CoinBase, uint?> OrderedCoinsWithQuantity =>
			this._coinsWithQuantity
				.OrderByDescending(coinWithQuantity => (decimal)coinWithQuantity.Key)
				.ToDictionary(
					keySelector: coinWithQuantity => coinWithQuantity.Key,
					elementSelector: coinWithQuantity => coinWithQuantity.Value);

		private readonly Dictionary<CoinBase, uint?> _coinsWithQuantity;

		public Currency Currency { get; }

		public decimal? Amount => this._coinsWithQuantity.Values.Any(quantity => quantity is null)
			? null 
			: this._coinsWithQuantity.Sum(coinAndQuantity => coinAndQuantity.Key * coinAndQuantity.Value);

		public static implicit operator decimal?(Wallet wallet) => wallet.Amount;

		public Wallet(WalletName name, Dictionary<CoinType, uint?> coinTypesWithQuantity, Currency? currency = null)
			: this(
				name,
				coinsWithQuantity: coinTypesWithQuantity
					.Select(coinTypeWithQuantity => (Coin: CoinBase.GetCoinByType(coinTypeWithQuantity.Key), Quantity: coinTypeWithQuantity.Value))
					.ToDictionary(
						keySelector: coindAndQuantity => coindAndQuantity.Coin,
						elementSelector: coindAndQuantity => coindAndQuantity.Quantity), 
				currency)
		{
		}

		public Wallet(WalletName name, Dictionary<CoinBase, uint?> coinsWithQuantity, Currency? currency = null)
		{
			// Get the implementation of the coin type and create a dictionary with quantity as value
			this._coinsWithQuantity = coinsWithQuantity;

			if (coinsWithQuantity.Values.Any(quantity => quantity == 0))
			{
				throw new Exception($"A coin is defined for a {name} but no quantity is provided.");
			}

			this.Name = name;
			this.Currency = ValidateWalletCurrency();

			Currency ValidateWalletCurrency()
			{
				var uniqueCoinCurrencies = this._coinsWithQuantity
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
					if (!this._coinsWithQuantity.Any())
					{
						throw new Exception("Cannot create wallet: Currency is unknown when no coins are provided.");
					}

					return this._coinsWithQuantity.First().Key.Value.Currency;
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
		public void TransferAllCoins(Wallet destinationWallet)
		{
			// Create a new (materialized) list so this collection won't be modified.
			// Therefore the enumeration below can procede.
			var coinsAndQuantity = this._coinsWithQuantity
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

			foreach (var (coin, quantity) in wallet._coinsWithQuantity)
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
		public bool TransferOneCoin(Wallet destinationWallet, CoinBase coin)
		{
			if (destinationWallet.Currency != this.Currency)
			{
				throw new Exception("The currency should be the same of the wallets that transfer money.");
			}

			if (!this._coinsWithQuantity.TryGetValue(coin, out var sourceWalletCoinQuantity) || sourceWalletCoinQuantity == 0)
			{
				return false;
			}

			return this.RemoveCoin(coin) && destinationWallet.AddCoin(coin);
		}

		private bool RemoveCoin(CoinBase coin)
		{
			// Add the coin to the destination wallet
			if (!this._coinsWithQuantity.TryGetValue(coin, out var coinQuantity))
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
					this._coinsWithQuantity.Remove(coin);
					break;
				default:
					this._coinsWithQuantity[coin]--;
					break;
			}

			return true;
		}

		private bool AddCoin(CoinBase coin)
		{
			// Add the coin to the wallet
			if (this._coinsWithQuantity.TryGetValue(coin, out var destinationCoinQuantity))
			{
				if (destinationCoinQuantity != null) this._coinsWithQuantity[coin]++;
			}
			else
			{
				this._coinsWithQuantity.Add(coin, 1);
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
				name: WalletName.Change,
				coinsWithQuantity: new Dictionary<CoinBase, uint?>(),
				currency: this.Currency);

			var availableCoinsWallet = new Wallet(
				name: WalletName.VendingMachine,
				coinsWithQuantity: new Dictionary<CoinBase, uint?>(this._coinsWithQuantity),
				currency: this.Currency);

			return GoIntoNode(deposit) ? changeWallet : null;

			bool GoIntoNode(decimal remainingDeposit, CoinBase? coin = null)
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
					if (coin != null && coinToCheck.Value > coin) continue;

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
}
