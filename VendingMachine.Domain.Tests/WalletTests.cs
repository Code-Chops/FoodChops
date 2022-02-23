using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Domain.Coins;
using VendingMachine.Helpers.Amounts;
using Xunit;

namespace VendingMachine.Domain.Tests
{
	public class WalletTests
	{
		/// <summary>
		/// Tests the construction (and therefore valid state) of a wallet.
		/// </summary>
		/// <param name="constructionShouldBeSuccessful">The expected result of the test</param>
		/// <param name="coinTypes"></param>
		/// <param name="currencyCode">Unfortunately an attribute argument must be a compile-time constant. Therefore a currency code (string) is being used.</param>
		[Theory]
		[InlineData(true, new [] { CoinType.EurCent50 })]
		[InlineData(true, new CoinType[] { }, "EUR")]
		[InlineData(false, new CoinType[] { })]
		[InlineData(false, new [] { CoinType.Eur1 }, "USD")]
		public void ConstructionTest(bool constructionShouldBeSuccessful, CoinType[] coinTypes, string? currencyCode = null)
		{
			var constructionSuccessful = true;

			try
			{
				var wallet = new Wallet(
					name: WalletName.User,
					coinTypesWithQuantity: coinTypes
						.GroupBy(coinType => coinType)
						.ToDictionary(coinTypesByType => coinTypesByType.Key, coinTypesByType => (uint?)coinTypesByType.Count()),
					currency: currencyCode is null ? null : (Currency)currencyCode);
			}
#pragma warning disable 168
			catch (Exception e)
#pragma warning restore 168
			{
				constructionSuccessful = false;
			}

			Assert.Equal(constructionShouldBeSuccessful, constructionSuccessful);
		}
	}
}
