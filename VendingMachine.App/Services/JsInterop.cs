using System.Drawing;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using VendingMachine.App.DomainExtensions;
using VendingMachine.App.Resources;

namespace VendingMachine.App.Services
{
	public class JsInterop
	{
		private IJSRuntime JsRuntime { get; }

		public JsInterop(IJSRuntime jsRuntime)
		{
			this.JsRuntime = jsRuntime;
		}

		internal async Task<string> PlaySound(string soundName)
		{
			return await this.JsRuntime.InvokeAsync<string>(nameof(this.PlaySound), soundName.ToString());
		}
	}
}
