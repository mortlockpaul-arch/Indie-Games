using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public interface IResourceFactory
{
	IBaseTextureAnimated CreateAnimatedTexture(int width, int height, int layerCount, TextureDataFormat dataFormat);
}
