namespace Microsoft.XboxLive.MathUtilities;

public interface IBaseTexture
{
	int Width { get; }

	int Height { get; }

	bool IsEmptyTransparent { get; }

	bool IsEmptyOpaque { get; }

	void SetPixels(Colorb[] pixels);

	void SetPixels(IBaseTexture sourceTexture);

	Colorb[] GetPixels();

	int GetMemoryUsage();
}
