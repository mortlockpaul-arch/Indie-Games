using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

internal class ResourceFactory : IResourceFactory
{
	internal ResourceFactory()
	{
	}

	public IBaseTextureAnimated CreateAnimatedTexture(int width, int height, int layerCount, TextureDataFormat dataFormat)
	{
		return new AnimatedTexture(width, height, layerCount, dataFormat);
	}
}
