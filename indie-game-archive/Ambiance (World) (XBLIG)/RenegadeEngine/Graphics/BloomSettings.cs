namespace RenegadeEngine.Graphics;

public struct BloomSettings(float bloomThreshold, float blurAmount, float bloomIntensity, float baseIntensity, float bloomSaturation, float baseSaturation)
{
	public float BloomThreshold = bloomThreshold;

	public float BlurAmount = blurAmount;

	public float BloomIntensity = bloomIntensity;

	public float BaseIntensity = baseIntensity;

	public float BloomSaturation = bloomSaturation;

	public float BaseSaturation = baseSaturation;
}
