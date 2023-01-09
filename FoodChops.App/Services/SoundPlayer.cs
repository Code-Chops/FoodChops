using CodeChops.MagicEnums;

namespace CodeChops.FoodChops.App.Services;

public record SoundName : MagicStringEnum<SoundName>
{
	public static SoundName CoinInsert		{ get; } = CreateMember();
	public static SoundName ButtonClick		{ get; } = CreateMember();
	public static SoundName CoinDrop		{ get; } = CreateMember();
	public static SoundName ProductDrop		{ get; } = CreateMember();
	public static SoundName Error			{ get; } = CreateMember();
}

public class SoundPlayer
{
	private JsInterop JsInterop { get; }

	public SoundPlayer(JsInterop jsInterop)
	{
		this.JsInterop = jsInterop;
	}

	public async Task Play(SoundName soundName)
	{
		if (soundName is null) throw new ArgumentNullException(nameof(soundName));
		await this.JsInterop.PlaySound(soundName.ToString()!);
	}
}