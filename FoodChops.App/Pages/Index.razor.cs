using CodeChops.FoodChops.App.Domain;
using CodeChops.FoodChops.App.DomainExtensions;
using CodeChops.FoodChops.App.Services;
using Microsoft.AspNetCore.Components;

namespace CodeChops.FoodChops.App.Pages;

public class FoodChopsComponent : ComponentBase
{
	[Inject] internal SoundPlayer SoundPlayer { get; set; } = null!;
	[Inject] internal Machine Machine { get; set; } = null!;
	[Inject] internal User User { get; set; } = null!;

	internal static EventHandler<FoodChopsComponent> UpdateViewHandler { get; private set; } = null!;
	internal ProductStack? SelectedProductStack { get; private set; }
	internal Wallet? CoinChangeWallet { get; private set; }
	internal MarkupString? Message { get; private set; }

	protected override void OnInitialized()
	{
		this.Message = this.Machine.GetMessageIfAvailable();
		UpdateViewHandler += (_, _) => this.InvokeAsync(this.StateHasChanged);
	}

	internal async Task OnClickProductStack(ProductStack stack)
	{
		if (stack.AvailablePortions == 0) return;

		this.SelectedProductStack = this.SelectedProductStack == stack ? null : stack;

		await this.SoundPlayer.Play(SoundName.ButtonClick);

		this.Message = this.Machine.GetMessageIfAvailable();
		this.UpdateView();
	}

	/// <summary>
	/// This will update the state between all connected Websockets-clients.
	/// </summary>
	private void UpdateView()
	{
		UpdateViewHandler.Invoke(sender: null, this);
	}

	internal async Task OnClickBuyProduct()
	{
		// Cannot buy a product if no product is selected.
		if (this.SelectedProductStack is null) 
			return;

		// Check for insufficient amount.
		if (this.SelectedProductStack.Product.Price > this.Machine.UserInsertedCoinsWallet.Amount)
		{
			await ShowMessageAndUpdateStacks(message: Resources.Messages.InsufficientAmount, soundName: SoundName.Error);
			return;
		}

		var coinChangeWallet = this.Machine.BuyProduct(this.User, this.SelectedProductStack);
		
		// Check if unable to provide change.
		if (coinChangeWallet is null)
		{
			await ShowMessageAndUpdateStacks(message: Resources.Messages.UnableToProvideChange, soundName: SoundName.Error);
			return;
		}

		// Give back change.
		if (coinChangeWallet.Amount > 0)
		{
			await this.SoundPlayer.Play(SoundName.CoinDrop);
			this.CoinChangeWallet = coinChangeWallet;
		}

		await ShowMessageAndUpdateStacks(message: Resources.Messages.ThankYou, soundName: SoundName.ProductDrop, removeSelection: true);
	}

	private async Task ShowMessageAndUpdateStacks(string message, SoundName soundName, bool removeSelection = false)
	{
		this.Message = new MarkupString(message);
		await this.SoundPlayer.Play(soundName);
		this.UpdateView();
		await Task.Delay(1000);
		this.Message = null;

		if (removeSelection) this.SelectedProductStack = null;
		this.StateHasChanged();
		this.Message = this.Machine.GetMessageIfAvailable();
	}

	internal async Task UserInsertsCoin(Coin? coin)
	{
		if (coin is null) 
			return;

		this.User.Wallet.TransferOneCoin(this.Machine.UserInsertedCoinsWallet, coin);
		await this.SoundPlayer.Play(SoundName.CoinInsert);
		this.UpdateView();
	}

	internal async Task ReleaseCoins()
	{
		if (this.Machine.UserInsertedCoinsWallet.Amount == 0) 
			return;

		this.Machine.UserInsertedCoinsWallet.TransferAllCoinsTo(this.User.Wallet);
		await ShowMessageAndUpdateStacks(message: Resources.Messages.MoneyEjected, soundName: SoundName.CoinDrop, removeSelection: false);
		this.CoinChangeWallet = null;
		this.UpdateView();
	}
}