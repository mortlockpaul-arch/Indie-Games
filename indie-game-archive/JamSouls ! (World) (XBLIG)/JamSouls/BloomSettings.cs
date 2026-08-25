namespace JamSouls;

public class BloomSettings
{
	public readonly string Name;

	public readonly float BloomThreshold;

	public readonly float BlurAmount;

	public readonly float BloomIntensity;

	public readonly float BaseIntensity;

	public readonly float BloomSaturation;

	public readonly float BaseSaturation;

	public static BloomSettings[] PresetSettings = new BloomSettings[3]
	{
		new BloomSettings("DeathMatch", 0.5f, 2f, 1f, 1f, 1f, 1f),
		new BloomSettings("DeathSpecial", 0.25f, 4f, 2f, 1f, 2f, 0f),
		new BloomSettings("BlackAndWhite", 0.35f, 0f, 0f, 0.5f, 1f, 0f)
	};

	public BloomSettings(string name, float bloomThreshold, float blurAmount, float bloomIntensity, float baseIntensity, float bloomSaturation, float baseSaturation)
	{
		Name = name;
		BloomThreshold = bloomThreshold;
		BlurAmount = blurAmount;
		BloomIntensity = bloomIntensity;
		BaseIntensity = baseIntensity;
		BloomSaturation = bloomSaturation;
		BaseSaturation = baseSaturation;
	}
}
