namespace Microsoft.XboxLive.MathUtilities;

public interface IBaseTextureAnimated : IBaseTexture
{
	int LayersCount { get; }

	void SetTextureLayer(int layerIndex, Colorb[] pixels);

	Colorb[] GetTextureLayerPixels(int layerIndex);

	void SetTextureLayer(int layerIndex, byte[] data, int dataWidth, int dataHeight);

	void SelectTextureLayer(int layerIndex);
}
