using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VendingMachine.Domain.Coins;
using VendingMachine.Domain.Coins.Eur;
using Xunit;

namespace VendingMachine.Domain.Tests
{
	public class LeastQuantityOfCoinsForChangeTests
	{
		[Theory]

		[InlineData(
			1,
			new[]
			{
				CoinType.Eur1,
			},
			new[]
			{
				CoinType.Eur1
			})]

		[InlineData(
			0.4,
			new[]
			{
				CoinType.Eur1,
			},
			null)]

		[InlineData(
			0.5,
			new[]
			{
				CoinType.Eur1,
				CoinType.EurCent20,
				CoinType.EurCent50
			},
			new[]
			{
				CoinType.EurCent50
			})]

		[InlineData(
			1.1,
			new[]
			{
				CoinType.EurCent50, CoinType.EurCent50,
				CoinType.EurCent20, CoinType.EurCent20, CoinType.EurCent20
			},
			new[]
			{
				CoinType.EurCent20, CoinType.EurCent20, CoinType.EurCent20,
				CoinType.EurCent50
			})]

		[InlineData(
			1.2,
			new[]
			{
				CoinType.EurCent20, CoinType.EurCent20,
				CoinType.EurCent50, CoinType.EurCent50, CoinType.EurCent50,
				CoinType.EurCent10, CoinType.EurCent10, CoinType.EurCent10
			},
			new[]
			{
				CoinType.EurCent20,
				CoinType.EurCent50, CoinType.EurCent50,
			})]

		[InlineData(
			1.8,
			new[]
			{
				CoinType.EurCent20, CoinType.EurCent20,
				CoinType.EurCent50, CoinType.EurCent50, CoinType.EurCent50,
				CoinType.EurCent10, CoinType.EurCent10, CoinType.EurCent10, CoinType.EurCent10, CoinType.EurCent10
			},
			new[]
			{
				CoinType.EurCent50, CoinType.EurCent50, CoinType.EurCent50,
				CoinType.EurCent20,
				CoinType.EurCent10,
			})]

		[InlineData(
			1.6,
			new[]
			{
				CoinType.Eur1, CoinType.Eur1,
				CoinType.EurCent10, CoinType.EurCent10,
				CoinType.EurCent50, CoinType.EurCent50, CoinType.EurCent50, CoinType.EurCent50
			},
			new[]
			{
				CoinType.Eur1,
				CoinType.EurCent50,
				CoinType.EurCent10
			})]

		[InlineData(
			2.1,
			new[]
			{
				CoinType.Eur1, CoinType.Eur1, CoinType.Eur1, CoinType.Eur1,
				CoinType.EurCent20, CoinType.EurCent20, CoinType.EurCent20,
				CoinType.EurCent50, CoinType.EurCent50
			},
			new[]
			{
				CoinType.EurCent20, CoinType.EurCent20, CoinType.EurCent20,
				CoinType.EurCent50,
				CoinType.Eur1,
			})]

		[InlineData(
			2.8,
			new[]
			{
				CoinType.Eur1, CoinType.Eur1, CoinType.Eur1, CoinType.Eur1,
				CoinType.EurCent20, CoinType.EurCent20, CoinType.EurCent20,
				CoinType.EurCent50
			},
			null)]

		public void CorrectChangeTest(decimal changeAmount, CoinType[] availableCoinTypes, CoinType[]? correctChangeCoins)
		{
			var availableCoinTypesWithQuantity = availableCoinTypes
				.GroupBy(coinTypeWithQuantity => coinTypeWithQuantity)
				.ToDictionary(coinTypeWithQuantity => coinTypeWithQuantity.Key, coinTypeWithQuantity => (uint?)coinTypeWithQuantity.Count());

			var wallet = new Wallet(WalletName.User, availableCoinTypesWithQuantity);

			var change = wallet.CalculateChangeWithLowestCoinQuantity(changeAmount)?.OrderedCoinsWithQuantity;

			var correctChange = correctChangeCoins?
				.OrderBy(coin => coin)
				.GroupBy(coinTypeWithQuantity => coinTypeWithQuantity)
				.ToDictionary(coinTypeWithQuantity => CoinBase.GetCoinByType(coinTypeWithQuantity.Key), coinTypeWithQuantity => (uint?)coinTypeWithQuantity.Count());

			if (change is null)
			{
				Assert.True(correctChange is null);
			}
			else if (correctChange is null)
			{
				Assert.True(change is null);
			}
			else
			{
				var isEqual = change
					.OrderBy(coinWithQuantity => (decimal)coinWithQuantity.Key)
					.SequenceEqual(correctChange.OrderBy(coinWithQuantity => (decimal)coinWithQuantity.Key));

				Assert.True(isEqual);
			}
		}

		[Theory]
		[InlineData(70.7,
			1,
			CoinType.Eur1, 50,
			CoinType.EurCent50, 40,
			CoinType.EurCent20, 30,
			CoinType.EurCent10, 10)]
		public void SpeedTest(decimal changeAmount, int maxSpeedInSeconds, CoinType coinType1, uint quantityCoinType1, CoinType coinType2,
			uint quantityCoinType2, CoinType coinType3, uint quantityCoinType3, CoinType coinType4, uint quantityCoinType4)
		{
			var availableCoinTypesWithQuantity = new Dictionary<CoinType, uint?>()
			{
				[coinType1] = quantityCoinType1,
				[coinType2] = quantityCoinType2,
				[coinType3] = quantityCoinType3,
				[coinType4] = quantityCoinType4,
			};

			var stopWatch = Stopwatch.StartNew();

			new Wallet(WalletName.User, availableCoinTypesWithQuantity)
				.CalculateChangeWithLowestCoinQuantity(changeAmount);

			stopWatch.Stop();

			Debug.WriteLine(stopWatch.Elapsed);

			Assert.True(stopWatch.Elapsed < TimeSpan.FromSeconds(maxSpeedInSeconds));
		}

	}
}
