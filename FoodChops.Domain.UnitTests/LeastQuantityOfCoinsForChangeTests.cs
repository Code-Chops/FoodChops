using System.Diagnostics;
using CodeChops.FoodChops.App.Domain;
using Xunit;

namespace CodeChops.FoodChops.Domain.UnitTests;

public class LeastQuantityOfCoinsForChangeTests
{
	[Theory]

	[InlineData(
		1,
		new[]
		{
			nameof(Coin.Eur1),
		},
		new[]
		{
			nameof(Coin.Eur1),
		})]

	[InlineData(
		0.4,
		new[]
		{
			nameof(Coin.Eur1),
		},
		null)]

	[InlineData(
		0.5,
		new[]
		{
			nameof(Coin.Eur1),
			nameof(Coin.EurCent20),
			nameof(Coin.EurCent50),
		},
		new[]
		{
			nameof(Coin.EurCent50),
		})]

	[InlineData(
		1.1,
		new[]
		{
			nameof(Coin.EurCent50), nameof(Coin.EurCent50),
			nameof(Coin.EurCent20), nameof(Coin.EurCent20), nameof(Coin.EurCent20),
		},
		new[]
		{
			nameof(Coin.EurCent20), nameof(Coin.EurCent20), nameof(Coin.EurCent20),
			nameof(Coin.EurCent50),
		})]

	[InlineData(
		1.2,
		new[]
		{
			nameof(Coin.EurCent20), nameof(Coin.EurCent20),
			nameof(Coin.EurCent50), nameof(Coin.EurCent50), nameof(Coin.EurCent50),
			nameof(Coin.EurCent10), nameof(Coin.EurCent10), nameof(Coin.EurCent10),
		},
		new[]
		{
			nameof(Coin.EurCent20),
			nameof(Coin.EurCent50), nameof(Coin.EurCent50),
		})]

	[InlineData(
		1.8,
		new[]
		{
			nameof(Coin.EurCent20), nameof(Coin.EurCent20),
			nameof(Coin.EurCent50), nameof(Coin.EurCent50), nameof(Coin.EurCent50),
			nameof(Coin.EurCent10), nameof(Coin.EurCent10), nameof(Coin.EurCent10), nameof(Coin.EurCent10), nameof(Coin.EurCent10)
		},
		new[]
		{
			nameof(Coin.EurCent50), nameof(Coin.EurCent50), nameof(Coin.EurCent50),
			nameof(Coin.EurCent20),
			nameof(Coin.EurCent10),
		})]

	[InlineData(
		1.6,
		new[]
		{
			nameof(Coin.Eur1),      nameof(Coin.Eur1),
			nameof(Coin.EurCent10),	nameof(Coin.EurCent10),
			nameof(Coin.EurCent50), nameof(Coin.EurCent50), nameof(Coin.EurCent50), nameof(Coin.EurCent50),
		},
		new[]
		{
			nameof(Coin.Eur1),
			nameof(Coin.EurCent50),
			nameof(Coin.EurCent10),
		})]

	[InlineData(
		2.1,
		new[]
		{
			nameof(Coin.Eur1),		nameof(Coin.Eur1),      nameof(Coin.Eur1),		nameof(Coin.Eur1),
			nameof(Coin.EurCent20), nameof(Coin.EurCent20), nameof(Coin.EurCent20),
			nameof(Coin.EurCent50), nameof(Coin.EurCent50)
		},
		new[]
		{
			nameof(Coin.EurCent20), nameof(Coin.EurCent20), nameof(Coin.EurCent20),
			nameof(Coin.EurCent50),
			nameof(Coin.Eur1),
		})]

	[InlineData(
		2.8,
		new[]
		{
			nameof(Coin.Eur1),      nameof(Coin.Eur1),      nameof(Coin.Eur1),		nameof(Coin.Eur1),
			nameof(Coin.EurCent20), nameof(Coin.EurCent20), nameof(Coin.EurCent20),
			nameof(Coin.EurCent50),
		},
		null)]

	public void CorrectChangeTest(decimal changeAmount, string[] availableCoinNames, string[]? correctChangeCoinNames)
	{
		var availableCoinsWithQuantity = availableCoinNames.Select(coin => Coin.GetSingleMember(coin))
			.GroupBy(coinWithQuantity => coinWithQuantity)
			.ToDictionary(coinWithQuantity => coinWithQuantity.Key, coinWithQuantity => (uint?)coinWithQuantity.Count());

		var wallet = new Wallet(WalletType.User, availableCoinsWithQuantity);

		var change = wallet.CalculateChangeWithLowestCoinQuantity(changeAmount)?.OrderedCoinsWithQuantity;

		var correctChange = correctChangeCoinNames?.Select(coin => Coin.GetSingleMember(coin))
			.OrderBy(coin => coin.Value)
			.GroupBy(coinWithQuantity => coinWithQuantity)
			.ToDictionary(coinWithQuantity => coinWithQuantity.Key, coinWithQuantity => (uint?)coinWithQuantity.Count());

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
				.OrderBy(coinWithQuantity => (decimal)coinWithQuantity.Key.Value)
				.SequenceEqual(correctChange.OrderBy(coinWithQuantity => (decimal)coinWithQuantity.Key.Value));

			Assert.True(isEqual);
		}
	}

	[Theory]
	[InlineData(70.7,
		1,
		nameof(Coin.Eur1),		50,
		nameof(Coin.EurCent50), 40,
		nameof(Coin.EurCent20), 30,
		nameof(Coin.EurCent10), 10)]
	public void SpeedTest(decimal changeAmount, int maxSpeedInSeconds, string coin1, uint quantityCoin1, string coin2,
		uint quantityCoin2, string coin3, uint quantityCoin3, string coin4, uint quantityCoin4)
	{
		var availableCoinsWithQuantity = new Dictionary<Coin, uint?>()
		{
			[Coin.GetSingleMember(coin1)] = quantityCoin1,
			[Coin.GetSingleMember(coin2)] = quantityCoin2,
			[Coin.GetSingleMember(coin3)] = quantityCoin3,
			[Coin.GetSingleMember(coin4)] = quantityCoin4,
		};

		var stopWatch = Stopwatch.StartNew();

		new Wallet(WalletType.User, availableCoinsWithQuantity)
			.CalculateChangeWithLowestCoinQuantity(changeAmount);

		stopWatch.Stop();

		Debug.WriteLine(stopWatch.Elapsed);

		Assert.True(stopWatch.Elapsed < TimeSpan.FromSeconds(maxSpeedInSeconds));
	}
}