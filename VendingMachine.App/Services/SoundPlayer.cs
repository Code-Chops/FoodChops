namespace VendingMachine.App.Services;

public enum SoundName
{
	CoinInsert,
	ButtonClick,
	CoinDrop,
	ProductDrop,
	Error,
}

public class SoundPlayer
{
	private JsInterop JsInterop { get; }

	public SoundPlayer(JsInterop jsInterop)
	{
		this.JsInterop = jsInterop;
	}

	public async Task Play(SoundName name)
	{
		await this.JsInterop.PlaySound(name.ToString());
	}
}
