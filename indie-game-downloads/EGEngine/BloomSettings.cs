namespace EGEngine;

public class BloomSettings
{
	public readonly string Name;

	public float BloomThreshold;

	public float BlurAmount;

	public float BloomIntensity;

	public float BaseIntensity;

	public float BloomSaturation;

	public float BaseSaturation;

	public readonly float PixelStep;

	public readonly float TexelReadStep;

	public static BloomSettings[] PresetSettings = new BloomSettings[13]
	{
		new BloomSettings("NoBloom", 0f, 1f, 0f, 1f, 0f, 1f, 0f, 0f),
		new BloomSettings("Default", 0.25f, 4f, 1.25f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("Soft", 0f, 3f, 1f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("Desaturated", 0.5f, 8f, 2f, 1f, 0f, 1f, 2f, 1.5f),
		new BloomSettings("Saturated", 0.25f, 4f, 2f, 1f, 2f, 0f, 2f, 1.5f),
		new BloomSettings("Blurry", 0f, 2f, 1f, 0.1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("Subtle", 0.5f, 2f, 1f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("SoftTight", 0.2f, 4f, 1f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("NightTime", 0.075f, 1f, 1f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("AvR", 0.1f, 2f, 4f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("AvR1", 0f, 8f, 1f, 1f, 1f, 1f, 2f, 1.5f),
		new BloomSettings("EoDSirvivor", 0.8f, 4f, 1f, 1f, 0f, 1f, 2f, 1.5f),
		new BloomSettings("ToyPlane", 0.25f, 2f, 0.5f, 1f, 1f, 1f, 2f, 1.5f)
	};

	public BloomSettings(string name, float bloomThreshold, float blurAmount, float bloomIntensity, float baseIntensity, float bloomSaturation, float baseSaturation, float pixelStep, float texelStep)
	{
		Name = name;
		BloomThreshold = bloomThreshold;
		BlurAmount = blurAmount;
		BloomIntensity = bloomIntensity;
		BaseIntensity = baseIntensity;
		BloomSaturation = bloomSaturation;
		BaseSaturation = baseSaturation;
		PixelStep = pixelStep;
		TexelReadStep = texelStep;
	}
}
