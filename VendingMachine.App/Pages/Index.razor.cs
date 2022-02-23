using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VendingMachine.App.DomainExtensions;
using VendingMachine.App.Services;
using VendingMachine.Domain;
using VendingMachine.Domain.Coins;

namespace VendingMachine.App.Pages
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public class VendingMachineComponent : ComponentBase
	{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		[Inject] internal SoundPlayer SoundPlayer { get; set; }
        [Inject] internal Machine Machine { get; set; }
        [Inject] internal User User { get; set; }
        internal static EventHandler<VendingMachineComponent> UpdateViewHandler { get; private set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		internal ProductStack? SelectedProductStack { get; private set; }
        internal Wallet? CoinChangeWallet { get; private set; }
        internal MarkupString? Message { get; private set; }


        protected override void OnInitialized()
        {
            this.Message = this.Machine.GetMessageIfAvailable();
            UpdateViewHandler += (o, e) => this.InvokeAsync(this.StateHasChanged);
        }

        internal async Task OnClickProductStack(ProductStack stack)
        {
            if (stack.AvailablePortions == 0) return;

            this.SelectedProductStack = (this.SelectedProductStack == stack) ? null : stack;

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
            // Cannot buy a product if no product is selected
            if (this.SelectedProductStack is null) return;

            if (this.SelectedProductStack.Product.Price > this.Machine.UserInsertedCoinsWallet.Amount)
            {
                this.Message = new MarkupString(Resources.Messages.InsufficientAmount);
                await this.SoundPlayer.Play(SoundName.Error);
                await Task.Delay(800);
                this.Message = null;
                return;
            }

            var coinChangeWallet = this.Machine.BuyProduct(this.User, this.SelectedProductStack);
            if (coinChangeWallet is null)
            {
                this.Message = new MarkupString(Resources.Messages.UnableToProvideChange);
                await this.SoundPlayer.Play(SoundName.Error);

                await Task.Delay(800);
                this.Message = null;
            }
            else
            {
                if (coinChangeWallet.Amount > 0)
                {
                    await this.SoundPlayer.Play(SoundName.CoinDrop);

                    this.CoinChangeWallet = coinChangeWallet;
                }
                else
                {
                    this.CoinChangeWallet = null;
                }

                await this.SoundPlayer.Play(SoundName.ProductDrop);

                this.Message = new MarkupString(Resources.Messages.ThankYou);

                this.StateHasChanged();
                await Task.Delay(800);
                this.Message = this.Machine.GetMessageIfAvailable();
            }

            this.SelectedProductStack = null;
            this.UpdateView();
        }

        internal async Task UserInsertsCoin(CoinBase coin)
        {
            this.User.Wallet.TransferOneCoin(this.Machine.UserInsertedCoinsWallet, coin);
            await this.SoundPlayer.Play(SoundName.CoinInsert);
            this.UpdateView();
        }

        internal async Task ReleaseCoins()
        {
            this.Machine.UserInsertedCoinsWallet.TransferAllCoins(this.User.Wallet);
            await this.SoundPlayer.Play(SoundName.CoinDrop);
            this.CoinChangeWallet = null;
            this.UpdateView();
        }
    }
}
